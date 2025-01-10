using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel.Data;


namespace VGS_Main
{
    public static class Planarization_Methods
    {
        public static void QUAD(bool ConsiderVe, List<List<string>> Vnms, List<List<string>> eNs, List<string[]> MPlanarG, List<string[]> AddBackG, List<List<string[]>> embeded_circle, out DataTree<string> st2d_s, out List<string> st1_v, out List<string> st1_c, out List<string> st1_e, out List<List<List<string>>> nN_infor, out List<string> ve_list)
        {
            //Repack the entered data
            List<List<string>> MaxPlannar_Vnm = MPlanarG.Select(p=>new List<string>() { p[0],p[1]}).ToList();
            List<List<string>> AddBack_Vnm = AddBackG.Select(p => new List<string>() { p[0], p[1] }).ToList();
            List<List<string>> Ori_edgevN = Vnms.Select(p=>new List<string>(p)).ToList();
            List<List<string>> Ori_edgeID = eNs.Select(p => new List<string>(p)).ToList();

            //[Circles]Initializes the embedded cycles of the maximum planar graph
            List<List<List<string>>> circle_order = embeded_circle.Select(p => p.Select(k => new List<string>() { k[0], k[1] }).ToList()).ToList();

            //Duplicate edge detection and Inserting. IO (The output from oldversion is [vn,vm]——>[Q1,Q2])
            List<string[]> ori_Vnm;
            List<string> ori_edgeID;
            List<List<string[]>> circle_vnm_MP;
            IO_Modification.DupliEdge_Modification(Ori_edgevN, Ori_edgeID, circle_order, out ori_Vnm, out ori_edgeID, out circle_vnm_MP);

            List<HalfEdgePlanarize.Vn_circle> Circles = new List<HalfEdgePlanarize.Vn_circle>();
            Circles = circle_vnm_MP.Select(p => new HalfEdgePlanarize.Vn_circle(p)).ToList();
            HalfEdgePlanarize.embeded_data Em = new HalfEdgePlanarize.embeded_data(Circles);//Em: embeded Maxplanar data

            List<string[]> addB_Vnm;//the full names[vn,vm] of the edges that added back to group
            List<string> addB_edgeID;//the edge names[eN] of the edges that added back to group
            List<string[]> Vnm_Mp;
            List<string> edgeID_Mp;
            IO_Modification.AddBack_Modification(AddBack_Vnm, ori_Vnm, ori_edgeID, out addB_Vnm, out addB_edgeID, out Vnm_Mp, out edgeID_Mp);

            //Main__:
            HalfEdgePlanarize.vv_id_dictionary DIC = new HalfEdgePlanarize.vv_id_dictionary(Vnm_Mp, edgeID_Mp);
            List<List<string[]>> HE_groups;

            HalfEdgePlanarize.WallFollow(Em, out HE_groups);

            for (int i = 0; i < addB_Vnm.Count; i++)
            {
                string vFrom = addB_Vnm[i][0];
                string vTo = addB_Vnm[i][1];
                string edgeID = addB_edgeID[i];
                HalfEdgePlanarize.AddBackSingleEdge(ref HE_groups, ref DIC, vFrom, vTo, edgeID, ConsiderVe);
            }

            List<string> Vs;
            List<List<int>> VF;
            List<List<string>> FV;
            List<List<string[]>> EM;
            HalfEdgePlanarize.VertexFaceAdjacency(HE_groups, out Vs, out VF, out FV, out EM);

            List<List<string>> vnms = DIC.all_halfedges.Select(p => new List<string>() { p[0], p[1], p[2] }).ToList();
            List<List<string>> IDs = DIC.edgeIDs.Select(p => new List<string>() { p }).ToList();

            nN_infor = PostManAlgorithm.OrganizeLabels(ref vnms, ref IDs);

            //output infor
            //[circle_orders]
            DataTree<string> circle_order_tree = new DataTree<string>();
            for (int i = 0; i < EM.Count; i++)
            {
                for (int j = 0; j < EM[i].Count; j++)
                {
                    circle_order_tree.AddRange(new List<string>() { EM[i][j][0], EM[i][j][1], EM[i][j][2] }, new GH_Path(i, j));
                }
            }
            List<List<List<string>>> EM_circle = EM.Select(p => p.Select(k => new List<string>() { k[0], k[1], k[2] }).ToList()).ToList();
            List<List<string>> edgeids_circle = EM.Select(p => p.Select(k => edgeIDFromVnm3d(k, vnms, IDs)).ToList()).ToList();
            PostManAlgorithm.ForceDiagramAssemblingOrder(EM_circle, edgeids_circle, EM.Select(p => p[0][0]).ToList(), vnms, IDs, out st2d_s, out st1_v, out st1_c, out st1_e, out ve_list, false);
        }

