using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using MathNet.Numerics.LinearAlgebra;

namespace VGS_Main
{

    public class MODEL : Equilibrium
    {
        //public variables
        public DIAGRAM Diagram = new DIAGRAM();
        public string report = "";
        public string report_error = "";
        public int int0_NodeSelect = int.MaxValue;
        string st0_Unit = Rhino.RhinoDoc.ActiveDoc.GetUnitSystemName(true, true, true, true);
        public string model_status = "[not calculated]";
        public string GUI = "NONE";
        public bool transformed = false;

        //initial import API
        public List<Line> ln1_Edge = new List<Line>();
        public List<double> db1_Force = new List<double>();
        public List<Line> ln1_Load = new List<Line>();
        public List<Point3d> pt1_Support = new List<Point3d>();
        public List<string> st1_Support = new List<string>();

        public List<int> in1_EdgeSelfStress = new List<int>();
        public List<double> db1_EdgeSelfStress = new List<double>();

        //build the basic model
        List<Point3d> pt1_Node = new List<Point3d>();
        List<List<int>> in2_EdgeNode = new List<List<int>>();
        public List<List<int>> in2_EdgeNodePlan = new List<List<int>>();
        List<int> in1_LoadNode = new List<int>();
        List<int> in1_SupportNode = new List<int>();
        List<List<int>> in2_NodeNode = new List<List<int>>();
        List<List<string>> st2_NodeNode = new List<List<string>>();
        List<List<int>> in2_NodeEdge = new List<List<int>>();
        public List<List<string>> st2_NodeEdge = new List<List<string>>();
        List<Vector3d> vc1_EdgeVec = new List<Vector3d>();
        List<List<double>> vc2p_Load = new List<List<double>>();
        List<double> vc2p_Resultant = new List<double>();
        Vector3d vc0_R_Resultant = new Vector3d(0, 0, 0);
        Vector3d vc0_M_Resultant = new Vector3d(0, 0, 0);
        Vector3d vc0_M0_Resultant = new Vector3d(0, 0, 0);
        Vector3d vc0_M1_Resultant = new Vector3d(0, 0, 0);
        Point3d pt0_CentralAxis = new Point3d(0, 0, 0);

        List<double> db1_EdgeForce = new List<double>();
        List<Vector3d> vc1_Reac = new List<Vector3d>();

