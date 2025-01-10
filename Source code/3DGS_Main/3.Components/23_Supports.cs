using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using VGS_Main;
using System.Drawing;
using GH_IO.Serialization;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace GraphicStatic
{
    public class Attributes_Custom_Support : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public Attributes_Custom_Support(GH_Component owner) : base(owner) { }//Rewrite the constructor, and use base() to retain all the methods of the constructor in the base class;

        #region XYZ
        public bool X = false;
        public bool Y = false;
        public bool Z = false;
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("X",X);
            writer.SetBoolean("Y", Y);
            writer.SetBoolean("Z", Z);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            X=reader.GetBoolean("X");
            Y = reader.GetBoolean("Y");
            Z = reader.GetBoolean("Z");
            return base.Read(reader);
        }
        #endregion XYZ
        public string Status
        {
            get {
                if (X == true && Y == false && Z == false) { return "X"; }
                else if (X == false && Y == true && Z == false) { return "Y"; }
                else if (X == false && Y == false && Z == true) { return "Z"; }
                else if (X == true && Y == true && Z == false) { return "XY"; }
                else if (X == true && Y == false && Z == true) { return "XZ"; }
                else if (X == false && Y == true && Z == true) { return "YZ"; }
                else if (X == true && Y == true && Z == true) { return "XYZ"; }
                else { return "NONE"; }
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
            { }
            if (channel == GH_CanvasChannel.Objects)
            {
                GH_Capsule button_X = DrawButton(Button_X, "X", X);
                GH_Capsule button_Y = DrawButton(Button_Y, "Y", Y);
                GH_Capsule button_Z = DrawButton(Button_Z, "Z", Z);

                button_X.Render(graphics, Selected, Owner.Locked, false);
                button_Y.Render(graphics, Selected, Owner.Locked, false);
                button_Z.Render(graphics, Selected, Owner.Locked, false);

                button_X.Dispose();
                button_Y.Dispose();
                button_Z.Dispose();
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
                    if (X == true) { X = false; } else {X=true; }
                    (base.Owner as Support).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_Y.Contains(e.CanvasLocation))
                {
                    if (Y == true) { Y = false; } else { Y = true; }
                    (base.Owner as Support).Update(Status);//
                    return GH_ObjectResponse.Handled;
                }
                else if (B_Z.Contains(e.CanvasLocation))
                {
                    if (Z == true) { Z = false; } else { Z = true; }
                    (base.Owner as Support).Update(Status);//
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
    public class Support : GH_Component
    {
        public string config_XYZ = "NONE";
        public double text_size =System_Configuration.Text_scale;
        public List<Point3d> Supports = new List<Point3d>();
        public Support()
          : base("3.Supports (VGS)", "Supports",
              "Set the supports of the structure",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_02)
        {
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetString("XYZ",config_XYZ);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            config_XYZ= reader.GetString("XYZ");
            return base.Read(reader);
        }
        public override void CreateAttributes()//Override the function that calls battery properties
        {
            base.m_attributes = new Attributes_Custom_Support(this);
        }

        public void Update(string Status)//This is very important. It will be called by another trigger event (mouse) to transfer data and update the battery status.
        {
            config_XYZ = Status;
            this.OnAttributesChanged();
            this.ExpireSolution(true);

            
        }

        public void Update()//Override this update function to facilitate other batteries to update it;
        {
            this.OnAttributesChanged();
            this.ExpireSolution(true);
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (Supports.Count == 0)
                return;

            Plane pl0_Plane;
            args.Viewport.GetFrustumFarPlane(out pl0_Plane);
            foreach(Point3d pt in Supports)
            {
                pl0_Plane.Origin = pt;
                if (config_XYZ != "NONE"&&this.Locked==false)
                {
                    Rhino.Display.Text3d drawText = new Rhino.Display.Text3d(config_XYZ, pl0_Plane, text_size);
                    args.Display.Draw3dText(drawText, Color.Black);
                    drawText.Dispose();
                }
            }
        }
        public override BoundingBox ClippingBox
        {
            get
            {
                return BoundingBox.Empty;
            }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddPointParameter("Pt","Pt", "List of points corresponding to the supports", GH_ParamAccess.list);Input[0].Optional=true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("Support", "Support", "List of supports", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess data)
        {
            System_dynamic.temp.ifchange +=  new System_Configuration.change(Update);
            text_size = System_Configuration.Text_scale;
            Message = config_XYZ;
            this.Supports.Clear();
            this.Supports = new List<Point3d>();
            if (!data.GetDataList("Pt", Supports)) {return; }
            if (config_XYZ == "NONE") { Supports.Clear(); }
            Support_point SP = new Support_point(Supports, config_XYZ);
            data.SetData("Support", SP);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return GraphicStatic.Properties.Resources.VGS_51;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ee7ff4ae-f25a-4072-9012-06446ff7ad38"); }
        }
    }
}