        public static string edgeIDFromVnm3d(string[] Vnm, List<List<string>> Vnms, List<List<string>> IDs)
        {
            int ind = -1;
            for (int i = 0; i < Vnms.Count; i++)
            {
                if ((Vnms[i][0] == Vnm[0] && Vnms[i][1] == Vnm[1] && Vnms[i][2] == Vnm[2]) || (Vnms[i][1] == Vnm[0] && Vnms[i][0] == Vnm[1] && Vnms[i][2] == Vnm[2]))
                { ind = i; break; }
            }
            if (ind == -1) { return "null"; }
            return IDs[ind][0];
        }

        public static class IO_Modification
        {
            public static string AddBack_Modification(List<List<string>> AddBack_Vnm_ori, List<string[]> ori_Vnm, List<string> ori_edgeID, out List<string[]> addB_Vnm, out List<string> addB_edgeID, out List<string[]> Vnm_Mp, out List<string> edgeID_Mp)
            {
                string Report = "\n[AddBack_Modification]:\n";
                addB_Vnm = new List<string[]>();
                addB_edgeID = new List<string>();
                Vnm_Mp = new List<string[]>();
                edgeID_Mp = new List<string>();

                List<string> LIB = AddBack_Vnm_ori.Select(p => string.Format("[{0},{1}]", p[0], p[1])).ToList();
                LIB.AddRange(AddBack_Vnm_ori.Select(p => string.Format("[{1},{0}]", p[0], p[1])).ToList());

                for (int i = 0; i < ori_Vnm.Count; i++)
                {
                    string[] vnm = ori_Vnm[i];
                    string check = string.Format("[{0},{1}]", vnm[0], vnm[1]);
                    //Report += check + "\n";
                    if (LIB.Contains(check)) { addB_Vnm.Add(ori_Vnm[i]); addB_edgeID.Add(ori_edgeID[i]); }
                    else
                    { Vnm_Mp.Add(ori_Vnm[i]); edgeID_Mp.Add(ori_edgeID[i]); }
                }

                //Report += string.Join("|", LIB.ToArray());
                return Report;
            }

