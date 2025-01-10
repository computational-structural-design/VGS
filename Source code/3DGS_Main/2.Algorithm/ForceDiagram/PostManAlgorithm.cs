using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Data;
using Grasshopper;
using Rhino.Render;
using Grasshopper.Kernel.Special;
using MathNet.Numerics;
using Rhino.DocObjects.Tables;

namespace VGS_Main
{
    /// <summary>
    /// This algo is inspired by +++
    /// </summary>
    public static class PostManAlgorithm
    {

        public static List<List<string>> AutoFindVe(List<List<List<string>>> Cycle, out string report)
        {
            report = "\n[Function AutoFindVe]:";
            List<List<List<string>>> Cycle_cal = DeepCopyStringList(Cycle);
            //Check if there is Ve cicle in the list, if so find it out.
            List<List<string>> ve_cycle = new List<List<string>>();
            for (int i = 0; i < Cycle.Count; i++)
            {
                List<List<string>> check = Cycle[i].Where(p => p[0] == "ve").ToList();
                if (check.Count > 0) { ve_cycle = Cycle[i].Select(p => new List<string>(p)).ToList(); break; }
            }

            if (ve_cycle.Count != 0) //there is no ve cycle in the cycle list;
            { return ve_cycle; }

            //check the duplication
            List<List<string>> All_Vnms = new List<List<string>>();
            foreach (List<List<string>> sub_cycle in Cycle)
            {
                All_Vnms.AddRange(sub_cycle);
            }

            List<string> Lib_all_Vnm = new List<string>();//all the [vn,vm,k] in the main cycle
            for (int i = 0; i < All_Vnms.Count; i++)
            {
                List<string> vnm = All_Vnms[i];
                string vnm_id = String.Format("[{0},{1},{2}]", vnm[0], vnm[1], vnm[2]);
                Lib_all_Vnm.Add(vnm_id);
            }

            List<string> Lib_bound_Vnm = new List<string>();// the not-coupled [vn,vm,k]
            for (int i = 0; i < All_Vnms.Count; i++)
            {
                List<string> vnm = All_Vnms[i];
                string vnm_id = String.Format("[{0},{1},{2}]", vnm[0], vnm[1], vnm[2]);
                string vmn_id = String.Format("[{0},{1},{2}]", vnm[1], vnm[0], vnm[2]);

                if (!Lib_all_Vnm.Contains(vmn_id))//check if the opposite [vn,vm,k] is in the list (is a pair)
                {
                    Lib_bound_Vnm.Add(vnm_id);
                }
            }
            report += "\n  The Lib_bound_Vnm:" + " " + Lib_bound_Vnm.Count.ToString() + "\n" + string.Join(" ", Lib_bound_Vnm.ToArray());

            List<List<string>> CurrentCycle = Cycle_cal[0].Select(p => new List<string>(p)).ToList();
            Cycle_cal.RemoveAt(0);

            while (true)
            {
                report += string.Format("\n  CurrentCycle.Count: {0} |Cycle_cal.Count: {1}", CurrentCycle.Count.ToString(), Cycle_cal.Count.ToString());

                bool LoopDone = true;//if CurrentCycle still contains non-boundary half-edge
                foreach (List<string> F_vnm in CurrentCycle)
                {
                    string F_vnm_id = String.Format("[{0},{1},{2}]", F_vnm[0], F_vnm[1], F_vnm[2]);
                    if (!Lib_bound_Vnm.Contains(F_vnm_id)) { LoopDone = false; break; }
                }
                if (LoopDone) { break; }

                while (true)//shuffle the cycle to make sure the first half-edge is not a boundary one
                {
                    List<string> F_vnm = CurrentCycle[0];
                    string F_vnm_id = String.Format("[{0},{1},{2}]", F_vnm[0], F_vnm[1], F_vnm[2]);
                    if (Lib_bound_Vnm.Contains(F_vnm_id)) //which means the first element is a boundary half-edge
                    {
                        //shuffle the cycle
                        CurrentCycle.Add(CurrentCycle[0]);
                        CurrentCycle.RemoveAt(0);
                    }
                    else { break; }
                }

                List<string> c_vnm = CurrentCycle[0];
                bool found = false;
                for (int i = 0; i < Cycle_cal.Count; i++)
                {
                    int index = -1;
                    for (int j = 0; j < Cycle_cal[i].Count; j++)
                    {
                        List<string> comp = Cycle_cal[i][j];
                        if (c_vnm[0] == comp[1] && c_vnm[1] == comp[0] && c_vnm[2] == comp[2])
                        {
                            report += string.Format("\n    MatchSucess [{0},{1},{2}]&[{3},{4},{5}]", c_vnm[0], c_vnm[1], c_vnm[2], comp[0], comp[1], comp[2]);
                            found = true; index = j; break;
                        }
                    }

                    if (found)
                    {
                        List<List<string>> TobeMergedCycle = new List<List<string>>();
                        for (int k = index; k < Cycle_cal[i].Count; k++)
                        {
                            TobeMergedCycle.Add(Cycle_cal[i][k]);
                        }

                        for (int k = 0; k < index; k++)
                        {
                            TobeMergedCycle.Add(Cycle_cal[i][k]);
                        }
                        Cycle_cal.RemoveAt(i);

                        TobeMergedCycle.RemoveAt(0);
                        CurrentCycle.AddRange(TobeMergedCycle);
                        CurrentCycle.RemoveAt(0);

                        List<List<string>> NonDupli_CurrentCycle = new List<List<string>>();
                        for (int j = 0; j < CurrentCycle.Count; j++)
                        {

                            bool same = false;
                            for (int k = 0; k < CurrentCycle.Count; k++)
                            {
                                if (j == k) { continue; }
                                List<string> ext = CurrentCycle[j];
                                List<string> inn = CurrentCycle[k];
                                if (ext[0] == inn[1] && ext[1] == inn[0] && ext[2] == inn[2]) { same = true; break; }
                            }

                            if (!same) { NonDupli_CurrentCycle.Add(CurrentCycle[j]); }
                        }
                        CurrentCycle = NonDupli_CurrentCycle;

                        break;
                    }

                }

                if (!found)
                {
                    report += string.Format("\n    NotAbleToFind [{0},{1},{2}] in:\n", c_vnm[0], c_vnm[1], c_vnm[2]);
                    report += string.Join("\n", Cycle_cal.Select(p => string.Join(",", p.Select(k => string.Format("[{0},{1},{2}]", k[0], k[1], k[2])))));
                }
            }
            ve_cycle = CurrentCycle;
            ve_cycle.Reverse();
            return ve_cycle;
        }

