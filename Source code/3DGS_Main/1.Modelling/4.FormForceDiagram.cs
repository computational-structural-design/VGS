using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using System.Drawing;
using KangarooSolver;
using Grasshopper;


namespace VGS_Main
{

    public class DIAGRAM : DrawForceDiagram
    {
        //Form Diagram attributes 
        public int int0_NodeSelect;
        public List<NODE> Nodes;
        public List<EDGE> Edges;
        public List<LOAD> Loads;
        public List<REACT> Reacts;
        public List<RESULT> Result;

        //Force Diagram attributes
        public FcDiagramAssembleData planarGraph; //The planarized information (The sequence of circles| edgeIDs in circles| Connection between the circles|nN information for ranging the duplicate edges)
        public List<List<Line>> ln2_Edge;
        public List<List<string[]>> st2_vn_Edge;
        public List<List<string>> st2_Edge;
        public List<List<Color>> cl2_Edge;
        public List<List<double>> db2_EdgeForce;
        
        public List<Line> ln1_Edge;
        public List<string> st1_Edge;
        public List<Color> cl1_Edge;
        public List<double> db1_EdgeForce;

        public List<List<int[]>> Fc_Pts_Addr;
        public DIAGRAM()
        {
            this.int0_NodeSelect = int.MaxValue;
            this.Nodes = new List<NODE>();
            this.Edges = new List<EDGE>();
            this.Loads = new List<LOAD>();
            this.Reacts = new List<REACT>();
            this.Result = new List<RESULT>();

            planarGraph = new FcDiagramAssembleData();
            ln2_Edge = new List<List<Line>>();
            st2_vn_Edge = new List<List<string[]>>();
            st2_Edge = new List<List<string>>();
            cl2_Edge = new List<List<Color>>();
            db2_EdgeForce = new List<List<double>>();
            ln1_Edge = new List<Line>();
            st1_Edge = new List<string>();
            cl1_Edge = new List<Color>();
            db1_EdgeForce = new List<double>();

            Fc_Pts_Addr = new List<List<int[]>>();//力图解中所有点在ln2_Edge中的地址
        }
        public DIAGRAM CopySelf()
        {
            DIAGRAM temp = new DIAGRAM();
            temp.int0_NodeSelect = this.int0_NodeSelect;
            foreach (NODE node in this.Nodes) { temp.Nodes.Add(node.CopySelf()); }
            foreach (EDGE edge in this.Edges) { temp.Edges.Add(edge.CopySelf()); }
            foreach (LOAD load in this.Loads) { temp.Loads.Add(load.CopySelf()); }
            foreach (REACT react in this.Reacts) { temp.Reacts.Add(react.CopySelf()); }
            foreach (RESULT result in this.Result) { temp.Result.Add(result.CopySelf()); }

            temp.planarGraph = this.planarGraph.CopySelf();
            //temp.PG = this.PG.CopySelf();
            temp.ln2_Edge = ListListCopy(ln2_Edge);
            temp.st2_vn_Edge = ListListCopy(st2_vn_Edge);
            temp.st2_Edge = ListListCopy(st2_Edge);
            temp.cl2_Edge = ListListCopy(cl2_Edge);
            temp.db2_EdgeForce = ListListCopy(db2_EdgeForce);

            temp.ln1_Edge = ListCopy(ln1_Edge);
            temp.st1_Edge = ListCopy(st1_Edge);
            temp.cl1_Edge = ListCopy(cl1_Edge);
            temp.db1_EdgeForce = ListCopy(db1_EdgeForce);

            temp.Fc_Pts_Addr = Fc_Pts_Addr.Select(p => p.Select(k => new int[] { k[0], k[1], k[2] }).ToList()).ToList();
            return temp;
        }
        public string UpdateNodes()
        {
            List<string> main_id = new List<string>();
            List<string> Edges_id = Edges.Select(p => p.ID).ToList();
            List<string> Loads_id = Loads.Select(p => p.ID).ToList();
            List<string> Reacts_id = Reacts.Select(p => p.ID).ToList();
            main_id.AddRange(Edges_id); main_id.AddRange(Loads_id); main_id.AddRange(Reacts_id);

            List<Line> main_ln = new List<Line>();
            List<Line> Edges_ln = Edges.Select(p => p.ln).ToList();
            List<Line> Loads_ln = Loads.Select(p => p.ln).ToList();
            List<Line> Reacts_ln = Reacts.Select(p => p.ln).ToList();
            main_ln.AddRange(Edges_ln); main_ln.AddRange(Loads_ln); main_ln.AddRange(Reacts_ln);

            List<Line> ext_ln = new List<Line>();
            ext_ln.AddRange(Loads_ln); ext_ln.AddRange(Reacts_ln);

            foreach (NODE N in Nodes)
            {
                if (N.adj_Edge_ID.Count == 1)
                {
                    Line ln1 = main_ln[main_id.IndexOf(N.adj_Edge_ID[0])];

                    for (int k = 0; k < Edges_ln.Count; k++)
                    {
                        if (Edges_id[k] == N.adj_Edge_ID[0]) { continue; }
                        Line ln2 = Edges_ln[k];
                        if (ComparePts(ln1.From, ln2.From, System_Configuration.Sys_Tor)) { N.pt = ln1.To; break; }
                        else if (ComparePts(ln1.From, ln2.To, System_Configuration.Sys_Tor)) { N.pt = ln1.To; break; }
                        else if (ComparePts(ln1.To, ln2.From, System_Configuration.Sys_Tor)) { N.pt = ln1.From; break; }
                        else if (ComparePts(ln1.To, ln2.To, System_Configuration.Sys_Tor)) { N.pt = ln1.From; break; }
                    }
                }
                else
                {
                    Line ln1 = main_ln[main_id.IndexOf(N.adj_Edge_ID[0])];
                    Line ln2 = main_ln[main_id.IndexOf(N.adj_Edge_ID[1])];
                    if (ComparePts(ln1.From, ln2.From, System_Configuration.Sys_Tor)) { N.pt = ln1.From; }
                    else if (ComparePts(ln1.From, ln2.To, System_Configuration.Sys_Tor)) { N.pt = ln1.From; }
                    else if (ComparePts(ln1.To, ln2.From, System_Configuration.Sys_Tor)) { N.pt = ln1.To; }
                    else if (ComparePts(ln1.To, ln2.To, System_Configuration.Sys_Tor)) { N.pt = ln1.To; }
                    else { return "F_updateNode Error: corresponding lines nor intersecting"; }
                }
            }

            return "F_updateNode Done";
        }
        public void MoveFm(Vector3d move)
        {
            foreach (NODE node in Nodes) { node.pt = new Point3d(node.pt + move); }
            foreach (EDGE edge in Edges) { edge.ln = new Line(new Point3d(edge.ln.From + move), new Point3d(edge.ln.To + move)); }
            foreach (LOAD load in Loads) { load.ln = new Line(new Point3d(load.ln.From + move), new Point3d(load.ln.To + move)); }
            foreach (REACT react in Reacts) { react.ln = new Line(new Point3d(react.ln.From + move), new Point3d(react.ln.To + move)); }
            foreach (RESULT result in Result) { result.ln = new Line(new Point3d(result.ln.From + move), new Point3d(result.ln.To + move)); }
        }
        public void MoveFc(Vector3d move)
        {
            for (int i = 0; i < ln2_Edge.Count; i++)
            {
                for (int j = 0; j < ln2_Edge[i].Count; j++)
                {
                    ln2_Edge[i][j] = new Line(new Point3d(ln2_Edge[i][j].From + move), new Point3d(ln2_Edge[i][j].To + move));//move the force diagram
                }
            }

            for (int i = 0; i < ln1_Edge.Count; i++)
            {
                ln1_Edge[i] = new Line(new Point3d(ln1_Edge[i].From + move), new Point3d(ln1_Edge[i].To + move));
            }

        }
        public List<IGoal> FcConvergeGoals(double k, List<List<int[]>> node_address, List<List<Line>> FC_edge_ln2)
        {
            List<IGoal> Goal = new List<IGoal>();
            for (int i = 0; i < node_address.Count; i++)
            {
                List<Point3d> sub_cluster = new List<Point3d>();
                for (int j = 0; j < node_address[i].Count; j++)
                {
                    int[] addr = node_address[i][j];
                    if (addr[2] == 0) { sub_cluster.Add(FC_edge_ln2[addr[0]][addr[1]].From); }
                    else if (addr[2] == 1) { sub_cluster.Add(FC_edge_ln2[addr[0]][addr[1]].To); }
                }
                Goal.Add(new SamePoint(sub_cluster, k));
            }
            return Goal;
        }

