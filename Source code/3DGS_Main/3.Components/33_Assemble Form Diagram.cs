using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

namespace GraphicStatic
{
    public class API_Model_Assembler : GH_Component
    {
        static public bool zerobareliminated = false;
        public class Attributes_Custom_API : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
        {
            public Attributes_Custom_API(GH_Component owner) : base(owner) { }//Override the constructor, and use base() to retain all methods of the constructor in the base class

            #region ZeroBarEliminated
            public bool ZeroBarEliminated = false;
            private string eliminated { get { if (ZeroBarEliminated) { return "Remove Null Forces"; } else { return "Keep Null Forces"; } } }

            public override bool Write(GH_IWriter writer)
            {
                writer.SetBoolean("ZeroBarEliminated", ZeroBarEliminated);
                return base.Write(writer);
            }
            public override bool Read(GH_IReader reader)
            {
                ZeroBarEliminated = reader.GetBoolean("ZeroBarEliminated");
                zerobareliminated = ZeroBarEliminated;
                return base.Read(reader);
            }
            #endregion ZeroBarEliminated

            #region ReWritePanel
            private System.Drawing.Rectangle Button_X { get; set; }

            protected override void Layout()
            {
                base.Layout();

                System.Drawing.Rectangle rec0 = GH_Convert.ToRectangle(Bounds);//GH_Attributes<IGH_Component>.Bounds  RectangleF 类 获取电池的边界数据
                rec0.Height += 22;

                System.Drawing.Rectangle rec_alg_X = rec0;

                rec_alg_X.Y = rec_alg_X.Bottom - 22; 
                rec_alg_X.Height = 22; 
                int width = rec0.Width;
                rec_alg_X.Width = width; 
                rec_alg_X.X = rec0.X;
                rec_alg_X.Inflate(-2, -2);

                Button_X = rec_alg_X;
                Bounds = rec0;
            }
            protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
            {

                base.Render(canvas, graphics, channel);
                if (channel == GH_CanvasChannel.Objects)
                { }
                if (channel == GH_CanvasChannel.Objects)
                {

                    GH_Capsule button_X = DrawButton(Button_X, eliminated, ZeroBarEliminated);

                    button_X.Render(graphics, Selected, Owner.Locked, false);

                    button_X.Dispose();
                }
            }
            #endregion
            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    System.Drawing.RectangleF B_X = Button_X;

                    if (B_X.Contains(e.CanvasLocation))
                    {
                        return GH_ObjectResponse.Handled;
                    }
                }
                return base.RespondToMouseDown(sender, e);
            }
            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    System.Drawing.RectangleF B_X = Button_X;

                    if (B_X.Contains(e.CanvasLocation))
                    {
                        if (ZeroBarEliminated == true) { ZeroBarEliminated = false; } else { ZeroBarEliminated = true; }
                        (base.Owner as API_Model_Assembler).Update(ZeroBarEliminated);//
                        return GH_ObjectResponse.Handled;
                    }
                }

                return base.RespondToMouseDown(sender, e);
            }
            #region AXUI_Funtion
            public GH_Capsule DrawButton(System.Drawing.Rectangle Button_bound, string text, bool press)
            {
                GH_Capsule button;
                if (press) { button = GH_Capsule.CreateTextCapsule(Button_bound, Button_bound, GH_Palette.Black, text, 2, 0); }
                else { button = GH_Capsule.CreateTextCapsule(Button_bound, Button_bound, GH_Palette.Transparent, text, 2, 0); }
                return button;
            }
            public System.Drawing.PointF GetMidPoint(System.Drawing.Rectangle rec)
            {
                System.Drawing.PointF point = rec.Location;
                point.X += rec.Width / 2;
                point.Y += rec.Height / 2;
                return point;
            }
            #endregion
        }
        public API_Model_Assembler()
          : base("3.Assemble Form Diagram (VGS)", "Assemble Form Diagram",
              "Assemble the form diagram based on internal and external forces",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }
        public override void CreateAttributes()//Override functions that call battery properties
        {
            base.m_attributes = new Attributes_Custom_API(this);
        }
        public void Update(bool boolean)//This is very important. It will be called by another trigger event (mouse) to transfer data and update the battery status
        {
            zerobareliminated = boolean;
            this.OnAttributesChanged();
            this.ExpireSolution(true);
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("IntForce", "IntForce", "List of the internal forces of the structure", GH_ParamAccess.list); 
            Input.AddGenericParameter("ExtForce", "ExtForce", "List of the external forces of the structure", GH_ParamAccess.list); Input[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("F", "F", "Form diagram", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            Edge_line internalForces = new Edge_line();
            Load_line externalForces = new Load_line();
            Support_point supports = new Support_point();

            List<Edge_line> internalforces_input= new List<Edge_line>();
            List<Load_line> externalforces_input = new List<Load_line>();
            data.GetDataList(0, internalforces_input);
            data.GetDataList(1, externalforces_input);
            
            foreach (Edge_line inforce in internalforces_input) { if (inforce != null) { internalForces.AddData(inforce.lines, inforce.forces); } }
            foreach (Load_line exforce in externalforces_input) { if (exforce != null) { externalForces.AddData(exforce.lines); } }

            if(zerobareliminated)
            {
            //Delete the zero bars
            for (int i = 0; i < internalForces.forces.Count; i++)
            {
                if (Math.Abs(internalForces.forces[i]) < System_Configuration.Sys_Tor) { internalForces.forces.RemoveAt(i); internalForces.lines.RemoveAt(i); i--; }
            }

            for (int i = 0; i < externalForces.lines.Count; i++)
            {
                if (externalForces.lines[i].Length < System_Configuration.Sys_Tor) { externalForces.lines.RemoveAt(i); i--; }
            }
            //
             }

            if (internalForces.lines.Count < 3) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "At least more than three edges should be input"); return; }

            List<string> support_status = new List<string>() { "XYZ","YZ","Z"};
            List<Point3d> support_pts = new List<Point3d>();
   
            //Randomly set the supports for the Inputted model
            for (int i = 0; i < internalForces.lines.Count; i++)
            {
                bool add = true;
                Point3d p0 = internalForces.lines[i].From;
                for (int j = 0; j < support_pts.Count; j++)
                {
                    if (VgsCommon.ComparePts(support_pts[j], p0, System_Configuration.Sys_Tor))
                    {
                        add = false;break;
                    }
                }
                if (add) { support_pts.Add(internalForces.lines[i].From); }
                if (support_pts.Count== 3) { break; }
            }

            supports.AddData(support_pts,support_status);

            MODEL model = new MODEL();
            model.GUI="MODEL:"+System.Guid.NewGuid().ToString();
            model.Import_class(internalForces, externalForces, supports);
            model.Check_values();
            model.BASIC_Model();
            MODEL FD = model.CopySelf();
            FD.API_MODE();

            data.SetData("F", FD);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_63;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ab54f785-5cb4-4a8d-8ee8-a20909e0afed"); }//
        }
    }
}