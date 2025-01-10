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
    public class Attributes_Custom_Node_FlexConfig : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public Attributes_Custom_Node_FlexConfig(GH_Component owner) : base(owner) { }

        #region XYZ
        public bool X = false;
        public bool Y = false;
        public bool Z = false;
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("X_lock", X);
            writer.SetBoolean("Y_lock", Y);
            writer.SetBoolean("Z_lock", Z);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            X = reader.GetBoolean("X_lock");
            Y = reader.GetBoolean("Y_lock");
            Z = reader.GetBoolean("Z_lock");
            return base.Read(reader);
        }
        #endregion XYZ
        public bool[] Status
        {
            get
            {
                return new bool[] {X, Y, Z};
            }
        }


        #region ReWritePanel
        private System.Drawing.Rectangle Button_X { get; set; }
        private System.Drawing.Rectangle Button_Y { get; set; }
        private System.Drawing.Rectangle Button_Z { get; set; }
        protected override void Layout()
        {
            base.Layout();

            System.Drawing.Rectangle rec0 = GH_Convert.ToRectangle(Bounds);//GH_Attributes<IGH_Component>.Bounds  RectangleF 类 获取电池的边界数据
            rec0.Height += 22;

            System.Drawing.Rectangle rec_alg_X = rec0;
            System.Drawing.Rectangle rec_alg_Y = rec0;
            System.Drawing.Rectangle rec_alg_Z = rec0;

            rec_alg_X.Y = rec_alg_X.Bottom - 22; rec_alg_Y.Y = rec_alg_Y.Bottom - 22; rec_alg_Z.Y = rec_alg_Z.Bottom - 22;
            rec_alg_X.Height = 22; rec_alg_Y.Height = 22; rec_alg_Z.Height = 22;
            int width = rec0.Width / 3;
            rec_alg_X.Width = width; rec_alg_Y.Width = width; rec_alg_Z.Width = width;
            rec_alg_X.X = rec0.X; rec_alg_Y.X = rec0.X + width; rec_alg_Z.X = rec0.X + width * 2;
            rec_alg_X.Inflate(-2, -2); rec_alg_Y.Inflate(-2, -2); rec_alg_Z.Inflate(-2, -2);

            Button_X = rec_alg_X;
            Button_Y = rec_alg_Y;
            Button_Z = rec_alg_Z;
            Bounds = rec0;
        }
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {

            base.Render(canvas, graphics, channel);
            if (channel == GH_CanvasChannel.Objects)
            {
                GH_Capsule button_A = DrawButton(Button_X, "X", X);
                GH_Capsule button_D = DrawButton(Button_Y, "Y", Y);
                GH_Capsule button_L = DrawButton(Button_Z, "Z", Z);

                button_A.Render(graphics, Selected, Owner.Locked, false);
                button_D.Render(graphics, Selected, Owner.Locked, false);
                button_L.Render(graphics, Selected, Owner.Locked, false);

                button_A.Dispose();
                button_D.Dispose();
                button_L.Dispose();
            }
        }
        #endregion
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF B_X = Button_X;
                System.Drawing.RectangleF B_Y = Button_Y;
                System.Drawing.RectangleF B_Z = Button_Z;

                if (B_X.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                else if (B_Y.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                else if (B_Z.Contains(e.CanvasLocation))
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
                System.Drawing.RectangleF B_Y = Button_Y;
                System.Drawing.RectangleF B_Z = Button_Z;

                if (B_X.Contains(e.CanvasLocation))
                {
                    if (X == true) { X = false; } else { X = true; }
                    (base.Owner as Node_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_Y.Contains(e.CanvasLocation))
                {
                    if (Y == true) { Y = false; } else { Y = true; }
                    (base.Owner as Node_FlexConfig).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_Z.Contains(e.CanvasLocation))
                {
                    if (Z == true) { Z = false; } else { Z = true; }
                    (base.Owner as Node_FlexConfig).Update(Status);//
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
    public class Node_FlexConfig : GH_Component
    {
        public bool[] XYZ = new bool[] { false, false, false };
        public string report
        {
            get
            {
                string[] condition = new string[] { "unlock", "unlock", "unlock" };
                if (XYZ[0] == true) { condition[0] = "locked"; } else { condition[0] = "unlock"; }
                if (XYZ[1] == true) { condition[1] = "locked"; } else { condition[1] = "unlock"; }
                if (XYZ[2] == true) { condition[2] = "locked"; } else { condition[2] = "unlock"; }
                return string.Format("X-direction: {0}\nY-direction: {1}\nZ-direction: {2}", condition[0], condition[1], condition[2]);
            }
        }
        public Node_FlexConfig()
          : base("6.Constrain Node (VGS)", "Constrain Node",
              "Constrain nodes during the transformation",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_04)
        {
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("XYZ0", XYZ[0]);
            writer.SetBoolean("XYZ1", XYZ[1]);
            writer.SetBoolean("XYZ2", XYZ[2]);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            XYZ[0] = reader.GetBoolean("XYZ0");
            XYZ[1] = reader.GetBoolean("XYZ1");
            XYZ[2] = reader.GetBoolean("XYZ2");
            return base.Read(reader);
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new Attributes_Custom_Node_FlexConfig(this);
        }

        public void Update(bool[] Status)
        {
            XYZ = Status;
            this.OnAttributesChanged();
            this.ExpireSolution(true);
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddTextParameter("NodesID", "NodesID", "List of indices of the nodes to be constrained", GH_ParamAccess.list); Input[0].Optional = true;
            Input.AddNumberParameter("Strength", "Strength", "", GH_ParamAccess.item, 10.0); Input[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("Constr", "Constr", "List of constraints on the node", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<string> vN_ids = new List<string>();
            double Strength = 10.0;
            data.GetDataList(0, vN_ids);
            data.GetData(1, ref Strength);

            lock_config node_lock_config = new lock_config();

            foreach (string id in vN_ids)
            {
                if (id.Substring(0, 1) == "v"|| id.Substring(0, 1) == "V")
                {
                    node_lock_config.vN_lock.Add(id);
                    node_lock_config.vN_lock_xyz.Add(XYZ);
                    node_lock_config.vN_lock_s.Add(Strength);
                }
            }

            data.SetData("Constr", node_lock_config);
            Message = report;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_69;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8495d050-ed58-4fda-927a-86c7a61756b1"); }//
        }
    }
}