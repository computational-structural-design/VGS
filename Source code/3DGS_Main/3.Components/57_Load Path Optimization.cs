using System;
using System.Collections.Generic;
using VGS_Main;
using static VGS_Main.Diagram_Transformation_Kangaroo;
using Grasshopper.Kernel;
using Rhino.Geometry;
using KangarooSolver;
using KangarooSolver.Goals;

namespace GraphicStatic
{
    public class Loadpath_Optimization: GH_Component
    {
        public Loadpath_Optimization()
          : base("7.Load Path Optimization (VGS)", "Load Path Optimization",
              "Minimize the load path of the structure during the transformation",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_04)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F", "F", "Form diagram", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddGenericParameter("F*", "F*", "Force diagram", GH_ParamAccess.item); Input[1].Optional = true;
            Input.AddNumberParameter("Strength", "Strength", "Strength factor for the optimization", GH_ParamAccess.item, 0.02); Input[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("LP", "LP", "Load path optimization goal (Kangaroo2)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL Fm = new MODEL(); MODEL Fc = new MODEL();
            if (!data.GetData(0, ref Fm)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: None Form Diagram Input"); return; }
            if (!data.GetData(1, ref Fc)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: None Force Diagram Input"); return; }
            if (Fm.GUI != Fc.GUI) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: The Input Form and Force are not matched"); return; };
            double k = 0.02;
            data.GetData(2, ref k);
            MODEL Fm_Diagram = Fm.CopySelf();
            MODEL Fc_Diagram = Fc.CopySelf();
            TransfTempInfo tp =new TransfTempInfo();
            List<IGoal> Goal = Transformation_model.Transformation_LoadPath_Optimize(Fm_Diagram, Fc_Diagram, k,ref tp, System_Configuration.Sys_Tor);
            data.SetDataList(0, Goal);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_80;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("5ee455f3-22b8-46c6-b86e-02c990682644"); }//
        }
    }
}