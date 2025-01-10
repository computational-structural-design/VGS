using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;

namespace GraphicStatic
{
    public class SelectNode: GH_Component
    {
        public SelectNode()
          : base("7.Select Nodes (VGS)", "Select Nodes",
              "Select specific nodal points in the diagrams",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F|F*", "F|F*", "Form diagram or force diagram", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddTextParameter("NodeID", "NodeID", "List of indices of the nodes to be selected", GH_ParamAccess.list); Input[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddPointParameter("NodePt", "NodePt", "List of points representing the selected nodes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL FD = new MODEL();
            if (!data.GetData(0, ref FD)) { return; }
            MODEL FM = FD;

            List<string> select_node = new List<string>();
            data.GetDataList(1, select_node);
            List<Point3d> selected = new List<Point3d>();
            if (FD.model_status == "[ELASTIC Model]" || FD.model_status == "[PLASTIC Model]" || FD.model_status == "[Form Diagram]")
            {
                List<Point3d> nodes = FD.Diagram.Nodes.Select(p => p.pt).ToList();
                List<string> nodes_id = FD.Diagram.Nodes.Select(p => p.ID).ToList();
                foreach (string id in select_node)
                {
                    int index = nodes_id.IndexOf(id);
                    if (index >= 0) { selected.Add(nodes[index]); }
                }
            }
            else if(FD.model_status == "[Force Diagram]")
            {
                List<List<Point3d>>FC_converge = FD.Diagram.FcConvergePts(FD.Diagram.Fc_Pts_Addr, FD.Diagram.ln2_Edge);
                foreach (string id in select_node)
                {
                    int index = System.Convert.ToInt32(id.Substring(1));
                    if (index < 0 || index > FC_converge.Count - 1) { continue; }
                    List<Point3d> nodes = FC_converge[index];
                    selected.Add(nodes[0]);
                }
            }

            data.SetDataList(0, selected);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_59;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("aba8eedb-b529-4974-9c5a-0f81bf0d94e6"); }//
        }
    }
}