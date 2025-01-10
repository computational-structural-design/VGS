using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GraphicStatic
{
    public class Creat_Edges: GH_Component
    {

        public Creat_Edges()
          : base("1.Edges (VGS)", "Edges",
              "Set the edges of the structure",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_02)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddLineParameter("Ln", "Ln", "List of lines representing the edges of the structure", GH_ParamAccess.list);Input[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("Edge", "Edge", "List of edges that constitute the structure", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            Edge_line edges_set = new Edge_line();
            List<Line> line_set = new List<Line>();
            List<double> force_set = new List<double>();
            data.GetDataList("Ln", line_set);
            edges_set.AddData(line_set,force_set);
            data.SetData("Edge", edges_set);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return GraphicStatic.Properties.Resources.VGS_71;
            }
           
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("0eb0afa6-20b8-415e-b058-378d1b62bf22"); }
        }
    }
}