        public List<IGoal> FcPolygonClose(double k, List<List<Line>> FC_edge_ln2)
        {
            List<IGoal> Goal = new List<IGoal>();
            foreach (List<Line> Fc_polygon in FC_edge_ln2)
            {
                for (int i = 0; i < Fc_polygon.Count; i++)
                {
                    if (i != Fc_polygon.Count - 1) { Goal.Add(new SamePoint(new List<Point3d>() { Fc_polygon[i].To, Fc_polygon[i + 1].From }, k)); }
                    else { Goal.Add(new SamePoint(new List<Point3d>() { Fc_polygon[i].To, Fc_polygon[0].From }, k)); }
                }
            }
            return Goal;
        }
        public List<List<Point3d>> FcConvergePts(List<List<int[]>> node_address, List<List<Line>> FC_edge_ln2)
        {
            List<List<Point3d>> point_clusters = new List<List<Point3d>>();
            for (int i = 0; i < node_address.Count; i++)
            {
                List<Point3d> sub_cluster = new List<Point3d>();
                for (int j = 0; j < node_address[i].Count; j++)
                {
                    int[] addr = node_address[i][j];
                    if (addr[2] == 0) { sub_cluster.Add(FC_edge_ln2[addr[0]][addr[1]].From); }
                    else if (addr[2] == 1) { sub_cluster.Add(FC_edge_ln2[addr[0]][addr[1]].To); }
                }
                point_clusters.Add(sub_cluster);
            }
            return point_clusters;
        }
        public List<List<int[]>> FindForceDiagramNodeAddrs()
        {
            ConvergeForceDiagram(planarGraph.st2d_s, planarGraph.st1_v, ln2_Edge, planarGraph.ve_list, out List<List<eN>> e_info, out List<FcN> nodes);
            List<List<int[]>> node_address = NodesAddressNoVe(nodes, ln2_Edge);
            this.Fc_Pts_Addr = node_address.Select(p => p.Select(k => new int[] { k[0], k[1], k[2] }).ToList()).ToList();//Set values in to the class attributes

            return node_address;
            List<List<int[]>> NodesAddressNoVe(List<FcN> Fcnodes, List<List<Line>> edge_ln2_input)//找出没有ve cycle的点地址表
            {
                int max_ind = edge_ln2_input.Count;
                List<List<Line>> edge_ln2 = ListListCopy(edge_ln2_input);
                List<List<int[]>> node_address_Ve = Fcnodes.Select(p => p.addr).ToList();
                List<List<int[]>> node_address_noVe = node_address_Ve.Select(p => p.Where(k => k[0] != max_ind).ToList()).ToList();
                return node_address_noVe;
            }
        }
        private void ConvergeForceDiagram(DataTree<string> st2d_s, List<string> st1_v, List<List<Line>> edge_ln2_input, List<string> ve_list, out List<List<eN>> e_info, out List<FcN> nodes)
        {
            //st2d_s is a 2d tree structure of eN and QN;

            List<List<string>> st2d_s_list = new List<List<string>>();
            List<List<string>> st2d_s_list_comp = new List<List<string>>();
            List<List<Line>> edge_ln2 = ListListCopy(edge_ln2_input);
            
            for (int i = 0; i < st2d_s.BranchCount; i++)
            {
                st2d_s_list.Add(st2d_s.Branch(i));
                st2d_s_list_comp.Add(st2d_s.Branch(i));
            }

            //sort the st2d_s_list with the number index of st1_v(vn...);
            st2d_s_list.Sort((p1, p2) => getnum(st1_v[st2d_s_list_comp.IndexOf(p1)]).CompareTo(getnum(st1_v[st2d_s_list_comp.IndexOf(p2)])));

            //Re-Search the ve_list according to the import list
            //ve_ln is a set of lines that is reversed to the Qns in the cycles;
            List<Line> ve_ln = new List<Line>();

            foreach (string ve in ve_list)
            {
                for (int i = 0; i < st2d_s_list.Count; i++)
                {
                    bool find = false;
                    for (int j = 0; j < st2d_s_list[i].Count; j++)
                    {
                        //if there are more than one Qn edge in st2d_s_list, some Qn will be missed [221114 DEBUG not]
                        if (ve == st2d_s_list[i][j]) { ve_ln.Add(new Line(edge_ln2[i][j].To, edge_ln2[i][j].From)); find = true; break; }
                    }
                    if (find) { break; }
                }
            }

            if (ve_list.Count > 0)
            {//[1010] modify for selfstress cases
                st2d_s_list.Add(ve_list);//Add the ve list back;

                edge_ln2.Add(ve_ln);//Add the ve line list back;
            }
            //_End_Completing the ve list cycle in the data;

            List<string> Fc_ids = new List<string>();//the edge names;
            List<eN> Fc_eNs = new List<eN>();//the eN list match the order of edge names;
            List<FcN> Fc_Ns = new List<FcN>();//The list includes all the extremes;

            List<List<eN>> main_eNs = new List<List<eN>>();

            for (int i = 0; i < st2d_s_list.Count; i++)
            {
                List<eN> sub_cy = new List<eN>();
                for (int j = 0; j < st2d_s_list[i].Count; j++)
                {
                    string id = st2d_s_list[i][j];
                    Line ln = edge_ln2[i][j];
                    sub_cy.Add(new eN(id, ln.From, ln.To, -1, -1, new int[] { i, j }));
                }
                main_eNs.Add(sub_cy);
            }

            for (int i = 0; i < st2d_s_list.Count; i++)
            {
                for (int j = 0; j < st2d_s_list[i].Count; j++)
                {
                    string id = st2d_s_list[i][j];//The id of each edge in st2d_s_list;
                    Line ln = edge_ln2[i][j];//The according directed edge;

                    List<int[]> indcode = findInbetween(id, st2d_s_list, i);

                    int[] ind = indcode[0];
                    if (main_eNs[ind[0]][ind[1]].FN == -1)
                    {
                        Fc_Ns.Add(new FcN("v" + Fc_Ns.Count.ToString()));//New nodes;
                        int index = Fc_Ns.Count - 1;
                        for (int k = 0; k < indcode.Count; k++)
                        {
                            int[] p = indcode[k];
                            if (k % 2 == 0)//even numbers;
                            { main_eNs[p[0]][p[1]].FN = index; Fc_Ns[index].Pts.Add(main_eNs[p[0]][p[1]].From); Fc_Ns[index].addr.Add(new int[] { p[0], p[1], 0 }); }
                            else
                            { main_eNs[p[0]][p[1]].TN = index; Fc_Ns[index].Pts.Add(main_eNs[p[0]][p[1]].To); Fc_Ns[index].addr.Add(new int[] { p[0], p[1], 1 }); }
                        }
                    }
                }
            }

            e_info = main_eNs;
            nodes = Fc_Ns;


            List<int[]> findInbetween(string id, List<List<string>> cycles, int initial)//Finding all the halfedges relations on the cycled faces(They share a point in the fc diagram);
            {
                //cycles is a 2d tree of eN or QN；{{en...Qn},{em...}}
                int ind0 = cycles[initial].IndexOf(id);
                int len = cycles[initial].Count;
                //while getting to this id, then the searching is done; the id is the next half-edge in the cycle;
                string arrive_id = cycles[initial][nextInd(len, ind0)];

                //the address of the edges in cycles,that have a extreme sharing the same point
                List<int[]> indcode = new List<int[]>() { new int[] { initial, nextInd(len, ind0) }, new int[] { initial, ind0 } };

                string id0 = id;
                int st_ind = initial;
                while (true)
                {
                    int Cy_id = findCycleInd(id0, cycles, st_ind);//Finding according face id;
                    int ind1 = cycles[Cy_id].IndexOf(id0);//The index of the edge in the acorrding face;
                    int len1 = cycles[Cy_id].Count;//The number of the edges in the acorrding face;
                    int ind2 = frontInd(len1, ind1);//The previous index of the edge in the acorrding face;
                    string id_next = cycles[Cy_id][ind2];
                    indcode.Add(new int[] { Cy_id, ind1 }); indcode.Add(new int[] { Cy_id, ind2 });
                    if (id_next == arrive_id) { break; }
                    else
                    {
                        id0 = id_next;
                        st_ind = Cy_id;
                    }
                }
                return indcode;
            }
            int findCycleInd(string edgename, List<List<string>> cycles, int exclind)
            {
                List<int> match_ind = new List<int>();
                for (int i = 0; i < cycles.Count; i++)
                {
                    if (i == exclind) { continue; }
                    if (cycles[i].Contains(edgename)) { match_ind.Add(i); }
                    //if(match_ind.Count == 2){match_ind.Remove(exclind);break;}
                }
                return match_ind[0];
            }
            int nextInd(int len, int ind)
            {
                if (ind == len - 1) { return 0; }
                else { return ind + 1; }
            }
            int frontInd(int len, int ind)
            {
                if (ind == 0) { return len - 1; }
                else { return ind - 1; }
            }
            int getnum(string st) { return System.Convert.ToInt32(st.Substring(1)); }
        }
        public List<fc_d1> TransToSingleLineFc(double tol)
        {
            List<List<fc_d1>> sameid_list = new List<List<fc_d1>>();
            List<string> title_list = new List<string>();

            for (int i = 0; i < ln2_Edge.Count; i++)
            {
                for (int j = 0; j < ln2_Edge[i].Count; j++)
                {
                    if (!title_list.Contains(st2_Edge[i][j]))
                    {
                        title_list.Add(st2_Edge[i][j]);
                        sameid_list.Add(new List<fc_d1>() { new fc_d1(ln2_Edge[i][j], st2_Edge[i][j], cl2_Edge[i][j], db2_EdgeForce[i][j]) });
                    }
                    else
                    {
                        int index = title_list.IndexOf(st2_Edge[i][j]);
                        sameid_list[index].Add(new fc_d1(ln2_Edge[i][j], st2_Edge[i][j], cl2_Edge[i][j], db2_EdgeForce[i][j]));
                    }
                }
            }
            List<fc_d1> cleaned_list = new List<fc_d1>();
            foreach (List<fc_d1> sameid in sameid_list)
            {
                List<fc_d1> temp = new List<fc_d1>();
                List<Line> ln1 = new List<Line>();
                foreach (fc_d1 unit in sameid)
                {
                    if (!sameline_midpoint_list(unit.ln0_Edge, ln1, tol)) { temp.Add(unit); ln1.Add(unit.ln0_Edge); }
                }
                cleaned_list.AddRange(temp);
            }

            return cleaned_list;
        }
        public void align_Fc_1d_2d()
        {
            List<string> st1 = new List<string>();
            List<Line> ln1 = new List<Line>();
            List<Color> cl1 = new List<Color>();
            List<double> db1 = new List<double>();

            for (int i = 0; i < ln2_Edge.Count; i++)
            {
                for (int j = 0; j < ln2_Edge[i].Count; j++)
                {
                    if (!st1.Contains(st2_Edge[i][j])) { st1.Add(st2_Edge[i][j]); ln1.Add(ln2_Edge[i][j]); cl1.Add(cl2_Edge[i][j]); db1.Add(db2_EdgeForce[i][j]); }
                }
            }

            ln1_Edge = ln1;
            st1_Edge = st1;
            cl1_Edge = cl1;
            db1_EdgeForce = db1;
        }
        public void Sort_Fc_1d()
        {
            List<fc_d1> eN = new List<fc_d1>();
            List<fc_d1> QN = new List<fc_d1>();
            List<fc_d1> RN = new List<fc_d1>();

            for (int i = 0; i < ln1_Edge.Count; i++)
            {
                if (st1_Edge[i].Substring(0, 1) == "e")
                { eN.Add(new fc_d1(ln1_Edge[i], st1_Edge[i], cl1_Edge[i], db1_EdgeForce[i])); }
                else if (st1_Edge[i].Substring(0, 1) == "Q")
                { QN.Add(new fc_d1(ln1_Edge[i], st1_Edge[i], cl1_Edge[i], db1_EdgeForce[i])); }
                else if (st1_Edge[i].Substring(0, 1) == "R")
                { RN.Add(new fc_d1(ln1_Edge[i], st1_Edge[i], cl1_Edge[i], db1_EdgeForce[i])); }
            }

            eN.Sort((x, y) => x.id.CompareTo(y.id));
            QN.Sort((x, y) => x.id.CompareTo(y.id));
            RN.Sort((x, y) => x.id.CompareTo(y.id));

            eN.AddRange(QN); eN.AddRange(RN);
            ln1_Edge = eN.Select(p => p.ln0_Edge).ToList();
            st1_Edge = eN.Select(p => p.st0_Edge).ToList();
            cl1_Edge = eN.Select(p => p.cl0_Edge).ToList();
            db1_EdgeForce = eN.Select(p => p.db0_EdgeForce).ToList();

        }
        public Point3d extremPt_Fm()
        {
            if (Nodes.Count <= 0) { return new Point3d(0, 0, 0); }
            Point3d compt = Nodes[0].pt;

            foreach (NODE node in Nodes)
            {
                for (int i = 1; i < Nodes.Count; i++)
                {
                    //if (node.pt.X < compt.X) { compt.X = node.pt.X; }//[1021 Modify for letting the location point of the structure be the N0 of the form diagram]
                    //if (node.pt.Y < compt.Y) { compt.Y = node.pt.Y; }
                    //if (node.pt.Z < compt.Z) { compt.Z = node.pt.Z; }
                }
            }
            return compt;
        }
        public Point3d extremPt_Fc()
        {
            Point3d compt = new Point3d(0.0, 0.0, 0.0);
            if (Fc_Pts_Addr.Count > 0 && ln2_Edge.Count > 0)
            {
                List<List<Point3d>> FcPts = FcConvergePts(this.Fc_Pts_Addr, this.ln2_Edge);
                compt = FcPts[0][0];
            }
            return compt;
        }
        public class eN
        {
            public string id;
            public Point3d From;
            public Point3d To;
            public Line ln { get { return new Line(this.From, this.To); } }
            public int FN;//From extreme index in the force diagram Vn
            public int TN;//  To extreme index in the force diagram Vn
            public int[] address;//the [i,j] address in st2d_s_list
            public eN(string ename, Point3d F, Point3d T, int fn, int tn, int[] ad)
            {
                id = ename;
                From = F; To = T;
                FN = fn;
                TN = tn;
                address = ad;
            }
        }
        public class FcN
        {

