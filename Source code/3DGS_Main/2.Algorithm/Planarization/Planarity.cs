using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphPlanarityTesting;
using GraphPlanarityTesting.Graphs.DataStructures;

namespace VGS_Main
{
    /// <summary>
    /// reference
    /// </summary>
    public static class Planarity
    {
        public static List<string> IO_DetectVericesList(List<List<string>> Vnms)
        {
            List<int> key = new List<int>();
            List<string> vlist = new List<string>();

            List<string> allVn = new List<string>();
            for (int i = 0; i < Vnms.Count; i++)
            {
                string vn = Vnms[i][0];
                string vm= Vnms[i][1];
                if (!allVn.Contains(vn)) {allVn.Add(vn);}
                if (!allVn.Contains(vm)) { allVn.Add(vm);}
            }


            for (int i = 0; i < allVn.Count; i++)
            {

                int index = key.Count;
                string num = allVn[i].Substring(1);
                int k = int.MaxValue;
                if (num != "e") { k = System.Convert.ToInt32(num); }
                for (int j = 0; j < key.Count; j++)
                {
                    if (k > key[j]) { continue; }
                    index = j; break;
                }

                key.Insert(index, k); vlist.Insert(index, allVn[i]);
            }

            return vlist;

        }
        public static bool PlanarityTest(List<string> Vertices, List<string[]> Edges, out List<List<string[]>> embeded_circle)
        {
            var Test = new GraphPlanarityTesting.PlanarityTesting.BoyerMyrvold.BoyerMyrvold<string>();
            IGraph<string> graph = new UndirectedAdjacencyListGraph<string>();

            foreach (string v in Vertices) { graph.AddVertex(v); }
            foreach (string[] e in Edges) { graph.AddEdge(e[0], e[1]); }

            GraphPlanarityTesting.PlanarityTesting.BoyerMyrvold.PlanarEmbedding<string> embedding;

            //Test Planarity
            bool isPlanar = Test.IsPlanar(graph, out embedding);

            //Embedding info
            embeded_circle = new List<List<string[]>>();
            if (isPlanar == true)
            {
                foreach (string v in Vertices)
                {

                    List<string[]> sub_circle = new List<string[]>();

                    List<IEdge<string>> edge_list = embedding.GetEdgesAroundVertex(v);
                    foreach (IEdge<string> edge in edge_list)
                    {
                        string v0 = edge.Source;
                        string v1 = edge.Target;

                        if (v == v0) { sub_circle.Add(new string[] { v0, v1 }); }
                        else { sub_circle.Add(new string[] { v1, v0 }); }

                    }

                    embeded_circle.Add(sub_circle);
                }
            }

            return isPlanar;
        }
        public static void MaxPlanarGraph_E(List<string> Vertices, List<string[]> Edges, out List<string[]> MPlanarG, out List<string[]> AddBackG)
        {
            //initial Edges
            List<string[]> PlanarEdges = new List<string[]>();
            List<string[]> AddBackEdges = new List<string[]>();

            //Check external edges and them into inital planar set
            foreach (string[] edge in Edges)
            {
                if (edge[0] == "ve" || edge[1] == "ve") { PlanarEdges.Add(edge); }
                else { AddBackEdges.Add(edge); }
            }

            //EdgeIncrimental method
            List<string[]> LeftoverEdges = new List<string[]>();
            foreach (string[] edge in AddBackEdges)
            {
                List<string[]> temp_PlanarEdges = PlanarEdges.Select(p => new string[] { p[0], p[1] }).ToList();
                temp_PlanarEdges.Add(edge);
                List<List<string[]>> embeded_circle;
                if (PlanarityTest(Vertices, temp_PlanarEdges, out embeded_circle))
                { PlanarEdges = temp_PlanarEdges.Select(p => new string[] { p[0], p[1] }).ToList(); }
                else { LeftoverEdges.Add(edge); }
            }

            //Output results
            MPlanarG = PlanarEdges;
            AddBackG = LeftoverEdges;
        }
        public static void MaxPlanarGraph_V(List<string> Vertices, List<string[]> Edges, out List<string[]> MPlanarG, out List<string[]> AddBackG)
        {
            //initial Edges
            List<string[]> PlanarEdges = new List<string[]>();
            List<string[]> AddBackEdges = new List<string[]>();

            //Check external edges and them into inital planar set : ***We can put ve in the first index to do this***
            foreach (string[] edge in Edges)
            {
                if (edge[0] == "ve" || edge[1] == "ve") { PlanarEdges.Add(edge); }
                else { AddBackEdges.Add(edge); }
            }

            //Put make Vertices-Edge corresponding list; VE_Matrix
            List<string>connection_lib=new List<string>(); foreach(string[] e in Edges) { connection_lib.AddRange(new List<string>() { e[0] +e[1] , e[1] + e[0] }); }

            List<List<string[]>> VE_Matrix = new List<List<string[]>>();
            foreach (string v0 in Vertices)
            {
                List<string[]> adj_edge = new List<string[]>(Vertices.Count);
                foreach (string v1 in Vertices)
                {
                    if (connection_lib.Contains(v0 + v1) || connection_lib.Contains(v1 + v0)) { adj_edge.Add(new string[] {v0,v1}); connection_lib.Remove(v0+v1); connection_lib.Remove(v1+v0); }
                    else { adj_edge.Add(new string[] { }); }
                }
                VE_Matrix.Add(adj_edge);
            }

            List<string[]> LeftoverEdges = new List<string[]>();
            //VertexIncrimental method
            for (int i = 0; i < Vertices.Count; i++)
            {
                string v=Vertices[i];
                List<string[]>temp_addedge= VE_Matrix[i].Where(p => p.Length > 0).ToList(); 
                List<string[]>testList= PlanarEdges.Select(p => new string[] { p[0], p[1] }).ToList();
                testList.AddRange(temp_addedge);

                List<List<string[]>> embeded_circle;
                if (PlanarityTest(Vertices, testList, out embeded_circle)) { PlanarEdges.AddRange(temp_addedge); }
                else 
                {
                    foreach (string[] e in temp_addedge)
                    {
                        testList = PlanarEdges.Select(p => new string[] { p[0], p[1] }).ToList(); testList.Add(e);
                        if (PlanarityTest(Vertices, testList, out embeded_circle)) { PlanarEdges.Add(e); } else { LeftoverEdges.Add(e);}
                    }
                }
            }

            //Output results
            MPlanarG = PlanarEdges;
            AddBackG = LeftoverEdges;
        }

    }
}
