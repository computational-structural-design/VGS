using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using VGS_Main;

namespace GraphicStatic
{
    public class System_configuration : GH_Component
    {
        public System_configuration()
          : base("1.System Config (VGS)", "System Config",
              "Configure the system settings",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_01)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddNumberParameter("Tolerance", "Tolerance", "Reference tolerance value of the system", GH_ParamAccess.item,VGS_Main.System_Configuration.Sys_Tor);
            Input.AddNumberParameter("ScaleTextDisplay", "ScaleTextDisplay", "Scale factor for displaying text in the viewports", GH_ParamAccess.item, VGS_Main.System_Configuration.Text_scale);
            Input.AddIntegerParameter("MaxIteration", "MaxIteration", "Maximum number of iterations for the graph planarization algorithm", GH_ParamAccess.item, VGS_Main.System_Configuration.maxiteration);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            try { System_dynamic.temp.changing(); } catch (Exception) { }
            if (!data.GetData("Tolerance", ref System_Configuration.Sys_Tor)) { return; }
            if (!data.GetData("ScaleTextDisplay", ref System_Configuration.Text_scale)) { return; }
            if (!data.GetData("MaxIteration", ref System_Configuration.maxiteration)) { return; }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return GraphicStatic.Properties.Resources.VGS_76;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d49e55d1-53f7-47b5-9ee7-5287081347ff"); }
        }
    }
}