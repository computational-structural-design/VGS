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
    public class PhysicalSystem_Configuarion: GH_Component
    {
        public PhysicalSystem_Configuarion()
           : base("2.Simulation Config (VGS)", "Simulation Config",
               "Configure the setting for the simulation",
               System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_04)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddIntegerParameter("FrameGap", "FrameGap", "The count of the mini-Iteration in the K2 in background", GH_ParamAccess.item, 10); Input[0].Optional = true;
            Input.AddIntegerParameter("TimeGap", "TimeGap", "Set the time gap between each iteration of the simulation", GH_ParamAccess.item, 50); Input[1].Optional = true;
            Input.AddNumberParameter("Thresh", "Thresh", "Set the threshold for the convergence of the simulation", GH_ParamAccess.item, 0.0000000001); Input[2].Optional = true;
            Input.AddNumberParameter("FormDistortion", "FormDistortion", "Set the maximum percentage of elongation of the edges during the simulation", GH_ParamAccess.item, 50.0); Input[3].Optional = true;
            Input.AddNumberParameter("ForceDistortion", "ForceDistortion", "Set the maximum percentage of elongation of the edges during the simulation", GH_ParamAccess.item, 50.0); Input[4].Optional = true;
            Input.AddNumberParameter("DistortionStrength", "DistortionStrength", "", GH_ParamAccess.item, 0.1); Input[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("SimConfig", "SimConfig", "Settings for the simulation", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            PhysicalSystem_config config = new PhysicalSystem_config();

            int Step_MiniIter = 10;
            data.GetData(0, ref Step_MiniIter);
            int animation_gap = 50;
            data.GetData(1, ref animation_gap);
            double DR_threshold = 0.0000000001;
            data.GetData(2, ref DR_threshold);
            double distortionFm = 50.0;
            data.GetData(3, ref distortionFm);
            double distortionFc = 50.0;
            data.GetData(4, ref distortionFc);
            double distorionStrength = 0.1;
            data.GetData(5, ref distorionStrength);

            config.SetValues(Step_MiniIter, animation_gap, DR_threshold, distortionFm, distortionFc, distorionStrength);
            data.SetData(0, config);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_67;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("5b1e58ca-193d-4ea0-9f50-6cfaabbb0290"); }//
        }
    }
}