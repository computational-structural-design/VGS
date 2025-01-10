using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GraphicStatic
{
    public class SelfStress : GH_Component
    {
        public SelfStress()
          : base("4.Self-stresses (VGS)", "Self-stresses",
              "Set the self-stresses of the structure",
              System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_02)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddIntegerParameter("Index", "Index", "List of indices of the edges with self-stresses", GH_ParamAccess.list);Input[0].Optional = true;
            Input.AddNumberParameter("Mag", "Mag", "List of magnitudes of the self-stresses in the selected edges (kN)", GH_ParamAccess.list); Input[1].Optional = true;
        }

           protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("SelfStress", "SelfStress", "List of self-stresses of the structure", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<int> index = new List<int>();
            List<double> forces = new List<double>();
            if (!data.GetDataList(0, index)) { return; }
            if (!data.GetDataList(1, forces)) { return; }
            SelfStress_index SS = new SelfStress_index(index,forces);
            
            data.SetData(0,SS);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return GraphicStatic.Properties.Resources.VGS_52;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("5a189009-57e6-488b-8cfe-62c39d721596"); }
        }
    }
}