using System;
using System.Timers;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using VGS_Main;
using KangarooSolver;
using KangarooSolver.Goals;
using static VGS_Main.Diagram_Transformation_Kangaroo;
using System.Diagnostics;
using Grasshopper.Kernel.Parameters;
using System.Threading.Tasks;

namespace GraphicStatic
{

    public class Transformation: GH_Component
    {
        VgsCommon.MiniWatch MiniWatch = new VgsCommon.MiniWatch();

        double vsum = double.MaxValue;
        int counter;
        bool reset, run;
        public bool shouldUpdateGoalSetting = true;


        public int Mini_Iteration = 10;
        public int animation_gap = 50;
        public double DR_threshold = 0.0000000001;
        public double distortionFm = 50;
        public double distortionFc = 50;

        List<Object> goals_external = new List<object>();
        List<IGoal> Goals_FixFc = new List<IGoal>();
        List<IGoal> Goals_Cpoly = new List<IGoal>();
        List<IGoal> goals_reciprocal = new List<IGoal>();
        TransfTempInfo tp;

        MODEL Fm_Diagram;
        MODEL Fc_Diagram;
        PhysicalSystem_K_test Dynamic_Relaxiation = new PhysicalSystem_K_test();//[DEBUG 221115]

        List<Object> output_objects;
        
        public Transformation()
          : base("1.Transformation(VGS)", "Transformation",
              "Transform form and force diagrams using the Kangaroo2 library",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_04)
        {
        }

        private void ScheduleCallBack(GH_Document doc)
        {
            Update();
            this.ExpireSolution(false);
        }
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();
            if (!run) { return; }
            GH_Document ghDocument = base.OnPingDocument();
            if (ghDocument == null) return;
            ghDocument.ScheduleSolution(animation_gap, new GH_Document.GH_ScheduleDelegate(this.ScheduleCallBack));
        }

        //build a custom input to detect the change in the input
        private class FormDiagram_Input : Param_GenericObject
        {

            public override void ExpireSolution(bool recompute)
            {
                base.ExpireSolution(recompute);
                if (Attributes?.GetTopLevel?.DocObject is Transformation owner)
                {
                    owner.shouldUpdateGoalSetting = true;
                }
            }
        }

        private class ForceDiagram_Input : Param_GenericObject
        {

            public override void ExpireSolution(bool recompute)
            {
                base.ExpireSolution(recompute);
                if (Attributes?.GetTopLevel?.DocObject is Transformation owner)
                {
                    owner.shouldUpdateGoalSetting = true;
                }
            }
        }

        private class Goals_Input : Param_GenericObject
        {

            public override void ExpireSolution(bool recompute)
            {
                base.ExpireSolution(recompute);
                if (Attributes?.GetTopLevel?.DocObject is Transformation owner)
                {
                    owner.shouldUpdateGoalSetting = true;
                }
            }
        }

        private class Constr_Input : Param_GenericObject
        {

            public override void ExpireSolution(bool recompute)
            {
                base.ExpireSolution(recompute);
                if (Attributes?.GetTopLevel?.DocObject is Transformation owner)
                {
                    owner.shouldUpdateGoalSetting = true;
                }
            }
        }

        private class SimConfig_Input : Param_GenericObject
        {

            public override void ExpireSolution(bool recompute)
            {
                base.ExpireSolution(recompute);
                if (Attributes?.GetTopLevel?.DocObject is Transformation owner)
                {
                    owner.shouldUpdateGoalSetting = true;
                }
            }
        }

        private class InterConfig_Input : Param_GenericObject
        {

