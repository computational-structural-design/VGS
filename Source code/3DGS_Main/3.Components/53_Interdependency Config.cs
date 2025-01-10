using System;
using System.Collections.Generic;
using static VGS_Main.Diagram_Transformation_Kangaroo;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System.Drawing;
using GH_IO.Serialization;
using VGS_Main;

namespace GraphicStatic
{
    public class Reciprocity_Configuarion : GH_Component
    {
        public Reciprocity_Configuarion()
             : base("3.Interdependency Config (VGS)", "Interdependency Config",
                 "Configure the settings for the form-force diagrams interdependency",
                 System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_04)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddNumberParameter("ParallelStrength", "ParallelStrength", "Strength factor for imposing the parallelism between pairs of corresponding edges in form and force diagrams", GH_ParamAccess.item, 10); Input[0].Optional = true;
            Input.AddNumberParameter("DuplicateStrength", "DuplicateStrength", "Strength factor for imposing the equality between pairs of duplicated edges in the force diagram", GH_ParamAccess.item, 10); Input[1].Optional = true;
            Input.AddNumberParameter("CoincidentNodesStrength", "CoincidentNodesStrength", "Strength factor for imposing force cycles in the force diagram to be closed", GH_ParamAccess.item, 0.1); Input[2].Optional = true;
            Input.AddNumberParameter("ClosePolygonStrength", "ClosePolygonStrength", "Strength factor for imposing force cycles in the force diagram to be closed", GH_ParamAccess.item, 0.0); Input[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("InterConfig", "InterConfig", "Settings for the form-force diagrams interdependency", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            Reciprocal_force_config rc_config = new Reciprocal_force_config();
            double p_force = 10;
            double d_force = 10;
            double cN_force = 0.1;
            double cP_force = 0.0;
            data.GetData(0, ref p_force);
            data.GetData(1, ref d_force);
            data.GetData(2, ref cN_force);
            data.GetData(3, ref cP_force);

            rc_config.SetValues(p_force, d_force, cN_force, cP_force);
            data.SetData(0, rc_config);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_58;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("955a1b06-55a4-4868-be1f-9f6146c4827d"); }//
        }
    }
}