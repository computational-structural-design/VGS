using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using VGS_Main;
using static System.Net.Mime.MediaTypeNames;

namespace GraphicStatic._3.Components
{
    public class TEST_ObjectChange : GH_Component
    {
        public class Attributes_Custom_TEST_ObjectChange : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
        {
            public Attributes_Custom_TEST_ObjectChange(GH_Component owner) : base(owner) { }//Rewrite the constructor, and use base() to retain all the methods of the constructor in the base class;
        }

        public TEST_ObjectChange()
          : base("TEST_ObjectChange", "TEST_ObjectChange",
              "221013",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_06)
        {
        }

        public override void CreateAttributes()//Override the function that calls battery properties
        {
            base.m_attributes = new Attributes_Custom_TEST_ObjectChange(this);
            this.ObjectChanged += new IGH_DocumentObject.ObjectChangedEventHandler(test);
        }

        public void test(IGH_DocumentObject o, GH_ObjectChangedEventArgs e)
        {
            detect_change++;
        }

        bool run = false;
        bool reset = false;
        int detect_change= 0;
        int Iteration=0;
        int MainLoop = 0;
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("P", "P", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "Reset", "Reset the transformation", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Run", "Run", "Run the transformation", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            data.GetData(1, ref reset);
            data.GetData(2, ref run);
            if (reset) { Iteration = 0; MainLoop = 0; }
            MainLoop++;
        }


        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();
            if (!run) { return; }
            GH_Document ghDocument = base.OnPingDocument();
            if (ghDocument == null) return;
            ghDocument.ScheduleSolution(50, new GH_Document.GH_ScheduleDelegate(this.ScheduleCallBack));
        }

        private void ScheduleCallBack(GH_Document doc)
        {
            Iteration++;
            Message = "Iteration: "+Iteration.ToString()+ "\nDetect_change"+ detect_change.ToString() + "\nMain_Loop" + MainLoop.ToString();
            this.ExpireSolution(false);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("39845E41-83AA-4C78-88E4-BD3CEEF76AD3"); }
        }
    }
}