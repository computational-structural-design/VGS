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
    public class Attributes_Custom_ExternalForce_FlexConfig : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public Attributes_Custom_ExternalForce_FlexConfig(GH_Component owner) : base(owner) { }

        #region ADLI
        public bool A = false;
        public bool D = false;
        public bool L = false;
        public bool I = false;
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("A", A);
            writer.SetBoolean("D", D);
            writer.SetBoolean("L", L);
            writer.SetBoolean("I", I);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            A = reader.GetBoolean("A");
            D = reader.GetBoolean("D");
            L = reader.GetBoolean("L");
            I = reader.GetBoolean("I");
            return base.Read(reader);
        }
        #endregion ADLI
        public bool[] Status
        {
            get
            {
                return new bool[] { A,D,L,I};
            }
        }


        #region ReWritePanel
        private System.Drawing.Rectangle Button_A { get; set; }
        private System.Drawing.Rectangle Button_D { get; set; }
        private System.Drawing.Rectangle Button_L { get; set; }
        private System.Drawing.Rectangle Button_I { get; set; }
        protected override void Layout()
        {
            base.Layout();

            System.Drawing.Rectangle rec0 = GH_Convert.ToRectangle(Bounds);//GH_Attributes<IGH_Component>.Bounds  RectangleF 类 获取电池的边界数据
            rec0.Height += 22;

            System.Drawing.Rectangle rec_alg_A = rec0;
            System.Drawing.Rectangle rec_alg_D = rec0;
            System.Drawing.Rectangle rec_alg_L = rec0;
            System.Drawing.Rectangle rec_alg_I = rec0;

            rec_alg_A.Y = rec_alg_A.Bottom - 22; rec_alg_D.Y = rec_alg_D.Bottom - 22; rec_alg_L.Y = rec_alg_L.Bottom - 22; rec_alg_I.Y = rec_alg_I.Bottom - 22;
            rec_alg_A.Height = 22; rec_alg_D.Height = 22; rec_alg_L.Height = 22; rec_alg_I.Height = 22;
            int width = rec0.Width / 4;
            rec_alg_A.Width = width; rec_alg_D.Width = width; rec_alg_L.Width = width; rec_alg_I.Width = width;
            rec_alg_A.X = rec0.X; rec_alg_D.X = rec0.X + width; rec_alg_L.X = rec0.X + width * 2; rec_alg_I.X = rec0.X + width * 3;
            rec_alg_A.Inflate(-2, -2); rec_alg_D.Inflate(-2, -2); rec_alg_L.Inflate(-2, -2); rec_alg_I.Inflate(-2, -2);

            Button_A = rec_alg_I;
            Button_D = rec_alg_D;
            Button_L = rec_alg_L;
            Button_I = rec_alg_A;
            Bounds = rec0;
        }
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {

            base.Render(canvas, graphics, channel);
            if (channel == GH_CanvasChannel.Objects)
            { }
            if (channel == GH_CanvasChannel.Objects)
            {
                GH_Capsule button_A = DrawButton(Button_A, "App", A);
                GH_Capsule button_D = DrawButton(Button_D, "Dir", D);
                GH_Capsule button_L = DrawButton(Button_L, "Len", L);
                GH_Capsule button_I = DrawButton(Button_I, "Act", I);

                button_A.Render(graphics, Selected, Owner.Locked, false);
                button_D.Render(graphics, Selected, Owner.Locked, false);
                button_L.Render(graphics, Selected, Owner.Locked, false);
                button_I.Render(graphics, Selected, Owner.Locked, false);

                button_A.Dispose();
                button_D.Dispose();
                button_L.Dispose();
                button_I.Dispose();
            }
        }
        #endregion
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF B_A = Button_A;
                System.Drawing.RectangleF B_D = Button_D;
                System.Drawing.RectangleF B_L = Button_L;
                System.Drawing.RectangleF B_I = Button_I;

                if (B_A.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                else if (B_D.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                else if (B_L.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                else if (B_I.Contains(e.CanvasLocation))
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
                System.Drawing.RectangleF B_A = Button_A;
                System.Drawing.RectangleF B_D = Button_D;
                System.Drawing.RectangleF B_L = Button_L;
                System.Drawing.RectangleF B_I = Button_I;

                if (B_A.Contains(e.CanvasLocation))
                {
                    if (A== true) { A = false; } else { A = true; }
                    (base.Owner as ExternalForce_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_D.Contains(e.CanvasLocation))
                {
                    if (D == true) { D = false; } else { D = true; }
                    (base.Owner as ExternalForce_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_L.Contains(e.CanvasLocation))
                {
                    if (L == true) {L = false; } else { L = true; }
                    (base.Owner as ExternalForce_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_I.Contains(e.CanvasLocation))
                {
                    if (I== true) { I= false; } else { I = true; }
                   (base.Owner as ExternalForce_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }

            }

            return base.RespondToMouseUp(sender, e);
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
    public class ExternalForce_FlexConfig : GH_Component
    {
        public bool[] ADLI = new bool[] { false, false, false, false};
        public string report
        {
            get
            {
                string[] condition = new string[] { "unlock", "unlock", "unlock", "unlock" };
                if (ADLI[0] == true) { condition[0] = "locked"; } else { condition[0] = "unlock"; }
                if (ADLI[1] == true) { condition[1] = "locked"; } else { condition[1] = "unlock"; }
                if (ADLI[2] == true) { condition[2] = "locked"; } else { condition[2] = "unlock"; }
                if (ADLI[3] == true) { condition[3] = "locked"; } else { condition[3] = "unlock"; }
                return string.Format("LineAction: {3}\nDirection: {1}\nLength: {2}\nPointApp: {0}", condition[0], condition[1], condition[2], condition[3]);
            }
        }
        public ExternalForce_FlexConfig()
          : base("4.Constrain External Force (VGS)", "Constrain External Force",
              "Constrain external forces during the transformation",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_04)
        {
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("ADL0", ADLI[0]);
            writer.SetBoolean("ADL1", ADLI[1]);
            writer.SetBoolean("ADL2", ADLI[2]);
            writer.SetBoolean("ADL3", ADLI[3]);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            ADLI[0] = reader.GetBoolean("ADL0");
            ADLI[1] = reader.GetBoolean("ADL1");
            ADLI[2] = reader.GetBoolean("ADL2");
            ADLI[3] = reader.GetBoolean("ADL3");
            return base.Read(reader);
        }
        public override void CreateAttributes()
        {
            base.m_attributes = new Attributes_Custom_ExternalForce_FlexConfig(this);
        }

        public void Update(bool[] Status)
        {
            ADLI = Status;
            this.OnAttributesChanged();
            this.ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddTextParameter("ExtForceID", "ExtForceID", "List of indices of the external forces to be constrained ", GH_ParamAccess.list);Input[0].Optional = true;
            Input.AddNumberParameter("Strength", "Strength", "", GH_ParamAccess.item,10.0); Input[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("Constr", "Constr", "List of constraints on the external forces", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<string> RQ_ids = new List<string>();
            double Strength = 10.0;
            data.GetData(1,ref Strength);
            data.GetDataList(0, RQ_ids);
            lock_config ext_lock_config = new lock_config();

            foreach (string id in RQ_ids)
            {
                if (id.Substring(0, 1) == "Q" || id.Substring(0, 1) == "R")
                {
                    ext_lock_config.RQ_lock.Add(id);
                    ext_lock_config.RQ_lock_ADLI.Add(ADLI);
                    ext_lock_config.RQ_lock_s.Add(Strength);
                }
            }

            data.SetData("Constr", ext_lock_config);
            Message = report;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_79;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("615e305b-91b2-4931-895a-187841a2d9e0"); }//
        }
    }
}