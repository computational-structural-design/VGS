using System;
using System.Collections.Generic;
using VGS_Main;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GraphicStatic
{
    public class InternalForces : GH_Component
    {
        public InternalForces()
          : base("1.Internal Forces (VGS)", "Internal Forces",
              "Import the internal forces of the structure from another plug-in",
             System_Configuration.PATH.Version, System_Configuration.PATH.MenuName_03)
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddLineParameter("EdgeLn", "EdgeLn", "List of lines representing the edges of the structure", GH_ParamAccess.list);Input[0].Optional = true;
            Input.AddNumberParameter("EdgeMag", "EdgeMag", "List of internal force magnitudes in the edges of the structure (kN)", GH_ParamAccess.list); Input[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("IntForce", "IntForce", "List of the internal forces of the structure", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            Edge_line edges_set = new Edge_line();
            List<Line> line_set = new List<Line>();
            List<double> force_set = new List<double>();
            data.GetDataList(0, line_set);
            data.GetDataList(1, force_set);
            if (line_set.Count == force_set.Count) { edges_set.AddData(line_set, force_set); }
            else if (force_set.Count == 1)
            {
                double force = force_set[0];
                line_set = new List<Line>();
                foreach (Line ln in line_set) { force_set.Add(force); }
                edges_set.AddData(line_set, force_set);
            }
            else if (line_set.Count != force_set.Count) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "[INPUT ERROR]: The number of forces and edges are not equal "); return; }
            data.SetData(0, edges_set);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VGS_61;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("5830e93b-b11f-4002-9cc3-d9de17689de6"); }//
        }
    }
}