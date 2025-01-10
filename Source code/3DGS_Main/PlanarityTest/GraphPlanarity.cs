using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphPlanarityTesting;
using GraphPlanarityTesting.Graphs.DataStructures;

namespace Algorithm_3DGS
{
    public static class GraphPlanarity
    {
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
    }

}
