using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GraphicStatic
{
    public class Assembling_Model : GH_Component
    {
        public Assembling_Model()
          : base("5.Structural Model (VGS)", "Structural Model",
              "Build the structural model based on edges, loads and supports",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_02)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("Edge", "Edge", "List of edges of the structure", GH_ParamAccess.list);
            Input[0].Optional = true;

            Input.AddGenericParameter("Load", "Load", "List of applied loads of the structure", GH_ParamAccess.list);
            Input[1].Optional = true;

            Input.AddGenericParameter("Support", "Support", "List of supports of the structure", GH_ParamAccess.list);
            Input[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("Model", "Model", "Structural model", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            Edge_line edges = new Edge_line();
            Load_line loads = new Load_line();
            Support_point supports = new Support_point();

            List<Edge_line> edges_input = new List<Edge_line>();
            List<Load_line> loads_input = new List<Load_line>();
            List<Support_point> supports_input = new List<Support_point>();

            data.GetDataList(0, edges_input);
            data.GetDataList(1, loads_input);
            data.GetDataList(2, supports_input);

            foreach (Edge_line edge in edges_input) { edges.AddData(edge.lines, edge.forces); };
            foreach (Load_line load in loads_input) { loads.AddData(load.lines); };
            foreach (Support_point support in supports_input) { supports.AddData(support.locate, support.status); };

            MODEL model = new MODEL();
            model.Import_class(edges, loads, supports);
            if (!model.Check_values()) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, model.report_error); model.model_status = "[not calculated]"; }
            else { model.BASIC_Model(); }
            

            data.SetData("Model", model);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return GraphicStatic.Properties.Resources.VGS_73;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("a06f5cdd-1f5c-47bc-b984-c29c47f0e3fd"); }//
        }
    }
}