        public MODEL CopySelf()
        {
            MODEL newInstance = new MODEL();
            newInstance.Diagram = this.Diagram.CopySelf();
            newInstance.report = this.report;
            newInstance.report_error = this.report_error;
            newInstance.int0_NodeSelect = this.int0_NodeSelect;
            newInstance.st0_Unit = this.st0_Unit;
            newInstance.model_status = this.model_status;
            newInstance.GUI = this.GUI;
            newInstance.transformed = this.transformed;

            newInstance.ln1_Edge = ListCopy(this.ln1_Edge);
            newInstance.db1_Force = ListCopy(this.db1_Force);
            newInstance.ln1_Load = ListCopy(this.ln1_Load);
            newInstance.pt1_Support = ListCopy(this.pt1_Support);
            newInstance.st1_Support = ListCopy(this.st1_Support);

            newInstance.in1_EdgeSelfStress = ListCopy(this.in1_EdgeSelfStress);
            newInstance.db1_EdgeSelfStress = ListCopy(this.db1_EdgeSelfStress);

            newInstance.pt1_Node = ListCopy(this.pt1_Node);//
            newInstance.in2_EdgeNode = ListListCopy(this.in2_EdgeNode);//
            newInstance.in2_EdgeNodePlan = ListListCopy(this.in2_EdgeNodePlan);//
            newInstance.in1_LoadNode = ListCopy(this.in1_LoadNode);//
            newInstance.in1_SupportNode = ListCopy(this.in1_SupportNode);//
            newInstance.in2_NodeNode = ListListCopy(this.in2_NodeNode);//
            newInstance.st2_NodeNode = ListListCopy(this.st2_NodeNode);//
            newInstance.in2_NodeEdge = ListListCopy(this.in2_NodeEdge);//
            newInstance.st2_NodeEdge = ListListCopy(this.st2_NodeEdge);//
            newInstance.vc1_EdgeVec = ListCopy(this.vc1_EdgeVec);//
            newInstance.vc2p_Load = ListListCopy(this.vc2p_Load);//
            newInstance.vc2p_Resultant = ListCopy(this.vc2p_Resultant);//

            newInstance.vc0_R_Resultant = this.vc0_R_Resultant;//
            newInstance.vc0_M_Resultant = this.vc0_M_Resultant;//
            newInstance.vc0_M0_Resultant = this.vc0_M0_Resultant;//
            newInstance.vc0_M1_Resultant = this.vc0_M1_Resultant;//
            newInstance.pt0_CentralAxis = this.pt0_CentralAxis;//

            newInstance.db1_EdgeForce = ListCopy(this.db1_EdgeForce);
            newInstance.vc1_Reac = ListCopy(this.vc1_Reac);

            return newInstance;
        }
        public override string ToString()
        {
            string display = model_status + "\n" + report;
            return display;
        }
        public void Import_class(Edge_line edges, Load_line loads, Support_point supports)
        {
            ln1_Edge = edges.lines;
            db1_Force = edges.forces;
            ln1_Load = loads.lines;
            pt1_Support = supports.locate;
            st1_Support = supports.status;
        }
        public bool Check_values()
        {
            bool bl0_CheckInput = true;

            // Check Rhino Unit
            st0_Unit = Rhino.RhinoDoc.ActiveDoc.GetUnitSystemName(true, true, true, true);
            if (st0_Unit != "m" && st0_Unit != "米")
            {
                bl0_CheckInput = false;
                report_error += "UNITS\n";
                report_error += "The current units are set to " + st0_Unit + "\n";
                report_error += "Only Meters are supported\n";
                report_error += "\n";
            }
            else
            {
                report += "UNITS\n";
                report += "Length: " + st0_Unit + "\n";
                report += "Force: kN\n";
                report += "Stress: MPa\n";
                report += "\n";
            }


            if (ln1_Edge.Count < 3)
            {
                bl0_CheckInput = false;
                report_error += "More than 3 edges (ln1_Edge) have to be provided\n";
                report_error += "\n";
            }

            if (pt1_Support.Count < 3)
            {
                bl0_CheckInput = false;
                report_error += "More than 3 support nodes (pt1_Support) have to be provided\n";
                report_error += "\n";
            }

            if (pt1_Support.Count != st1_Support.Count)
            {
                bl0_CheckInput = false;
                report_error += "The number of support nodes (pt1_Support) should be the same as the strings of support force variables (st1_Support)\n";
                report_error += "\n";
            }
            return bl0_CheckInput;
        }
        public void BASIC_Model()
        {
            Construct_Model(ln1_Edge, System_Configuration.Sys_Tor, out pt1_Node, out in2_EdgeNode, out in2_EdgeNodePlan);
            CreatListof_Loads(ln1_Load, pt1_Node, System_Configuration.Sys_Tor, out in1_LoadNode);
            CreatListof_Supports(pt1_Node, pt1_Support, System_Configuration.Sys_Tor, out in1_SupportNode);
            CreatListof_Nodes_NeighbNodes(pt1_Node, in2_EdgeNode, out in2_NodeNode, out st2_NodeNode);
            CreatListof_Nodes_NeighbEdges(pt1_Node, in2_EdgeNode, out in2_NodeEdge, out st2_NodeEdge);
            CreateListof_DirectionVectors_perEdge(pt1_Node, in2_EdgeNode, out vc1_EdgeVec);
            GlobalEquilibrium(ln1_Load, System_Configuration.in0_T, out vc2p_Load, out vc2p_Resultant, out vc0_R_Resultant, out vc0_M_Resultant, out vc0_M0_Resultant, out vc0_M1_Resultant, out pt0_CentralAxis);
            model_status = "[Initial Model]";
        }
        public void API_MODE()
        {
            Diagram = new DIAGRAM();
            Diagram.int0_NodeSelect = int0_NodeSelect;
            API_Initialize_InnerForces_Reactions(pt1_Node, db1_Force, ln1_Edge, ln1_Load, in1_SupportNode, in2_NodeNode, in2_NodeEdge, in1_LoadNode, ref report, out db1_EdgeForce, out vc1_Reac);
            vc1_Reac = vc1_Reac.Select(p => new Vector3d(0, 0, 0)).ToList();//[20210531]To delete the reaction force while input model is API
            AssembleData(int0_NodeSelect, pt1_Node, ln1_Edge, db1_EdgeForce, ln1_Load, in1_LoadNode, pt1_Support, in1_SupportNode, st1_Support, vc1_Reac, vc0_R_Resultant, vc0_M0_Resultant, pt0_CentralAxis, st2_NodeEdge, st2_NodeNode,
            System_Configuration.Sys_Tor,
            ref Diagram,
            ref report);

            model_status = "[Form Diagram]";
        }
        public void EQUILIBRIUM_MODE(bool rank, List<int> in1_EdgeSelfStress, List<double> db1_EdgeSelfStress)
        {
            Diagram = new DIAGRAM();
            Diagram.int0_NodeSelect = int0_NodeSelect;
            //Create_IncidenceMatrix(ln1_Edge,pt1_Node,in2_EdgeNode, out MathNet.Numerics.LinearAlgebra.Matrix<double> db2M_I);
            //CreatAdjacency_Matrix(pt1_Node, in2_NodeNode, out MathNet.Numerics.LinearAlgebra.Matrix<double> db2M_Ad);
            Coordinatizing_Matrix(ln1_Edge, pt1_Node, vc1_EdgeVec, in2_EdgeNode, out List<List<double>> db2_CoordMatr);//
            Support_Matrix(pt1_Node, st1_Support, in1_SupportNode, out List<List<double>> db2_SupportMatr);//
            Activate_NumericNativeProviders_Multithreading();
            Create_KinematicTransformation_Matrix(db2_CoordMatr, db2_SupportMatr, out List<List<double>> db2_CoeffMatr, out double[,] db2a_C, out Matrix<double> db2M_C, System_Configuration.Sys_Tor);//
            Create_Equilibrium_MatrixA(db2M_C, db2_SupportMatr, ln1_Edge, pt1_Node, rank, ref report, out int in0_Null, out int in0_Mech, out Matrix<double> db2M_A);
            PLASTIC_Initialize_InnerForces_Reactions(System_Configuration.in0_T, in0_Null, in0_Mech, in1_EdgeSelfStress, db1_EdgeSelfStress, st1_Support, pt1_Node, ln1_Edge, ln1_Load, in1_LoadNode, db2_CoordMatr, db2_CoeffMatr, db2_SupportMatr, db2a_C, db2M_C, db2M_A,
            System_Configuration.Sys_Tor, ref report, out db1_EdgeForce, out vc1_Reac);

            AssembleData(int0_NodeSelect, pt1_Node, ln1_Edge, db1_EdgeForce, ln1_Load, in1_LoadNode, pt1_Support, in1_SupportNode, st1_Support, vc1_Reac, vc0_R_Resultant, vc0_M0_Resultant, pt0_CentralAxis, st2_NodeEdge, st2_NodeNode,
            System_Configuration.Sys_Tor,
            ref Diagram,
            ref report);
            model_status = "[PLASTIC Model]";//[1211]
        }

        public void ExportData(out List<List<string>> Vnms, out List<List<string>> eNs)
        {
            this.ExportDataForPlanarize(this.in2_EdgeNodePlan, this.Diagram, System_Configuration.Sys_Tor, out Vnms, out eNs);
        }
    }
    public class Model_set : VgsCommon
    {
        public string Type = "NONE";
        public List<double> SectionArea;
        public List<double> ElasticModule;
        public double ScaleDispl;
        public bool rank;
        public SelfStress_index Selfstress;

        public Model_set() { }
        public Model_set(SelfStress_index self, bool rank)
        {
            this.Type = "PLASTIC";
            this.Selfstress = new SelfStress_index();
            this.Selfstress.AddData(self.index, self.forces);
            this.rank = rank;
        }
        public Model_set(List<double> SectionArea, List<double> ElasticModule, double ScaleDispl, bool rank)
        {
            this.Type = "ELASTIC";
            this.SectionArea = ListCopy(SectionArea);
            this.ElasticModule = ListCopy(ElasticModule);
            this.ScaleDispl = ScaleDispl;
            this.rank = rank;
        }
    }
}
