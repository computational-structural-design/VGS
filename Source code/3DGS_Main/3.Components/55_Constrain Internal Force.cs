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

    public class Attributes_Custom_InternalForce_FlexConfig : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public Attributes_Custom_InternalForce_FlexConfig(GH_Component owner) : base(owner) { }
        #region D L M
        public bool D = false;
        public bool L = false;
        public bool M = false;
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("D", D);
            writer.SetBoolean("L", L);
            writer.SetBoolean("M",M);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            D = reader.GetBoolean("D");
            L = reader.GetBoolean("L");
            M = reader.GetBoolean("M");
            return base.Read(reader);
        }
        #endregion D L M
        public bool[] Status
        {
            get
            {
                return new bool[] { D, L ,M};
            }
        }


        #region ReWritePanel
        private System.Drawing.Rectangle Button_D { get; set; }
        private System.Drawing.Rectangle Button_L { get; set; }
        private System.Drawing.Rectangle Button_M { get; set; }
        protected override void Layout()
        {
            base.Layout();

            System.Drawing.Rectangle rec0 = GH_Convert.ToRectangle(Bounds);//GH_Attributes<IGH_Component>.Bounds  RectangleF 类 获取电池的边界数据
            rec0.Height += 22;

            System.Drawing.Rectangle rec_alg_D = rec0;
            System.Drawing.Rectangle rec_alg_L = rec0;
            System.Drawing.Rectangle rec_alg_M = rec0;

            rec_alg_D.Y = rec_alg_D.Bottom - 22; rec_alg_L.Y = rec_alg_L.Bottom - 22; rec_alg_M.Y = rec_alg_M.Bottom - 22;
            rec_alg_D.Height = 22; rec_alg_L.Height = 22; rec_alg_M.Height = 22;
            int width = rec0.Width / 3;
            rec_alg_D.Width = width; rec_alg_L.Width = width; rec_alg_M.Width = width;
            rec_alg_D.X = rec0.X ; rec_alg_L.X = rec0.X + width; rec_alg_M.X = rec0.X + width*2;
            rec_alg_D.Inflate(-2, -2); rec_alg_L.Inflate(-2, -2); rec_alg_M.Inflate(-2, -2);

            Button_D = rec_alg_D;
            Button_L = rec_alg_L;
            Button_M = rec_alg_M;
            Bounds = rec0;
        }
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {

            base.Render(canvas, graphics, channel);
            if (channel == GH_CanvasChannel.Objects)
            { }
            if (channel == GH_CanvasChannel.Objects)
            {
                GH_Capsule button_D = DrawButton(Button_D, "Dir", D);
                GH_Capsule button_L = DrawButton(Button_L, "Len", L);
                GH_Capsule button_M = DrawButton(Button_M, "Mag", M);

                button_D.Render(graphics, Selected, Owner.Locked, false);
                button_L.Render(graphics, Selected, Owner.Locked, false);
                button_M.Render(graphics, Selected, Owner.Locked, false);

                button_D.Dispose();
                button_L.Dispose();
                button_M.Dispose();
            }
        }
        #endregion
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF B_D = Button_D;
                System.Drawing.RectangleF B_L = Button_L;
                System.Drawing.RectangleF B_M = Button_M;

                if (B_D.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                else if (B_L.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                else if (B_M.Contains(e.CanvasLocation))
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
                System.Drawing.RectangleF B_D = Button_D;
                System.Drawing.RectangleF B_L = Button_L;
                System.Drawing.RectangleF B_M = Button_M;

                if (B_D.Contains(e.CanvasLocation))
                {
                    if (D == true) { D = false; } else { D = true; }
                    (base.Owner as InternalForce_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_L.Contains(e.CanvasLocation))
                {
                    if (L == true) { L = false; } else { L = true; }
                    (base.Owner as InternalForce_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_M.Contains(e.CanvasLocation))
                {
                    if (M == true) { M = false; } else { M = true; }
                   (base.Owner as InternalForce_FlexConfig).Update(Status);//
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
    public class InternalForce_FlexConfig : GH_Component
    {
        public bool[] DLM = new bool[] { false, false,false };
        public string report
        {
            get
            {
                string[] condition=new string[] { "unlock","unlock","unlock" };
                if (DLM[0] == true) { condition[0] = "locked"; } else { condition[0] = "unlock"; }
                if (DLM[1] == true) { condition[1] = "locked"; } else { condition[1] = "unlock"; }
                if (DLM[2] == true) { condition[2] = "locked"; } else { condition[2] = "unlock"; }
                return string.Format("Direction: {0}\nLength: {1}\nMagnitude: {2}", condition[0], condition[1],condition[2]);
            }
        }
        public InternalForce_FlexConfig()
          : base("5.Constrain Internal Force (VGS)", "Constrain Internal Force",
              "Constrain internal forces during the transformation",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_04)
        {
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("DLM0", DLM[0]);
            writer.SetBoolean("DLM1", DLM[1]);
            writer.SetBoolean("DLM2", DLM[2]);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            DLM[0] = reader.GetBoolean("DLM0");
            DLM[1] = reader.GetBoolean("DLM1");
            DLM[2] = reader.GetBoolean("DLM2");
            return base.Read(reader);
        }
        public override void CreateAttributes()
        {
            base.m_attributes = new Attributes_Custom_InternalForce_FlexConfig(this);
        }

        public void Update(bool[] Status)
        {
            DLM = Status;
            this.OnAttributesChanged();
            this.ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddTextParameter("EdgeID", "EdgeID", "List of indices of the edges to be constrained", GH_ParamAccess.list); Input[0].Optional = true;
            Input.AddNumberParameter("Strength", "Strength", "", GH_ParamAccess.item, 10.0); Input[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("Constr", "Constr", "List of constraints on the internal forces", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<string> eN_ids = new List<string>();
            double Strength = 10.0;
            data.GetDataList(0, eN_ids);
            data.GetData(1, ref Strength);

            lock_config int_lock_config = new lock_config();

            foreach (string id in eN_ids)
            {
                if (id.Substring(0, 1) == "e")
                {
                    int_lock_config.eN_lock.Add(id);
                    int_lock_config.eN_lock_DLM.Add(DLM);
                    int_lock_config.eN_lock_s.Add(Strength);
                }
            }

            data.SetData("Constr", int_lock_config);
            Message = report;
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_68;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("fa1f2e65-1a92-4235-a850-47a049be736b"); }//
        }
    }
}