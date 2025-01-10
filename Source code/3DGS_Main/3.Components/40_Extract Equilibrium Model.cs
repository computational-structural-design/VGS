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
    public class Extract_EqModel : GH_Component
    {

        public Extract_EqModel()
          : base("9.Extract Model (VGS)", "Extract Model",
              "Extract the data collected in the equilibrium model",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("Model", "Model", "Structural Equilibrium Model evaluated from the equilibrium matrix method", GH_ParamAccess.item);Input[0].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddLineParameter("EdgeLn", "EdgeLn", "List of lines representing the edges", GH_ParamAccess.list);//0
            Output.AddNumberParameter("EdgeMag", "EdgeMag", "List of force magnitudes in the edges (kN)", GH_ParamAccess.list);
            Output.AddPointParameter("NodePt", "NodePt", "List of points representing the nodes", GH_ParamAccess.list);//2
            Output.AddLineParameter("LoadLn", "LoadLn", "List of lines representing the applied loads", GH_ParamAccess.list);
            Output.AddNumberParameter("LoadMag", "LoadMag", "List of force magnitudes in the loads (kN)", GH_ParamAccess.list);
            Output.AddLineParameter("ReactionLn", "ReactionLn", "List of lines representing the reactions", GH_ParamAccess.list);
            Output.AddNumberParameter("ReactionMag", "ReactionMag", "List of force magnitudes in the reactions (kN)", GH_ParamAccess.list);
            Output.AddLineParameter("ResultantLn", "ResultantLn", "List of lines representing the resultants", GH_ParamAccess.list);
            Output.AddNumberParameter("ResultantMag", "ResultantMag", "List of force magnitudes in the resultant (kN)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL FD = new MODEL();
            if (!data.GetData("Model", ref FD)) { return; }
            MODEL FM = FD.CopySelf();

            //FM.Diagram.Edges;
            List<Line> edge_ln1 = new List<Line>();
            List<double> edge_db1 = new List<double>();

            foreach (EDGE edge in FM.Diagram.Edges)
            {
                edge_ln1.Add(edge.ln);
                edge_db1.Add(edge.force);
            }

            data.SetDataList(0, edge_ln1);
            data.SetDataList(1, edge_db1);

            //FM.Diagram.Nodes;
            List<Point3d> node_pt1 = new List<Point3d>();

            foreach (NODE node in FM.Diagram.Nodes)
            {
                int i = FM.Diagram.Nodes.IndexOf(node);
                node_pt1.Add(node.pt);
            }

            data.SetDataList(2, node_pt1);

            //FM.Diagram.Loads;
            List<Line> load_ln1 = new List<Line>();
            List<double> load_db1 = new List<double>();
            foreach (LOAD load in FM.Diagram.Loads)
            {
                load_ln1.Add(load.ln);
                load_db1.Add(load.ln.Length);
            }

            data.SetDataList(3, load_ln1);
            data.SetDataList(4, load_db1);

            //FM.Diagram.Reacts;
            List<Line> react_ln1 = new List<Line>();
            List<double> react_db1 = new List<double>();

            foreach (REACT react in FM.Diagram.Reacts)
            {
                react_ln1.Add(react.ln);
                react_db1.Add(react.ln.Length);
            }
   
            data.SetDataList(5, react_ln1);
            data.SetDataList(6, react_db1);

           //FM.Diagram.Result;

           List<Line> result_ln1 = new List<Line>();
           List<double> result_db1 = new List<double>();
           foreach (RESULT result in FM.Diagram.Result)
           {
               result_ln1.Add(result.ln);
               result_db1.Add(result.ln.Length);
           }

           data.SetDataList(7, result_ln1);
           data.SetDataList(8, result_db1);

        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_82;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dbee35d8-671c-4288-999b-44b147fcfa43"); }//
        }
    }
}