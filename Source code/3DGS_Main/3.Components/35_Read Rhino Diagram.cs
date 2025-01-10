using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino.DocObjects;
using static VGS_Main.VgsCommon;
using System.Windows.Forms;
using Rhino;
using System.Linq;

namespace GraphicStatic
{
    public class ReadRhino_ForceDiagram : GH_Component
    {
        public double db0_Line_Thick = 0.1;
        public double text_size = System_Configuration.Text_scale;

        public List<Curve> edges = new List<Curve>();
        public List<string> edgesID = new List<string>();
        public List<Point3d> nodes = new List<Point3d>();
        public List<string> nodesID = new List<string>();


        public FcDiagramAssembleData Saved_Tp;
        public List<List<int[]>> Pts_ddr;
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            Plane pl0_Plane;
            args.Viewport.GetFrustumFarPlane(out pl0_Plane);
            for (int i = 0; i < nodes.Count; i++)
            {
                DrawNode(pl0_Plane, nodesID[i], nodes[i],Color.Black,args);
            }

            for (int i = 0; i < edges.Count; i++)
            {
                DrawText(pl0_Plane, edgesID[i], edges[i].PointAtLength(edges[i].GetLength()*0.6), Color.Black,args);
            }
         
        }
        public ReadRhino_ForceDiagram()
             : base("5.Read Rhino Diagram (VGS)", "Read Rhino Diagram",
                 "Read a manually planarized graph of the form diagram to generate the force diagram",
                 System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F", "", "Form diagram", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddGenericParameter("ObjGuids", "", "Rhino geometries representing a manually planarized graph of the form diagram", GH_ParamAccess.list);
            Input.AddBooleanParameter("run", "", "", GH_ParamAccess.item,false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("F*", "", "Force diagram", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            bool Run = false;
            if (!data.GetData(2, ref Run)) { return; }

            MODEL Fm = new MODEL();
            if (!data.GetData(0, ref Fm)) { return; }
            MODEL FD = Fm.CopySelf();

            if (Run)
            {
                edges = new List<Curve>();
                edgesID = new List<string>();
                nodes = new List<Point3d>();
                nodesID = new List<string>();
                List<Guid> guids = new List<Guid>();
                if (!data.GetDataList(1, guids)) { return; }
                ReadRhinoPlanarGraph.ManualPlanarGraph(
                 guids,
                 Rhino.RhinoDoc.ActiveDoc,
                 System_Configuration.Sys_Tor,
                 out DataTree<string> st2d_s,
                 out List<string> st1_v,
                 out List<string> st1_c,
                 out List<string> st1_e,
                 out DataTree<string> nN,
                 out List<string> VE,
                 out edges,
                 out edgesID,
                 out nodes,
                 out nodesID
                 );

                
                DataTree<string> st2d_tree = new DataTree<string>();
                for (int i = 0; i < st2d_s.Branches.Count; i++)
                {
                    for (int j = 0; j < st2d_s.Branches[i].Count; j++)
                    {
                        st2d_tree.Add(st2d_s.Branches[i][j].ToString(), new GH_Path(1, i));
                    }
                }
                DataTree<string> nN_tree = new DataTree<string>();
                for (int i = 0; i < nN.Branches.Count; i += 2)
                {
                    for (int j = 0; j < nN.Branches[i].Count; j++)
                    {
                        nN_tree.Add(nN.Branches[i][j].ToString(), new GH_Path(i / 2, 0));
                    }
                    for (int j = 0; j < nN.Branches[i + 1].Count; j++)
                    {
                        nN_tree.Add(nN.Branches[i + 1][j].ToString(), new GH_Path(i / 2, 1));
                    }
                }

                FcDiagramAssembleData Tp = new FcDiagramAssembleData(st2d_tree, st1_v, st1_c, st1_e, nN_tree, VE);//[210608 here we have to find ve_cycle and ve_list ]

                Saved_Tp = Tp.CopySelf();

                if (Saved_Tp == null) { return; }

                FD.Diagram.planarGraph = Saved_Tp;
                FD.report = "";
                FD.Diagram.AssemblingForceDiagram(FD.Diagram, new Point3d(0, 0, 0), false, ref FD.report);
                FD.model_status = "[Force Diagram]";
                Pts_ddr=FD.Diagram.FindForceDiagramNodeAddrs();
                /**/
                //FD.report +="\n[VE]"+ string.Join(" ", VE.ToArray());
                data.SetData("F*", FD);
                
            }
            else if (Saved_Tp == null|| Pts_ddr==null) {return;}
            else
            {
                FD.Diagram.planarGraph = Saved_Tp;
                FD.report = "";
                FD.Diagram.AssemblingForceDiagram(FD.Diagram, new Point3d(0, 0, 0), false, ref FD.report);
                FD.model_status = "[Force Diagram]";
                FD.Diagram.Fc_Pts_Addr = Pts_ddr;
                data.SetData("F*", FD);
            }
            /**/

        }

        public void DrawText(Plane pl0_Plane, string text, Point3d location, Color color, IGH_PreviewArgs args)
        {
            pl0_Plane.Origin = location;
            Rhino.Display.Text3d drawText = new Rhino.Display.Text3d(text, pl0_Plane, text_size);
            args.Display.Draw3dText(drawText, color);
            drawText.Dispose();
        }
        public void DrawNode(Plane pl0_Plane, string text, Point3d location, Color color, IGH_PreviewArgs args)
        {
            pl0_Plane.Origin = location;
            Rhino.Geometry.TextDot textdot = new TextDot(text, location); textdot.FontHeight = 14;
            args.Display.DrawDot(textdot, color, Color.White, Color.White);
            textdot.Dispose();
        }
        public void DrawLines(Plane pl0_Plane, Line line, double magni, Color color, double pow, bool arrowhead, IGH_PreviewArgs args)
        {
            int thick;
            if (color == Color.DarkGray) { thick = 1; }
            else { thick = Convert.ToInt32(1.5 + Math.Pow(db0_Line_Thick * Math.Abs(magni), pow)); }
            if (!arrowhead)
            { args.Display.DrawLine(line, color, thick); }
            else
            {
                foreach (Line ln in AddArrowHead(line, pl0_Plane))
                { args.Display.DrawLine(ln, color, thick); }
            }
        }
        private List<Line> AddArrowHead(Line ln, Plane ViewPlane)
        {
            ViewPlane.Origin = ln.To;
            Vector3d vc0_Line = ViewPlane.Normal;
            Vector3d vc0_Arrow_A = new Vector3d(ln.UnitTangent);
            Vector3d vc0_Arrow_B = new Vector3d(ln.UnitTangent);
            vc0_Arrow_A.Rotate(2.8, vc0_Line);
            vc0_Arrow_B.Rotate(-2.8, vc0_Line);
            Line ln0_Arrow_A = new Line(ln.To, vc0_Arrow_A, ln.Length * 0.05);
            Line ln0_Arrow_B = new Line(ln.To, vc0_Arrow_B, ln.Length * 0.05);
            return new List<Line>() { ln, ln0_Arrow_A, ln0_Arrow_B };
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_75;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("ac35aa50-29c0-4e21-8f01-8f2af6e1e08d"); }
        }
    }
}