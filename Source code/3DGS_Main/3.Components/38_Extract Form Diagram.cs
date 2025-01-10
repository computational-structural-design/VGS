using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System.Drawing;

namespace GraphicStatic
{
    public class Extract_FormDiagram : GH_Component
    {

        public Extract_FormDiagram()
          : base("8.Extract Form Diagram (VGS)", "Extract Form Diagram",
              "Extract the data collected in the form diagram",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F","F", "Form diagram", GH_ParamAccess.item);Input[0].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddTextParameter("EdgeID", "EdgeID", "List of indices of the edges", GH_ParamAccess.list);//0
            Output.AddLineParameter("EdgeLn", "EdgeLn", "List of lines representing the edges", GH_ParamAccess.list);
            Output.AddColourParameter("EdgeCol", "EdgeCol", "List of colors of the edges", GH_ParamAccess.list);
            Output.AddNumberParameter("EdgeMag", "EdgeMag", "List of force magnitudes in the edges (kN)", GH_ParamAccess.list);//3
            
            Output.AddTextParameter("NodeID", "NodesID", "List of indices of the node", GH_ParamAccess.list);//4
            Output.AddPointParameter("NodePt", "NodePt", "List of points representing the nodes", GH_ParamAccess.list);
            Output.AddTextParameter("NodeNodeAdj", "NodeNodeAdj", "List of connected nodes for each node", GH_ParamAccess.tree);
            Output.AddTextParameter("NodeEdgeAdj", "NodeEdgeAdj", "List of connected edges for each node", GH_ParamAccess.tree);//7

            Output.AddTextParameter("ExtForceID", "ExtForceID", "List of indices of the applied loads", GH_ParamAccess.list);//8
            Output.AddPointParameter("ExtForceAppPt", "ExtForceAppPt", "List of points of application of the applied loads", GH_ParamAccess.list);
            Output.AddTextParameter("ExtForceAppID", "ExtForceAppID", "List of indices of the points of application of the applied loads", GH_ParamAccess.list);           
            Output.AddLineParameter("ExtForceLn", "ExtForceLn", "List of lines representing the applied loads", GH_ParamAccess.list);
            Output.AddColourParameter("ExtForceCol", "ExtForceCol", "List of colors of the applied loads", GH_ParamAccess.list);//12
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL FD = new MODEL();
            if (!data.GetData("F", ref FD)) { return; }
            MODEL FM = FD.CopySelf();
            
            //FM.Diagram.Nodes;
            List<Point3d> node_pt1 = new List<Point3d>();
            List<string> node_st1 = new List<string>();
            List<Color> node_cl1 = new List<Color>();
            DataTree<string> nodenode_st1 = new DataTree<string>();
            DataTree<string> nodeedge_st1 = new DataTree<string>();

            foreach (NODE node in FM.Diagram.Nodes)
            {
                int i = FM.Diagram.Nodes.IndexOf(node);
                node_pt1.Add(node.pt);
                node_st1.Add(node.ID);
                node_cl1.Add(node.col);
                nodenode_st1.AddRange(node.adj_Node_ID, new GH_Path(i));
                nodeedge_st1.AddRange(node.adj_Edge_ID, new GH_Path(i));
            }
            data.SetDataList(4, node_st1);
            data.SetDataList(5, node_pt1);
            data.SetDataTree(6, nodenode_st1);
            data.SetDataTree(7, nodeedge_st1);

            //FM.Diagram.Edges;
            List<string> edge_st1 = new List<string>();
            List<Line> edge_ln1 = new List<Line>();
            List<Color> edge_cl1 = new List<Color>();
            List<double> edge_db1 = new List<double>();

            foreach (EDGE edge in FM.Diagram.Edges)
            {
                edge_st1.Add(edge.ID);
                edge_ln1.Add(edge.ln);
                edge_cl1.Add(edge.col);
                edge_db1.Add(edge.force);
            }

            data.SetDataList(0, edge_st1);
            data.SetDataList(1, edge_ln1);
            data.SetDataList(2, edge_cl1);
            data.SetDataList(3, edge_db1);

            //FM.Diagram.Loads;
            List<Point3d> load_pt1 = new List<Point3d>();
            List<string> loadnode_st1 = new List<string>();
            List<string> load_st1 = new List<string>();
            List<Line> load_ln1 = new List<Line>();
            List<Color> load_cl1 = new List<Color>();
            foreach (LOAD load in FM.Diagram.Loads)
            {
                load_pt1.Add(load.ln.From);
                loadnode_st1.Add(load.ActionPt_ID);
                load_st1.Add(load.ID);
                load_ln1.Add(load.ln);
                load_cl1.Add(load.col);
            }

            data.SetDataList(8, load_st1);
            data.SetDataList(9, load_pt1);
            data.SetDataList(10, loadnode_st1);  
            data.SetDataList(11, load_ln1);
            data.SetDataList(12, load_cl1);

        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_54;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("edd76ffb-ce5b-43be-bad7-ad2ba83d0469"); }//
        }
    }
}