using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GraphicStatic
{
    public class ExternalForces : GH_Component
    {
        public ExternalForces()
          : base("2.External Forces (VGS)", "External Forces",
              "Import the external forces (loads and reactions) of the structure from another plug-in",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddPointParameter("ExtForceAppPt", "ExtForceAppPt", "List of points of application of the external forces (loads and reactions)", GH_ParamAccess.list);Input[0].Optional = true;
            Input.AddVectorParameter("ExtForceVec", "ExtForceVec", "List of vectors representing the directions and magnitudes (kN) of the external forces (loads and reactions)", GH_ParamAccess.list); Input[1].Optional = true;
            Input.AddLineParameter("ExtForceLn", "ExtForceLn", "List of lines representing the points of application, directions and magnitudes (kN) of the external forces (loads and reactions)", GH_ParamAccess.list); Input[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("ExtForce", "ExtForce", "List of the external forces of the structure", GH_ParamAccess.item);
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
            data.SetData(0,load_set);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_62;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ca8b6eb7-c5cb-455e-a47a-4a257c602aa6"); }//
        }
    }
}