using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using VGS_Main;
using AUXI = VGS_Main.VgsCommon;
using GH_IO.Serialization;
using System.Linq;

namespace GraphicStatic
{
    public class Attributes_Custom_Planarization : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public Attributes_Custom_Planarization(GH_Component owner) : base(owner) { }//Rewrite the constructor, and use base() to retain all the methods of the constructor in the base class;
        private System.Drawing.Rectangle Button_IntegratedFc { get; set; }
        private System.Drawing.Rectangle Button_DiscretedFc { get; set; }

        public bool display_IntegratedFc = true;
        public bool display_DiscretedFc = false;
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("display_IntegratedFc", display_IntegratedFc);
            writer.SetBoolean("display_DiscretedFc", display_DiscretedFc);

            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            display_IntegratedFc = reader.GetBoolean("display_IntegratedFc");
            display_DiscretedFc = reader.GetBoolean("display_DiscretedFc");

            return base.Read(reader);
        }
        protected override void Layout()
        {
            base.Layout();

            System.Drawing.Rectangle rec0 = GH_Convert.ToRectangle(Bounds);//GH_Attributes<IGH_Component>.Bounds  RectangleF class acquare the boundary attributes of the component 
            int col_num = 2;
            int row_num = 1;
            rec0.Height += 22 * col_num; 
            System.Drawing.Rectangle rec_alg_R = rec0;

            Button_IntegratedFc = SetRectangle(rec0, 1, col_num, row_num, 1, 22);
            Button_DiscretedFc = SetRectangle(rec0, 2, col_num, row_num, 1, 22);

            Bounds = rec0;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF B_JFc = Button_IntegratedFc;
                if (B_JFc.Contains(e.CanvasLocation))
                {
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_DFc = Button_DiscretedFc;
                if (B_DFc.Contains(e.CanvasLocation))
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
                System.Drawing.RectangleF B_JFc = Button_IntegratedFc;
                if (B_JFc.Contains(e.CanvasLocation))
                {
                    if (display_IntegratedFc == true) { display_IntegratedFc = false; display_DiscretedFc = true; } else { display_IntegratedFc = true; display_DiscretedFc = false; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF B_DFc = Button_DiscretedFc;
                if (B_DFc.Contains(e.CanvasLocation))
                {
                    if (display_DiscretedFc == true) { display_DiscretedFc = false; display_IntegratedFc = true; } else { display_DiscretedFc = true; display_IntegratedFc = false; }
                    ResponseUpdate();
                    return GH_ObjectResponse.Handled;
                }
            }

            return base.RespondToMouseUp(sender, e);
        }
        
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {

            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                GH_Capsule button_IntegratedFc = DrawButton(Button_IntegratedFc, "Integrated F*", display_IntegratedFc);
                GH_Capsule button_DiscretedFc = DrawButton(Button_DiscretedFc, "Discreted F*", display_DiscretedFc);

                button_IntegratedFc.Render(graphics, Selected, Owner.Locked, false); button_IntegratedFc.Dispose();
                button_DiscretedFc.Render(graphics, Selected, Owner.Locked, false); button_DiscretedFc.Dispose();
            }
        }

        public GH_Capsule DrawButton(System.Drawing.Rectangle Button_bound, string text, bool press)
        {
            GH_Capsule button;
            if (press) { button = GH_Capsule.CreateTextCapsule(Button_bound, Button_bound, GH_Palette.Black, text, 2, 0); }
            else { button = GH_Capsule.CreateTextCapsule(Button_bound, Button_bound, GH_Palette.Transparent, text, 2, 0); }
            return button;
        }

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
            (base.Owner as Planarization).Update(display_IntegratedFc, display_DiscretedFc);//
        }
    }
        public class Planarization : GH_Component
    {
        public double Sys_Tor = System_Configuration.Sys_Tor;
        public double scale = System_Configuration.Scale;

        public string Error = "";
        public List<List<List<string>>> nN_infor;
        public DataTree<string> nN;
        public DataTree<string> st2d_s;
        public List<string> st1_v;
        public List<string> st1_c;
        public List<string> st1_e;

        public List<string> ve_list;

        public bool consider_Ve = true;

        public bool calculation_success = true;

        private bool FormFinding = false;
        private string FcStatusState
        {
            get { if (FormFinding == false) { return "Switch Force Diagram to Discreted"; } else { return "Switch Force Diagram to Integrated"; } }
        }
        private string FcStatus
        {
            get { if (FormFinding == false) { return "[Integrated Force Diagram]"; } else { return "[Discreted Force Diagram]"; } }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("FormFinding", FormFinding);

            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            FormFinding = reader.GetBoolean("FormFinding");
            return base.Read(reader);
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, FcStatus);
            Menu_AppendItem(menu, FcStatusState, Change);
            Menu_AppendSeparator(menu);
            base.AppendAdditionalMenuItems(menu);
        }

        private void Change(Object sender, EventArgs e)
        {
            if (FormFinding == false) { FormFinding = true; } else { FormFinding = false; }
        }

        public void Update(bool IFc,bool DFc)
        {
            if (IFc == true && DFc == false) { FormFinding = false; }
            else if (IFc == false && DFc == true) { FormFinding = true; }
            base.DestroyIconCache();//Thie line helps to update the icon
            this.OnAttributesChanged();
            this.ExpireSolution(true); 

        }

        public Planarization()
          : base("4.Generate Force Diagram (VGS)", "Generate Force Diagram",
              "Planarize the underlying graph of the form diagram and generate the force diagram",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }
        public override void CreateAttributes()//Override the function that calls battery properties
        {
            base.m_attributes = new Attributes_Custom_Planarization(this);
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            List<string> Select_edge = new List<string>() { "" };
            Input.AddGenericParameter("F", "F", "Form diagram", GH_ParamAccess.item); Input[0].Optional = true;
            Input.AddBooleanParameter("GlobEq", "GlobEq", "Assemble a closed cycle of the external forces in the force diagram (True) ", GH_ParamAccess.item, true);
            Input.AddBooleanParameter("Reset", "Reset", "Recalculate the graph", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("F*", "F*", "Force diagram", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            bool run = false;
            Sys_Tor = System_Configuration.Sys_Tor;
            scale = System_Configuration.Scale;
            bool reset = false;

            List<string> select_edge = new List<string>();

            if (!data.GetData("GlobEq", ref consider_Ve)) { return; }
            if (!data.GetData("Reset", ref reset)) { return; }

            //string path_Python_planarize;
            MODEL F_herit = new MODEL();
            MODEL FD = new MODEL();
            if (data.GetData("F", ref F_herit)) { FD = F_herit.CopySelf(); }

            //Initial paras
            List<List<string>> Vnms=new List<List<string>>();
            List<List<string>> eNs=new List<List<string>>();
            List<string[]> MPlanarG=new List<string[]>();
            List<string[]> AddBackG=new List<string[]>();
            List<List<string[]>> embeded_circle=new List<List<string[]>>();

            if (reset)
            {
                FD.ExportData(out Vnms,out eNs);//Export the Vertex_Edge.text to VGS（as the initial setting of the structural edges and vertices）
                List<string>Vns=Planarity.IO_DetectVericesList(Vnms);
                Planarity.MaxPlanarGraph_E(Vns,Vnms.Select(p => new string[] { p[0], p[1] }).ToList(), out MPlanarG,out AddBackG);
                Planarity.PlanarityTest(Vns, MPlanarG, out embeded_circle);
                run = true;
            }

            if (run)
            {

                try
                {
                    Planarization_Methods.QUAD(consider_Ve,Vnms,eNs,MPlanarG,AddBackG,embeded_circle, out st2d_s, out st1_v, out st1_c, out st1_e, out nN_infor, out ve_list);
                }
                catch (Exception e)
                {
                    Error += string.Format("{0} Exception caught.", e);
                }

                if (nN_infor != null)
                {
                    DataTree<string> nN_in = new DataTree<string>();
                    for (int i = 0; i < nN_infor[0].Count; i++)
                    {
                        nN_in.AddRange(nN_infor[0][i], new GH_Path(i, 0));
                        nN_in.AddRange(nN_infor[1][i], new GH_Path(i, 1));
                    }
                    nN = nN_in;
                }
            }

            try
            {
                if (st2d_s != null && st1_v != null && st1_c != null && st1_e != null)
                {
                    FcDiagramAssembleData Tp = new FcDiagramAssembleData(st2d_s, st1_v, st1_c, st1_e, nN, ve_list);
                    Tp.error = Error;
                    FD.Diagram.planarGraph = Tp;
                    F_herit.Diagram.planarGraph = Tp;
                    FD.report = "";
                    FD.Diagram.AssemblingForceDiagram(FD.Diagram, new Point3d(0, 0, 0), FormFinding, ref FD.report);
                    FD.model_status = "[Force Diagram]";
                    FD.Diagram.FindForceDiagramNodeAddrs();//Find the point set address in the force diagram and assign it to the corresponding value in the class
                    data.SetData("F*", FD);
                }
            }
            catch (Exception e)
            {
                Error += string.Format("{0} Exception caught.", e);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                if (!FormFinding) { return Properties.Resources.VGS_74; }
                else { return Properties.Resources.VGS_81; }
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("08152c96-ec1e-4d4a-9dd4-d0cb5a19044c"); }//
        }
    }

}