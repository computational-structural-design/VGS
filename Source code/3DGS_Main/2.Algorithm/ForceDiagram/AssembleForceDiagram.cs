using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VGS_Main
{
    public class DrawForceDiagram : VgsCommon
    {
        public class Polygon
        {

            public string name;

            public List<string> NodeNode_edgeID;//e1,e2++,e3+
            public List<string[]> NodeNode_ID;//(e1|v1,v2)
            public List<double> magnitude;//- compression + tension
            public List<Color> edge_color;
            public List<Line> force_line;

            public Polygon(string in_name, List<string> in_NodeNode_edgeID)
            {
                int num = in_NodeNode_edgeID.Count;
                this.name = in_name;
                this.NodeNode_edgeID = in_NodeNode_edgeID;
                this.NodeNode_ID = in_NodeNode_edgeID.Select(p => new string[] { "", "" }).ToList();

                this.magnitude = new List<double>(num);
                for (int i = 0; i < num; i++) { this.magnitude.Add(0.0); }

                this.edge_color = new List<Color>(num);
                for (int i = 0; i < num; i++) { this.edge_color.Add(System.Drawing.Color.Gray); }

                this.force_line = new List<Line>(num);
                for (int i = 0; i < num; i++) { this.force_line.Add(new Line()); }
            }
            public void Trans(Vector3d vec)
            {
                for (int i = 0; i < this.force_line.Count; i++)
                {
                    this.force_line[i] = new Line(this.force_line[i].From + vec, this.force_line[i].To + vec);
                }
            }
            public void ForcePolygon()//[1214]change to even force polygon
            {
                Vector3d vecsum = new Vector3d(0.0, 0.0, 0.0);

                foreach (Line ln in this.force_line) { vecsum += new Vector3d(ln.To - ln.From); }
                double len = vecsum.Length; vecsum.Unitize();
                Vector3d vec_Discret = -vecsum * (len / this.force_line.Count);

                List<Line> forcepolygon = new List<Line>();
                Point3d ed = this.force_line[0].From;
                foreach (Line line in this.force_line)
                {
                    Point3d DiscretEd = ed + vec_Discret;
                    Line temp = new Line(DiscretEd, new Vector3d(line.To - line.From));
                    forcepolygon.Add(temp);
                    ed = temp.To;
                }
                for (int i = 0; i < forcepolygon.Count; i++)
                {
                    this.force_line[i] = forcepolygon[i];
                }
            }

            public string VecSum()
            {
                Vector3d vecsum = new Vector3d(0.0, 0.0, 0.0);
                foreach (Line ln in this.force_line) { vecsum += new Vector3d(ln.To - ln.From); }
                double len = vecsum.Length; vecsum.Unitize();
                Vector3d vec_Discret = -vecsum * (len / this.force_line.Count);
                return vec_Discret.Length.ToString();
            }
            public string Report()
            {
                string report = string.Format("\n__NODE_:[{0}]\n", this.name);
                report += "__Polygon_VecSum: " + this.VecSum() + "\n";
                report += "__Node_edgeID: " + string.Join("|", this.NodeNode_edgeID.ToArray()) + "\n";
                report += "__Node_nodeID: " + string.Join("|", this.NodeNode_ID.Select(p => p[0] + "->" + p[1])) + "\n";
                report += "__Force_line: " + string.Join("|", this.force_line.Select(p => System.Math.Round(p.Length, 2).ToString()).ToArray()) + "\n";
                report += "__Color: " + string.Join("|", this.edge_color.Select(p => p.ToString()).ToArray()) + "\n";
                report += "__Magnitude: " + string.Join("|", this.magnitude.Select(p => System.Math.Round(p, 2).ToString()).ToArray()) + "\n";
                return report;
            }
        }
        private string RemovePlusLabel(string ID)
        {
            string temp;
            string[] names = ID.Split('+');
            temp = names[0];

            return temp;
        }

        public void AssemblingForceDiagram(DIAGRAM F, Point3d pt0_s_Origin, bool FormFinding, ref string report)
        {
            int int0_NodeSelect = int.MaxValue;
            DIAGRAM FD = F.CopySelf();
            FcDiagramAssembleData PG = FD.planarGraph.CopySelf();
            DataTree<string> st2d_s_m = PG.st2d_s;
            List<string> st1_v = PG.st1_v;
            List<string> st1_c = PG.st1_c;
            List<string> st1_s_m = PG.st1_e;
            DataTree<string> nN = PG.nN;
            #region import data
            // Edges Import
            List<string> st1_Edge = new List<string>(FD.Edges.Count);//Edgename:e0,e1,e2
            List<Line> ln1_Edge = new List<Line>(FD.Edges.Count);//Edge Line
            List<Point3d> pt1_Edge = new List<Point3d>(FD.Edges.Count);//Edge Point at 0.6 for display the edgename
            List<Color> cl1_Edge = new List<Color>(FD.Edges.Count);//Edge Color
            List<double> db1_EdgeForce = new List<double>(FD.Edges.Count);//Edge force magnitude

            for (int i = 0; i < FD.Edges.Count; i++)
            {
                st1_Edge.Add(FD.Edges[i].ID);
                ln1_Edge.Add(FD.Edges[i].ln);
                pt1_Edge.Add(FD.Edges[i].ln.PointAt(0.6));
                cl1_Edge.Add(FD.Edges[i].col);
                db1_EdgeForce.Add(FD.Edges[i].force);
            }

            // Loads
            List<string> st1_Load = new List<string>(FD.Loads.Count);
            List<Line> ln1_Load = new List<Line>(FD.Loads.Count);
            List<Color> cl1_Load = new List<Color>(FD.Loads.Count);

            for (int i = 0; i < FD.Loads.Count; i++)
            {
                st1_Load.Add(FD.Loads[i].ID);
                ln1_Load.Add(FD.Loads[i].ln);
                cl1_Load.Add(FD.Loads[i].col);
            }

            // Reactions
            List<string> st1_Reac = new List<string>(FD.Reacts.Count);
            List<Line> ln1_Reac = new List<Line>(FD.Reacts.Count);
            List<Color> cl1_Reac = new List<Color>(FD.Reacts.Count);

            for (int i = 0; i < FD.Reacts.Count; i++)
            {
                st1_Reac.Add(FD.Reacts[i].ID);
                ln1_Reac.Add(FD.Reacts[i].ln);
                cl1_Reac.Add(FD.Reacts[i].col);
            }

            // Nodes
            List<Point3d> Node_Pt = FD.Nodes.Select(p => p.pt).ToList();
            #endregion
            report += "The ve circle:" + string.Join("|",PG.ve_list.ToArray());
            // Generate Force Diagram

            // Edges
            List<List<string>> st2_s = new List<List<string>>();//e1 e2 e3
            List<List<string[]>> st2_vn_s = new List<List<string[]>>();//(v1,v2)(v3,v4)
            List<List<Line>> ln2_s = new List<List<Line>>();
            List<List<Color>> cl2_s = new List<List<Color>>();
            List<List<double>> db2_s = new List<List<double>>();

            List<string> Node_ID = FD.Nodes.Select(p => p.ID).ToList();
            List<string> Edge_ID = FD.Edges.Select(p => p.ID).ToList();

            for (int i = 0; i < FD.Nodes.Count; i++)//Foreach Node vN
            {

                List<Vector3d> vc1_NodeEdgeForce = new List<Vector3d>();

                List<string> st1_NodeNode = new List<string>();
                List<string[]> st1_NodeNode_vn = new List<string[]>();

                st1_NodeNode = FD.Nodes[i].adj_Node_ID;//nodenode vn
                                                       //Print(String.Join("; ", st1_NodeNode.ToArray()));

                //st1_NodeNode: Foreach Node, the adjacency node nodename (vn) ;
                //in1_NodeNodeIndex: Foreach Node, the adjacency node_index (i) ;
                List<int> in1_NodeNodeIndex = new List<int>();
                foreach (string st0_NodeNode in st1_NodeNode)//Foreach adajacency Node
                {
                    int in0_NodeNodeIndex = Node_ID.IndexOf(st0_NodeNode);
                    if (in0_NodeNodeIndex == -1) { throw new Exception("\nError in Assembling Force diagram:" +string.Format("The Node out of index range:{0}", st0_NodeNode)); }
                    in1_NodeNodeIndex.Add(in0_NodeNodeIndex);
                    Vector3d vc0_NodeEdgeForce = new Vector3d(Node_Pt[in0_NodeNodeIndex] - Node_Pt[i]);//The vector from the Mainloop Node to adajaceny node(Only viable while the vn order is from 0-n)

                    vc0_NodeEdgeForce.Unitize();
                    vc1_NodeEdgeForce.Add(vc0_NodeEdgeForce);//The force vector for each adjacency node force

                    string[] vn0_NodeNode = new string[] { Node_ID[i], Node_ID[in0_NodeNodeIndex] };
                    st1_NodeNode_vn.Add(vn0_NodeNode);
                }
                //vc1_NodeEdgeForce: the unitized force vector for the Nodes(vn) to their adacency nodes
                //st1_NodeNode:The adjacency nodename list for each mainNode
                //st1_NodeEdge:The adjacency node edge for each mainNode
                //in1_NodeEdgeIndex:The edge index for edge adajacency node edge

                List<string> st1_NodeEdge = FD.Nodes[i].adj_Edge_ID;

                List<int> in1_NodeEdgeIndex = new List<int>();
                
                
                foreach (string st0_NodeEdge in st1_NodeEdge)
                {
                    int int1_NodeEdge = Edge_ID.IndexOf(st0_NodeEdge);
                    if (int1_NodeEdge == -1) { throw new Exception("\nError in Assembling Force diagram:" + string.Format("The Edge out of index range:{0}", st0_NodeEdge)); }

                    in1_NodeEdgeIndex.Add(int1_NodeEdge);//Get the adj_edge IDs of the vN
                }

                List<Color> cl1_NodeEdge = new List<Color>();//The Color for adjacency node edge
                List<double> db1_NodeEdgeForce = new List<double>();//The force with magnitude for adjacency node edge
                List<Line> ln1_NodeEdge = new List<Line>();

                for (int j = 0; j < in1_NodeEdgeIndex.Count; j++)
                {
                    int index = in1_NodeEdgeIndex[j];

                    cl1_NodeEdge.Add(cl1_Edge[index]);
                    double db0_NodeEdgeForce = db1_EdgeForce[index];
                    db1_NodeEdgeForce.Add(db0_NodeEdgeForce);

                    Point3d pt0_Origin = pt0_s_Origin;//From the selected point , it is the first point
                    if (j != 0) pt0_Origin = ln1_NodeEdge[j - 1].To;//If not the start point

                    if (db0_NodeEdgeForce > 0)
                    {
                        Line ln0_NodeEdge = new Line(pt0_Origin, vc1_NodeEdgeForce[j], db0_NodeEdgeForce);//Start point , Unitized vector, magnitude construct the Line
                        ln1_NodeEdge.Add(ln0_NodeEdge);
                        st1_NodeNode_vn[j] = new string[] { st1_NodeNode_vn[j][0], st1_NodeNode_vn[j][1] };
                    }
                    else
                    {
                        Line ln0_NodeEdge = new Line(pt0_Origin, -vc1_NodeEdgeForce[j], -db0_NodeEdgeForce);
                        ln1_NodeEdge.Add(ln0_NodeEdge);
                        st1_NodeNode_vn[j] = new string[] { st1_NodeNode_vn[j][1], st1_NodeNode_vn[j][0] };
                    }
                }

                st2_s.Add(st1_NodeEdge);//String: From v0-vN ,the adjacency node edge eN List<List<string>>
                st2_vn_s.Add(st1_NodeNode_vn);
                ln2_s.Add(ln1_NodeEdge);//Force line: From v0-vN ,the adjacency Line (from self to adacent node) The force vector Line
                cl2_s.Add(cl1_NodeEdge);//Color: From v0-vN ,the adjacency force color
                db2_s.Add(db1_NodeEdgeForce);//Magnitude: From v0-vN ,the adjacency force magnitude
            }

            // Loads
            List<int> in1_LoadIndex = new List<int>();
            for (int i = 0; i < FD.Loads.Count; i++)
            {
                string st0_Load = FD.Loads[i].ID;
                Line ln0_Load = FD.Loads[i].ln;
                Color cl0_Load = FD.Loads[i].col;
                string st0_LoadIndex = FD.Loads[i].ActionPt_ID;
                int in0_LoadIndex = Node_ID.IndexOf(st0_LoadIndex);//the load in which vN circle
                if (in0_LoadIndex == -1) { throw new Exception("\nError in Assembling Force diagram:" + string.Format("The Load out of index range:{0}", st0_LoadIndex)); }
                Line ln0_NodeEdge = ln2_s[in0_LoadIndex][ln2_s[in0_LoadIndex].Count - 1];
                ln0_Load.Transform(Transform.Translation(ln0_NodeEdge.To - ln0_Load.From));//move the load line based on the last line in vN-Edges;

                st2_s[in0_LoadIndex].Add(st0_Load);
                st2_vn_s[in0_LoadIndex].Add(new string[] { st0_LoadIndex, "ve" });
                ln2_s[in0_LoadIndex].Add(ln0_Load);
                cl2_s[in0_LoadIndex].Add(cl0_Load);
                db2_s[in0_LoadIndex].Add(ln0_Load.Length);
            }

            // Reactions
            List<int> in1_ReacIndex = new List<int>();
            for (int i = 0; i < FD.Reacts.Count; i++)
            {
                string st0_Reac = FD.Reacts[i].ID;
                Line ln0_Reac = FD.Reacts[i].ln;
                Color cl0_Reac = FD.Reacts[i].col;
                string st0_ReacIndex = FD.Reacts[i].ActionPt_ID;
                int in0_ReacIndex = Node_ID.IndexOf(st0_ReacIndex);
                if (in0_ReacIndex == -1) { throw new Exception("\nError in Assembling Force diagram:" + string.Format("The ReactionForce out of index range:{0}", st0_ReacIndex)); }
                Line ln0_NodeEdge = ln2_s[in0_ReacIndex][ln2_s[in0_ReacIndex].Count - 1];
                ln0_Reac.Transform(Transform.Translation(ln0_NodeEdge.To - ln0_Reac.From));

                st2_s[in0_ReacIndex].Add(st0_Reac);
                st2_vn_s[in0_ReacIndex].Add(new string[] { st0_ReacIndex, "ve" });
                ln2_s[in0_ReacIndex].Add(ln0_Reac);
                cl2_s[in0_ReacIndex].Add(cl0_Reac);
                db2_s[in0_ReacIndex].Add(ln0_Reac.Length);
            }

            //st2_s.Add: From v0-vN ,the adjacency node edge eN List<List<string>>
            //ln2_s.Add: From v0-vN ,the adjacency Line (from self to adacent node) The force vector Line
            //cl2_s.Add: From v0-vN ,the adjacency force color
            //db2_s.Add: From v0-vN ,the adjacency force magnitude

            //Write information into Node class

            List<List<string>> circle_order = new List<List<string>>(st2d_s_m.BranchCount);//The embeded circles
            List<Polygon> Nodes = new List<Polygon>(st2d_s_m.BranchCount);
            for (int i = 0; i < st2d_s_m.BranchCount; i++)
            { Nodes.Add(new Polygon(st1_v[i], st2d_s_m.Branch(i))); }//initialize the Nodes with name and embeded circle
            Nodes.Sort((p1, p2) => System.Convert.ToInt32(p1.name.Substring(1)).CompareTo(System.Convert.ToInt32(p2.name.Substring(1))));//sort the Nodes according to the index of the name

            for (int i = 0; i < st2_s.Count; i++)//From v0-vN;
            {
                foreach (string edgeID in Nodes[i].NodeNode_edgeID)//Some newly added e cannot be found in the original adjacency##
                {
                    
                    int index_import = st2_s[i].IndexOf(RemovePlusLabel(edgeID));//the edge index in the selected Node[i] in st2_s(From v0-vN);the positions in st2_s

                    int index_class = Nodes[i].NodeNode_edgeID.IndexOf(edgeID);//the edge index in the selected Node[i] in Nodes;The poisions in NodeNode
                    //the circle information in st2_s(index_import) ---> the circle information in Nodes(index_class);Mapping information: copy the information of the side in st2_s to the side of the node corresponding to the nodes
                    if (index_import != -1)
                    {
                        Nodes[i].NodeNode_ID[index_class] = st2_vn_s[i][index_import];
                        Nodes[i].force_line[index_class] = ln2_s[i][index_import];
                        Nodes[i].edge_color[index_class] = cl2_s[i][index_import];
                        Nodes[i].magnitude[index_class] = db2_s[i][index_import];
                    }
                }
            }

            //nN information match(write nN information into Node class)
            List<List<string>> v_order = new List<List<string>>();
            List<List<string>> e_order = new List<List<string>>();
            for (int i = 0; i < nN.BranchCount / 2; i++)
            {
                v_order.Add(nN.Branch(i * 2));//_____v1___v2____v3_____v4;
                e_order.Add(nN.Branch(i * 2 + 1));//____e0+__e0++__e0+++;
            }

            for (int i = 0; i < v_order.Count; i++)//test[210901]For the general situation, we still need to use the old judgment method. The key is to identify ve
            {
                bool back = false;
                if (v_order[i][0] == "ve") { back = true; }
                for (int j = 1; j < v_order[i].Count - 1; j++)//v2--v3 delete the head and tail
                {
                    if (v_order[i][j] == "ve") { continue; }

                    int front = -1;
                    int index = -1;
                    string e_front = "";
                    string e_back = "";
                    int in_front = -1;
                    if (back)
                    {
                        front = System.Convert.ToInt32(v_order[i][v_order[i].Count - 1].Substring(1));
                        index = System.Convert.ToInt32(v_order[i][j].Substring(1));//vi =v2---v3
                        e_front = e_order[i][j];
                        e_back = e_order[i][j - 1];
                        in_front = Nodes[front].NodeNode_edgeID.IndexOf(e_order[i][e_order[i].Count - 1]);
                    }
                    else
                    {
                        front = System.Convert.ToInt32(v_order[i][0].Substring(1));
                        index = System.Convert.ToInt32(v_order[i][j].Substring(1));//vi =v2---v3
                        e_front = e_order[i][j - 1];//e0+---> vi front
                        e_back = e_order[i][j];//e0++---> vi back
                        in_front = Nodes[front].NodeNode_edgeID.IndexOf(e_order[i][0]);
                    }
                    if (in_front == -1) { throw new Exception("\nError in Assembling Force diagram:" + string.Format("The FrontIndexErorr In 354 Scode")); }


                    //v1 Node e0+ index extreme v1 e0+index
                    int in_e_front = Nodes[index].NodeNode_edgeID.IndexOf(e_front);//the edge index before the target point v2--v3
                    int in_e_back = Nodes[index].NodeNode_edgeID.IndexOf(e_back);//the edge index after the target point v2--v3

                    if (in_e_front == -1)
                    {
                        List<string> NNedgeID = Nodes[index].NodeNode_edgeID;
                        throw new Exception("\nError in Assembling Force diagram:" + string.Format("The in_e_front Erorr In 361 Scode: e_front{0} is out of [{2}]List:\n[{1}]", e_front,string.Join(",", NNedgeID), Nodes[index].name + "|" + index.ToString())); 
                    }
                    if (in_e_back == -1) 
                    {
                        List<string> NNedgeID = Nodes[index].NodeNode_edgeID;
                        throw new Exception("\nError in Assembling Force diagram:" + string.Format("The in_e_back Erorr In 362 Scode: e_back {0} is out of [{2}]List:\n[{1}]", e_back, string.Join(",", NNedgeID), Nodes[index].name+"|"+ index.ToString())); 
                    }

                    Nodes[index].force_line[in_e_front] = new Line(Nodes[front].force_line[in_front].To, Nodes[front].force_line[in_front].From);//opposite with the previous vector
                    Nodes[index].force_line[in_e_back] = new Line(Nodes[front].force_line[in_front].From, Nodes[front].force_line[in_front].To);//same direction with the previous vector

                    Nodes[index].NodeNode_ID[in_e_front] = new string[] { Nodes[front].NodeNode_ID[in_front][1], Nodes[front].NodeNode_ID[in_front][0] };
                    Nodes[index].NodeNode_ID[in_e_back] = new string[] { Nodes[front].NodeNode_ID[in_front][0], Nodes[front].NodeNode_ID[in_front][1] };

                    Nodes[index].edge_color[in_e_front] = Nodes[front].edge_color[in_front];
                    Nodes[index].edge_color[in_e_back] = Nodes[front].edge_color[in_front];

                    Nodes[index].magnitude[in_e_front] = Nodes[front].magnitude[in_front];
                    Nodes[index].magnitude[in_e_back] = Nodes[front].magnitude[in_front];

                }
            }//save for test[210901] For the problem that ve cannot be recognized in nN 

            foreach (Polygon node in Nodes) { report += node.Report(); }

            // Assemble Force Diagram

            //initialize polygon
            foreach (Polygon node in Nodes) { node.ForcePolygon(); }

            if (FormFinding)
            {
                //move the poygon[211214] dicreted force diagram（for certain form finding）
                List<string> namelist = FD.Nodes.Select(p => p.ID).ToList();

                for (int i = 0; i < st1_v.Count; i++)
                {
                    string Nid = st1_v[i];
                    Vector3d move = new Vector3d(0.0, 0.0, 0.0);
                    int in_0 = System.Convert.ToInt32(st1_v[i].Substring(1));

                    if (i == 0 || Nid.Substring(0, 1) == "v")
                    {
                        int in_f = System.Convert.ToInt32(st1_v[i].Substring(1));
                        Point3d pos = Node_Pt[in_0];
                        Point3d mid = new Point3d(0.0, 0.0, 0.0);
                        foreach (Line line in Nodes[in_f].force_line)
                        { mid += line.From; mid += line.To; }
                        mid /= Nodes[in_f].force_line.Count * 2;
                        move = new Vector3d(pos - mid);
                    }
                    else
                    {
                        int in_1 = System.Convert.ToInt32(st1_c[i - 1].Substring(1));

                        int move_line_index = Nodes[in_0].NodeNode_edgeID.IndexOf(st1_s_m[i - 1]);
                        int match_line_index = Nodes[in_1].NodeNode_edgeID.IndexOf(st1_s_m[i - 1]);

                        if (move_line_index == -1|| match_line_index==-1) { throw new Exception("\nError in Assembling Force diagram:" + string.Format("In form finding moving in Scode 414")); }

                        move = new Vector3d(Nodes[in_1].force_line[match_line_index].To - Nodes[in_0].force_line[move_line_index].From);
                    }

                    Nodes[in_0].Trans(move);
                }
            }
            else
            {
                //move the polygon
                for (int i = 1; i < st1_v.Count; i++)
                {
                    int in_0 = System.Convert.ToInt32(st1_v[i].Substring(1));
                    int in_1 = System.Convert.ToInt32(st1_c[i - 1].Substring(1));

                    int move_line_index = Nodes[in_0].NodeNode_edgeID.IndexOf(st1_s_m[i - 1]);
                    int match_line_index = Nodes[in_1].NodeNode_edgeID.IndexOf(st1_s_m[i - 1]);

                    if (move_line_index == -1 || match_line_index == -1) { throw new Exception("\nError in Assembling Force diagram:" + string.Format("In form finding moving in Scode 433")); }

                    Vector3d move = new Vector3d(Nodes[in_1].force_line[match_line_index].To - Nodes[in_0].force_line[move_line_index].From);
                    Nodes[in_0].Trans(move);
                }
            }

            //Color the selected edge__TEST
            if (int0_NodeSelect < Nodes.Count)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (i != int0_NodeSelect)
                    {
                        for (int j = 0; j < Nodes[i].edge_color.Count; j++)
                        { Nodes[i].edge_color[j] = System.Drawing.Color.DarkGray; }
                    }
                    else
                    {
                        for (int j = 0; j < Nodes[i].edge_color.Count; j++)
                        {
                            if (Nodes[i].edge_color[j] == System.Drawing.Color.DarkGray)
                            { Nodes[i].edge_color[j] = System.Drawing.Color.Black; }
                        }
                    }
                }
            }

            //Oganize information
            DataTree<Line> ln2d_s_t = new DataTree<Line>();
            DataTree<string> st2d_s_t = new DataTree<string>();
            DataTree<Color> cl2d_s_t = new DataTree<Color>();
            DataTree<double> db2d_s_t = new DataTree<double>();
            DataTree<int> st2d_s_m_t = new DataTree<int>();

            ln1_Edge = new List<Line>();
            st1_Edge = new List<string>();
            cl1_Edge = new List<Color>();
            db1_EdgeForce = new List<double>();

            List<List<Line>> ln2_Edge = new List<List<Line>>();
            List<List<string[]>> st2_vn_Edge = new List<List<string[]>>();
            List<List<string>> st2_Edge = new List<List<string>>();
            List<List<Color>> cl2_Edge = new List<List<Color>>();
            List<List<double>> db2_EdgeForce = new List<List<double>>();

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].name.Substring(0, 1) == "n") { break; }
                for (int j = 0; j < Nodes[i].NodeNode_edgeID.Count; j++)
                {
                    if (!st1_Edge.Contains(RemovePlusLabel(Nodes[i].NodeNode_edgeID[j])))
                    {
                        st1_Edge.Add(RemovePlusLabel(Nodes[i].NodeNode_edgeID[j]));
                        ln1_Edge.Add(Nodes[i].force_line[j]);
                        cl1_Edge.Add(Nodes[i].edge_color[j]);
                        db1_EdgeForce.Add(Nodes[i].magnitude[j]);
                    }
                }
            }
            ln2d_s_t.AddRange(ln1_Edge, new GH_Path(0, 0));
            st2d_s_t.AddRange(st1_Edge, new GH_Path(0, 0));
            cl2d_s_t.AddRange(cl1_Edge, new GH_Path(0, 0));
            db2d_s_t.AddRange(db1_EdgeForce, new GH_Path(0, 0));
            for (int i = 0; i < Nodes.Count; i++)
            {
                ln2d_s_t.AddRange(Nodes[i].force_line, new GH_Path(1, i));
                st2d_s_t.AddRange(Nodes[i].NodeNode_edgeID.Select(p => RemovePlusLabel(p)), new GH_Path(1, i));//
                cl2d_s_t.AddRange(Nodes[i].edge_color, new GH_Path(1, i));
                db2d_s_t.AddRange(Nodes[i].magnitude, new GH_Path(1, i));
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                ln2_Edge.Add(ln2d_s_t.Branch(1, i));
                st2_vn_Edge.Add(Nodes[i].NodeNode_ID);//[]
                st2_Edge.Add(st2d_s_t.Branch(1, i));
                cl2_Edge.Add(cl2d_s_t.Branch(1, i));
                db2_EdgeForce.Add(db2d_s_t.Branch(1, i));
            }

            F.ln1_Edge = ln1_Edge;
            F.st1_Edge = st1_Edge;
            F.cl1_Edge = cl1_Edge;
            F.db1_EdgeForce = db1_EdgeForce;
            F.ln2_Edge = ln2_Edge;
            F.st2_vn_Edge = st2_vn_Edge;
            F.st2_Edge = st2_Edge;
            F.cl2_Edge = cl2_Edge;
            F.db2_EdgeForce = db2_EdgeForce;
        }
    }

}