            public string id;
            public List<Point3d> Pts;
            public List<int[]> addr;

            public FcN(string name)
            {
                id = name;
                Pts = new List<Point3d>();
                addr = new List<int[]>();
            }
        }
        public class SamePoint : GoalObject
        {
            public double Strength;
            public List<Point3d> Pts;

            public SamePoint() { }


            public SamePoint(List<Point3d> Pts, double k)
            {
                PPos = Pts.Select(p => p).ToArray();
                Move = new Vector3d[Pts.Count];
                Weighting = Pts.Select(p => k).ToArray();
                this.Pts = Pts;
                Strength = k;

            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d midPoint = midPt(PIndex.Select(ind => p[ind].Position).ToList());
                for (int i = 0; i < PPos.Length; i++)
                {
                    Vector3d direction = new Vector3d(midPoint - p[PIndex[i]].Position);
                    if (Strength <= 1)
                    { Move[i] = direction * Strength; }
                    else
                    { Move[i] = direction; }
                }
            }

            private Point3d midPt(List<Point3d> pts)
            {
                Point3d mid = new Point3d(0.0, 0.0, 0.0);
                foreach (Point3d pt in pts)
                {
                    mid += pt;
                }
                mid /= pts.Count;
                return mid;
            }
        }

    }
    public class Form_Force_Evalution : VgsCommon
    {
        public double Global_convergence;
        public List<string> Edge_unmatch;
        public List<string> Dupli_unmatch;
        public double loadpath;

