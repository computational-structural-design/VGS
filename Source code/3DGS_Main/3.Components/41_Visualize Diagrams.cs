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
    public class Attributes_Custom_Visualization_Diagrams : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public Attributes_Custom_Visualization_Diagrams(GH_Component owner) : base(owner) { }

        #region button bool
        public bool display_Edge_line = true;
        public bool display_Edge_text = true;

        public bool display_Node_text = true;
        public bool display_Node_circle = true;

        public bool display_Load_line = true;
        public bool display_Load_text = true;

        public bool display_React_line = true;
        public bool display_React_text = true;

        public bool display_Result_line = false;
        public bool display_Result_text = false;
        #endregion
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("display_Edge_line", display_Edge_line);
            writer.SetBoolean("display_Edge_text", display_Edge_text);

            writer.SetBoolean("display_Node_text", display_Node_text);
            writer.SetBoolean("display_Node_circle", display_Node_circle);

            writer.SetBoolean("display_Load_line", display_Load_line);
            writer.SetBoolean("display_Load_text", display_Load_text);

            writer.SetBoolean("display_React_line", display_React_line);
            writer.SetBoolean("display_React_text", display_React_text);

            writer.SetBoolean("display_Result_line", display_Result_line);
            writer.SetBoolean("display_Result_text", display_Result_text);


            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            display_Edge_line = reader.GetBoolean("display_Edge_line");
            display_Edge_text = reader.GetBoolean("display_Edge_text");

            display_Node_text = reader.GetBoolean("display_Node_text");
            display_Node_circle = reader.GetBoolean("display_Node_circle");

            display_Load_line = reader.GetBoolean("display_Load_line");
            display_Load_text = reader.GetBoolean("display_Load_text");

            display_React_line = reader.GetBoolean("display_React_line");
            display_React_text = reader.GetBoolean("display_React_text");

            display_Result_line = reader.GetBoolean("display_Result_line");
            display_Result_text = reader.GetBoolean("display_Result_text");

            return base.Read(reader);
        }
        #region ReWritePanel
        private System.Drawing.Rectangle Button_Edgeline { get; set; }
        private System.Drawing.Rectangle Button_Edgetext { get; set; }
        private System.Drawing.Rectangle Button_Nodecircle { get; set; }
        private System.Drawing.Rectangle Button_Nodetext { get; set; }
        private System.Drawing.Rectangle Button_Loadline { get; set; }
        private System.Drawing.Rectangle Button_Loadtext { get; set; }

        protected override void Layout()
        {
            base.Layout();

            System.Drawing.Rectangle rec0 = GH_Convert.ToRectangle(Bounds);//GH_Attributes<IGH_Component>.Bounds  RectangleF 类 获取电池的边界数据
            int col_num = 3;
            int row_num = 2;
            rec0.Height += 22 * col_num; rec0.Width += 50;
            System.Drawing.Rectangle rec_alg_R = rec0;

            Button_Edgeline = SetRectangle(rec0, 1, col_num, row_num, 1, 22);
            Button_Edgetext = SetRectangle(rec0, 1, col_num, row_num, 2, 22);

            Button_Loadline = SetRectangle(rec0, 2, col_num, row_num, 1, 22);
            Button_Loadtext = SetRectangle(rec0, 2, col_num, row_num, 2, 22);

            Button_Nodecircle = SetRectangle(rec0, 3, col_num, row_num, 1, 22);
            Button_Nodetext = SetRectangle(rec0, 3, col_num, row_num, 2, 22);

            Bounds = rec0;
        }
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {

            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                GH_Capsule button_Edgeline = DrawButton(Button_Edgeline, "EdgeLn", display_Edge_line);
                GH_Capsule button_Edgetext = DrawButton(Button_Edgetext, "EdgeTxt", display_Edge_text);

                GH_Capsule button_Nodetext = DrawButton(Button_Nodetext, "NodeTxt", display_Node_text);
                GH_Capsule button_Nodecircle = DrawButton(Button_Nodecircle, "NodePt", display_Node_circle);

                GH_Capsule button_Loadline = DrawButton(Button_Loadline, "ExtForceLn", display_Load_line);
                GH_Capsule button_Loadtext = DrawButton(Button_Loadtext, "ExtForceTxt", display_Load_text);

                button_Edgeline.Render(graphics, Selected, Owner.Locked, false); button_Edgeline.Dispose();
                button_Edgetext.Render(graphics, Selected, Owner.Locked, false); button_Edgetext.Dispose();

                button_Nodetext.Render(graphics, Selected, Owner.Locked, false); button_Nodetext.Dispose();
                button_Nodecircle.Render(graphics, Selected, Owner.Locked, false); button_Nodetext.Dispose();

                button_Loadline.Render(graphics, Selected, Owner.Locked, false); button_Loadline.Dispose();
                button_Loadtext.Render(graphics, Selected, Owner.Locked, false); button_Loadtext.Dispose();

            }
        }
        #endregion
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF B_Edgeline = Button_Edgeline;
                if (B_Edgeline.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_Edgetext = Button_Edgetext;
                if (B_Edgetext.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_Nodetext = Button_Nodetext;
                if (B_Nodetext.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_Nodecircle = Button_Nodecircle;
                if (B_Nodecircle.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_Loadline = Button_Loadline;
                if (B_Loadline.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_Loadtext = Button_Loadtext;
                if (B_Loadtext.Contains(e.CanvasLocation))
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
                System.Drawing.RectangleF B_Edgeline = Button_Edgeline;
                if (B_Edgeline.Contains(e.CanvasLocation))
                {
                    if (display_Edge_line == true) { display_Edge_line = false; } else { display_Edge_line = true; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_Edgetext = Button_Edgetext;
                if (B_Edgetext.Contains(e.CanvasLocation))
                {
                    if (display_Edge_text == true) { display_Edge_text = false; } else { display_Edge_text = true; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }

                System.Drawing.RectangleF B_Nodetext = Button_Nodetext;
                if (B_Nodetext.Contains(e.CanvasLocation))
                {
                    if (display_Node_text == true) { display_Node_text = false; } else { display_Node_text = true; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }

                System.Drawing.RectangleF B_Nodecircle = Button_Nodecircle;
                if (B_Nodecircle.Contains(e.CanvasLocation))
                {
                    if (display_Node_circle == true) { display_Node_circle = false; } else { display_Node_circle = true; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }

                System.Drawing.RectangleF B_Loadline = Button_Loadline;
                if (B_Loadline.Contains(e.CanvasLocation))
                {
                    if (display_Load_line == true) { display_Load_line = false; } else { display_Load_line = true; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_Loadtext = Button_Loadtext;
                if (B_Loadtext.Contains(e.CanvasLocation))
                {
                    if (display_Load_text == true) { display_Load_text = false; } else { display_Load_text = true; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }               
            }

            return base.RespondToMouseUp(sender, e);
        }

        #region AXUI_Funtion
        public System.Drawing.Rectangle SetRectangle(System.Drawing.Rectangle bound, int level, int levelmax, int seg, int which, int unit_size)
        {
            System.Drawing.Rectangle rec = bound;
            rec.Y = rec.Bottom - unit_size * (levelmax - level + 1);
            rec.Height = unit_size;
            int width = bound.Width / seg;
            rec.Width = width;
            rec.X = bound.X + (which - 1) * width;
            rec.Inflate(-2, -2);
            return rec;
        }
        public void ResponseUpdate()
        {
            (base.Owner as Visualize_Diagrams).Update(display_Edge_line, display_Edge_text, display_Node_text, display_Node_circle, display_Load_line, display_Load_text, display_React_line, display_React_text, display_Result_line, display_Result_text);//
        }
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
    public class Visualize_Diagrams : GH_Component
    {
        public Visualize_Diagrams()
          : base("1.Visualize Diagrams (VGS)", "Visualize Diagrams",
              "Visualize the form or the force diagrams in the Rhino viewport",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_05)
        {
        }

        public Plane view_Point;
        public string report = "";
        public double db0_Line_Thick = 0.1;
        public double text_size = 0.3;
        public double pow = 1.0;
        string modeltype = "NONE";
        public int select_vN = int.MaxValue;
        Vector3d move = new Vector3d(0, 0, 0);
        public string Viewport = "";
        public string textbox_user { get { if (Viewport == "") { return "Select Viewport"; } else { return Viewport; } } }

        public string message_export
        {
            get
            {
                string text = "";
                if (modeltype == "NONE") { return "NONE MODEL INPUT"; }
                else if (modeltype == "Form_Diagram") { text += "[ FORM DIAGRAM ]"; }
                else if (modeltype == "Force_Diagram") { text += "[ FORCE DIAGRAM ]"; }
                if (Viewport != "") { text += "\nDISPLAY IN : " + Viewport; }
                return text;
            }
        }
        #region button bool
        public bool display_Edge_line = true;
        public bool display_Edge_text = true;

        public bool display_Node_circle = true;
        public bool display_Node_text = true;

        public bool display_Load_line = true;
        public bool display_Load_text = true;

        public bool display_React_line = true;
        public bool display_React_text = true;

        public bool display_Result_line = false;
        public bool display_Result_text = false;
        #endregion
        #region visualize attributes
        //form diagram
        List<string> edge_st1;
        List<Line> edge_ln1;
        List<Color> edge_cl1;
        List<double> edge_db1;

        List<Point3d> node_pt1;
        List<string> node_st1;
        List<Color> node_cl1;

        List<Point3d> load_pt1;
        List<string> load_st1;
        List<Line> load_ln1;
        List<Color> load_cl1;

        List<Point3d> react_pt1;
        List<string> react_st1;
        List<Line> react_ln1;
        List<Color> react_cl1;

        List<Point3d> result_pt1;
        List<string> result_st1;
        List<Line> result_ln1;
        List<Color> result_cl1;

        //force diagram
        public List<List<Line>> ln2_Edge;
        public List<List<string>> st2_Edge;
        public List<List<Color>> cl2_Edge;
        public List<List<double>> db2_EdgeForce;

        public List<fc_d1> single_fc;
        public List<string> node_edge;
        public List<List<Point3d>> FC_converge;
        #endregion

        public override void BakeGeometry(Rhino.RhinoDoc doc, List<Guid> obj_ids)
        {
            if (modeltype == "NONE") { System.Windows.Forms.MessageBox.Show("No Model Inside the Component"); return; }
            string name = "";
            ShowInputDialog("Bake Model Name :", ref name);
            if (name == "") { return; }

            Rhino.DocObjects.Layer main_title = Rhino.RhinoDoc.ActiveDoc.Layers.FindName(name);
            if (main_title == null)
            {
                main_title = AddLayer(name, Color.Black, -1, Rhino.RhinoDoc.ActiveDoc, ref report);
            }
            else
            {
                bool delete = false;
                int fm_index = Rhino.RhinoDoc.ActiveDoc.Layers.Find(main_title.Id, "Form_Diagram", -1);
                if (fm_index != -1 && modeltype == "Form_Diagram")
                {
                    delete = System.Windows.Forms.MessageBox.Show("The Form_Diagram is already exsiting , do you want to overwrite?", "Warning", System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK;
                    if (delete) { SetSafeLayer(Rhino.RhinoDoc.ActiveDoc); DeepPurge(Rhino.RhinoDoc.ActiveDoc.Layers[fm_index], Rhino.RhinoDoc.ActiveDoc); } else { return; }
                }
                int fc_index = Rhino.RhinoDoc.ActiveDoc.Layers.Find(main_title.Id, "Force_Diagram", -1);
                if (fc_index != -1 && modeltype == "Force_Diagram")
                {
                    delete = System.Windows.Forms.MessageBox.Show("The Force_Diagram is already exsiting , do you want to overwrite?", "Warning", System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK;
                    if (delete) { SetSafeLayer(Rhino.RhinoDoc.ActiveDoc); DeepPurge(Rhino.RhinoDoc.ActiveDoc.Layers[fc_index], Rhino.RhinoDoc.ActiveDoc); } else { return; }
                }
            }

            if (modeltype == "Form_Diagram")
            {
                Rhino.DocObjects.Layer Form_Diagram = AddLayer("Form_Diagram", Color.Black, main_title.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_Internal_force = AddLayer("Internal Force", Color.Black, Form_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_External_force = AddLayer("External Force", Color.DarkGreen, Form_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_Internal_tension = AddLayer("Tension", Color.FromArgb(228, 7, 20), Fm_Internal_force.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_Internal_compression = AddLayer("Compression", Color.FromArgb(5, 120, 191), Fm_Internal_force.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_Nodes = AddLayer("Nodes", Color.Black, Form_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);

                Rhino.DocObjects.Layer Fm_Label = AddLayer("Label", Color.Black, Form_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Node_Label = AddLayer("Node_Label", Color.Black, Fm_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_External_force_label = AddLayer("ExtForce_Label", Color.DarkGreen, Fm_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_Internal_tension_label = AddLayer("Tension_Label", Color.FromArgb(228, 7, 20), Fm_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fm_Internal_compression_label = AddLayer("Compression_Label", Color.FromArgb(5, 120, 191), Fm_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);

                int key = 0;
                for (int i = 0; i < node_pt1.Count; i++)
                {
                    string Fm_Group_name = name + "_Fm_" + key.ToString();
                    if (doc.Groups.FindName(Fm_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fm_Group_name).Index); }
                    doc.Groups.Add(Fm_Group_name);
                    key++;
                    BakeNode(node_pt1[i], Fm_Nodes.Index, node_st1[i], Fm_Group_name, node_cl1[i], 1, Rhino.RhinoDoc.ActiveDoc, ref report);
                    BakeText3D(node_st1[i], Node_Label.Index, name, Fm_Group_name, node_cl1[i], node_pt1[i], text_size * 1.25, Rhino.RhinoDoc.ActiveDoc, ref report);
                }

                for (int i = 0; i < edge_st1.Count; i++)
                {
                    string Fm_Group_name = name + "_Fm_" + key.ToString();
                    if (doc.Groups.FindName(Fm_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fm_Group_name).Index); }
                    doc.Groups.Add(Fm_Group_name);
                    key++;
                    if (edge_cl1[i] == Color.DarkGray)
                    {
                        double width = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(edge_db1[i]), pow);
                        BakeCurve(edge_ln1[i].ToNurbsCurve(), Fm_External_force.Index, edge_st1[i] + "_" + edge_db1[i].ToString(), Fm_Group_name, edge_cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(edge_st1[i], Fm_External_force_label.Index, name, Fm_Group_name, edge_cl1[i], edge_ln1[i].PointAt(0.5), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else if (edge_db1[i] < 0)
                    {
                        double width = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(edge_db1[i]), pow);
                        BakeCurve(edge_ln1[i].ToNurbsCurve(), Fm_Internal_compression.Index, edge_st1[i] + "_" + edge_db1[i].ToString(), Fm_Group_name, edge_cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(edge_st1[i], Fm_Internal_compression_label.Index, name, Fm_Group_name, edge_cl1[i], edge_ln1[i].PointAt(0.7), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else if (edge_db1[i] > 0)
                    {
                        double width = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(edge_db1[i]), pow);
                        BakeCurve(edge_ln1[i].ToNurbsCurve(), Fm_Internal_tension.Index, edge_st1[i] + "_" + edge_db1[i].ToString(), Fm_Group_name, edge_cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(edge_st1[i], Fm_Internal_tension_label.Index, name, Fm_Group_name, edge_cl1[i], edge_ln1[i].PointAt(0.7), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else
                    {
                        BakeCurve(edge_ln1[i].ToNurbsCurve(), Fm_Internal_force.Index, edge_st1[i] + "_" + edge_db1[i].ToString(), Fm_Group_name, edge_cl1[i], 1.5, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(edge_st1[i], Fm_Label.Index, name, Fm_Group_name, edge_cl1[i], edge_ln1[i].PointAt(0.7), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                }
                for (int i = 0; i < load_st1.Count; i++)
                {
                    string Fm_Group_name = name + "_Fm_" + key.ToString();
                    if (doc.Groups.FindName(Fm_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fm_Group_name).Index); }
                    doc.Groups.Add(Fm_Group_name); key++;
                    double width = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(load_ln1[i].Length), pow * 0.5);
                    BakeCurve(load_ln1[i].ToNurbsCurve(), Fm_External_force.Index, load_st1[i] + "_" + load_ln1[i].Length.ToString(), Fm_Group_name, load_cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                    BakeText3D(load_st1[i], Fm_External_force_label.Index, name, Fm_Group_name, load_cl1[i], load_ln1[i].PointAt(0.5), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                }
                for (int i = 0; i < react_st1.Count; i++)
                {
                    string Fm_Group_name = name + "_Fm_" + key.ToString();
                    if (doc.Groups.FindName(Fm_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fm_Group_name).Index); }
                    doc.Groups.Add(Fm_Group_name); key++;
                    if (react_ln1[i].Length < System_Configuration.Sys_Tor) { continue; }
                    double width = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(react_ln1[i].Length), pow * 0.5);
                    BakeCurve(react_ln1[i].ToNurbsCurve(), Fm_External_force.Index, react_st1[i] + "_" + react_ln1[i].Length.ToString(), Fm_Group_name, react_cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                    BakeText3D(react_st1[i], Fm_External_force_label.Index, name, Fm_Group_name, react_cl1[i], react_ln1[i].PointAt(0.5), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                }
            }
            else if (modeltype == "Force_Diagram")
            {
                Rhino.DocObjects.Layer Force_Diagram = AddLayer("Force_Diagram", Color.Black, main_title.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_force = AddLayer("Internal Force", Color.Black, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_External_force = AddLayer("External Force", Color.DarkGreen, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_tension = AddLayer("Tension", Color.FromArgb(228, 7, 20), Fc_Internal_force.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_compression = AddLayer("Compression", Color.FromArgb(5, 120, 191), Fc_Internal_force.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Nodes = AddLayer("Nodes", Color.Black, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);

                Rhino.DocObjects.Layer Fc_Label = AddLayer("Label", Color.Black, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Node_Label = AddLayer("Node_Label", Color.Black, Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_External_force_label = AddLayer("ExtForce_Label", Color.DarkGreen, Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_tension_label = AddLayer("Tension_Label", Color.FromArgb(228, 7, 20), Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_compression_label = AddLayer("Compression_Label", Color.FromArgb(5, 120, 191), Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);

                int key = 0;
                for (int i = 0; i < FC_converge.Count; i++)
                {
                    string Fc_Group_name = name + "_Fc_" + key.ToString();
                    if (doc.Groups.FindName(Fc_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fc_Group_name).Index); }
                    doc.Groups.Add(Fc_Group_name);
                    key++;

                    List<Point3d> pts = FC_converge[i];
                    Point3d midpt = new Point3d(0.0, 0.0, 0.0);
                    foreach (Point3d pt in pts)
                    { midpt += pt; }
                    midpt /= pts.Count;
                    bool converge = true;
                    foreach (Point3d pt in pts) { if (pt.DistanceTo(midpt) > System_Configuration.Sys_Tor) { converge = false; break; } }

                    if (converge)
                    {
                        BakeNode(FC_converge[i][0], Fc_Nodes.Index, "V" + i.ToString(), Fc_Group_name, Color.Black, 1, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D("V" + i.ToString(), Fc_Node_Label.Index, name, Fc_Group_name, Color.Black, FC_converge[i][0], text_size * 1.25, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else
                    {
                        for (int j = 0; j < FC_converge[i].Count; j++)
                        {
                            BakeNode(FC_converge[i][j], Fc_Nodes.Index, "V" + i.ToString(), Fc_Group_name, Color.Black, 1, Rhino.RhinoDoc.ActiveDoc, ref report);
                            BakeText3D("V" + i.ToString(), Fc_Node_Label.Index, name, Fc_Group_name, Color.Black, FC_converge[i][j], text_size * 1.25, Rhino.RhinoDoc.ActiveDoc, ref report);
                        }
                    }
                   
                }

                for (int i = 0; i < ln2_Edge.Count; i++)
                {
                    string Fc_Group_name = name + "_Fc_circle_" + i.ToString();
                    if (doc.Groups.FindName(Fc_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fc_Group_name).Index); }
                    doc.Groups.Add(Fc_Group_name);
                    for (int j = 0; j < ln2_Edge[i].Count; j++)
                    {
                        double width = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(db2_EdgeForce[i][j]), pow);
                        double width_e = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(db2_EdgeForce[i][j]), pow * 0.5);
                        if (cl2_Edge[i][j] == System.Drawing.Color.FromArgb(228, 7, 20))
                        {
                            BakeCurve(ln2_Edge[i][j].ToNurbsCurve(), Fc_Internal_tension.Index, st2_Edge[i][j] + "_" + db2_EdgeForce[i][j].ToString(), Fc_Group_name, cl2_Edge[i][j], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                            BakeText3D(st2_Edge[i][j], Fc_Internal_tension_label.Index, name, Fc_Group_name, cl2_Edge[i][j], ln2_Edge[i][j].PointAt(0.8), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                        }
                        else if (cl2_Edge[i][j] == System.Drawing.Color.FromArgb(5, 120, 191))
                        {
                            BakeCurve(ln2_Edge[i][j].ToNurbsCurve(), Fc_Internal_compression.Index, st2_Edge[i][j] + "_" + db2_EdgeForce[i][j].ToString(), Fc_Group_name, cl2_Edge[i][j], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                            BakeText3D(st2_Edge[i][j], Fc_Internal_compression_label.Index, name, Fc_Group_name, cl2_Edge[i][j], ln2_Edge[i][j].PointAt(0.8), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                        }
                        else if (cl2_Edge[i][j] == System.Drawing.Color.White || cl2_Edge[i][j] == System.Drawing.Color.DarkGray)
                        {
                            BakeCurve(ln2_Edge[i][j].ToNurbsCurve(), Fc_External_force.Index, st2_Edge[i][j] + "_" + db2_EdgeForce[i][j].ToString(), Fc_Group_name, cl2_Edge[i][j], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                            BakeText3D(st2_Edge[i][j], Fc_External_force_label.Index, name, Fc_Group_name, cl2_Edge[i][j], ln2_Edge[i][j].PointAt(0.8), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                        }
                        else
                        {
                            BakeCurve(ln2_Edge[i][j].ToNurbsCurve(), Fc_External_force.Index, st2_Edge[i][j] + "_" + db2_EdgeForce[i][j].ToString(), Fc_Group_name, cl2_Edge[i][j], width_e, Rhino.RhinoDoc.ActiveDoc, ref report);
                            BakeText3D(st2_Edge[i][j], Fc_External_force_label.Index, name, Fc_Group_name, cl2_Edge[i][j], ln2_Edge[i][j].PointAt(0.5), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                        }
                    }
                }
            }

        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("display_Edge_line", display_Edge_line);
            writer.SetBoolean("display_Edge_text", display_Edge_text);

            writer.SetBoolean("display_Node_text", display_Node_text);
            writer.SetBoolean("display_Node_circle", display_Node_circle);

            writer.SetBoolean("display_Load_line", display_Load_line);
            writer.SetBoolean("display_Load_text", display_Load_text);

            writer.SetBoolean("display_React_line", display_React_line);
            writer.SetBoolean("display_React_text", display_React_text);

            writer.SetBoolean("display_Result_line", display_React_line);
            writer.SetBoolean("display_Result_text", display_React_text);

            writer.SetString("Viewport", Viewport);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            Viewport = reader.GetString("Viewport");

            display_Edge_line = reader.GetBoolean("display_Edge_line");
            display_Edge_text = reader.GetBoolean("display_Edge_text");

            display_Node_text = reader.GetBoolean("display_Node_text");
            display_Node_circle = reader.GetBoolean("display_Node_circle");

            display_Load_line = reader.GetBoolean("display_Load_line");
            display_Load_text = reader.GetBoolean("display_Load_text");

            display_React_line = reader.GetBoolean("display_React_line");
            display_React_text = reader.GetBoolean("display_React_text");

            display_Result_line = reader.GetBoolean("display_Result_line");
            display_Result_text = reader.GetBoolean("display_Result_text");
            return base.Read(reader);
        }
        public override void CreateAttributes()//重写调用电池属性的函数
        {
            base.m_attributes = new Attributes_Custom_Visualization_Diagrams(this);
        }
        public void Update(bool Edge_line, bool Edge_text, bool Node_text, bool Node_circle, bool Load_line, bool Load_text, bool React_line, bool React_text, bool Result_line, bool Result_text)//这个很重要，会被另外一个触发事件（鼠标）调用，从而传递数据，并且让电池更新状态
        {
            display_Edge_line = Edge_line;
            display_Edge_text = Edge_text;

            display_Node_text = Node_text;
            display_Node_circle = Node_circle;

            display_Load_line = Load_line;
            display_Load_text = Load_text;

            display_React_line = React_line;
            display_React_text = React_text;

            display_Result_line = Result_line;
            display_Result_text = Result_text;
            this.OnAttributesChanged();
            this.ExpireSolution(true);
        }
        public void Update()//
        {
            this.OnAttributesChanged();
            this.ExpireSolution(true);
        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (Viewport != "")
            {
                if (args.Display.Viewport.Name != Viewport) { return; }
            }

            Plane pl0_Plane;
            args.Viewport.GetFrustumFarPlane(out pl0_Plane);
            view_Point = pl0_Plane;
            if (modeltype == "Form_Diagram")
            {
                #region draw form diagram
                //Draw Edge infor:
                for (int i = 0; i < edge_st1.Count; i++)
                {
                    //line
                    if (display_Edge_line) { DrawLines(pl0_Plane, edge_ln1[i], edge_db1[i], edge_cl1[i], pow, false, args); }

                    //text
                    if (display_Edge_text) { DrawText(pl0_Plane, edge_st1[i], edge_ln1[i].PointAt(0.7), edge_cl1[i], args); }
                }

                //Draw Node infor:
                for (int i = 0; i < node_pt1.Count; i++)
                {
                    //text
                    if (display_Node_text) { DrawText(pl0_Plane, node_st1[i], node_pt1[i], node_cl1[i], args); }
                    if (display_Node_circle) { DrawPoint(text_size * 0.6, node_pt1[i], node_cl1[i], args); }
                }

                //Draw Load infor
                for (int i = 0; i < load_ln1.Count; i++)
                {
                    //line
                    if (display_Load_line) { DrawLines(pl0_Plane, load_ln1[i], load_ln1[i].Length, load_cl1[i], pow, true, args); }
                    //text
                    if (display_Load_text) { DrawText(pl0_Plane, load_st1[i], load_pt1[i], load_cl1[i], args); }
                }

                //Draw React infor
                for (int i = 0; i < react_ln1.Count; i++)
                {
                    if (display_React_line) { DrawLines(pl0_Plane, react_ln1[i], react_ln1[i].Length * 0.5, react_cl1[i], pow, true, args); }
                    if (display_React_text) { DrawText(pl0_Plane, react_st1[i], react_pt1[i], react_cl1[i], args); }
                }

                //Draw Result infor
                for (int i = 0; i < result_ln1.Count; i++)
                {
                    if (display_Result_line) { DrawLines(pl0_Plane, result_ln1[i], result_ln1[i].Length * 0.5, result_cl1[i], pow, true, args); }
                    if (display_Result_text) { DrawText(pl0_Plane, result_st1[i], result_pt1[i], result_cl1[i], args); }
                }
                #endregion
            }
            else if (modeltype == "Force_Diagram")
            {
                #region draw force diagram
                for (int i = 0; i < ln2_Edge.Count; i++)
                {
                    for (int j = 0; j < ln2_Edge[i].Count; j++)
                    {
                        if (cl2_Edge[i][j] == Color.DarkGreen && ln2_Edge[i][j].Length > System_Configuration.Sys_Tor)
                        {
                            if (st2_Edge[i][j].Substring(0, 1) == "Q")
                            {
                                if (display_Load_line) { DrawLines(pl0_Plane, ln2_Edge[i][j], db2_EdgeForce[i][j], cl2_Edge[i][j], pow, true, args); }
                                if (display_Load_text) { DrawText(pl0_Plane, st2_Edge[i][j], ln2_Edge[i][j].PointAt(0.8), cl2_Edge[i][j], args); }
                            }
                            else if (st2_Edge[i][j].Substring(0, 1) == "R")
                            {
                                if (display_React_line) { DrawLines(pl0_Plane, ln2_Edge[i][j], db2_EdgeForce[i][j], cl2_Edge[i][j], pow, true, args); }
                                if (display_React_text) { DrawText(pl0_Plane, st2_Edge[i][j], ln2_Edge[i][j].PointAt(0.8), cl2_Edge[i][j], args); }
                            }
                        }
                        else if (ln2_Edge[i][j].Length > System_Configuration.Sys_Tor)
                        {
                            if (display_Edge_line) { DrawLines(pl0_Plane, ln2_Edge[i][j], db2_EdgeForce[i][j], cl2_Edge[i][j], pow, false, args); }
                            if (display_Edge_text) { DrawText(pl0_Plane, st2_Edge[i][j], ln2_Edge[i][j].PointAt(0.8), cl2_Edge[i][j], args); }
                        }
                    }
                }
                #endregion
                #region draw the Fc convergence
                for (int i = 0; i < FC_converge.Count; i++)
                {
                    List<Point3d> pts = FC_converge[i];
                    Point3d midpt = new Point3d(0.0, 0.0, 0.0);
                    foreach (Point3d pt in pts)
                    { midpt += pt; }
                    midpt /= pts.Count;
                    bool converge = true;
                    foreach (Point3d pt in pts) { if (pt.DistanceTo(midpt) > System_Configuration.Sys_Tor) { converge = false; break; } }

                    if (!converge) { foreach (Point3d p in pts) { if (display_Node_circle) { DrawNode(pl0_Plane, i.ToString(), 14, p, Color.Orange, args); } } }
                    else
                    {
                        if (display_Node_circle)
                        { DrawPoint(text_size * 0.6, pts[0], Color.Black, args); }
                        if (display_Node_text)
                        { DrawText(pl0_Plane, "V" + i.ToString(), midpt, Color.Black, args); }
                    }
                }
                #endregion
            }
        }
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Bake SingleLine Forcediagram", BakeSingleLineForceDiagram);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Only show the diagram in:");
            Menu_AppendTextItem(menu, textbox_user, StartToType, TypeFinished, true);
            Menu_AppendSeparator(menu);
            base.AppendAdditionalMenuItems(menu);
        }

        private void BakeSingleLineForceDiagram(Object sender, EventArgs e)
        {
            List<Line> ln1 = new List<Line>();
            List<string> st1 = new List<string>();
            List<Color> cl1 = new List<Color>();
            List<double> db1 = new List<double>();

            //Considering the selected circile and import fc data
            if (select_vN < st2_Edge.Count)
            {
                List<string> circle_ids = st2_Edge[select_vN];
                List<Line> circle_lns = ln2_Edge[select_vN];
                for (int i = 0; i < single_fc.Count; i++)
                {
                    ln1.Add(single_fc[i].ln0_Edge); st1.Add(single_fc[i].st0_Edge); db1.Add(single_fc[i].db0_EdgeForce);
                    if (!circle_ids.Contains(single_fc[i].st0_Edge) || (circle_ids.Contains(single_fc[i].st0_Edge) && !sameline_midpoint_list(single_fc[i].ln0_Edge, circle_lns, System_Configuration.Sys_Tor)))
                    { cl1.Add(Color.DarkGray); }
                    else if (!node_edge.Contains(single_fc[i].st0_Edge) && single_fc[i].st0_Edge.Substring(0, 1) != "Q" && single_fc[i].st0_Edge.Substring(0, 1) != "R") { cl1.Add(Color.Black); }
                    else { cl1.Add(single_fc[i].cl0_Edge); }
                }
            }
            else
            {
                ln1 = single_fc.Select(p => p.ln0_Edge).ToList();
                st1 = single_fc.Select(p => p.st0_Edge).ToList();
                cl1 = single_fc.Select(p => p.cl0_Edge).ToList();
                db1 = single_fc.Select(p => p.db0_EdgeForce).ToList();
            }

            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
            if (modeltype == "NONE") { System.Windows.Forms.MessageBox.Show("No Model Inside the Component"); return; }
            if (modeltype == "Form_Diagram") { System.Windows.Forms.MessageBox.Show("Only viable for force diagram"); return; }
            string name = "";
            ShowInputDialog("Bake Model Name :", ref name);
            if (name == "") { return; }
            Rhino.DocObjects.Layer main_title = Rhino.RhinoDoc.ActiveDoc.Layers.FindName(name);
            if (main_title == null)
            {
                main_title = AddLayer(name, Color.Black, -1, Rhino.RhinoDoc.ActiveDoc, ref report);
            }
            else
            {
                bool delete = false;
                int fm_index = doc.Layers.Find(main_title.Id, "Form_Diagram", -1);
                if (fm_index != -1 && modeltype == "Form_Diagram")
                {
                    delete = System.Windows.Forms.MessageBox.Show("The Form_Diagram is already exsiting , do you want to overwrite?", "Warning", System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK;
                    if (delete) { SetSafeLayer(doc); DeepPurge(doc.Layers[fm_index], Rhino.RhinoDoc.ActiveDoc); } else { return; }
                }
                int fc_index = Rhino.RhinoDoc.ActiveDoc.Layers.Find(main_title.Id, "Force_Diagram", -1);
                if (fc_index != -1 && modeltype == "Force_Diagram")
                {
                    delete = System.Windows.Forms.MessageBox.Show("The Force_Diagram is already exsiting , do you want to overwrite?", "Warning", System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK;
                    if (delete) { SetSafeLayer(doc); DeepPurge(doc.Layers[fc_index], doc); } else { return; }
                }
            }

            if (modeltype == "Force_Diagram")
            {
                Rhino.DocObjects.Layer Force_Diagram = AddLayer("Force_Diagram", Color.Black, main_title.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_force = AddLayer("Internal Force", Color.Black, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_External_force = AddLayer("External Force", Color.DarkGreen, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_tension = AddLayer("Tension", Color.FromArgb(228, 7, 20), Fc_Internal_force.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_compression = AddLayer("Compression", Color.FromArgb(5, 120, 191), Fc_Internal_force.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Nodes = AddLayer("Nodes", Color.Black, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);

                Rhino.DocObjects.Layer Fc_Label = AddLayer("Label", Color.Black, Force_Diagram.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Node_Label = AddLayer("Node_Label", Color.Black, Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_External_force_label = AddLayer("ExtForce_Label", Color.DarkGreen, Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_tension_label = AddLayer("Tension_Label", Color.FromArgb(228, 7, 20), Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);
                Rhino.DocObjects.Layer Fc_Internal_compression_label = AddLayer("Compression_Label", Color.FromArgb(5, 120, 191), Fc_Label.Index, Rhino.RhinoDoc.ActiveDoc, ref report);

                int key = 0;
                for (int i = 0; i < FC_converge.Count; i++)
                {
                    string Fc_Group_name = name + "_Fc_Single_" + key.ToString();
                    if (doc.Groups.FindName(Fc_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fc_Group_name).Index); }
                    doc.Groups.Add(Fc_Group_name);
                    key++;

                    List<Point3d> pts = FC_converge[i];
                    Point3d midpt = new Point3d(0.0, 0.0, 0.0);
                    foreach (Point3d pt in pts)
                    { midpt += pt; }
                    midpt /= pts.Count;
                    bool converge = true;
                    foreach (Point3d pt in pts) { if (pt.DistanceTo(midpt) > System_Configuration.Sys_Tor) { converge = false; break; } }

                    if (converge)
                    {
                        BakeNode(FC_converge[i][0], Fc_Nodes.Index, "V" + i.ToString(), Fc_Group_name, Color.Black, 1, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D("V" + i.ToString(), Fc_Node_Label.Index, name, Fc_Group_name, Color.Black, FC_converge[i][0], text_size * 1.25, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else
                    {
                        for (int j = 0; j < FC_converge[i].Count; j++)
                        {
                            BakeNode(FC_converge[i][j], Fc_Nodes.Index, "V" + i.ToString(), Fc_Group_name, Color.Black, 1, Rhino.RhinoDoc.ActiveDoc, ref report);
                            BakeText3D("V" + i.ToString(), Fc_Node_Label.Index, name, Fc_Group_name, Color.Black, FC_converge[i][j], text_size * 1.25, Rhino.RhinoDoc.ActiveDoc, ref report);
                        }
                    }

                }

                for (int i = 0; i < ln1.Count; i++)
                {
                    string Fc_Group_name = name + "_Fc_Single_" + i.ToString();
                    if (doc.Groups.FindName(Fc_Group_name) != null) { doc.Groups.Delete(doc.Groups.FindName(Fc_Group_name).Index); }
                    doc.Groups.Add(Fc_Group_name);

                    double width = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(db1[i]), pow);
                    double width_e = 1.5 + Math.Pow(db0_Line_Thick * Math.Abs(db1[i]), pow * 0.5);

                    if (cl1[i] == System.Drawing.Color.FromArgb(228, 7, 20))
                    {
                        BakeCurve(ln1[i].ToNurbsCurve(), Fc_Internal_tension.Index, st1[i] + "_" + db1[i].ToString(), Fc_Group_name, cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(st1[i], Fc_Internal_tension_label.Index, name, Fc_Group_name, cl1[i], ln1[i].PointAt(0.4), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else if (cl1[i] == System.Drawing.Color.FromArgb(5, 120, 191))
                    {
                        BakeCurve(ln1[i].ToNurbsCurve(), Fc_Internal_compression.Index, st1[i] + "_" + db1[i].ToString(), Fc_Group_name, cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(st1[i], Fc_Internal_compression_label.Index, name, Fc_Group_name, cl1[i], ln1[i].PointAt(0.4), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else if (cl1[i] == System.Drawing.Color.White || cl1[i] == System.Drawing.Color.DarkGray)
                    {
                        BakeCurve(ln1[i].ToNurbsCurve(), Fc_External_force.Index, st1[i] + "_" + db1[i].ToString(), Fc_Group_name, cl1[i], width, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(st1[i], Fc_External_force_label.Index, name, Fc_Group_name, cl1[i], ln1[i].PointAt(0.4), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }
                    else
                    {
                        BakeCurve(ln1[i].ToNurbsCurve(), Fc_External_force.Index, st1[i] + "_" + db1[i].ToString(), Fc_Group_name, cl1[i], width_e, Rhino.RhinoDoc.ActiveDoc, ref report);
                        BakeText3D(st1[i], Fc_External_force_label.Index, name, Fc_Group_name, cl1[i], ln1[i].PointAt(0.5), text_size, Rhino.RhinoDoc.ActiveDoc, ref report);
                    }

                }
            }
        }

        private void StartToType(Object sender, EventArgs e)
        {
            return;
        }
        private void TypeFinished(Object sender, string newtext)
        {
            Viewport = newtext;
            this.OnAttributesChanged();
            this.ExpireSolution(true);
            return;
        }
        public void DrawText(Plane pl0_Plane, string text, Point3d location, Color color, IGH_PreviewArgs args)
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
        public void DrawNode(Plane pl0_Plane, string text, int height, Point3d location, Color color, IGH_PreviewArgs args)
        {
            pl0_Plane.Origin = location;
            Rhino.Geometry.TextDot textdot = new TextDot(text, location); textdot.FontHeight = height;
            args.Display.DrawDot(textdot, color, Color.White, Color.White);
            textdot.Dispose();
        }
        public void DrawPoint(double radius, Point3d location, Color color, IGH_PreviewArgs args)
        {
            args.Display.DrawPoint(location, (Rhino.Display.PointStyle)4, color, Color.White, (float)radius, (float)radius * (float)0.4, (float)radius * (float)0.0, (float)3.14, false, true);
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
        public override BoundingBox ClippingBox
        {
            get
            {
                return BoundingBox.Empty;
            }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("F|F*", "F|F*", "Form diagram or force diagram", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddNumberParameter("ThickLn", "ThickLn", "Set the thickness of the lines for visualization", GH_ParamAccess.item, db0_Line_Thick);
            Input.AddNumberParameter("ThickLnVar", "ThickLnVar", "Set the variance of the line thickness for visualization", GH_ParamAccess.item, pow);
            Input.AddNumberParameter("SizeTxt", "SizeTxt", "Set the size of the text labels for visualization", GH_ParamAccess.item, text_size);
            Input.AddIntegerParameter("NodeID", "NodeID", "Set the IDs of the nodes to be visualized", GH_ParamAccess.item, int.MaxValue);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            MODEL FD = new MODEL();
            if (!data.GetData(0, ref FD)) { return; }
            MODEL FM = FD.CopySelf();

            VgsCommon auxi = new VgsCommon();
            Message = "";
            data.GetData(1, ref db0_Line_Thick);
            data.GetData(2, ref pow);
            data.GetData(3, ref text_size);
            data.GetData(4, ref select_vN);

            if (FD.model_status == "[ELASTIC Model]" || FD.model_status == "[PLASTIC Model]" || FD.model_status == "[Form Diagram]")
            {
                modeltype = "Form_Diagram";

                //Select Node
                List<string> adj_eN = new List<string>();
                if (select_vN < FM.Diagram.Nodes.Count)
                {
                    adj_eN = ListCopy(FM.Diagram.Nodes[select_vN].adj_Edge_ID);
                    string vN = FM.Diagram.Nodes[select_vN].ID; adj_eN.Add(vN);
                    foreach (LOAD load in FM.Diagram.Loads) { if (load.ActionPt_ID == vN) { adj_eN.Add(load.ID); } }
                    foreach (REACT React in FM.Diagram.Reacts) { if (React.ActionPt_ID == vN) { adj_eN.Add(React.ID); } }
                }
                else
                {
                    foreach (EDGE edge in FM.Diagram.Edges) { adj_eN.Add(edge.ID); }
                    foreach (NODE node in FM.Diagram.Nodes) { adj_eN.Add(node.ID); }
                    foreach (LOAD load in FM.Diagram.Loads) { adj_eN.Add(load.ID); }
                    foreach (REACT React in FM.Diagram.Reacts) { adj_eN.Add(React.ID); }
                    foreach (RESULT Result in FM.Diagram.Result) { adj_eN.Add(Result.ID); }
                }
                //
                #region FormDiagram
                //FM.Diagram.Edges;
                edge_st1 = new List<string>();
                edge_ln1 = new List<Line>();
                edge_cl1 = new List<Color>();
                edge_db1 = new List<double>();
                foreach (EDGE edge in FM.Diagram.Edges)
                {
                    edge_st1.Add(edge.ID);
                    edge_ln1.Add(edge.ln);
                    if (adj_eN.Contains(edge.ID)) { edge_cl1.Add(edge.col); } else { edge_cl1.Add(Color.DarkGray); }
                    edge_db1.Add(edge.force);
                }

                //FM.Diagram.Nodes;
                node_pt1 = new List<Point3d>();
                node_st1 = new List<string>();
                node_cl1 = new List<Color>();

                foreach (NODE node in FM.Diagram.Nodes)
                {
                    int i = FM.Diagram.Nodes.IndexOf(node);
                    node_pt1.Add(node.pt);
                    node_st1.Add(node.ID);
                    if (adj_eN.Contains(node.ID)) { node_cl1.Add(Color.Black); } else { node_cl1.Add(Color.DarkGray); }
                }

                //FM.Diagram.Loads;
                load_pt1 = new List<Point3d>();
                load_st1 = new List<string>();
                load_ln1 = new List<Line>();
                load_cl1 = new List<Color>();
                foreach (LOAD load in FM.Diagram.Loads)
                {
                    if (load.ln.Length < System_Configuration.Sys_Tor) { continue; }
                    load_pt1.Add(load.ln.PointAt(0.8));
                    load_st1.Add(load.ID);
                    load_ln1.Add(load.ln);
                    if (adj_eN.Contains(load.ID)) { load_cl1.Add(load.col); } else { load_cl1.Add(Color.DarkGray); }
                }

                //FM.Diagram.Reacts;
                react_pt1 = new List<Point3d>();
                react_st1 = new List<string>();
                react_ln1 = new List<Line>();
                react_cl1 = new List<Color>();
                foreach (REACT react in FM.Diagram.Reacts)
                {
                    if (react.ln.Length < System_Configuration.Sys_Tor) { continue; }
                    react_pt1.Add(react.ln.From);
                    react_st1.Add(react.ID);
                    react_ln1.Add(react.ln);
                    if (adj_eN.Contains(react.ID)) { react_cl1.Add(react.col); } else { react_cl1.Add(Color.DarkGray); }
                }

                //FM.Diagram.Result;
                result_pt1 = new List<Point3d>();
                result_st1 = new List<string>();
                result_ln1 = new List<Line>();
                result_cl1 = new List<Color>();
                foreach (RESULT result in FM.Diagram.Result)
                {
                    if (result.ln.Length < System_Configuration.Sys_Tor) { continue; }
                    result_pt1.Add(result.ln.From);
                    result_st1.Add(result.ID);
                    result_ln1.Add(result.ln);
                    if (adj_eN.Contains(result.ID)) { result_cl1.Add(result.col); } else { result_cl1.Add(Color.DarkGray); }
                }
                #endregion

            }
            else if (FD.model_status == "[Force Diagram]")
            {
                //System_Configuration.SetLocation_fc(FD.GUI, location);
                modeltype = "Force_Diagram";

                #region ForceDiagram

                ln2_Edge = ListListCopy(FD.Diagram.ln2_Edge);
                st2_Edge = ListListCopy(FD.Diagram.st2_Edge);
                cl2_Edge = ListListCopy(FD.Diagram.cl2_Edge);
                db2_EdgeForce = ListListCopy(FD.Diagram.db2_EdgeForce);

                FC_converge = new List<List<Point3d>>();
                FC_converge = FD.Diagram.FcConvergePts(FD.Diagram.Fc_Pts_Addr, FD.Diagram.ln2_Edge);

                single_fc = FD.Diagram.TransToSingleLineFc(System_Configuration.Sys_Tor);
                if (select_vN < FM.Diagram.Nodes.Count) { node_edge = ListCopy(FM.Diagram.Nodes[select_vN].adj_Edge_ID); }
                else { node_edge = FM.Diagram.Edges.Select(p => p.ID).ToList(); }
                #endregion

                for (int i = 0; i < ln2_Edge.Count; i++)
                {
                    for (int j = 0; j < ln2_Edge[i].Count; j++)
                    {
                        //ln2_Edge[i][j] = new Line(new Point3d(ln2_Edge[i][j].From + move), new Point3d(ln2_Edge[i][j].To + move));//move the force diagram
                        if (i != select_vN && select_vN < FD.Diagram.ln2_Edge.Count) { cl2_Edge[i][j] = Color.DarkGray; }// Select the circle
                        else if (i == select_vN && i < FD.st2_NodeEdge.Count)//For the med case, the duplicate should be black
                        {
                            if (!FD.st2_NodeEdge[i].Contains(st2_Edge[i][j]) && st2_Edge[i][j].Substring(0, 1) != "Q" && st2_Edge[i][j].Substring(0, 1) != "R") { cl2_Edge[i][j] = Color.Black; }
                        }
                    }
                }
            }

            Message = message_export;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_56;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("afa63ec7-986b-4c8d-963d-85b188d4ddbe"); }//
        }
    }
}