        public static void ForceDiagramAssemblingOrder(List<List<List<string>>> circle_order, List<List<string>> circle_full, List<string> Vn, List<List<string>> ideal_V, List<List<string>> ideal_E, out DataTree<string> st2d_s_out, out List<string> st1_v_out, out List<string> st1_c_out, out List<string> st1_e_out, out List<string> ve_list, bool display)
        {
            //[circle_order] the embeded list of [vn,vm][vi,vj];
            //[circle_full] the embeded list of eN;
            //[Vn] a list of all the vn;
            //[ideal_V] The planarized list of [Vn,Vm];
            //[ideal_E] The planarized list of [eN];

            //[220405]AutoMatically Find the Ve based on the exsiting embeding info
            string report = "";
            List<List<string>> ve_cycle = AutoFindVe(circle_order,out report);//AutoFindVe: A new way to find the boundary

            ve_list = ve_cycle.Select(p => CompareCode3d(p, ideal_V, ideal_E)).ToList();
            //End

            List<List<List<string>>> Embeded_list = DeepCopyStringList(circle_order);//Make a copy of the embeded order;

            Clean_ve(Embeded_list);//Delete the connection with ve (All the external forces are supposed to assemble once)
            List<List<List<string>>> HalfEdge_list = DeepCopyStringList(Embeded_list);
            int VnCount = HalfEdge_list.Count;

            List<string> index = new List<string>();//index named with string
            for (int i = 0; i < Embeded_list.Count; i++) { index.Add(Embeded_list[i][0][0]); }//defaulty, define the first element in the first edge of the embeded sequence as index
            List<string> compare_index = new List<string>(index);

            index.Sort((p1, p2) => Embeded_list[compare_index.IndexOf(p1)].Count.CompareTo(Embeded_list[compare_index.IndexOf(p2)].Count));//Sort according to the edge number of every cycle
            int pi = 0;
            string min = index[pi];//make sure the first starting point is alway the least length
            while (min.Substring(0, 1) == "n") { pi++; min = index[pi]; }//Keep the first assemble point is vN

            List<string> Vex_index = new List<string>();//The Final sequence of assembling;

            //[220404_decline delete]
            while (Vex_index.Count < VnCount)//停止循环的数量由decline值减少|decline:由抹除ve后产生的数量差
            {
                bool going;

                while (true)
                {
                    going = findpath(ref min, Vex_index, Embeded_list, compare_index);//Main_loop
                                                                                      //Vex_index:每次循环得出的下一个点；
                                                                                      //Embeded_list:整个(Vn,Vm)表；will be shrinking in the process
                                                                                      //compare_index：circle_order去掉ve后的每一排的第一个Vn，用于在进行Elimination时寻找在哪一行；

                    if (going == false && min != "None") { Vex_index.Add(min); break; }
                    else if (going == false && min == "None") { break; }
                }

                if (going == false && Vex_index.Count < VnCount)
                {

                    List<string> Left_index = CopyStringList(index);
                    //find the rest point
                    foreach (string i in Vex_index) { if (Left_index.Contains(i)) { Left_index.Remove(i); } }//check the connection with the left points index
                    List<string> choice = new List<string>();
                    bool connection = false;
                    foreach (string i in Left_index)
                    {
                        foreach (string j in Vex_index)
                        {
                            if (CompareCode(new List<string>() { i, j }, ideal_V, ideal_E) != "None")//There is a connection relationship with the original center point
                            { choice.Add(i); connection = true; break; }
                            if (i == Left_index[Left_index.Count - 1] && j == Vex_index[Vex_index.Count - 1])//When there is no result when the last corresponding element is checked;
                            {
                                if (CompareCode(new List<string>() { i, j }, ideal_V, ideal_E) == "None") { }
                            }
                        }
                        if (connection == true) { break; }
                    }
                    if (choice.Count > 0) { min = choice[0]; }
                }
            }

            //Print("FINALLIST_:" + CombstringList(Vex_index));
            DataTree<string> st2d_s = new DataTree<string>();
            List<string> edge = new List<string>();
            List<string> Vetice = new List<string>();
            List<string> Ve_adjust = new List<string>();

            for (int i = 1; i < Vex_index.Count; i++)//Here to find the connection relationship between the point sequence
            {
                string edgename = CompareCode(new List<string>() { Vex_index[i], Vex_index[i - 1] }, ideal_V, ideal_E);//Under normal circumstances, two adjacent points can find the corresponding connecting edge
                                                                                                                       //There might be a jump：When the two points are not adjacent (each point in nN does not belong to the connection relationship, you need to find the connection relationship with the previous v series)
                if (edgename == "None")
                {
                    List<string> valid_possibles = new List<string>();
                    for (int j = 0; j < i; j++) { valid_possibles.Add(Vex_index[j]); }//Find the information associated with the previously existing edges, so that JUMP can be connected
                    foreach (string k in valid_possibles)
                    {
                        string temp = CompareCode(new List<string>() { Vex_index[i], k }, ideal_V, ideal_E);
                        if (temp != "None") { edge.Add(temp); Ve_adjust.Add(k); break; }
                        if (k == valid_possibles[valid_possibles.Count - 1] && temp == "None") { }
                    }
                }
                else
                {
                    Ve_adjust.Add(Vex_index[i - 1]);
                    edge.Add(edgename);
                }
            }

           Vetice = Vex_index;

            foreach (string id in Vetice)
            {
                List<string> temp = new List<string>();
                temp = circle_full[Vn.IndexOf(id)];

                st2d_s.AddRange(temp, new GH_Path(1, Vetice.IndexOf(id)));
            }

            st2d_s_out = st2d_s;
            st1_v_out = Vex_index;
            st1_c_out = Ve_adjust;
            st1_e_out = edge;
        }
        public static List<List<List<string>>> OrganizeLabels(ref List<List<string>> edgeVnm, ref List<List<string>> edgeIDs)
        {
            string Report = "\n[OrganizeLabels]\n";

            List<List<string>> r_edgeVnm = new List<List<string>>();//No groups marked with a plus sign
            List<List<string>> r_edgeIDs = new List<List<string>>();

            List<List<string>> l_edgeVnm = new List<List<string>>();
            List<List<string>> l_edgeIDs = new List<List<string>>();

            List<List<string>> checked_list = new List<List<string>>();

            List<List<string>> order_vv = new List<List<string>>();
            List<List<string>> order_ee = new List<List<string>>();

            for (int i = 0; i < edgeIDs.Count; i++)
            {
                string id = edgeIDs[i][0];
                char[] chars = id.ToCharArray();
                if (chars[chars.Length - 1] != '+')
                { r_edgeVnm.Add(edgeVnm[i]); r_edgeIDs.Add(edgeIDs[i]); }
                else
                { l_edgeVnm.Add(edgeVnm[i]); l_edgeIDs.Add(edgeIDs[i]); }
            }

            List<string> LIB = new List<string>();
            List<List<List<string>>> LIB_vnm = new List<List<List<string>>>();//Same side group
            List<List<List<string>>> LIB_id = new List<List<List<string>>>();//Same side group
            for (int i = 0; i < l_edgeIDs.Count; i++)
            {
                string edgeID_title = RemoveLabel(l_edgeIDs[i]);
                if (LIB.Contains(edgeID_title))
                {
                    int ind = LIB.IndexOf(edgeID_title);
                    LIB_vnm[ind].Add(l_edgeVnm[i]);
                    LIB_id[ind].Add(l_edgeIDs[i]);
                }
                else
                {
                    LIB.Add(edgeID_title);
                    LIB_vnm.Add(new List<List<string>>() { l_edgeVnm[i] });
                    LIB_id.Add(new List<List<string>>() { l_edgeIDs[i] });
                }
            }

            for (int i = 0; i < LIB_vnm.Count; i++)
            {
                List<List<string>> vnm_group = LIB_vnm[i];
                List<List<string>> id_group = LIB_id[i];
                Sequence(ref vnm_group, ref id_group);

                Report += "Group" + i.ToString() + "\n";//
                Report += string.Join(" ", vnm_group.Select(p => string.Format("[{0},{1},{2}]", p[0], p[1], p[2]))) + "\n";//
                Report += string.Join(" ", id_group.Select(p => p[0])) + "\n";//

                List<string> vv = vnm_group.Select(p => p[0]).ToList(); vv.Add(vnm_group[vnm_group.Count - 1][1]);
                List<string> ee = id_group.Select(p => p[0]).ToList();

                order_vv.Add(vv);
                order_ee.Add(ee);

                r_edgeVnm.AddRange(vnm_group);
                r_edgeIDs.AddRange(id_group);
            }

            edgeVnm = r_edgeVnm;
            edgeIDs = r_edgeIDs;
            return new List<List<List<string>>>() { order_vv, order_ee };
        }
        public static void Sequence(ref List<List<string>> vnms, ref List<List<string>> ids)
        {
            List<List<string>> all_vnms = vnms.Select(p => new List<string>(p)).ToList();

            List<string> vnm0 = new List<string>(all_vnms[0]);
            all_vnms.RemoveAt(0);

            List<List<string>> jointed = new List<List<string>>() { vnm0 };

            while (all_vnms.Count > 0)
            {
                string front = jointed[0][0];
                string back = jointed[jointed.Count - 1][1];

                for (int i = 0; i < all_vnms.Count; i++)
                {
                    List<string> vnm1 = all_vnms[i];
                    if (front == vnm1[0]) { jointed.Insert(0, new List<string>() { vnm1[1], vnm1[0], vnm1[2] }); all_vnms.RemoveAt(i); break; }
                    else if (front == vnm1[1]) { jointed.Insert(0, new List<string>() { vnm1[0], vnm1[1], vnm1[2] }); all_vnms.RemoveAt(i); break; }
                    else if (back == vnm1[0]) { jointed.Add(new List<string>() { vnm1[0], vnm1[1], vnm1[2] }); all_vnms.RemoveAt(i); break; }
                    else if (back == vnm1[1]) { jointed.Add(new List<string>() { vnm1[1], vnm1[0], vnm1[2] }); all_vnms.RemoveAt(i); break; }
                }
            }

            vnms = jointed.Select(p => new List<string>(p)).ToList();
            string eN = RemoveLabel(ids[0]);
            ids = new List<List<string>>();

            for (int i = 0; i < vnms.Count; i++)
            {
                eN += "+";
                ids.Add(new List<string>() { eN });
            }

        }
        public static string RemoveLabel(List<string> ID)
        {
            string temp;
            string[] names = ID[0].Split('+');
            temp = names[0];

            return temp;
        }
        public static string CompareCode(List<string> id, List<List<string>> Edgecode, List<List<string>> edgemet)
        {
            string Edgefit = "None";
            for (int i = 0; i < Edgecode.Count; i++)
            {
                if ((id[0] == Edgecode[i][0] && id[1] == Edgecode[i][1]) || (id[1] == Edgecode[i][0] && id[0] == Edgecode[i][1]))
                { Edgefit = edgemet[i][0]; }
            }
            return Edgefit;
        }
        public static string CompareCode3d(List<string> id, List<List<string>> Edgecode, List<List<string>> edgemet)
        {
            string Edgefit = "None";
            for (int i = 0; i < Edgecode.Count; i++)
            {
                if ((id[0] == Edgecode[i][0] && id[1] == Edgecode[i][1] && id[2] == Edgecode[i][2]) || (id[1] == Edgecode[i][0] && id[0] == Edgecode[i][1] && id[2] == Edgecode[i][2]))
                { Edgefit = edgemet[i][0]; }
            }
            return Edgefit;
        }
        public static bool findpath(ref string startindex, List<string> Vex_index, List<List<List<string>>> initialList, List<string> compare_index)
        {
            Vex_index.Add(startindex);
            for (int i = 0; i < initialList.Count; i++)//Delete all point coordinates (pointed coordinates) that have a connection relationship with the starting point
            {
                for (int j = 0; j < initialList[i].Count; j++)
                {
                    if (initialList[i][j][1] == startindex)
                    { initialList[i].RemoveAt(j); j--; }
                }
            }

            string index_next = predict(initialList[compare_index.IndexOf(startindex)], initialList, compare_index);//
            if (index_next == "None") { startindex = index_next; return false; }//
            int next_edgeCount = initialList[compare_index.IndexOf(index_next)].Count;
            startindex = index_next;
            if (next_edgeCount == 0) { return false; }
            else { return true; }
        }
        public static string predict(List<List<string>> selectlist, List<List<List<string>>> EntireList, List<string> compare_index)
        {
            if (selectlist.Count != 0)
            {
                List<string> index = new List<string>();
                foreach (List<string> code in selectlist) { index.Add(code[1]); }
                //Compare the selected points at the moment, among the various destinations, which group has the smallest number of connections
                index.Sort((p1, p2) => EntireList[compare_index.IndexOf(p1)].Count.CompareTo(EntireList[compare_index.IndexOf(p2)].Count));
                return index[0];//Return the smallest one
            }
            else
            {
                return "None";
            }
        }
        public static void Clean_ve(List<List<List<string>>> list)
        {
            foreach (List<List<string>> sublist in list)
            { if (sublist[0][0] == "ve") { list.Remove(sublist); break; } }
            foreach (List<List<string>> sublist in list)
            {
                for (int i = 0; i < sublist.Count; i++)
                { if (sublist[i][1] == "ve") { sublist.RemoveAt(i); i--; } }
            }
        }
        public static List<List<string[]>> Find_ve(List<List<List<string>>> list)
        {
            //Try to get all the ordered E[vn,ve] in the entire cycle_list, only the edge connect to ve have E[v*,ve];

            List<List<string[]>> Venm = new List<List<string[]>>();
            foreach (List<List<string>> sublist in list)
            { if (sublist[0][0] == "ve") { list.Remove(sublist); break; } }
            foreach (List<List<string>> sublist in list)
            {
                List<string[]> subVe = new List<string[]>();
                List<string[]> temp = new List<string[]>();
                int k = 0; bool st = false;
                while (temp.Count != sublist.Count)//while a cycle start with arbitary E[vn,vm] then the left E[vn,ve] is in right order;
                {
                    if (sublist[k][1] != "ve" || st == true) { temp.Add(new string[] { sublist[k][0], sublist[k][1], sublist[k][2] }); st = true; }
                    k++; if (k > sublist.Count - 1) { k = 0; }
                }
                for (int i = 0; i < temp.Count; i++)
                { if (temp[i][1] == "ve") { subVe.Add(new string[] { temp[i][0], temp[i][1], temp[i][2] }); } }
                if (subVe.Count > 0) { Venm.Add(subVe); }
            }
            return Venm;
        }
        public static List<List<List<string>>> DeepCopyStringList(List<List<List<string>>> list)
        {
            List<List<List<string>>> newlist = new List<List<List<string>>>();
            foreach (List<List<string>> str in list)
            { newlist.Add(CopyStringListList(str)); }
            return newlist;
        }
        public static List<List<string>> CopyStringListList(List<List<string>> list)
        {
            List<List<string>> newlist = new List<List<string>>();
            foreach (List<string> sub in list)
            { newlist.Add(CopyStringList(sub)); }
            return newlist;
        }
        public static List<string> CopyStringList(List<string> list)
        {
            List<string> newlist = new List<string>();
            foreach (string str in list)
            { newlist.Add(str); }
            return newlist;
        }
    }

}
