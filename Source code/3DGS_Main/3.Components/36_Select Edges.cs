using System;
using System.Collections.Generic;
using VGS_Main;
using static VGS_Main.VgsCommon;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;

namespace GraphicStatic
{
    public class SelectEdge: GH_Component
    {
        public SelectEdge()
          : base("6.Select Edges (VGS)", "Select Edges",
              "Select specific edge lines in the diagrams",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F|F*", "F|F*", "Form diagram or force diagram", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddTextParameter("EdgeID", "EdgeID", "List of indices of the edges to be selected", GH_ParamAccess.list); Input[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddLineParameter("EdgeLn", "EdgeLn", "List of lines representing the selected edges", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL FD = new MODEL();
            if (!data.GetData(0, ref FD)) { return; }
            MODEL FM = FD;

            List<string> select_edge = new List<string>();
            data.GetDataList(1, select_edge);
            List<Line> selected = new List<Line>();


            if (FD.model_status == "[ELASTIC Model]" || FD.model_status == "[PLASTIC Model]" || FD.model_status == "[Form Diagram]")
            {
                List<Line> Fm_ln1_e = FD.Diagram.Edges.Select(p => p.ln).ToList();
                List<string> Fm_st1_e = FD.Diagram.Edges.Select(p => p.ID).ToList();

                List<Line> Fm_ln1_l = FD.Diagram.Loads.Select(p => p.ln).ToList();
                List<string> Fm_st1_l = FD.Diagram.Loads.Select(p => p.ID).ToList();

                List<Line> Fm_ln1_r = FD.Diagram.Reacts.Select(p => p.ln).ToList();
                List<string> Fm_st1_r = FD.Diagram.Reacts.Select(p => p.ID).ToList();

                List<Line> Fm_ln1 = new List<Line>(); Fm_ln1.AddRange(Fm_ln1_e); Fm_ln1.AddRange(Fm_ln1_l); Fm_ln1.AddRange(Fm_ln1_r);
                List<string> Fm_st1 = new List<string>(); Fm_st1.AddRange(Fm_st1_e); Fm_st1.AddRange(Fm_st1_l); Fm_st1.AddRange(Fm_st1_r);

                foreach (string id in select_edge)
                {
                    int index = Fm_st1.IndexOf(id);
                    if (index >= 0) { selected.Add(Fm_ln1[index]); }
                }
            }
            else if(FD.model_status == "[Force Diagram]")
            {
                List<Line> Fc_ln1 = Flattern2D(FD.Diagram.ln2_Edge);
                List<string> Fc_st1 = Flattern2D(FD.Diagram.st2_Edge);
                foreach (string id in select_edge)
                {
                    if (Fc_st1.Contains(id))
                    {
                        for (int i = 0; i < Fc_st1.Count; i++)
                        {
                            if (Fc_st1[i] == id) { selected.Add(Fc_ln1[i]); }
                        }
                    }
                }
            }

            data.SetDataList(0, selected);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_78;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("96a0473a-4006-4b9f-a196-02a7fb4d54ab"); }//
        }
    }
}