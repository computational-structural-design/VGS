using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System.Drawing;
using static VGS_Main.VgsCommon;
namespace GraphicStatic
{
    public class Extract_ForceDiagram : GH_Component
    {
        public Extract_ForceDiagram()
          : base("9.Extract Force Diagram (VGS)", "Extract Force Diagram",
              "Extract the data collected in the force diagram",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F*", "F*", "Force diagram", GH_ParamAccess.item); Input[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {


            Output.AddTextParameter("EdgeID", "EdgeID", "List of indices of the edges", GH_ParamAccess.list);//0
            Output.AddLineParameter("EdgeLn", "EdgeLn", "List of lines representing the edges", GH_ParamAccess.list);
            Output.AddColourParameter("EdgeCol", "EdgesCol", "List of colors of the edges", GH_ParamAccess.list);
            Output.AddNumberParameter("EdgeMag", "EdgeMag", "List of force magnitudes in the edges (kN)", GH_ParamAccess.list);//3

            Output.AddTextParameter("EdgeID2", "EdgeID2", "List of indices of the edges grouped by force cycle", GH_ParamAccess.tree);//4
            Output.AddLineParameter("EdgeLn2", "EdgeLn2", "List of lines representing the edges grouped by force cycle", GH_ParamAccess.tree);
            Output.AddColourParameter("EdgeCol2", "EdgeCol2", "List of colors of the edges grouped by force cycle", GH_ParamAccess.tree);
            Output.AddNumberParameter("EdgeMag2", "EdgeMag2", "List of force magnitudes in the edges grouped by force cycle (kN)", GH_ParamAccess.tree);//7
        }
        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL FD = new MODEL();
            if (!data.GetData("F*", ref FD)) { return; }

            List<List<Line>> ln2_Edge= ListListCopy(FD.Diagram.ln2_Edge);
            List<List<string>> st2_Edge = ListListCopy(FD.Diagram.st2_Edge);
            List<List<Color>> cl2_Edge = ListListCopy(FD.Diagram.cl2_Edge);
            List<List<double>> db2_Edge = ListListCopy(FD.Diagram.db2_EdgeForce);
            
            FD.Diagram.align_Fc_1d_2d();
            FD.Diagram.Sort_Fc_1d();//Sort the fc d1 in alphabetical order

            List<Line> ln1_Edge = FD.Diagram.ln1_Edge;
            List<string> st1_Edge = FD.Diagram.st1_Edge;
            List<Color> cl1_Edge = FD.Diagram.cl1_Edge;
            List<double> db1_EdgeForce = FD.Diagram.db1_EdgeForce;

            DataTree<Line>tree_ln2_Edge=VgsCommon.ListToTree2D(ln2_Edge);
            DataTree<string> tree_st2_Edge = VgsCommon.ListToTree2D(st2_Edge);
            DataTree<Color> tree_cl2_Edge = VgsCommon.ListToTree2D(cl2_Edge);
            DataTree<double> tree_db2_Edge = VgsCommon.ListToTree2D(db2_Edge);

            data.SetDataTree(4, tree_st2_Edge);
            data.SetDataTree(5, tree_ln2_Edge);
            data.SetDataTree(6, tree_cl2_Edge);
            data.SetDataTree(7, tree_db2_Edge);

            data.SetDataList(0, st1_Edge);
            data.SetDataList(1, ln1_Edge);
            data.SetDataList(2, cl1_Edge);
            data.SetDataList(3, db1_EdgeForce);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_64;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("61d734e8-c16a-4b60-90e0-ad274efcb67c"); }//
        }
    }
}