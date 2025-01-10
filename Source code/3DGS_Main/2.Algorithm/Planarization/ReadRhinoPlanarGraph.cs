using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace VGS_Main
{

    public static class ReadRhinoPlanarGraph
    {
        public static void ManualPlanarGraph(
            List<Guid> obj,
            Rhino.RhinoDoc doc,
            double tol,
            out DataTree<string> st2d_s,
            out List<string> st1_v,
            out List<string> st1_c,
            out List<string> st1_e,
            out DataTree<string> nN,
            out List<string> Ve,
            out List<Curve> edges,
            out List<string> edgesID,
            out List<Point3d> nodes,
            out List<string> nodesID
            )
        {

            string Report = "\n";
            List<Curve> im_edges;
            List<string> im_edgesID;
            List<Point3d> im_nodes;
            List<string> im_nodesID;
            ReadFromRhino(obj, doc, out im_edges, out im_edgesID, out im_nodes, out im_nodesID);
            CheckMultiEdge(ref im_edgesID);
            CheckOverlapNodes(ref im_nodes, ref im_nodesID, tol);

            List<Curve> oganized_edges;
            List<string> ogranized_edge_ids;
            List<Point3d> New_pts;
            GraphSelfIntersection(im_edges, im_edgesID, out oganized_edges, out ogranized_edge_ids, out New_pts, tol);

            im_nodes.AddRange(New_pts); im_nodesID.AddRange(New_pts.Select(p => "n").ToList());

            ReNameVmnEN(im_nodes, ref im_nodesID);

            List<string[]> Vnm;
            FindVnmID(oganized_edges, im_nodes, im_nodesID, out Vnm, tol);

            Report += string.Join("\n", Vnm.Select(p => string.Format("{3}:[{0},{1},{2}]", p[0], p[1], p[2], ogranized_edge_ids[Vnm.IndexOf(p)]))) + "\n";

            List<List<string>> Vnm_str1 = Vnm.Select(p => new List<string>() { p[0], p[1], p[2] }).ToList();
            List<List<string>> Vnm_ids = ogranized_edge_ids.Select(p => new List<string>() { p }).ToList();

            List<List<List<string>>> nN_infor = OrganizeLabels(ref Vnm_str1, ref Vnm_ids);

            //Sort the order of the edges:
            List<Curve> Vnm_cv;
            OrderEdgeWithVnm(Vnm, oganized_edges, Vnm_str1, out Vnm_cv);

            //This tims： Vnm_str1(Vnm);Vnm_ids(edgeID);Vnm_cv(Curves);
            Report += string.Join("\n", Vnm_str1.Select(p => string.Format("{3}:[{0},{1},{2}]", p[0], p[1], p[2], Vnm_ids[Vnm_str1.IndexOf(p)][0]))) + "\n";

            //Checking in clockwise order
            //ClockwiseSort(List<Curve>curves, List<string>curve_names,List<string[]>curve_vnms, Point3d central_node,out List<string[]>new_vnms,out List<string> new_names, double tol)
            // im_nodes;im_nodesID;
            List<List<List<string>>> circle_order;
            List<List<string>> circle_order_eN;
            MakeCircles(im_nodes, im_nodesID, Vnm_str1, Vnm_ids, Vnm_cv, out circle_order, out circle_order_eN, tol);

            Report += "\n[circle_order]\n" + string.Join("\n", circle_order.Select(p => string.Join("", p.Select(k => string.Format("[{0},{1},{2}]", k[0], k[1], k[2])))));
            Report += "\n[circle_order_eN]\n" + string.Join("\n", circle_order_eN.Select(p => string.Join(" ", p.ToArray())));

            PostManAlgorithm.ForceDiagramAssemblingOrder(circle_order, circle_order_eN, im_nodesID, Vnm_str1, Vnm_ids, out st2d_s, out st1_v, out st1_c, out st1_e, out Ve, false);

            //Nn
            nN = new DataTree<string>();
            for (int i = 0; i < nN_infor[0].Count; i++)
            {
                nN.AddRange(nN_infor[0][i], new GH_Path(i, 0));
                nN.AddRange(nN_infor[1][i], new GH_Path(i, 1));
            }

            edges = Vnm_cv;
            edgesID = Vnm_ids.Select(p => p[0]).ToList();
            nodes = im_nodes;
            nodesID = im_nodesID;

        }
        private static void ReadFromRhino(List<Guid> obj, Rhino.RhinoDoc doc, out List<Curve> edges, out List<string> edge_names, out List<Point3d> nodes, out List<string> node_names)
        {
            edges = new List<Curve>();
            edge_names = new List<string>();
            nodes = new List<Point3d>();
            node_names = new List<string>();

            foreach (Guid guid in obj)
            {
                if (guid == null)
                { continue; }

                Rhino.DocObjects.RhinoObject finded_obj = doc.Objects.Find(guid);
                string obj_type = finded_obj.Geometry.ObjectType.ToString();

                if (obj_type == "Curve")
                {
                    edges.Add((Rhino.Geometry.Curve)finded_obj.Geometry);
                    edge_names.Add(readtitle(finded_obj.Name));
                }
                else if (obj_type == "Point")
                {
                    nodes.Add(new Point3d(((Rhino.Geometry.Point)finded_obj.Geometry).Location));
                    if (finded_obj.Name != null) { node_names.Add(readtitle(finded_obj.Name)); }
                    else { node_names.Add("n"); }
                }
            }
        }
        private static void CheckMultiEdge(ref List<string> edge_names)
        {
            List<string> LIBeN = new List<string>();
            List<List<int>> SameNameCluster = new List<List<int>>();
            for (int i = 0; i < edge_names.Count - 1; i++)
            {
                List<int> sameNameInd = new List<int>() { i };
                string eN = edge_names[i];
                if (LIBeN.Contains(eN)) { continue; } else { LIBeN.Add(eN); }
                for (int j = i + 1; j < edge_names.Count; j++)
                {
                    if (eN == edge_names[j]) { sameNameInd.Add(j); }
                }
                SameNameCluster.Add(sameNameInd);
            }

            for (int i = 0; i < SameNameCluster.Count; i++)
            {
                for (int j = 0; j < SameNameCluster[i].Count; j++)
                {
                    if (SameNameCluster[i].Count > 1)
                    {
                        edge_names[SameNameCluster[i][j]] += "+";
                    }
                }
            }
        }
        private static bool CheckOverlapNodes(ref List<Point3d> nodes, ref List<string> node_names, double tol)
        {
            bool valid = true;

            List<Point3d> LIBpt = new List<Point3d>();
            List<string> LIBptID = new List<string>();
            List<List<Point3d>> SamePtCluster = new List<List<Point3d>>();
            List<List<string>> SamePtClusterID = new List<List<string>>();

            List<int> record_multi_index = new List<int>();
            bool addbackLastNode = true;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                bool find = false;
                List<Point3d> sPt = new List<Point3d>() { nodes[i] };
                List<string> sPtID = new List<string>() { node_names[i] };
                Point3d P0 = nodes[i];
                if (ContainDupliNode(P0, LIBpt, tol)) { continue; } else { LIBpt.Add(nodes[i]); LIBptID.Add(node_names[i]); }
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    Point3d P1 = nodes[j];
                    if (P1.DistanceTo(P0) < tol)
                    {
                        sPt.Add(P1); sPtID.Add(node_names[j]); find = true;
                        record_multi_index.Add(LIBpt.Count - 1);
                        if (j == nodes.Count - 1) { addbackLastNode = false; }
                    }
                }
                if (find)
                {
                    SamePtCluster.Add(sPt);
                    SamePtClusterID.Add(sPtID);
                }
            }
            if (addbackLastNode) { LIBpt.Add(nodes[nodes.Count - 1]); LIBptID.Add(node_names[nodes.Count - 1]); }

            List<Point3d> pts = new List<Point3d>();
            List<string> ptsID = new List<string>();
            for (int i = 0; i < LIBpt.Count; i++)
            {
                if (record_multi_index.Contains(i)) { continue; }
                pts.Add(LIBpt[i]);
                ptsID.Add(LIBptID[i]);
            }

            for (int i = 0; i < SamePtCluster.Count; i++)
            {
                List<Point3d> sPts = SamePtCluster[i];
                List<string> sPtsID = SamePtClusterID[i];
                string name = "n";
                int vnum = 0;
                foreach (string id in sPtsID) { if (id.Substring(0, 1) == "v") { name = id; vnum++; } }
                if (vnum < 2) { pts.Add(sPts[0]); ptsID.Add(name); }
                else { valid = false; }//Two vn overlapping；
            }

            nodes = new List<Point3d>(pts);
            node_names = new List<string>(ptsID);

            return valid;
        }
        private static void MakeCircles(List<Point3d> im_nodes, List<string> im_nodesID, List<List<string>> Vnm_str1, List<List<string>> Vnm_ids, List<Curve> Vnm_cv, out List<List<List<string>>> circle_order, out List<List<string>> circle_order_eN, double tol)
        {
            circle_order = new List<List<List<string>>>();
            circle_order_eN = new List<List<string>>();
            for (int i = 0; i < im_nodes.Count; i++)
            {
                Point3d node = im_nodes[i];
                string nodeID = im_nodesID[i];

                List<Curve> adj_cvs = new List<Curve>();
                List<string> adj_cv_ids = new List<string>();
                List<string[]> adj_vnm = new List<string[]>();

                for (int j = 0; j < Vnm_str1.Count; j++)
                {
                    string[] vnm = new string[] { Vnm_str1[j][0], Vnm_str1[j][1], Vnm_str1[j][2] };
                    if (nodeID == vnm[0])
                    {
                        adj_cvs.Add(Vnm_cv[j]);
                        adj_cv_ids.Add(Vnm_ids[j][0]);
                        adj_vnm.Add(vnm);
                    }
                    else if (nodeID == vnm[1])
                    {
                        adj_cvs.Add(Vnm_cv[j]);
                        adj_cv_ids.Add(Vnm_ids[j][0]);
                        adj_vnm.Add(new string[] { vnm[1], vnm[0], vnm[2] });
                    }
                }
                List<string[]> circle_vnm;
                List<string> circle_cv_ids;

                ClockwiseSort(adj_cvs, adj_cv_ids, adj_vnm, node, out circle_vnm, out circle_cv_ids, tol);

                circle_order.Add(circle_vnm.Select(p => p.ToList()).ToList());
                circle_order_eN.Add(circle_cv_ids);
            }
        }
        public static List<List<List<string>>> OrganizeLabels(ref List<List<string>> edgeVnm, ref List<List<string>> edgeIDs)
        {
            string Report = "\n[OrganizeLabels]\n";

            List<List<string>> r_edgeVnm = new List<List<string>>();//没有被标记加号的组
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
            List<List<List<string>>> LIB_vnm = new List<List<List<string>>>();//同边成组
            List<List<List<string>>> LIB_id = new List<List<List<string>>>();//同边成组
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
        private static void Sequence(ref List<List<string>> vnms, ref List<List<string>> ids)
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
        private static void ClockwiseSort(List<Curve> curves, List<string> curve_names, List<string[]> curve_vnms, Point3d central_node, out List<string[]> new_vnms, out List<string> new_names, double tol)
        {
            List<Vector3d> tan = new List<Vector3d>();
            List<double> atan = new List<double>();

            foreach (Curve curve in curves)
            {
                if (ComparePts(central_node, curve.PointAtStart, tol))
                {
                    tan.Add(curve.TangentAtStart);
                    atan.Add(Math.Atan(curve.TangentAtStart.Y / curve.TangentAtStart.X));
                }
                else if (ComparePts(central_node, curve.PointAtEnd, tol))
                {
                    if (curve.IsLinear())
                    {
                        Vector3d temp = new Vector3d(curve.PointAtStart - curve.PointAtEnd);
                        temp.Unitize();
                        tan.Add(temp);
                        atan.Add(Math.Atan(temp.Y / temp.X));
                    }
                    else
                    {
                        Vector3d temp = -curve.TangentAtEnd;
                        tan.Add(temp);
                        atan.Add(Math.Atan(temp.Y / temp.X));
                    }
                }
            }

            for (int i = 0; i < tan.Count; i++)
            {
                if (tan[i].X >= 0)
                { atan[i] += Math.PI; }
            }

            List<Curve> new_curves = curves.Select(p => p).ToList();
            new_curves.Sort((p1, p2) => atan[curves.IndexOf(p1)].CompareTo(atan[curves.IndexOf(p2)]));
            new_curves.Reverse();

            new_vnms = new_curves.Select(p => curve_vnms[curves.IndexOf(p)]).ToList();
            new_names = new_curves.Select(p => curve_names[curves.IndexOf(p)]).ToList();
        }
        private static void OrderEdgeWithVnm(List<string[]> ori_vnm, List<Curve> ori_edge, List<List<string>> reordered_vnm, out List<Curve> reordered_edge)
        {
            reordered_edge = new List<Curve>();
            foreach (List<string> vnm1 in reordered_vnm)
            {
                for (int i = 0; i < ori_vnm.Count; i++)
                {
                    string[] vnm0 = ori_vnm[i];
                    if ((vnm1[0] == vnm0[0] && vnm1[1] == vnm0[1] && vnm1[2] == vnm0[2]) || (vnm1[0] == vnm0[1] && vnm1[1] == vnm0[0] && vnm1[2] == vnm0[2])) { reordered_edge.Add(ori_edge[i]); }
                }
            }
        }
        private static void FindVnmID(List<Curve> edges, List<Point3d> pts, List<string> pt_ids, out List<string[]> Vnm, double tol)
        {
            string ve = "ve";
            string Report = "\n[FindVnmID]:\n";
            Vnm = new List<string[]>();
            List<string> LIB = new List<string>();
            foreach (Curve edge in edges)
            {
                string id0 = "";
                string id1 = "";
                for (int i = 0; i < pts.Count; i++)
                {
                    Point3d pt = pts[i];
                    string id = pt_ids[i];
                    if (edge.PointAtStart.DistanceTo(pt) < tol) { id0 = id; }
                    if (edge.PointAtEnd.DistanceTo(pt) < tol) { id1 = id; }
                    if (id0 != "" && id1 != "") { break; }
                }

                if (id0 != "" && id1 != "")
                {
                    string check = string.Format("[{0},{1}]", id0, id1);
                    int dupli = LIB.Where(p => p == check).ToList().Count;
                    LIB.Add(string.Format("[{0},{1}]", id0, id1));
                    LIB.Add(string.Format("[{1},{0}]", id0, id1));

                    Vnm.Add(new string[] { id0, id1, dupli.ToString() });
                }
                else if (id0== "" && id1 != "")
                {
                    string check = string.Format("[{0},{1}]", id1,ve);
                    int dupli = LIB.Where(p => p == check).ToList().Count;
                    LIB.Add(string.Format("[{0},{1}]", id1, ve));
                    LIB.Add(string.Format("[{1},{0}]", id1, ve));

                    Vnm.Add(new string[] { id1, ve, dupli.ToString() });
                }
                else if (id0 != "" && id1 == "")
                {
                    string check = string.Format("[{0},{1}]", id0, ve);
                    int dupli = LIB.Where(p => p == check).ToList().Count;
                    LIB.Add(string.Format("[{0},{1}]", id0, ve));
                    LIB.Add(string.Format("[{1},{0}]", id0, ve));

                    Vnm.Add(new string[] { id0, ve, dupli.ToString() });
                }
                else
                { Report += "unMatched Edge index:" + edges.IndexOf(edge).ToString(); }
            }
        }
        private static void ReNameVmnEN(List<Point3d> nodes, ref List<string> node_ids)
        {
            int count = 0;
            bool rename = false;
            foreach (string name in node_ids)
            {
                if (name.Substring(0, 1) == "v" && name.Substring(1, 1) != "e") { int num = System.Convert.ToInt32(name.Substring(1)); if (num > count) { count = num; } }
                if (name == "n") { rename = true; }
            }

            count++;
            if (rename)
            {
                for (int i = 0; i < node_ids.Count; i++)
                { if (node_ids[i].Substring(0, 1) == "n") { node_ids[i] = "n" + count.ToString(); count++; } }
            }
        }
        private static void GraphSelfIntersection(List<Curve> edges, List<string> edge_ids, out List<Curve> oganized_edges, out List<string> ogranized_edge_ids, out List<Point3d> new_pts, double tol)
        {
            oganized_edges = new List<Curve>();
            ogranized_edge_ids = new List<string>();
            new_pts = new List<Point3d>();

            List<Curve> nonlnt_edges = new List<Curve>();
            List<string> nonlnt_edge_ids = new List<string>();
            List<Curve> Int_edges = new List<Curve>();
            List<string> lnt_edge_ids = new List<string>();

            shrinkGraph(edges, edge_ids, out nonlnt_edges, out nonlnt_edge_ids, out Int_edges, out lnt_edge_ids, tol);

            oganized_edges.AddRange(nonlnt_edges);
            ogranized_edge_ids.AddRange(nonlnt_edge_ids);

            int protect = 1;
            //loop
            while (true)
            {
                List<Curve> c_lnt_edges = new List<Curve>();
                List<string> c_lnt_edge_ids = new List<string>();
                List<Point3d> new_pts_sub = new List<Point3d>();

                SplitGraph(Int_edges, lnt_edge_ids, out c_lnt_edges, out c_lnt_edge_ids, out new_pts_sub, tol);
                new_pts.AddRange(new_pts_sub);

                shrinkGraph(c_lnt_edges, c_lnt_edge_ids, out nonlnt_edges, out nonlnt_edge_ids, out Int_edges, out lnt_edge_ids, tol);

                oganized_edges.AddRange(nonlnt_edges);
                ogranized_edge_ids.AddRange(nonlnt_edge_ids);

                protect++;
                if (Int_edges.Count == 0 || protect > 10000) { break; }
            }


        }
        private static void SplitGraph(List<Curve> Int_edges, List<string> lnt_edge_ids, out List<Curve> changed_edges, out List<string> changed_edge_ids, out List<Point3d> Pts_new, double tol)
        {
            changed_edges = new List<Curve>(Int_edges);
            changed_edge_ids = new List<string>(lnt_edge_ids);

            Curve[] cv0_new = new Curve[] { };
            Curve[] cv1_new = new Curve[] { };
            Pts_new = new List<Point3d>();

            if (Int_edges.Count == 0) { return; }

            Curve C0 = Int_edges[0];
            string C0_id = lnt_edge_ids[0];

            for (int i = 1; i < Int_edges.Count; i++)
            {
                SplitCurves(C0, Int_edges[i], out cv0_new, out cv1_new, out Pts_new, tol);

                if (Pts_new.Count > 0)
                {
                    changed_edges.RemoveAt(i); changed_edge_ids.RemoveAt(i);
                    List<string> c1_new_ids = cv1_new.ToList().Select(p => lnt_edge_ids[i] + "+").ToList();
                    changed_edges.InsertRange(i, cv1_new.ToList()); changed_edge_ids.InsertRange(i, c1_new_ids);

                    changed_edges.RemoveAt(0); changed_edge_ids.RemoveAt(0);
                    List<string> c0_new_ids = cv0_new.ToList().Select(p => lnt_edge_ids[0] + "+").ToList();
                    changed_edges.InsertRange(0, cv0_new.ToList()); changed_edge_ids.InsertRange(0, c0_new_ids);

                    break;
                }
            }
        }
        private static void shrinkGraph(List<Curve> edges, List<string> edge_ids, out List<Curve> nonInt_edges, out List<string> nonlnt_edge_ids, out List<Curve> Int_edges, out List<string> lnt_edge_ids, double tol) //首先去掉已经不相交的边
        {
            nonlnt_edge_ids = new List<string>();
            lnt_edge_ids = new List<string>();
            nonInt_edges = new List<Curve>();
            Int_edges = new List<Curve>();
            for (int i = 0; i < edges.Count; i++)
            {
                bool cross = false;
                for (int j = 0; j < edges.Count; j++)
                {
                    if (i == j) { continue; }
                    Curve[] cv0_new;
                    Curve[] cv1_new;
                    List<Point3d> Pts_new;
                    SplitCurves(edges[i], edges[j], out cv0_new, out cv1_new, out Pts_new, tol);
                    if (Pts_new.Count > 0) { cross = true; break; }
                }
                if (cross) { Int_edges.Add(edges[i]); lnt_edge_ids.Add(edge_ids[i]); }
                else { nonInt_edges.Add(edges[i]); nonlnt_edge_ids.Add(edge_ids[i]); }
            }
        }
        private static List<List<double>> IntersectCurves(Curve cv0, Curve cv1, double tol)
        {
            List<double> intersectLocates_cv0 = new List<double>();
            List<double> intersectLocates_cv1 = new List<double>();
            var events = Rhino.Geometry.Intersect.Intersection.CurveCurve(cv0, cv1, tol, tol);
            for (int i = 0; i < events.Count; i++)
            {
                var ccx_event = events[i];
                intersectLocates_cv0.Add(ccx_event.ParameterA);
                intersectLocates_cv1.Add(ccx_event.ParameterB);
            }
            List<List<double>> intersect_locations = new List<List<double>>() { intersectLocates_cv0, intersectLocates_cv1 };
            return intersect_locations;
        }
        private static bool[] SplitCurves(Curve cv0, Curve cv1, out Curve[] cv0_new, out Curve[] cv1_new, out List<Point3d> Pts_new, double tol)
        {

            Pts_new = new List<Point3d>();
            Curve C0 = (Curve)cv0.Duplicate();
            Curve C1 = (Curve)cv1.Duplicate();
            List<List<double>> intersect_locations = IntersectCurves(C0, C1, tol);
            List<double> cut0 = intersect_locations[0];
            List<double> cut1 = intersect_locations[1];

            Curve[] C0s = C0.Split(cut0);
            Curve[] C1s = C1.Split(cut1);

            bool cv0_cross = false;
            bool cv1_cross = false;
            if (C0s.Length > 1) { cv0_cross = true; }
            if (C1s.Length > 1) { cv1_cross = true; }

            Pts_new.AddRange(cut0.Select(p => C0.PointAt(p)).ToList());
            List<Point3d> extremPts = new List<Point3d>() { cv0.PointAtStart, cv0.PointAtEnd, cv1.PointAtStart, cv1.PointAtEnd };
            Pts_new = Pts_new.Where(p => !ContainDupliNode(p, extremPts, tol)).ToList();

            if (C0s.Length == 0) { cv0_new = new Curve[] { C0 }; } else { cv0_new = C0s; }
            if (C1s.Length == 0) { cv1_new = new Curve[] { C1 }; } else { cv1_new = C1s; }

            return new bool[] { cv0_cross, cv1_cross };
        }
        private static bool ContainDupliNode(Point3d pt, List<Point3d> pts, double tol)
        {
            foreach (Point3d P in pts)
            {
                if (P.DistanceTo(pt) < tol) { return true; }
            }
            return false;
        }
        private static string readtitle(string Input)
        {
            return Input.Split(new char[] { '_' })[0];
        }
        private static string RemoveLabel(List<string> ID)
        {
            string temp;
            string[] names = ID[0].Split('+');
            temp = names[0];

            return temp;
        }
        private static bool ComparePts(Point3d Pt_1, Point3d Pt_2, double Threshold)
        {
            if (System.Math.Abs(Pt_1.X - Pt_2.X) < Threshold && System.Math.Abs(Pt_1.Y - Pt_2.Y) < Threshold && System.Math.Abs(Pt_1.Z - Pt_2.Z) < Threshold)
            { return true; }
            else
            { return false; }
        }
    }
}
