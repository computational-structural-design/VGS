using System;
using System.Collections.Generic;
using System.Linq;
/*
@Misc{vgs2021,
author = {D'Acunto, Pierluigi and Shen, Yuchi and Jasienski, Jean-Philippe and Ohlbrock, Patrick Ole},
title = {{VGS Tool: Vector-based Graphic Statics}},
year = {2021},
note = {Release 1.00 Beta},
url = { https://github.com/computational-structural-design/VGS.git },
}
 */

using System.Drawing;
using Rhino.Geometry;
using Rhino.DocObjects;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using System.Windows.Forms;
using System.Diagnostics;


namespace VGS_Main
{
    public class VgsCommon
    {
        public class MiniWatch
        {
            public Stopwatch sw;
            public TimeSpan ts;
            List<double> TimeGap;
            List<string> Title;

            public MiniWatch()
            {
                sw = new Stopwatch();
                ts = new TimeSpan(0);
                TimeGap = new List<double>();
                Title = new List<string>();
                if (!sw.IsRunning) { sw.Restart(); }
            }

            public void Record(string title)
            {
                Title.Add(title);
                sw.Stop();
                ts = sw.Elapsed;
                TimeGap.Add(Math.Ceiling(ts.TotalMilliseconds * 10) / 10);
                if (!sw.IsRunning) { sw.Restart(); }
            }

            public void End()
            {
                sw.Stop();
            }

            public string GetTime()
            {
                if (sw.IsRunning) { sw.Stop(); }
                return string.Join("\n", TimeGap.Select((p, index) => string.Format("{0}_:{1} ms", Title[index], p.ToString())));
            }

            public string totalTime()
            {
                if (sw.IsRunning) { sw.Stop(); }
                double sum = 0.0; foreach (double time in TimeGap) { sum += time; }
                return string.Format("Total_:{0} ms", sum.ToString());
            }
        }

        #region//[02]Organize
        public static List<double> Remap(List<double> source, double min, double max)
        {
            if (source.Count == 0) { return new List<double>(); }
            if (source.Count == 1) { return new List<double>() { max }; }
            double min_s = source.Min(); double max_s = source.Max();
            double rate = (max - min) / (max_s - min_s);
            List<double> result = source.Select(p => min + rate * (p - min_s)).ToList();
            return result;
        }
        public static List<T> Flattern2D<T>(List<List<T>> t1)
        {
            List<T> temp = new List<T>();
            foreach (List<T> t0 in t1)
            {
                temp.AddRange(t0);
            }
            return temp;
        }
        public static DataTree<T> ListToTree2D<T>(List<List<T>> set)
        {
            DataTree<T> temp = new DataTree<T>();
            for (int i = 0; i < set.Count; i++)
            {
                for (int j = 0; j < set[i].Count; j++)
                {
                    temp.Add(set[i][j], new GH_Path(i));
                }
            }
            return temp;
        }
        public T[,] To2DArray<T>(List<List<T>> xx2_List_in)
        {
            var xx2a_Array = new T[xx2_List_in.Count, xx2_List_in[0].Count];
            for (int i = 0; i < xx2_List_in.Count; i++)
            {
                for (int j = 0; j < xx2_List_in[i].Count; j++)
                {
                    xx2a_Array[i, j] = xx2_List_in[i][j];
                }
            }
            return xx2a_Array;
        }

        #endregion Organize

