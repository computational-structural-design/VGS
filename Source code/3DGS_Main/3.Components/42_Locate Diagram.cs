using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using VGS_Main;

namespace GraphicStatic
{
    public class LocateDiagram : GH_Component
    {

        public LocateDiagram()
          : base("2.Locate Diagram (VGS)", "Locate Diagram",
              "Move the form or the force diagrams to a specific location of the Rhino viewport",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_05)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F|F*", "F|F*", "Form diagram or force diagram", GH_ParamAccess.item);
            Input.AddPointParameter("LocPt", "LocPt", "Point where the diagram should be located", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("F|F*", "F|F*", "Form diagram or force diagram", GH_ParamAccess.item);
        }

           protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL FD_input = new MODEL();
            if (!data.GetData(0, ref FD_input)) { return; }
            MODEL FD = FD_input.CopySelf();

            Point3d location = new Point3d(0,0,0);
            if (!data.GetData(1, ref location)) {return; }

            //if (FD.transformed == true) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "ERROR: The model should be located before the transformation"); return; }

            if (FD.model_status == "[ELASTIC Model]" || FD.model_status == "[PLASTIC Model]" || FD.model_status == "[Form Diagram]")
            {
                Point3d compt = FD.Diagram.extremPt_Fm();
                location = new Point3d(location - compt);
                Vector3d move = new Vector3d(location.X, location.Y, location.Z);
                FD.Diagram.MoveFm(move);
                Message = "Form Diagram";
            }
            else if (FD.model_status == "[Force Diagram]")
            {
                Point3d compt = FD.Diagram.extremPt_Fc();
                location = new Point3d(location - compt);
                Vector3d move = new Vector3d(location.X, location.Y, location.Z);
                FD.Diagram.MoveFc(move);
                Message = "Force Diagram";
            }

            data.SetData(0, FD);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_65;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("3341eca9-5f24-4b0a-8eda-d3d1ec89cba3"); }//
        }
    }
}