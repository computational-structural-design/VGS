using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GraphicStatic
{
    public class From_Force_Evalution : GH_Component
    {
        public From_Force_Evalution()
          : base("3.Diagrams Evaluation (VGS)", "Diagrams Evaluation",
              "Evaluate the interdependency between form and force diagrams",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_05)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F", "F", "Form diagram", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddGenericParameter("F*", "F*", "Force diagram", GH_ParamAccess.item); Input[1].Optional = true;
            Input.AddNumberParameter("AngTol", "AngTol", "Tolerance for the evaluation of angles (rad)", GH_ParamAccess.item,0.02); Input[2].Optional = true;
            Input.AddNumberParameter("LenTol", "LenTol", "Tolerance for the evaluation of lengths (m)", GH_ParamAccess.item,0.001); Input[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddNumberParameter("Convergence", "Convergence", "Factor of convergence of the transformation", GH_ParamAccess.item);
            Output.AddTextParameter("UnparEdge", "UnparEdge", "List of indices of the pairs of corresponding edges in form and force diagrams that are not parallel after the transformation", GH_ParamAccess.list);
            Output.AddTextParameter("UndupEdge", "UndupEdge", "List of indices of the pairs of duplicated edges in the force diagrams that are not equivalent after the transformation", GH_ParamAccess.list);
            Output.AddNumberParameter("LoadPath", "LoadPath", "Total absolute load path of the structure (kNm)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL Fm = new MODEL(); MODEL Fc = new MODEL();
            if (!data.GetData(0, ref Fm)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: None Form Diagram Input"); return; }
            if (!data.GetData(1, ref Fc)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: None Force Diagram Input"); return; }
            if (Fm.GUI != Fc.GUI) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: The Input Form and Force are not matched"); return; };

            double ang_theshold = 0.02; double len_theshold = 0.001;
            data.GetData(2, ref ang_theshold);data.GetData(3, ref len_theshold);

            MODEL Fm_Diagram = Fm.CopySelf();
            MODEL Fc_Diagram = Fc.CopySelf();
            Form_Force_Evalution evaluate = new Form_Force_Evalution();

            evaluate.Run_Evalution(Fm_Diagram, Fc_Diagram, ang_theshold, len_theshold);
            data.SetData(0,evaluate.Global_convergence);
            data.SetDataList(1, evaluate.Edge_unmatch);
            data.SetDataList(2, evaluate.Dupli_unmatch);
            data.SetData(3, evaluate.loadpath);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return GraphicStatic.Properties.Resources.VGS_70;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("acf36631-3fe3-4d05-9780-e413eabd741b"); }//
        }
    }
}