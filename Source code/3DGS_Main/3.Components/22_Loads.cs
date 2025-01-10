using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GraphicStatic
{
    public class Creat_Loads : GH_Component
    {
        public Creat_Loads()
          : base("2.Loads (VGS)", "Loads",
              "Set the loads applied to the structure",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_02)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddPointParameter("Pt", "Pt", "List of points of application of the loads", GH_ParamAccess.list);Input[0].Optional = true;
            Input.AddVectorParameter("Vec", "Vec", "List of vectors representing the directions and magnitudes (kN) of the loads", GH_ParamAccess.list); Input[1].Optional = true;
            Input.AddLineParameter("Ln", "Ln", "List of lines representing the points of application, directions and magnitudes (kN) of the loads", GH_ParamAccess.list); Input[2].Optional = true;
        }   

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("Load", "Load", "List of loads applied to the structure", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<Point3d> act_pt = new List<Point3d>();
            List<Line> ln = new List<Line>();
            List<Vector3d> vec = new List<Vector3d>();
            
            if (!data.GetDataList(0, act_pt)) { act_pt = new List<Point3d>(); }
            if (!data.GetDataList(1, vec)) { vec = new List<Vector3d>(); }
            if (!data.GetDataList(2, ln)) { ln = new List<Line>(); }

            Load_line load_set = new Load_line(ln,act_pt,vec);
            data.SetData(0, load_set);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return GraphicStatic.Properties.Resources.VGS_72;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8b6d9bc5-bcba-4547-b840-8dc836c1e8d3"); }
        }
    }
}