        #region//[03]Reading
        public static bool ComparePts(Point3d Pt_1, Point3d Pt_2, double Threshold)
        {
            if (System.Math.Abs(Pt_1.X - Pt_2.X) < Threshold && System.Math.Abs(Pt_1.Y - Pt_2.Y) < Threshold && System.Math.Abs(Pt_1.Z - Pt_2.Z) < Threshold)
            { return true; }
            else
            { return false; }
        }
        public static bool sameline_midpoint_list(Line ln, List<Line> lns, double tol)
        {
            bool same = false;
            foreach (Line line in lns)
            {
                if (ComparePts(0.5 * line.From + 0.5 * line.To, 0.5 * ln.From + 0.5 * ln.To, tol)) { same = true; return same; }
            }
            return same;
        }
        #endregion Reading
        #region//[04]Copy
        public static List<T> ListCopy<T>(List<T> set)
        {
            List<T> temp = new List<T>();
            foreach (T sub in set)
            { temp.Add(sub); }
            return temp;
        }
        public static List<List<T>> ListListCopy<T>(List<List<T>> set)
        {
            List<List<T>> temp = new List<List<T>>();
            foreach (List<T> sub in set)
            { temp.Add(ListCopy(sub)); }
            return temp;
        }
        public List<string> CopyStringList(List<string> list)
        {
            List<string> newlist = new List<string>();
            foreach (string str in list)
            { newlist.Add(str); }
            return newlist;
        }
        #endregion Copy
        #region//[05]Baking
        public static Rhino.DocObjects.Layer AddLayer(string layer_name, Color color, int father_index, Rhino.RhinoDoc doc, ref string report)
        {
            if (layer_name == null || Rhino.DocObjects.Layer.IsValidName(layer_name) == false) { return null; }

            Rhino.DocObjects.Tables.LayerTable layerTable = doc.Layers;
            int layerIndex;
            if (layerTable.FindName(layer_name) != null) { layerIndex = layerTable.FindName(layer_name).Index; }
            else { layerIndex = -1; }

            Rhino.DocObjects.Layer myLayer = new Rhino.DocObjects.Layer();
            if (father_index != -1) { myLayer.ParentLayerId = doc.Layers[father_index].Id; }
            myLayer.Color = color;
            myLayer.Name = layer_name;
            layerIndex = layerTable.Add(myLayer);//Add the lay;
            report += "\n" + string.Format("New Layer [{0}][{1}] Added", layer_name, layerIndex.ToString());
            return doc.Layers[layerIndex];
        }
        public static DialogResult ShowInputDialog(string title, ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(800, 130);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = title;

            inputBox.WindowState = FormWindowState.Normal;
            inputBox.StartPosition = FormStartPosition.Manual;
            inputBox.BringToFront();
            inputBox.Location = Cursor.Position;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 20, 40);
            textBox.Location = new System.Drawing.Point(10, 10);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(150, 40);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 160 - 160, 80);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(150, 40);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 160, 80);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }
        public static void BakeNode(Point3d position, int lay_index, string vname, string GroupName, Color color, double width, Rhino.RhinoDoc doc, ref string report)
        {
            Group gp = doc.Groups.FindName(GroupName);//find the group to insert the model

            Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes();
            att.Name = vname;
            if (lay_index != -1) { att.LayerIndex = lay_index; } else { report += "\n" + "Fail baking"; return; }

            if (color != null)
            {
                att.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                att.ObjectColor = color;
                att.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject;
                att.PlotColor = color;
            }

            att.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject;
            if (width != 0.0) { att.PlotWeight = width; }

            Guid current_guid = doc.Objects.AddPoint(position, att);
            doc.Groups.AddToGroup(gp.Index, current_guid);//insert to group by the guid
        }
        public static void BakeCurve(Curve curve, int lay_index, string edgename, string GroupName, Color color, double width, Rhino.RhinoDoc doc, ref string report)
        {
            Group gp = doc.Groups.FindName(GroupName); //find the group to insert the model

            Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes();
            att.Name = edgename;
            if (lay_index != -1) { att.LayerIndex = lay_index; } else { report += "\n" + "Fail baking"; return; }

            if (color != null)
            {
                att.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                att.ObjectColor = color;
                att.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject;
                att.PlotColor = color;
            }

            att.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject;
            if (width != 0.0) { att.PlotWeight = width; }

            Guid current_guid = doc.Objects.AddCurve(curve, att);
            if (GroupName != "") { doc.Groups.AddToGroup(gp.Index, current_guid); }//insert to group by the guid
        }
        public static void BakeText3D(string text, int lay_index, string model_name, string GroupName, Color color, Point3d location, double height, Rhino.RhinoDoc doc, ref string report)
        {
            Rhino.Geometry.Plane view_Point = new Rhino.Geometry.Plane(location, doc.Views.ActiveView.ActiveViewport.CameraX, doc.Views.ActiveView.ActiveViewport.CameraY);
            Group gp = doc.Groups.FindName(GroupName);//find the group to insert the model
            Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes();
            att.Name = model_name;
            if (lay_index != -1) { att.LayerIndex = lay_index; } else { report += "\n" + "Fail baking"; return; }

            if (color != null)
            {
                att.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                att.ObjectColor = color;
                att.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject;
                att.PlotColor = color;
            }
            Rhino.Display.Text3d TEXT = new Rhino.Display.Text3d(text, view_Point, height);

            Guid current_guid = doc.Objects.AddText(TEXT, att);
            if (GroupName != "") { doc.Groups.AddToGroup(gp.Index, current_guid); }//insert to group by the guid
        }
        public static void SetSafeLayer(Rhino.RhinoDoc doc)
        {
            int ind;
            Rhino.DocObjects.Layer lay = doc.Layers.FindName("Default");
            if (lay == null)
            {
                Rhino.DocObjects.Layer newlay = new Rhino.DocObjects.Layer();
                newlay.Name = "Default";
                doc.Layers.Add(newlay);
                ind = newlay.Index;
            }
            else { ind = lay.Index; }
            doc.Layers.SetCurrentLayerIndex(ind, true);
        }
        public static void DeepPurge(Rhino.DocObjects.Layer Lay, Rhino.RhinoDoc doc)
        {
            while (true)
            {
                if (Lay.GetChildren() == null) { doc.Layers.Purge(Lay.Id, false); break; }
                else
                {
                    List<Rhino.DocObjects.Layer> sublay = new List<Rhino.DocObjects.Layer>(Lay.GetChildren());
                    foreach (Rhino.DocObjects.Layer lay in sublay)
                    {
                        DeepPurge(lay, doc);
                    }
                }
            }
        }
        #endregion
        #region//[06]Statistic
        public class Statistic
        {
            public string type = "None";
            private double bound_height;
            private double bound_width;
            private int value_num;
            private double a_p;
            private double A;

            public List<Line> frame_lines;
            public List<Line> col_lines;
            private List<double> display_values;
            private List<string> value_tags;
            private List<Color> colors;
            public Statistic() { }
            public Statistic(string graph_type, Point3d Location, double b_height, double b_width, double ap_rate, List<double> sta_values, List<string> tags, List<Color> cols)
            {
                this.type = graph_type;
                this.bound_height = b_height;
                this.bound_width = b_width;
                this.value_num = sta_values.Count;
                this.a_p = ap_rate;
                this.value_tags = tags;
                this.display_values = sta_values;
                this.colors = cols;
                //Move the graph
                double x = Location.X; double y = Location.Y; double z = Location.Z;

                //a and p;
                double p = bound_width / (a_p * value_num + value_num + 1);
                double a = a_p * p; A = a;

                //draw the frame
                int frame_num = value_num * 2 + 2;
                frame_lines = new List<Line>(frame_num);
                double x_c = 0.0;
                for (int i = 0; i < frame_num; i++)
                {
                    if (i == 0) { x_c += 0; }
                    else if (i % 2 == 1) { x_c += p; }
                    else { x_c += a; }

                    frame_lines.Add(new Line(new Point3d(x_c + x, 0 + y, 0 + z), new Point3d(x_c + x, bound_height + y, 0 + z)));
                }

                //draw the columns
                col_lines = new List<Line>(value_num);
                if (type == "LoadPath")
                {
                    List<double> col_len = Remap(sta_values, bound_height * 0.1, bound_height * 0.9);
                    for (int i = 0; i < value_num; i++)
                    {
                        double x_cor = p * ((i + 1) * a_p + (i + 1) - 0.5 * a_p);

                        col_lines.Add(new Line(new Point3d(x_cor + x, 0.0 + y, 0.0 + z), new Point3d(x_cor + x, col_len[i] + y, 0.0 + z)));
                    }
                }
                else if (type == "Forces")
                {
                    List<double> values = sta_values.Select(k => System.Math.Abs(k)).ToList();
                    List<double> col_len = Remap(values, bound_height * 0.05, bound_height * 0.45);
                    for (int i = 0; i < value_num; i++)
                    {
                        double x_cor = p * ((i + 1) * a_p + (i + 1) - 0.5 * a_p);
                        double y_cor = 0.0;
                        if (sta_values[i] > 0) { y_cor = bound_height * 0.5 + col_len[i]; }
                        else { y_cor = bound_height * 0.5 - col_len[i]; }

                        col_lines.Add(new Line(new Point3d(x_cor + x, bound_height * 0.5 + y, 0.0 + z), new Point3d(x_cor + x, y_cor + y, 0.0 + z)));
                    }
                }
            }
            public void Display(IGH_PreviewArgs args)
            {
                Rhino.Geometry.Plane pl0_Plane;
                args.Viewport.GetFrustumFarPlane(out pl0_Plane);
                //draw frame
                foreach (Line ln in frame_lines)
                {
                    DrawLines(pl0_Plane, ln, 0.5, 1, Color.DarkGray, 1, false, args);
                }
                //draw columns
                for (int i = 0; i < col_lines.Count; i++)
                {
                    Line ln = col_lines[i];
                    Color col = this.colors[i];
                    if (type == "LoadPath") { DrawLines(pl0_Plane, ln, 5 * A, 1, Color.Black, 1, false, args); }
                    else if (type == "Forces") { DrawLines(pl0_Plane, ln, 5 * A, 1, col, 1, false, args); }

                    DrawText(pl0_Plane, Math.Round(display_values[i], 2).ToString(), 0.3 * A, ln.To, Color.Black, args);
                    DrawText(pl0_Plane, value_tags[i], 0.3 * A, ln.From, Color.Black, args);
                }

            }
        }

        #endregion
        #region //[07]DisplayinView
        public static void DrawText(Plane pl0_Plane, string text, double size, Point3d location, Color color, IGH_PreviewArgs args)
        {
            pl0_Plane.Origin = location;
            Rhino.Display.Text3d drawText = new Rhino.Display.Text3d(text, pl0_Plane, size);
            drawText.HorizontalAlignment = (TextHorizontalAlignment)2;
            drawText.VerticalAlignment = (TextVerticalAlignment)1;
            args.Display.Draw3dText(drawText, color);
            drawText.Dispose();
        }
        public static void DrawLines(Rhino.Geometry.Plane pl0_Plane, Line line, double magni, double thickness, Color color, double pow, bool arrowhead, IGH_PreviewArgs args)
        {
            int thick;
            if (color == Color.DarkGray) { thick = 1; }
            else { thick = Convert.ToInt32(1.5 + Math.Pow(thickness * Math.Abs(magni), pow)); }
            if (!arrowhead)
            { args.Display.DrawLine(line, color, thick); }
            else
            {
                foreach (Line ln in AddArrowHead(line, pl0_Plane))
                { args.Display.DrawLine(ln, color, thick); }
            }
        }
        private static List<Line> AddArrowHead(Line ln, Rhino.Geometry.Plane ViewPlane)
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
        #endregion

        //data struct
        public struct fc_d1
        {
            public Line ln0_Edge;
            public string st0_Edge;
            public Color cl0_Edge;
            public double db0_EdgeForce;

            public int id
            {
                get { return System.Convert.ToInt32(st0_Edge.Substring(1)); }
            }

            public fc_d1(Line ln0, string st0, Color cl0, double db0)
            {
                ln0_Edge = ln0;
                st0_Edge = st0;
                cl0_Edge = cl0;
                db0_EdgeForce = db0;
            }
        }
    }
}