            public override void ExpireSolution(bool recompute)
            {
                base.ExpireSolution(recompute);
                if (Attributes?.GetTopLevel?.DocObject is Transformation owner)
                {
                    owner.shouldUpdateGoalSetting = true;
                }
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            //customise the input param
            var para_0 = new FormDiagram_Input
            {
                Access = GH_ParamAccess.item,
                Name = "F",
                NickName = "F",
                Description = "Form diagram"
            };
            Input.AddParameter(para_0); Input[0].Optional = true;

            var para_1 = new ForceDiagram_Input
            {
                Access = GH_ParamAccess.item,
                Name = "F*",
                NickName = "F*",
                Description = "Force diagram"
            };
            Input.AddParameter(para_1); Input[1].Optional = true;

            var para_2 = new Goals_Input
            {
                Access = GH_ParamAccess.list,
                Name = "Goals",
                NickName = "Goals",
                Description = "Set the goals (Kangaroo2) for the transformation"
            };
            Input.AddParameter(para_2); Input[2].Optional = true;

            var para_3 = new Constr_Input
            {
                Access = GH_ParamAccess.list,
                Name = "Constr",
                NickName = "Constr",
                Description = "Set the constraints of the transformation"
            };
            Input.AddParameter(para_3); Input[3].Optional = true;

            var para_4 = new SimConfig_Input
            {
                Access = GH_ParamAccess.item,
                Name = "SimConfig",
                NickName = "SimConfig",
                Description = "Configure the settings for the simulation"
            };
            Input.AddParameter(para_4); Input[4].Optional = true;

            var para_5 = new InterConfig_Input
            {
                Access = GH_ParamAccess.item,
                Name = "InterConfig",
                NickName = "InterConfig",
                Description = "Configure the settings for the form-force diagrams interdependency"
            };
            Input.AddParameter(para_5); Input[5].Optional = true;
            //

            //Input.AddGenericParameter("F", "F", "Form diagram", GH_ParamAccess.item); Input[0].Optional = true;
            //Input.AddGenericParameter("F*", "F*", "Force diagram", GH_ParamAccess.item); Input[1].Optional = true;
            //Input.AddGenericParameter("Goals", "Goals", "Set the goals (Kangaroo2) for the transformation", GH_ParamAccess.list); Input[2].Optional = true;
            //Input.AddGenericParameter("Constr", "Constr", "Set the constraints of the transformation", GH_ParamAccess.list); Input[3].Optional = true;
            //Input.AddGenericParameter("SimConfig", "SimConfig", "Configure the settings for the simulation", GH_ParamAccess.item); Input[4].Optional = true;
            //Input.AddGenericParameter("InterConfig", "InterConfig", "Configure the settings for the form-force diagrams interdependency", GH_ParamAccess.item); Input[5].Optional = true;
            Input.AddBooleanParameter("Reset", "Reset", "Reset the transformation", GH_ParamAccess.item, false);
            Input.AddBooleanParameter("Run", "Run", "Run the transformation", GH_ParamAccess.item, false);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("F", "F", "The transformed form diagram", GH_ParamAccess.item);
            Output.AddGenericParameter("F*", "F*", "The transformed force diagram", GH_ParamAccess.item);
            Output.AddIntegerParameter("Iter", "iteration", "Number of iterations", GH_ParamAccess.item);
            Output.AddGenericParameter("Output", "Output", "Output geometry", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {


            #region 01_Inputs
            if (!data.GetData(6, ref reset)) { return; }//[Read]
            if (!data.GetData(7, ref run)) { return; }//[Read]

            PhysicalSystem_config S_config = new PhysicalSystem_config();
            if (data.GetData(4, ref S_config))//[Read]
            {
                Mini_Iteration = S_config.Step_MiniIter;
                animation_gap = S_config.GAP_animation;
                DR_threshold = S_config.DR_th;
                distortionFm = S_config.DistortionFm;
                distortionFc = S_config.DistortionFc;
            }

            Reciprocal_force_config R_config = new Reciprocal_force_config();
            data.GetData(5, ref R_config);//[Read]
            List<lock_config> Constr_config = new List<lock_config>();
            lock_config lock_default_input = new lock_config();
            data.GetDataList(3, Constr_config);//[Read]
            foreach (lock_config cf in Constr_config)
            { lock_default_input.ReadData(cf); }

            if (reset)
            {
                Dynamic_Relaxiation.Reset();
                output_objects = Dynamic_Relaxiation.pip(reset, goals_external, goals_reciprocal, Mini_Iteration);
                vsum = double.MaxValue;
                shouldUpdateGoalSetting = true;
            }

            MODEL Fm = new MODEL(); MODEL Fc = new MODEL();
            if (!data.GetData(0, ref Fm)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: None Form Diagram Input"); return; }//[Read]
            if (!data.GetData(1, ref Fc)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: None Force Diagram Input"); return; }//[Read]
            if (Fm.GUI != Fc.GUI) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ERROR: The Input Form and Force are not matched"); return; };
            Fm_Diagram = Fm.CopySelf();
            Fc_Diagram = Fc.CopySelf();

            //import external geometric constraints
            List<Object> Geometric_constraints = new List<Object>();
            data.GetDataList(2, Geometric_constraints);//[Read]
            #endregion Inputs
            MiniWatch.Record("Input");
            #region 02_IGoal Setups
            if (shouldUpdateGoalSetting)
            {
                List<object> Goals_Ext = new List<object>();
                this.tp = new TransfTempInfo();
                //The Goals to Fix the force diagram
                Goals_FixFc = new List<IGoal>();
                double strength_CloseNode = R_config.CoincidentN_force;//get the strength that close the force polygons;
                if (strength_CloseNode > 0) { Goals_FixFc = Fc.Diagram.FcConvergeGoals(strength_CloseNode, Fc.Diagram.Fc_Pts_Addr, Fc.Diagram.ln2_Edge); Goals_Ext.AddRange(Goals_FixFc); }

                //The Goals to close the force polygon only
                Goals_Cpoly = new List<IGoal>();
                double strength_ClosePolygon = R_config.CPolygon_force;//
                if (strength_ClosePolygon > 0) { Goals_Cpoly = Fc.Diagram.FcPolygonClose(strength_ClosePolygon, Fc.Diagram.ln2_Edge); Goals_Ext.AddRange(Goals_Cpoly); }

                //The Goals to Set the Nodes in Forcediagram[220331]
                List<List<Point3d>> FC_converge = Fc_Diagram.Diagram.FcConvergePts(Fc_Diagram.Diagram.Fc_Pts_Addr, Fc_Diagram.Diagram.ln2_Edge);
                List<IGoal> VnAchors = new List<IGoal>();
                for (int i = 0; i < lock_default_input.vN_lock.Count; i++)
                {
                    string vID = lock_default_input.vN_lock[i];
                    bool[] setting = lock_default_input.vN_lock_xyz[i];
                    double vNstrength = lock_default_input.vN_lock_s[i];
                    if (lock_default_input.vN_lock[i].Substring(0, 1) == "V")
                    {
                        int index = System.Convert.ToInt32(lock_default_input.vN_lock[i].Substring(1)); if (index < 0 || index > FC_converge.Count - 1) { continue; }
                        foreach (Point3d Vn in FC_converge[index]) { VnAchors.Add(new AnchorXYZ(Vn, setting[0], setting[1], setting[2], vNstrength)); }
                    }
                }
                Goals_Ext.AddRange(VnAchors);
                //[220331]
                goals_reciprocal = Transformation_model.Transformation_Reciprocal(ref tp, Fm_Diagram, Fc_Diagram, lock_default_input, R_config, S_config, System_Configuration.Sys_Tor);//Consuming

                //The external input from K2 in Grasshopper are all GH_ObjectWrapper type
                foreach (Grasshopper.Kernel.Types.GH_ObjectWrapper O in Geometric_constraints)
                { Goals_Ext.Add(O.Value); }
                goals_external = Goals_Ext;

                Dynamic_Relaxiation.Setup_Pindex(goals_external, goals_reciprocal,0.00001);//@[231122]

                shouldUpdateGoalSetting = false;
            }
            #endregion 02_IGoal Setups
            MiniWatch.Record("Setup");
            if (output_objects != null && tp != null)
            {
                Transformation_model.WirteBackToModel(ref tp, Fm_Diagram, Fc_Diagram, output_objects);
                Fm_Diagram.Diagram.UpdateNodes();
                Fc_Diagram.Diagram.UpdateNodes();
            }

            data.SetDataList(3, output_objects);
            Fm_Diagram.transformed = true;
            Fc_Diagram.transformed = true;
            data.SetData(0, Fm_Diagram);
            data.SetData(1, Fc_Diagram);

            MiniWatch.Record("DataTrans");
            if (!run) { Message = "VGS Pause"; return; }
            if (reset) { counter = 0; }
            if (vsum < DR_threshold && counter > 2) { run = false; vsum = double.MaxValue; Message = "VGS Converged"; }
            else if (counter > 0) 
            {
                Message = "VGS Running" +
                    "\n" + "PCount:" + Dynamic_Relaxiation.PS.ParticleCount().ToString()
                    + "\n" + MiniWatch.totalTime()
                    +"\n"+MiniWatch.GetTime();
            }

            data.SetData(2, counter);

            MiniWatch = new VgsCommon.MiniWatch();
        }

        private void Update()
        {
            if (reset)
            {
                output_objects = Dynamic_Relaxiation.pip_(reset, goals_external, goals_reciprocal, Mini_Iteration);//@[231122]
                MiniWatch.Record("Iter_pip_");
            }
            else
            {
                output_objects = Dynamic_Relaxiation.pip(reset, goals_external, goals_reciprocal, Mini_Iteration);//@[231122]
                MiniWatch.Record("Iter_pip");
            }
            
            vsum = Dynamic_Relaxiation.PS.GetvSum();
            counter++;//This runs iteratively
            

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_57;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("dec44250-10c2-4c90-b952-c9ef162d0f53"); }//
        }
    }
}