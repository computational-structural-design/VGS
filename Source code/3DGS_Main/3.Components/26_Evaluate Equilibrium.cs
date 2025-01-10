using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;

namespace GraphicStatic
{
    public class Equilibrium_MAIN : GH_Component
    {
        public Equilibrium_MAIN()
          : base("6.Evaluate Equilibrium (VGS)", "Evaluate Equilibrium",
              "Evaluate the equilibrium of the structure",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_02)
        {
        }

        public MODEL ASSIST;//Assistant model for showing the selfstress and kinematic status of the model
        public double text_size = System_Configuration.Text_scale;
        public double db0_Line_Thick = 0.1;
        public double pow = 1.0;
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            text_size = System_Configuration.Text_scale;
            Plane pl0_Plane;
            args.Viewport.GetFrustumFarPlane(out pl0_Plane);
            if (ASSIST.model_status != "[not calculated]")
            {
                for (int i = 0; i < ASSIST.Diagram.Edges.Count;i++)
                {
                    EDGE edge = ASSIST.Diagram.Edges[i];
                    DrawLines(pl0_Plane, edge.ln, edge.force, edge.col, pow, false, args);
                    DrawText(pl0_Plane, edge.ID, edge.ln.PointAt(0.5), edge.col,text_size, args);
                }
                for (int i = 0; i < ASSIST.Diagram.Nodes.Count; i++)
                {
                    NODE node = ASSIST.Diagram.Nodes[i];
                    DrawText(pl0_Plane, node.ID, node.pt, node.col,text_size, args);
                }
                for (int i = 0; i < ASSIST.Diagram.Loads.Count; i++)
                {
                    LOAD load = ASSIST.Diagram.Loads[i];
                    DrawLines(pl0_Plane, load.ln, load.ln.Length*0.5, load.col, pow, true, args);
                    //DrawText(pl0_Plane, load.ID, load.ln.PointAt(0.8), load.col,text_size*0.5, args);
                }
                for (int i = 0; i < ASSIST.Diagram.Reacts.Count; i++)
                {
                    REACT react = ASSIST.Diagram.Reacts[i];
                    DrawLines(pl0_Plane, react.ln, react.ln.Length * 0.2, react.col, pow, true, args);
                    //DrawText(pl0_Plane, react.ID, react.ln.PointAt(0.8), react.col,text_size * 0.5, args);
                }
            }
        }
        public void DrawText(Plane pl0_Plane, string text, Point3d location, Color color, double text_size, IGH_PreviewArgs args)
        {
            pl0_Plane.Origin = location;
            Rhino.Display.Text3d drawText = new Rhino.Display.Text3d(text, pl0_Plane, text_size);
            args.Display.Draw3dText(drawText, color);
            drawText.Dispose();
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
        public void Update()//Reconstruct this update function to facilitate other batteries to update it;
        {
            this.OnAttributesChanged();
            this.ExpireSolution(true);   
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("Model", "Model", "Structural model for the evaluation of the equilibrium", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddGenericParameter("SelfStress", "SelfStress", "List of self-stresses of the structure", GH_ParamAccess.list); Input[1].Optional = true;
            Input.AddBooleanParameter("MatrRank", "MatrRank", "Calculate the rank of the equilibrium matrix (True)", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("IntForce", "IntForce", "List of the internal forces of the structure", GH_ParamAccess.item);
            Output.AddGenericParameter("ExtForce", "ExtForce", "List of the external forces of the structure", GH_ParamAccess.item);
            Output.AddGenericParameter("Model", "Model", "The evaluated structural model", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            System_dynamic.temp.ifchange += new System_Configuration.change(Update);

            //Setting the rank、selfstress etc.
            List<SelfStress_index> selfstress_list = new List<SelfStress_index>();
            SelfStress_index selfstress = new SelfStress_index();
            bool rank = true;
            data.GetData(2, ref rank);
            if (data.GetDataList(1, selfstress_list))
            { foreach (SelfStress_index Selfs in selfstress_list) { selfstress.AddData(Selfs.index, Selfs.forces); } }
            Model_set Pla_set = new Model_set(selfstress, rank);
            //

            ASSIST = new MODEL();
            MODEL FD = new MODEL();
            if (!data.GetData(0, ref FD)) { return; }
            if (FD.model_status == "[not calculated]") { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: The Input model is not calculated because: \n" + FD.report_error); return; }

            FD = FD.CopySelf();
          
            if (Pla_set.Type == "PLASTIC")//[1211] Eq solution
            { Message = "EQUILIBRIUM SOLUTION"; FD.EQUILIBRIUM_MODE(Pla_set.rank, Pla_set.Selfstress.index, Pla_set.Selfstress.forces); data.SetData(2, FD); ASSIST = FD.CopySelf(); }
            else { Message = "INVALID SETTING"; }

            Edge_line edges_set = new Edge_line();
            Load_line load_set = new Load_line();
            if (FD.model_status != "[not calculated]")
            {
                List<Line> line_set = new List<Line>();
                List<double> force_set = new List<double>();
                for (int i = 0; i < FD.Diagram.Edges.Count; i++)
                {
                    EDGE edge = FD.Diagram.Edges[i];
                    line_set.Add(edge.ln);
                    force_set.Add(edge.force);
                }
                edges_set.AddData(line_set, force_set);

                List<Point3d> act_pt = new List<Point3d>();
                List<Line> ln = new List<Line>();
                List<Vector3d> vec = new List<Vector3d>();
                for (int i = 0; i < FD.Diagram.Loads.Count; i++)
                {
                    LOAD load = FD.Diagram.Loads[i];
                    ln.Add(load.ln);
                    act_pt.Add(load.ln.From);
                }
                for (int i = 0; i < FD.Diagram.Reacts.Count; i++)
                {
                    REACT react = FD.Diagram.Reacts[i];
                    ln.Add(react.ln);
                    act_pt.Add(react.ln.From);
                }
                load_set = new Load_line(ln, act_pt, vec);
            }
            data.SetData(0, edges_set); data.SetData(1, load_set);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_55;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("3cf08adf-be75-43c6-b79c-2fc2fa2d79ad"); }//
        }
    }
}