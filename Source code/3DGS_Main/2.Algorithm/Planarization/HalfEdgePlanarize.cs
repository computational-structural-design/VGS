using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGS_Main
{
    public static class HalfEdgePlanarize
    {
        /// <summary>
        /// authors: Shen, Yuchi
        /// </summary>
        //_Parameters_
        //public List<string[]>maxP_edge;//The edges of the maximum planar graph
        //public List<string[]>addB_edge;//The left-over edges to be added back to the graph
        //public embeded_data Em;//One embeded setting of the graph（initial import）

        //_structures_
        public struct vv_id_dictionary
        {
            public List<string[]> all_halfedges;
            public List<string> edgeIDs;

            public vv_id_dictionary(List<string[]> hes, List<string> ids)
            {
                all_halfedges = hes.Select(p => (string[])p.Clone()).ToList();
                edgeIDs = new List<string>(ids);
            }

            public string getID(string[] halfedge)
            {
                string ID = "null";
                for (int i = 0; i < all_halfedges.Count; i++)
                {
                    if (halfedge[0] == all_halfedges[i][0] && halfedge[1] == all_halfedges[i][1] && halfedge[2] == all_halfedges[i][2])
                    { ID = edgeIDs[i]; break; }
                    else if (halfedge[0] == all_halfedges[i][1] && halfedge[1] == all_halfedges[i][0] && halfedge[2] == all_halfedges[i][2])
                    { ID = edgeIDs[i]; break; }
                }
                return ID;
            }

            public int getIndex(string[] halfedge)
            {
                int ID = -1;
                for (int i = 0; i < all_halfedges.Count; i++)
                {
                    if (halfedge[0] == all_halfedges[i][0] && halfedge[1] == all_halfedges[i][1] && halfedge[2] == all_halfedges[i][2])
                    { ID = i; break; }
                    else if (halfedge[0] == all_halfedges[i][1] && halfedge[1] == all_halfedges[i][0] && halfedge[2] == all_halfedges[i][2])
                    { ID = i; break; }
                }
                return ID;
            }

            public void update(string[] halfedge, List<string[]> new_halfedges)
            {
                int index = getIndex(halfedge);
                if (index == -1) { return; }
                string id = edgeIDs[index];
                if (new_halfedges.Count > 0)
                {
                    all_halfedges.RemoveAt(index);
                    edgeIDs.RemoveAt(index);
                }
                foreach (string[] nh in new_halfedges)
                {
                    id += "+";
                    all_halfedges.Add(nh);
                    edgeIDs.Add(id);
                }
            }

            public string report()
            {
                string Report = "\n[DIC]:";
                for (int i = 0; i < all_halfedges.Count; i++)
                {
                    Report += string.Format("\n{0}: {1}", edgeIDs[i], string.Format("[{0},{1},{2}]", all_halfedges[i][0], all_halfedges[i][1], all_halfedges[i][2]));
                }
                return Report;
            }
        }//Comparison table of edge ID and half edge data
        public struct embeded_data
        {
            public List<Vn_circle> AllVnCircles;

            public embeded_data(List<Vn_circle> AllVnCircles)
            { this.AllVnCircles = AllVnCircles; }

            public string PrintSelf()
            {
                string output = "";
                foreach (Vn_circle circle in this.AllVnCircles)
                {
                    string temp = circle.Vn + ":";
                    foreach (string[] st in circle.clock_list)
                    {
                        temp += " " + st[1] + "_" + st[2];
                    }
                    temp += "\n"; output += temp;
                }
                return output;
            }//Output the information of the structure

            public bool IsEmpty()
            {
                bool empty = true;
                foreach (Vn_circle vn_circle in AllVnCircles)
                {
                    empty = vn_circle.IsEmpty();
                }
                return empty;
            }//Determine whether there are nodes without adjacent edges in this plane embedding

            public int IndexOf(string str) //Enter the ID of the node, and return the index of the node in the plane embedding
            {
                for (int i = 0; i < AllVnCircles.Count; i++) { if (AllVnCircles[i].Vn == str) { return i; } }
                return -1;
            }

        }//Plane embedding structure of graph
        public struct Vn_circle
        {
            public List<string[]> clock_list;//[vn,vm,id]Sequence array #Need to give the edge array id in the input information at the beginning to deal with the situation of repeated edges

            public string Vn { get { return clock_list[0][0]; } }//Returns the name of the node

            public string[] Ahead(int n) //The id of the previous vector ([vm,id])
            {
                int i = n - 1;
                if (i == -1) { i = clock_list.Count - 1; }
                return new string[] { clock_list[i][1], clock_list[i][2] };
            }

            public int IndexOf(string[] str) //Return its index in circle according to [vn,vm,id]
            {
                for (int i = 0; i < clock_list.Count; i++)
                { if (str[0] == clock_list[i][0] && str[1] == clock_list[i][1] && str[2] == clock_list[i][2]) { return i; } }
                return -1;
            }

            public bool IsEmpty()
            {
                if (clock_list.Count == 0) { return true; } else { return false; }
            }

            public Vn_circle(List<string[]> clock_list)
            { this.clock_list = clock_list; }
        }//Construction of a single vector polygon
        public struct Adj_HF
        {
            public List<string[]> halfedges_adj;//The adjoining half of one face relative to the other face
            public Adj_HF(List<string[]> halfedges)
            {
                halfedges_adj = halfedges.Select(p => (string[])p.Clone()).ToList();
            }
            public bool isNone()
            {
                if (halfedges_adj.Count == 0) { return true; }
                return false;
            }
            public bool isMulti()
            {
                if (halfedges_adj.Count > 1) { return true; }
                return false;
            }
            public string NumState
            {
                get { return halfedges_adj.Count.ToString(); }
            }
            public string FullState
            {
                get
                {
                    string fullnames = "";
                    foreach (string[] id in halfedges_adj) { fullnames += string.Format("[{0},{1},{2}]", id[0], id[1], id[2]); }
                    return fullnames;
                }
            }
        }//Adjacent edge set information of face and face

        //_Functions_
        //01_Wall-around algorithm: get half-edge structure data
        public static string WallFollow(embeded_data Em, out List<List<string[]>> halfedge_groups)
        {
            halfedge_groups = new List<List<string[]>>();
            List<string> lib = new List<string>();//Record all categorized halves

            string Report = "[WallFollow]:\n";
            for (int i = 0; i < Em.AllVnCircles.Count; i++)//For all nodes
            {
                string start = Em.AllVnCircles[i].Vn;//Starting node name

                for (int j = 0; j < Em.AllVnCircles[i].clock_list.Count; j++)//For radial edges in nodes
                {
                    string check = start;

                    string[] halfedge_st = Em.AllVnCircles[i].clock_list[j];//Full name of the starting half
                    string vm = halfedge_st[1];//The name of the next node pointed to by the start half
                    string halfedge_st_str = string.Format("[{0},{1},{2}]", halfedge_st[0], halfedge_st[1], halfedge_st[2]);//Characterization of the first half of the full name
                    if (lib.Contains(halfedge_st_str)) { continue; }//If [vn,vm,id] already exists in lib, skip

                    List<string[]> halfedges = new List<string[]>() { halfedge_st };//The set of all halves in the region

                    Report += "[CHECK]: " + check + ":\n";

                    string[] next;//[Vm,id] of the next node
                    string[] checkFromEdge = new string[] { halfedge_st[1], halfedge_st[0], halfedge_st[2] };//The full name of the vector used to determine the sequence number of the incoming edge in the next node
                    bool keep_iteration = true;//Used to determine whether to continue searching for tree edges
                    List<string> sub_lib = new List<string>() { string.Format("[{0},{1},{2}]", halfedge_st[0], halfedge_st[1], halfedge_st[2]) };//Record the half of the area that has been collected

                    do
                    {
                        int target = Em.AllVnCircles[Em.IndexOf(vm)].IndexOf(checkFromEdge);//Coming serial number
                        if (target == -1) { Report += string.Format("error: no target found [{0}]\n", string.Format("[{0},{1},{2}]", checkFromEdge[0], checkFromEdge[1], checkFromEdge[2])); break; }

                        next = Em.AllVnCircles[Em.IndexOf(vm)].Ahead(target);//The leading bit of the sequence number of the incoming edge: [vm,id] of the next node
                        string[] halfedge = new string[3] { vm, next[0], next[1] };//Full name of the next half
                        halfedges.Add(halfedge);//Add the searched half edge to the half edge set of the region
                        string halfedge_str = string.Format("[{0},{1},{2}]", halfedge[0], halfedge[1], halfedge[2]);//Characterization of the full name of the next half
                        lib.Add(halfedge_str);
                        sub_lib.Add(halfedge_str);

                        checkFromEdge = new string[] { halfedge[1], halfedge[0], halfedge[2] };//update
                        vm = next[0];//update next node

                        //It is the same as the above process, but it is used to predict whether the tree edge situation is encountered. If it is the tree edge situation, it needs to continue to loop until the predicted [next preposition edge] is already concentrated in the area.
                        int target_pre = Em.AllVnCircles[Em.IndexOf(vm)].IndexOf(checkFromEdge);
                        if (target_pre == -1) { Report += string.Format("error: no target_pre found [{0}]\n", string.Format("[{0},{1},{2}]", checkFromEdge[0], checkFromEdge[1], checkFromEdge[2])); break; }
                        string[] next_pre = Em.AllVnCircles[Em.IndexOf(vm)].Ahead(target_pre);
                        string[] halfedge_pre = new string[3] { vm, next_pre[0], next_pre[1] };
                        string halfedge_pre_str = string.Format("[{0},{1},{2}]", halfedge_pre[0], halfedge_pre[1], halfedge_pre[2]);
                        if (sub_lib.Contains(halfedge_pre_str)) { keep_iteration = false; }
                    }
                    while (next[0] != start || keep_iteration == true);

                    halfedge_groups.Add(halfedges);
                    foreach (string[] e in halfedges) { string hedge = string.Format("[{0},{1},{2}]", e[0], e[1], e[2]); Report += hedge; }//
                    Report += "\n";
                }
            }
            return Report;
        }
        //The halfedge_groups obtained by the default wall-around algorithm is the default area order

        //02_FaceAdjacency
        public static string FaceAdjacency(List<List<string[]>> halfedge_groups, out List<List<Adj_HF>> FF)
        {
            string Report = "\n";
            //backup halfedge_d3(dimension 3)
            List<List<string[]>> halfedge_d3 = halfedge_groups.Select(p => p.Select(k => (string[])k.Clone()).ToList()).ToList();//Clone is just a shallow copy
                                                                                                                                 //Name the adjacency matrix of all regions
            List<List<string>> Face_adjMatrix = halfedge_groups.Select(p => halfedge_groups.Select(k => "-").ToList()).ToList();//Just a matrix of adjacency relations
            List<List<Adj_HF>> Face_adjMatrix_detail = halfedge_groups.Select(p => halfedge_groups.Select(k => new Adj_HF(new List<string[]>() { })).ToList()).ToList();//Matrix containing adjacent edge information
                                                                                                                                                                        //Check adjacency
            for (int i = 0; i < halfedge_d3.Count; i++)
            {
                for (int j = 0; j < halfedge_d3[i].Count; j++)
                {
                    string[] check_edge = halfedge_d3[i][j];
                    //Self-check (whether there is an opposite side inside the inner loop)
                    bool selfcheck = false;
                    if (j != halfedge_d3[i].Count - 1)
                    {
                        for (int k = j + 1; k < halfedge_d3[i].Count; k++)
                        {
                            string[] check_edge_self = halfedge_d3[i][k];
                            if (check_edge[0] == check_edge_self[1] && check_edge[1] == check_edge_self[0] && check_edge[2] == check_edge_self[2])
                            {
                                halfedge_d3[i].RemoveAt(k); halfedge_d3[i].RemoveAt(j);
                                j--; selfcheck = true; break;
                            }
                        }
                    }
                    //Query oppsite edges in other regions
                    if (!selfcheck)
                    {
                        for (int k = 0; k < halfedge_d3.Count; k++)
                        {
                            if (k == i) { continue; }
                            for (int p = 0; p < halfedge_d3[k].Count; p++)
                            {
                                string[] check_edge_oth = halfedge_d3[k][p];
                                if (check_edge[0] == check_edge_oth[1] && check_edge[1] == check_edge_oth[0] && check_edge[2] == check_edge_oth[2])//find oppsite edges
                                {
                                    Face_adjMatrix[i][k] = "1"; Face_adjMatrix[k][i] = "1";//Assign a value to the adjacency matrix after finding the opposite edge
                                    Face_adjMatrix_detail[i][k].halfedges_adj.Add((string[])halfedge_d3[i][j].Clone()); Face_adjMatrix_detail[k][i].halfedges_adj.Add((string[])halfedge_d3[k][p].Clone());//Assign a value to the adjacency matrix after finding the opposite edge (Detail)
                                    halfedge_d3[k].RemoveAt(p); halfedge_d3[i].RemoveAt(j);
                                    j--; break;
                                }
                            }
                        }
                    }

                }
            }

            //Report += "[Face Adjacency Matrix]:\n";
            //foreach(string str in Face_adjMatrix.Select(p => string.Join("  ", p.ToArray())).ToList()){Report += str + "\n";}
            //Report += "[Face Adjacency Matrix (Num)]:\n";
            //foreach(string str in Face_adjMatrix_detail.Select(p => string.Join("  ", p.Select(k => k.NumState))).ToList()){Report += str + "\n";}

            FF = Face_adjMatrix_detail;
            return Report;
            //
        }

        //03_Adjacency relationship between point and area
        public static string VertexFaceAdjacency(List<List<string[]>> halfedge_groups, out List<string> Vs, out List<List<int>> VF, out List<List<string>> FV, out List<List<string[]>> EM)
        {
            //_Method objective_For any point sequence Vn~Vm, the adjacency relationship and sequence between each node and the area
            string Report = "\n";

            //Sort out all nodes Vn~Vm from the area information
            List<string> lib = new List<string>();//Store the recorded points
            for (int i = 0; i < halfedge_groups.Count; i++)
            {
                for (int j = 0; j < halfedge_groups[i].Count; j++)
                {
                    string vn = halfedge_groups[i][j][0];
                    if (!lib.Contains(vn)) { lib.Add(vn); }
                }
            }
            List<string> Vns = new List<string>(lib);//[All Vertices]

            //Find vertices for all regions
            List<List<string>> halfedge_groups_vns = new List<List<string>>();//[Face|Vertices]
            for (int i = 0; i < halfedge_groups.Count; i++)
            {
                List<string> sub = halfedge_groups[i].Select(p => p[0]).ToList();
                List<string> sub1 = sub.Where((x, y) => sub.FindIndex(z => z == x) == y).ToList();//Deduplicated expression
                halfedge_groups_vns.Add(sub1);
            }

            // Find the index of all halfedge regions adjacent to all nodes Vn~Vm
            List<List<int>> VertexFace_matchInd = new List<List<int>>();//[Vertices|Face]
            for (int i = 0; i < Vns.Count; i++)
            {
                List<int> sub = new List<int>();
                for (int j = 0; j < halfedge_groups.Count; j++) { if (halfedge_groups_vns[j].Contains(Vns[i])) { sub.Add(j); } }
                VertexFace_matchInd.Add(sub);
            }


            //Find the full names of the edges in clockwise order
            List<List<string[]>> Vertex_Circle = new List<List<string[]>>();//[Vertices|Edges in clock order]
                                                                            //Find the fullnames of the outgoing and incoming edges in all regions, and then arrange them end to end
            for (int i = 0; i < VertexFace_matchInd.Count; i++)//
            {
                List<int> adjFace_Ind = VertexFace_matchInd[i];//For each node's adjacency area indlist

                List<List<string[]>> all_AdjEdges = new List<List<string[]>>();//All pairs of outgoing and incoming edges

                for (int k = 0; k < adjFace_Ind.Count; k++)//Find all outgoing and incoming edges related to the node
                {
                    List<string[]> fullnames = halfedge_groups[adjFace_Ind[k]];//All halves in the adjacent region

                    for (int w = 0; w < fullnames.Count; w++)
                    {
                        if (fullnames[w][0] == Vns[i])//If the starting node of the edge is the same as the node
                        {
                            string[] out_edge = fullnames[w];//Full name of the outgoing side
                            int x = w - 1; if (x == -1) { x = fullnames.Count - 1; }
                            string[] in_edge = fullnames[x];//Incoming full name
                            all_AdjEdges.Add(new List<string[]>() { out_edge, in_edge });
                        }
                    }
                }

                List<List<string[]>> ordered = new List<List<string[]>>();//End-to-end sequence of all outgoing edges and incoming edge groups

                List<string[]> check = all_AdjEdges[0];//Any starting group

                while (ordered.Count != all_AdjEdges.Count)
                {
                    ordered.Add(check);

                    string[] In_edge = check[1];

                    foreach (List<string[]> pair in all_AdjEdges)
                    {
                        string[] Out_edge = pair[0];
                        if (Out_edge[0] == In_edge[1] && Out_edge[1] == In_edge[0] && Out_edge[2] == In_edge[2])
                        {
                            check = pair; break;
                        }
                    }
                }

                List<string[]> ordered_adj_halfedges = ordered.Select(p => p[0]).ToList();
                Vertex_Circle.Add(ordered_adj_halfedges);
            }

            //Report += "[All Vertices]:\n" + string.Join(" ", Vns.ToArray());
            //Report += "\n";
            //Report += "[Vertices|Face]:\n" + string.Join("\n", VertexFace_matchInd.Select(p => string.Join(" ", p.Select(k => k.ToString()))).ToArray());
            //Report += "\n";
            //Report += "[Face|Vertices]:\n" + string.Join("\n", halfedge_groups_vns.Select(p => string.Join(" ", p.ToArray())).ToArray());
            //Report += "\n";
            //Report += "[Vertices|Edges in clock order]:\n" + string.Join("\n", Vertex_Circle.Select(p => string.Join(" ", p.Select(k => string.Format("[{0},{1},{2}]", k[0], k[1], k[2])))));

            Vs = Vns;
            VF = VertexFace_matchInd;
            FV = halfedge_groups_vns;
            EM = Vertex_Circle;
            return Report;
        }

        //The action of cutting half of the region
        //FaceIndex：The number of the region to be operated
        //Divide：Segmentation information {[vn,vn],[vm,vm]} represents point to point; {[vn,vn],[vx,vy]} represents point to edge; {[vn,vm], [vx,vy]} represents edge to edge [vn vm must be entered in the order of half edges!]
        public static string SubdivideFace(List<List<string[]>> halfedge_groups, ref vv_id_dictionary DIC, int FaceIndex, List<string[]> Divide, bool herit, string insert_edgeID, out List<List<string[]>> new_circles)
        {
            string Report = "\n[Subdivide a halfedge circle]:\n";

            //Sort out all nodes Vn~Vm from the area information, and then mark the name of the new node according to the largest value that appears
            List<string> lib = new List<string>();//Store the recorded points
            for (int i = 0; i < halfedge_groups.Count; i++)
            {
                for (int j = 0; j < halfedge_groups[i].Count; j++)
                {
                    string vn = halfedge_groups[i][j][0];
                    if (!lib.Contains(vn)) { lib.Add(vn); }
                }
            }
            List<string> all_Vns = new List<string>(lib);//[All Vertices]
            List<string> all_nums = all_Vns.Where(p => p.Substring(1) != "e").ToList();
            List<int> ids = all_nums.Select(p => System.Convert.ToInt32(p.Substring(1))).ToList(); ids.Sort(); ids.Reverse();
            //List<int> ids = all_Vns.Select(p => System.Convert.ToInt32(p.Substring(1))).ToList();ids.Sort();ids.Reverse();
            int N = ids[0] + 1;//Base value of node
            if (herit) { N--; }//If inherited, then the value of N starts from the last auxiliary point nN that was cut, and N is the maximum value in halfedge_groups;

            //Back up all half-edge information halfedge_d3 (dimension 3)
            List<List<string[]>> halfedge_d3 = halfedge_groups.Select(p => p.Select(k => (string[])k.Clone()).ToList()).ToList();//Clone is just a shallow copy

            List<string[]> object_halfedges = halfedge_d3[FaceIndex].Select(p => (string[])p.Clone()).ToList();//Target area information

            List<string> Vns0 = object_halfedges.Select(p => p[0]).ToList();//Node order of the target area

            //If [vn,vn]: the sequence number of vn starts and the sequence number -1 of vn is the end
            //If [vn, vm]: it is the sequence number of vn starting and vn sequence number -1 as the end (the beginning and ending points of the two divided rings are the same)
            string[] start = Divide[0];
            string[] end = Divide[1];

            int start_ind = Vns0.IndexOf(start[0]);
            if (start[0] != start[1]) { start_ind = object_halfedges.IndexOf(object_halfedges.Where(p => p[0] == start[0] && p[1] == start[1]).ToList()[0]); }

            //Reorder according to starting point
            List<string[]> front_list = object_halfedges.Where(p => object_halfedges.IndexOf(p) >= start_ind).ToList();
            List<string[]> back_list = object_halfedges.Where(p => object_halfedges.IndexOf(p) < start_ind).ToList();
            List<string[]> reordered_list = new List<string[]>(); reordered_list.AddRange(front_list); reordered_list.AddRange(back_list);
            List<string> Vns = reordered_list.Select(p => p[0]).ToList();//According to the reordering order

            int end_ind = Vns.IndexOf(end[0]);
            if (end[0] != end[1]) { end_ind = reordered_list.IndexOf(reordered_list.Where(p => p[0] == end[0] && p[1] == end[1]).ToList()[0]); }
            List<string[]> sub1 = reordered_list.Where(p => reordered_list.IndexOf(p) > 0 && reordered_list.IndexOf(p) < end_ind).ToList();//0-----1(0 and 1 are the starting and ending vectors for removal）
            List<string[]> sub2 = reordered_list.Where(p => reordered_list.IndexOf(p) > end_ind).ToList();//1----0（-----是sub）
                                                                                                          //According to the characteristics of end-to-end segmentation, add back edges

            //Divide the start and end vectors according to the characteristics of Divide, and insert sub1 and sub2
            List<string[]> cut_start = new List<string[]>();
            List<string[]> cut_end = new List<string[]>();
            List<string[]> test_all = new List<string[]>();//When solving the case of the adjacent edge, repeat the identification problem of the half edge [v2—>v2v4]
            if (start[0] == start[1])
            {
                if (start[0] != end[0]) { sub1.Insert(0, reordered_list[0]); }
            }
            else if (start[0] != start[1])
            {
                string[] hedge0 = reordered_list[0];
                sub1.Insert(0, new string[] { "n" + N.ToString(), hedge0[1], hedge0[2] }); test_all.Add(new string[] { hedge0[1], "n" + N.ToString(), hedge0[2] });
                sub2.Add(new string[] { hedge0[0], "n" + N.ToString(), hedge0[2] }); test_all.Add(new string[] { "n" + N.ToString(), hedge0[0], hedge0[2] });
                cut_start.Add(sub2[sub2.Count - 1]);
                cut_start.Add(sub1[0]);
                DIC.update(hedge0, cut_start);
                N++;
            }

            if (end[0] == end[1])
            {
                sub2.Insert(0, reordered_list[end_ind]);
            }
            else if (end[0] != end[1])
            {
                string[] hedge1 = reordered_list[end_ind];
                sub1.Add(new string[] { hedge1[0], "n" + N.ToString(), hedge1[2] }); test_all.Add(new string[] { "n" + N.ToString(), hedge1[0], hedge1[2] });
                sub2.Insert(0, new string[] { "n" + N.ToString(), hedge1[1], hedge1[2] }); test_all.Add(new string[] { hedge1[1], "n" + N.ToString(), hedge1[2] });
                cut_end.Add(sub1[sub1.Count - 1]);
                cut_end.Add(sub2[0]);
                DIC.update(hedge1, cut_end);
            }

            //Here you need to check the id of all edges that are the same as this half of vn vm to ensure that the added edge is not confused with other edges
            List<List<string[]>> halfedge_test = halfedge_groups.Select(p => p.Select(k => (string[])k.Clone()).ToList()).ToList();//Clone is just a shallow copy
            halfedge_test.RemoveAt(FaceIndex); halfedge_test.Add(sub1); halfedge_test.Add(sub2); halfedge_test.Add(test_all);

            string s1_0 = sub1[sub1.Count - 1][1]; string s1_1 = sub1[0][0];
            string s2_0 = sub2[sub2.Count - 1][1]; string s2_1 = sub2[0][0];
            List<int> s1 = new List<int>();
            List<int> s2 = new List<int>();
            foreach (List<string[]> halfedge in halfedge_test)
            {
                foreach (string[] id in halfedge)
                {
                    if (id[0] == s1_0 && id[1] == s1_1) { s1.Add(System.Convert.ToInt32(id[2])); }
                    if (id[0] == s2_0 && id[1] == s2_1) { s2.Add(System.Convert.ToInt32(id[2])); }
                }
            }
            s1.Sort(); s2.Sort(); s1.Reverse(); s2.Reverse();
            int check_sub1 = 0; if (s1.Count > 0) { check_sub1 = s1[0] + 1; }
            int check_sub2 = 0; if (s2.Count > 0) { check_sub2 = s2[0] + 1; }//According to all the half sides of the same vnm that are queried, the id serial number increases

            //The ends of sub1 and sub2 are the newly added half edges (only the newly added half edge may overlap with the previous edge)
            sub1.Add(new string[] { sub1[sub1.Count - 1][1], sub1[0][0], check_sub1.ToString() });
            sub2.Add(new string[] { sub2[sub2.Count - 1][1], sub2[0][0], check_sub2.ToString() });

            List<string[]> new_hedge_pair = new List<string[]>() { sub1[sub1.Count - 1], sub2[sub2.Count - 1] };

            DIC.all_halfedges.Add(new_hedge_pair[0]); DIC.edgeIDs.Add(insert_edgeID + "+");

            new_circles = new List<List<string[]>>();
            new_circles.Add(sub1); new_circles.Add(sub2);
            //export
            Report += "Cut Start: " + string.Join(" ", cut_start.Select(p => string.Format("[{0},{1},{2}]", p[0], p[1], p[2]))) + "\n";
            Report += "Cut End:  " + string.Join(" ", cut_end.Select(p => string.Format("[{0},{1},{2}]", p[0], p[1], p[2]))) + "\n";
            Report += "New half-edges:  " + string.Join(" ", new_hedge_pair.Select(p => string.Format("[{0},{1},{2}]", p[0], p[1], p[2]))) + "\n";
            Report += string.Join(" ", sub1.Select(p => string.Format("[{0},{1},{2}]", p[0], p[1], p[2]))) + "\n";
            Report += string.Join(" ", sub2.Select(p => string.Format("[{0},{1},{2}]", p[0], p[1], p[2]))) + "\n";
            return Report;
        }

        //Find Path
        public static string FindPath_VFV(bool consider_Ve, List<List<string[]>> halfedge_groups, List<List<Adj_HF>> FF, List<string> Vs, List<List<int>> VF, string Vst, string Ved, out List<List<string[]>> ST, out List<int> PT)
        {
            string Report = "\n" + "\n" + string.Format("[FindPath]: From {0} to {1}", Vst, Ved);
            //FF can be preprocessed according to the situation of ve (delete the half-edge connection related to ve)（Delete the half-edge connection related to ve）***
            if (consider_Ve)
            {
                for (int i = 0; i < FF.Count; i++)
                {
                    for (int j = 0; j < FF[i].Count; j++)
                    {
                        List<string[]> h_e = FF[i][j].halfedges_adj;
                        for (int k = 0; k < h_e.Count; k++)
                        {
                            if (h_e[k][0] == "ve" || h_e[k][1] == "ve") { FF[i][j].halfedges_adj.RemoveAt(k); k--; }
                        }
                    }
                }
            }
            //Establish a matrix of adjacent edge numbers (non-square) for each area through FF
            List<List<int>> JointMatrix = new List<List<int>>();
            foreach (List<Adj_HF> adjs in FF)
            {
                List<int> sub = new List<int>();
                for (int i = 0; i < adjs.Count; i++)
                {
                    if (!adjs[i].isNone()) { sub.Add(i); }
                }
                JointMatrix.Add(sub);
            }
            //Area number corresponding to each pre-connected node
            List<int> adjF_st = VF[Vs.IndexOf(Vst)];
            List<int> adjF_ed = VF[Vs.IndexOf(Ved)];
            //Reference the shortest path method library
            ShortPathGraph SolvePath = new ShortPathGraph();
            SolvePath.JointMetrix = JointMatrix.Select(p => new List<int>(p)).ToList(); SolvePath.NoneRights();

            //The shortest possibility of the connection between those areas (may make the calculation slow)
            int st_ind = int.MaxValue;
            int ed_ind = int.MaxValue;
            int step = int.MaxValue;
            for (int i = 0; i < adjF_st.Count; i++)
            {
                for (int j = 0; j < adjF_ed.Count; j++)
                {
                    int st = adjF_st[i]; int ed = adjF_ed[j];
                    List<int> SPath = SolvePath.Dijkstra(st, ed);
                    if (SPath.Count > 0 && SPath.Count < step) { st_ind = st; ed_ind = ed; step = SPath.Count; }//【BUG】SPath.Count >= 0;The coplanar situation is not considered
                    else if (SPath.Count < 0) { Report += "\n[ERROR]: " + string.Format("Face{0} and Face{1} are not connected", i.ToString(), j.ToString()); }
                }
            }
            List<int> SPath_opti = SolvePath.Dijkstra(st_ind, ed_ind);//Best path strategy

            //According to the best path strategy of the face, the operation strategy for each face is generated
            List<List<string[]>> div_strategy = new List<List<string[]>>();//[Divide Strategy]
            string[] From_halfedge = new string[] { "-", "-", "-" };
            for (int i = 0; i < SPath_opti.Count; i++)//Coplanar situation
            {

                if (SPath_opti.Count == 1) { div_strategy.Add(new List<string[]>() { new string[] { Vst, Vst }, new string[] { Ved, Ved } }); break; }

                if (i == 0)//The first strategy
                {
                    int F0ind = SPath_opti[i]; int F1ind = SPath_opti[i + 1];
                    int indexFF = FF[F0ind][F1ind].halfedges_adj.Count; indexFF = (int)System.Math.Round(((double)indexFF - 0.1) / 2);//When two faces have multiple boundaries, try to take the middle
                    string[] Cut_halfedge = FF[F0ind][F1ind].halfedges_adj[indexFF];

                    List<string[]> div_sub = new List<string[]>() { new string[] { Vst, Vst }, new string[] { Cut_halfedge[0], Cut_halfedge[1] } };//Point-to-surface strategy
                    div_strategy.Add(div_sub);

                    From_halfedge = new string[] { Cut_halfedge[1], Cut_halfedge[0], Cut_halfedge[2] };//update From_halfedge
                }

                if (i > 0 && i < SPath_opti.Count - 1)//Face-to-face strategy
                {
                    int F0ind = SPath_opti[i]; int F1ind = SPath_opti[i + 1];
                    int indexFF = FF[F0ind][F1ind].halfedges_adj.Count; indexFF = (int)System.Math.Round(((double)indexFF - 0.1) / 2);//When two faces have multiple boundaries, try to take the middle
                    string[] Cut_halfedge = FF[F0ind][F1ind].halfedges_adj[indexFF];

                    List<string[]> div_sub = new List<string[]>() { new string[] { From_halfedge[0], From_halfedge[1] }, new string[] { Cut_halfedge[0], Cut_halfedge[1] } };
                    div_strategy.Add(div_sub);

                    From_halfedge = new string[] { Cut_halfedge[1], Cut_halfedge[0], Cut_halfedge[2] };//update From_halfedge
                }

                if (i == SPath_opti.Count - 1 && i != 0)//The last stategy
                {
                    List<string[]> div_sub = new List<string[]>() { new string[] { From_halfedge[0], From_halfedge[1] }, new string[] { Ved, Ved } };
                    div_strategy.Add(div_sub);
                }

            }

            ST = div_strategy;
            PT = SPath_opti;
            Report += "\n[Divide Strategy]:" + string.Join("—>", div_strategy.Select(p => string.Format("[{0},{1}][{2},{3}]", p[0][0], p[0][1], p[1][0], p[1][1]))) + ";";
            Report += "\n[StartF]:" + st_ind.ToString() + "[EndF]:" + ed_ind.ToString() + ";";
            Report += "\n[The Best Shortest Path Found]:" + string.Join("—>", SPath_opti.Select(p => p.ToString())) + ";\n";
            return Report;
        }

        //Add back a single edge loop
        public static string AddBackSingleEdge(ref List<List<string[]>> HE_groups, ref vv_id_dictionary DIC, string Vst, string Ved, string edgeID, bool ConsiderVe)
        {
            string Report = "";

            List<List<Adj_HF>> FF;
            Report += FaceAdjacency(HE_groups, out FF);

            List<string> Vs;
            List<List<int>> VF;
            List<List<string>> FV;
            List<List<string[]>> EM;
            Report += VertexFaceAdjacency(HE_groups, out Vs, out VF, out FV, out EM);

            List<List<string[]>> ST;//strategies of divide
            List<int> PT;//face indexs
            Report += FindPath_VFV(ConsiderVe, HE_groups, FF, Vs, VF, Vst, Ved, out ST, out PT);

            //ITERATION
            for (int i = 0; i < ST.Count; i++)
            {
                List<string[]> Divide = ST[i];
                int faceindex = PT[i];
                bool herit = false; if (i > 0) { herit = true; }
                List<List<string[]>> new_groups;
                Report += SubdivideFace(HE_groups, ref DIC, faceindex, Divide, herit, edgeID, out new_groups);
                HE_groups.AddRange(new_groups);
            }

            //Remove the subdivided halfedge circles
            List<List<string[]>> HE_groups_out = new List<List<string[]>>();

            for (int i = 0; i < HE_groups.Count; i++)
            {
                if (PT.Contains(i)) { continue; }
                HE_groups_out.Add(HE_groups[i]);
            }

            HE_groups = HE_groups_out.Select(p => p.Select(k => (string[])k.Clone()).ToList()).ToList();
            return Report;
        }
    }

}