            //Detect the duplication and modify the [vn,vm] to [vn,vm,k];
            public static string DupliEdge_Modification(List<List<string>> all_Vnm, List<List<string>> all_edgeID, List<List<List<string>>> circle_vnm, out List<string[]> all_Vnm_m, out List<string> all_edgeID_m, out List<List<string[]>> circle_vnm_m)
            {
                string Report = "\n[DupliEdge_Modification]:\n";

                all_Vnm_m = new List<string[]>();
                all_edgeID_m = new List<string>();
                circle_vnm_m = circle_vnm.Select(p => p.Select(k => new string[] { k[0], k[1], "0" }).ToList()).ToList();

                List<string> LIB = new List<string>();
                for (int i = 0; i < circle_vnm_m.Count; i++)
                {
                    for (int j = 0; j < circle_vnm_m[i].Count; j++)
                    {
                        string[] vnm = circle_vnm_m[i][j];
                        if (LIB.Contains(string.Format("[{0},{1}]", vnm[0], vnm[1]))) { continue; }
                        LIB.Add(string.Format("[{0},{1}]", vnm[0], vnm[1])); LIB.Add(string.Format("[{0},{1}]", vnm[1], vnm[0]));

                        List<string> edgeIDs = CheckMulti(vnm, all_Vnm, all_edgeID);

                        if (edgeIDs.Count > 1)
                        {
                            Report += string.Join("|", edgeIDs.ToArray()) + "\n";
                            List<string[]> M_vnms = MultiPvnm_halfedge(vnm, edgeIDs.Count);
                            Report += edgeIDs.Count.ToString() + string.Join(" ", M_vnms.Select(p => string.Format("[{0},{1},{2}]", p[0], p[1], p[2]))) + "\n";
                            circle_vnm_m[i].RemoveAt(j); circle_vnm_m[i].InsertRange(j, M_vnms); j += M_vnms.Count - 1;
                            //
                            List<string[]> re_M_vnms = M_vnms.Select(p => new string[] { p[1], p[0], p[2] }).ToList(); re_M_vnms.Reverse();
                            int[] ind = FindindexOfReverse(circle_vnm_m, vnm);
                            circle_vnm_m[ind[0]].RemoveAt(ind[1]); circle_vnm_m[ind[0]].InsertRange(ind[1], re_M_vnms);
                        }
                    }
                }

                for (int i = 0; i < all_Vnm.Count; i++)
                {
                    string[] vnm = new string[] { all_Vnm[i][0], all_Vnm[i][1], "0" };
                    List<string> edgeIDs = CheckMulti(vnm, all_Vnm, all_edgeID);
                    if (edgeIDs.Count > 1)
                    {
                        List<string[]> M_vnms = MultiPvnm_halfedge(vnm, edgeIDs.Count);
                        all_Vnm_m.AddRange(M_vnms); all_edgeID_m.AddRange(edgeIDs);
                    }
                    else
                    { all_Vnm_m.Add(vnm); all_edgeID_m.Add(edgeIDs[0]); }
                }

                //Report_st
                for (int i = 0; i < all_Vnm_m.Count; i++)
                {
                    string vnm_n = string.Format("[{0},{1},{2}]", all_Vnm_m[i][0], all_Vnm_m[i][1], all_Vnm_m[i][2]);
                    Report += string.Format("{0}:{1}", all_edgeID_m[i], vnm_n) + "\n";
                }

                Report += string.Join("\n", circle_vnm_m.Select(p => string.Join("", p.Select(k => string.Format("[{0},{1},{2}]", k[0], k[1], k[2]))))) + "\n";
                //Report_ed
                return Report;

            }

            //Use a vnm as a clue to find the index of the opposite side in the other ring
            public static int[] FindindexOfReverse(List<List<string[]>> circle_vnm_m, string[] vnm_here)
            {
                int[] index = new int[] { };
                for (int i = 0; i < circle_vnm_m.Count; i++)
                {
                    if (circle_vnm_m[i][0][0] != vnm_here[1]) { continue; }
                    for (int j = 0; j < circle_vnm_m[i].Count; j++)
                    {
                        if (circle_vnm_m[i][j][1] == vnm_here[0]) { index = new int[] { i, j }; break; }
                    }
                }
                return index;
            }
            //Copy an edge according to the number of mounts
            private static List<string[]> MultiPvnm_halfedge(string[] vnm, int mount) //make a single vnm to duplicate vnms
            {
                List<string[]> M_halfedges = new List<string[]>();
                for (int i = 0; i < mount; i++)
                {
                    M_halfedges.Add(new string[] { vnm[0], vnm[1], i.ToString() });
                }
                return M_halfedges;
            }
            //Check the duplication of a vnm in a directory
            private static List<string> CheckMulti(string[] vnm, List<List<string>> all_Vnm, List<List<string>> all_edgeID)
            {
                int ind = -1;
                for (int i = 0; i < all_Vnm.Count; i++)
                {
                    if ((all_Vnm[i][0] == vnm[0] && all_Vnm[i][1] == vnm[1]) || (all_Vnm[i][1] == vnm[0] && all_Vnm[i][0] == vnm[1])) { ind = i; break; }
                }
                if (ind == -1) { return new List<string>(); }
                return all_edgeID[ind];
            }
        }
    }
}