        public void Run_Evalution(MODEL Fm, MODEL Fc, double angle_threshold, double length_threshold)
        {
            List<Line> Fm_ln1_e = Fm.Diagram.Edges.Select(p => p.ln).ToList();
            List<string> Fm_st1_e = Fm.Diagram.Edges.Select(p => p.ID).ToList();

            List<Line> Fm_ln1_l = Fm.Diagram.Loads.Select(p => p.ln).ToList();
            List<string> Fm_st1_l = Fm.Diagram.Loads.Select(p => p.ID).ToList();

            List<Line> Fm_ln1_r = Fm.Diagram.Reacts.Select(p => p.ln).ToList();
            List<string> Fm_st1_r = Fm.Diagram.Reacts.Select(p => p.ID).ToList();

            List<Line> Fm_ln1 = new List<Line>(); Fm_ln1.AddRange(Fm_ln1_e); Fm_ln1.AddRange(Fm_ln1_l); Fm_ln1.AddRange(Fm_ln1_r);
            List<string> Fm_st1 = new List<string>(); Fm_st1.AddRange(Fm_st1_e); Fm_st1.AddRange(Fm_st1_l); Fm_st1.AddRange(Fm_st1_r);

            List<Line> Fc_ln1 = Flattern2D(Fc.Diagram.ln2_Edge);
            List<string> Fc_st1 = Flattern2D(Fc.Diagram.st2_Edge);

            reciprocal_evalution(Fm_ln1, Fm_st1, Fc_ln1, Fc_st1, angle_threshold, length_threshold);
            LoadPath_evalution(Fm_ln1_e, Fm_st1_e, Fc_ln1, Fc_st1);
        }
        public void reciprocal_evalution(List<Line> Fm_ln, List<string> Fm_st, List<Line> Fc_ln, List<string> Fc_st, double angle_threshold, double length_threshold)
        {
            double overall_score = 0.0;
            List<string> edge_unmatch = new List<string>();
            List<string> dupli_unmatch = new List<string>();
            List<List<int>> dupli_int = new List<List<int>>();

            for (int i = 0; i < Fm_ln.Count; i++)
            {
                List<int> dupli = new List<int>();
                for (int j = 0; j < Fc_ln.Count; j++)
                {
                    if (Fm_st[i] == Fc_st[j])
                    {
                        dupli.Add(j);
                        double sub_score = angle_check(Fm_ln[i], Fc_ln[j]);
                        overall_score += sub_score;
                        if (sub_score >= angle_threshold)
                        { if (!edge_unmatch.Contains(Fm_st[i])) { edge_unmatch.Add(Fm_st[i]); }; }
                    }
                }
                if (dupli.Count > 2) { dupli_int.Add(dupli); }
            }
            Global_convergence = overall_score;
            Edge_unmatch = edge_unmatch;

            foreach (List<int> dupli in dupli_int)
            {
                List<int> index;
                if (!length_check(dupli, Fc_ln, length_threshold, out index)) { dupli_unmatch.Add(Fc_st[index[0]]); }
            }

            Dupli_unmatch = dupli_unmatch;
        }
        public void LoadPath_evalution(List<Line> Fm_ln, List<string> Fm_st, List<Line> Fc_ln, List<string> Fc_st)
        {
            double score = 0.0;

            for (int i = 0; i < Fm_st.Count; i++)
            {
                if (Fm_st[i].Substring(0, 1) != "e") { continue; }
                double all_forces = 0.0;
                int dupli_count = 0;
                for (int j = 0; j < Fc_st.Count; j++)
                {
                    if (Fm_st[i] == Fc_st[j]) { all_forces += Fc_ln[j].Length; dupli_count += 1; Fc_st.RemoveAt(j); Fc_ln.RemoveAt(j); j--; }
                }
                double force = all_forces / dupli_count;
                score += Fm_ln[i].Length * force;
            }
            loadpath = score;
        }
        public bool length_check(List<int> ids, List<Line> lns, double difference, out List<int> ind)
        {
            bool same_len = true;
            ind = new List<int>();
            for (int i = 0; i < ids.Count - 1; i++)
            {
                for (int j = i + 1; j < ids.Count; j++)
                {
                    double subscore = System.Math.Abs(lns[ids[i]].Length - lns[ids[j]].Length);
                    if (subscore > difference)//The same length in the duplicate edges
                    {
                        same_len = false;
                        if (!ind.Contains(ids[i])) { ind.Add(ids[i]); }
                        if (!ind.Contains(ids[j])) { ind.Add(ids[j]); }
                    }
                }
            }
            return same_len;
        }
        public double angle_check(Line lnA, Line lnB)
        {
            if (lnA.Length <= 0.00001 || lnB.Length <= 0.00001)
            { return 0.0; }
            Vector3d VecA = new Vector3d(lnA.To - lnA.From); VecA.Unitize();
            Vector3d VecB = new Vector3d(lnB.To - lnB.From); VecB.Unitize();
            return System.Math.Abs(System.Math.Abs(VecA.X * VecB.X + VecA.Y * VecB.Y + VecA.Z * VecB.Z) - 1);
        }

    }
}
