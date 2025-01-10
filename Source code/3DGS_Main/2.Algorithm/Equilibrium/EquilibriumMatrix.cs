using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using MathNet.Numerics.LinearAlgebra;
using System.Drawing;

namespace VGS_Main
{
    /// <summary>
    /// authors:D'Acunto, Pierluigi and Shen, Yuchi & Yuchi Shen
    /// </summary>
    public class Equilibrium : VgsCommon
    {
        public void Construct_Model(List<Line> ln1_Edge, double db0_T, out List<Point3d> pt1_Node, out List<List<int>> in2_EdgeNode, out List<List<int>> in2_EdgeNodePlan)
        {
            in2_EdgeNodePlan = new List<List<int>>();
            pt1_Node = new List<Point3d>(ln1_Edge.Count / 3);
            in2_EdgeNode = new List<List<int>>(ln1_Edge.Count);

            if (ln1_Edge.Count != 0) pt1_Node.Add(ln1_Edge[0].From);
            foreach (Line ln0_Edge in ln1_Edge)
            {
                List<int> in1_EdgeNode = new List<int>(2);
                Point3d pt0_Start = ln0_Edge.From;
                bool bl0_Check = false;
                for (int i = 0; i < pt1_Node.Count; i++)
                {
                    if (Math.Abs(pt0_Start.X - pt1_Node[i].X) < db0_T && Math.Abs(pt0_Start.Y - pt1_Node[i].Y) < db0_T && Math.Abs(pt0_Start.Z - pt1_Node[i].Z) < db0_T)
                    {
                        in1_EdgeNode.Add(i);
                        bl0_Check = true;
                        break;
                    }
                }
                if (bl0_Check == false)
                {
                    pt1_Node.Add(pt0_Start);
                    in1_EdgeNode.Add(pt1_Node.Count - 1);
                }
                Point3d pt0_End = ln0_Edge.To;
                bl0_Check = false;
                for (int i = 0; i < pt1_Node.Count; i++)
                {
                    if (Math.Abs(pt0_End.X - pt1_Node[i].X) < db0_T && Math.Abs(pt0_End.Y - pt1_Node[i].Y) < db0_T && Math.Abs(pt0_End.Z - pt1_Node[i].Z) < db0_T)
                    {
                        in1_EdgeNode.Add(i);
                        bl0_Check = true;
                        break;
                    }
                }
                if (bl0_Check == false)
                {
                    pt1_Node.Add(pt0_End);
                    in1_EdgeNode.Add(pt1_Node.Count - 1);
                }
                in2_EdgeNode.Add(in1_EdgeNode);
            }
            in2_EdgeNodePlan = in2_EdgeNode;
        }
        public void Create_IncidenceMatrix(List<Line> ln1_Edge, List<Point3d> pt1_Node, List<List<int>> in2_EdgeNode, out Matrix<double> db2M_I)
        {
            List<List<double>> db2_IncMatr = new List<List<double>>(ln1_Edge.Count);
            for (int i = 0; i < ln1_Edge.Count; i++)
            {
                List<double> db1_IncMatr = new List<double>(new double[pt1_Node.Count]);
                db1_IncMatr[in2_EdgeNode[i][0]] = 1;
                db1_IncMatr[in2_EdgeNode[i][1]] = 1;
                db2_IncMatr.Add(db1_IncMatr);
            }
            double[,] db2a_I = To2DArray(db2_IncMatr);
            db2M_I = Matrix<double>.Build.SparseOfArray(db2a_I);
        }
        public void CreatListof_Loads(List<Line> ln1_Load, List<Point3d> pt1_Node, double db0_T, out List<int> in1_LoadNode)
        {
            in1_LoadNode = new List<int>(ln1_Load.Count);
            foreach (Line ln0_Load in ln1_Load)
            {
                for (int i = 0; i < pt1_Node.Count; i++)
                {
                    if (Math.Abs(ln0_Load.From.X - pt1_Node[i].X) < db0_T && Math.Abs(ln0_Load.From.Y - pt1_Node[i].Y) < db0_T && Math.Abs(ln0_Load.From.Z - pt1_Node[i].Z) < db0_T)
                    {
                        in1_LoadNode.Add(i);
                        break;
                    }
                }
            }
        }
        public void CreatListof_Supports(List<Point3d> pt1_Node, List<Point3d> pt1_Support, double db0_T, out List<int> in1_SupportNode)
        {
            in1_SupportNode = new List<int>(pt1_Support.Count);
            foreach (Point3d pt0_Support in pt1_Support)
            {
                for (int i = 0; i < pt1_Node.Count; i++)
                {
                    if (Math.Abs(pt0_Support.X - pt1_Node[i].X) < db0_T && Math.Abs(pt0_Support.Y - pt1_Node[i].Y) < db0_T && Math.Abs(pt0_Support.Z - pt1_Node[i].Z) < db0_T)
                    {
                        in1_SupportNode.Add(i);
                        break;
                    }
                }
            }
        }
        public void CreatListof_Nodes_NeighbNodes(List<Point3d> pt1_Node, List<List<int>> in2_EdgeNode, out List<List<int>> in2_NodeNode, out List<List<string>> st2_NodeNode)
        {
            in2_NodeNode = new List<List<int>>(pt1_Node.Count);
            st2_NodeNode = new List<List<string>>();
            foreach (Point3d pt0_Node in pt1_Node)
            {
                List<int> in1_NodeNode = new List<int>();
                in2_NodeNode.Add(in1_NodeNode);
            }
            foreach (List<int> in1_EdgeNode in in2_EdgeNode)
            {
                in2_NodeNode[in1_EdgeNode[0]].Add(in1_EdgeNode[1]);
                in2_NodeNode[in1_EdgeNode[1]].Add(in1_EdgeNode[0]);
            }
            foreach (List<int> in1_NodeNode in in2_NodeNode)
            {
                List<string> st1_NodeNode = new List<string>();
                foreach (int in0_NodeNode in in1_NodeNode)
                {
                    st1_NodeNode.Add("v" + in0_NodeNode.ToString());
                }
                st2_NodeNode.Add(st1_NodeNode);
            }
        }
        public void CreatAdjacency_Matrix(List<Point3d> pt1_Node, List<List<int>> in2_NodeNode, out Matrix<double> db2M_Ad)
        {
            List<List<double>> db2_AdjMatr = new List<List<double>>(pt1_Node.Count);
            for (int i = 0; i < in2_NodeNode.Count; i++)
            {
                List<double> db1_AdjMatr = new List<double>(new double[pt1_Node.Count]);
                for (int j = 0; j < in2_NodeNode[i].Count; j++) db1_AdjMatr[in2_NodeNode[i][j]] = 1;
                db2_AdjMatr.Add(db1_AdjMatr);
            }
            double[,] db2a_Ad = To2DArray(db2_AdjMatr);
            db2M_Ad = Matrix<double>.Build.DenseOfArray(db2a_Ad);
        }
        public void CreatListof_Nodes_NeighbEdges(List<Point3d> pt1_Node, List<List<int>> in2_EdgeNode, out List<List<int>> in2_NodeEdge, out List<List<string>> st2_NodeEdge)
        {
            in2_NodeEdge = new List<List<int>>(pt1_Node.Count);
            st2_NodeEdge = new List<List<string>>();
            foreach (Point3d pt0_Node in pt1_Node)
            {
                List<int> in1_NodeEdge = new List<int>();
                in2_NodeEdge.Add(in1_NodeEdge);
            }
            for (int i = 0; i < in2_EdgeNode.Count; i++)
            {
                in2_NodeEdge[in2_EdgeNode[i][0]].Add(i);
                in2_NodeEdge[in2_EdgeNode[i][1]].Add(i);
            }
            foreach (List<int> in1_NodeEdge in in2_NodeEdge)
            {
                List<string> st1_NodeEdge = new List<string>();
                foreach (int in0_NodeEdge in in1_NodeEdge)
                {
                    st1_NodeEdge.Add("e" + in0_NodeEdge.ToString());
                }
                st2_NodeEdge.Add(st1_NodeEdge);
            }
        }
        public void CreateListof_DirectionVectors_perEdge(List<Point3d> pt1_Node, List<List<int>> in2_EdgeNode, out List<Vector3d> vc1_EdgeVec)
        {
            vc1_EdgeVec = new List<Vector3d>(in2_EdgeNode.Count);
            foreach (List<int> in1_EdgeNode in in2_EdgeNode)
            {
                Vector3d vc0_EdgeVec = new Vector3d(pt1_Node[in1_EdgeNode[1]] - pt1_Node[in1_EdgeNode[0]]);
                vc0_EdgeVec.Unitize();
                vc1_EdgeVec.Add(vc0_EdgeVec);
            }
        }
        public void GlobalEquilibrium(
            List<Line> ln1_Load, int in0_T,
            out List<List<double>> vc2p_Load,
            out List<double> vc2p_Resultant,
            out Vector3d vc0_R_Resultant,
            out Vector3d vc0_M_Resultant,
            out Vector3d vc0_M0_Resultant,
            out Vector3d vc0_M1_Resultant,
            out Point3d pt0_CentralAxis)
        {
            vc2p_Load = new List<List<double>>();
            foreach (Line ln0_Load in ln1_Load)
            {
                double q1 = ln0_Load.ToX;
                double q2 = ln0_Load.ToY;
                double q3 = ln0_Load.ToZ;
                double p1 = ln0_Load.FromX;
                double p2 = ln0_Load.FromY;
                double p3 = ln0_Load.FromZ;
                vc2p_Load.Add(new List<double> { q1 - p1, q2 - p2, q3 - p3, -(q2 * p3 - q3 * p2), -(q3 * p1 - q1 * p3), -(q1 * p2 - q2 * p1) });
            }

            vc2p_Resultant = new List<double>();
            if (vc2p_Load.Count != 0)
            {
                int in0_Length = vc2p_Load.First().Count;
                vc2p_Resultant = vc2p_Load.SelectMany(x => x)
                  .Select((v, i) => new { Value = v, Index = i % in0_Length })
                  .GroupBy(x => x.Index)
                  .Select(y => y.Sum(z => z.Value))
                  .ToList();
            }

            vc0_R_Resultant = new Vector3d();
            vc0_M_Resultant = new Vector3d();
            vc0_M0_Resultant = new Vector3d();
            vc0_M1_Resultant = new Vector3d();
            pt0_CentralAxis = new Point3d(0, 0, 0);

            if (vc2p_Resultant.Count != 0)
            {
                // Define force resultant (R)
                vc0_R_Resultant = new Vector3d(vc2p_Resultant[0], vc2p_Resultant[1], vc2p_Resultant[2]);

                // Define moment resultant (M)
                vc0_M_Resultant = new Vector3d(vc2p_Resultant[3], vc2p_Resultant[4], vc2p_Resultant[5]);

                if (Math.Abs(vc0_R_Resultant.Length) > Math.Pow(10, -in0_T))
                {
                    vc0_M0_Resultant = Vector3d.Multiply(vc0_M_Resultant, vc0_R_Resultant) * (1 / Math.Pow(vc0_R_Resultant.Length, 2)) * vc0_R_Resultant;
                    vc0_M1_Resultant = vc0_M_Resultant - vc0_M0_Resultant;
                }

                // Locate central axis
                if (Math.Abs(vc0_R_Resultant.Length) > Math.Pow(10, -in0_T))
                {
                    Vector3d vc0_RM_Cross = Vector3d.CrossProduct(vc0_R_Resultant, vc0_M_Resultant);
                    pt0_CentralAxis = new Point3d((1 / Math.Pow(vc0_R_Resultant.Length, 2)) * vc0_RM_Cross);
                }
            }
        }
        public void Coordinatizing_Matrix(List<Line> ln1_Edge, List<Point3d> pt1_Node, List<Vector3d> vc1_EdgeVec, List<List<int>> in2_EdgeNode, out List<List<double>> db2_CoordMatr)
        {
            db2_CoordMatr = new List<List<double>>(ln1_Edge.Count);
            for (int i = 0; i < ln1_Edge.Count; i++)
            {
                List<double> db1_CoordMatr = new List<double>(new double[3 * pt1_Node.Count]);
                Vector3d vc0_EdgeVec = vc1_EdgeVec[i];
                db1_CoordMatr[3 * in2_EdgeNode[i][0]] = vc0_EdgeVec.X;
                db1_CoordMatr[3 * in2_EdgeNode[i][0] + 1] = vc0_EdgeVec.Y;
                db1_CoordMatr[3 * in2_EdgeNode[i][0] + 2] = vc0_EdgeVec.Z;
                db1_CoordMatr[3 * in2_EdgeNode[i][1]] = -vc0_EdgeVec.X;
                db1_CoordMatr[3 * in2_EdgeNode[i][1] + 1] = -vc0_EdgeVec.Y;
                db1_CoordMatr[3 * in2_EdgeNode[i][1] + 2] = -vc0_EdgeVec.Z;
                db2_CoordMatr.Add(db1_CoordMatr);
            }
        }
        public void Support_Matrix(List<Point3d> pt1_Node, List<string> st1_Support, List<int> in1_SupportNode, out List<List<double>> db2_SupportMatr)
        {
            db2_SupportMatr = new List<List<double>>(st1_Support.Count);
            for (int i = 0; i < st1_Support.Count; i++)
            {
                List<double> db1_SupportMatr = new List<double>();
                List<double> db1_SupportMatrX = new List<double>(new double[3 * pt1_Node.Count]);
                List<double> db1_SupportMatrY = new List<double>(new double[3 * pt1_Node.Count]);
                List<double> db1_SupportMatrZ = new List<double>(new double[3 * pt1_Node.Count]);
                switch (st1_Support[i])
                {
                    case "XYZ":
                        db1_SupportMatrX[3 * in1_SupportNode[i]] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrX);
                        db1_SupportMatrY[3 * in1_SupportNode[i] + 1] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrY);
                        db1_SupportMatrZ[3 * in1_SupportNode[i] + 2] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrZ);
                        break;

                    case "XZ":
                        db1_SupportMatrX[3 * in1_SupportNode[i]] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrX);
                        db1_SupportMatrZ[3 * in1_SupportNode[i] + 2] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrZ);
                        break;


                    case "XY":
                        db1_SupportMatrX[3 * in1_SupportNode[i]] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrX);
                        db1_SupportMatrY[3 * in1_SupportNode[i] + 1] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrY);
                        break;

                    case "YZ":
                        db1_SupportMatrY[3 * in1_SupportNode[i] + 1] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrY);
                        db1_SupportMatrZ[3 * in1_SupportNode[i] + 2] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrZ);
                        break;

                    case "X":
                        db1_SupportMatrX[3 * in1_SupportNode[i]] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrX);
                        break;

                    case "Y":
                        db1_SupportMatrY[3 * in1_SupportNode[i] + 1] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrY);
                        break;

                    case "Z":
                        db1_SupportMatrZ[3 * in1_SupportNode[i] + 2] = 1.0;
                        db2_SupportMatr.Add(db1_SupportMatrZ);
                        break;
                }
            }
        }
        public void Activate_NumericNativeProviders_Multithreading()
        {
            MathNet.Numerics.Control.UseMultiThreading();
            MathNet.Numerics.Control.MaxDegreeOfParallelism = Environment.ProcessorCount;
        }
        public void Create_KinematicTransformation_Matrix(List<List<double>> db2_CoordMatr, List<List<double>> db2_SupportMatr, out List<List<double>> db2_CoeffMatr, out double[,] db2a_C, out Matrix<double> db2M_C, double db0_T)
        {
            db2_CoeffMatr = new List<List<double>>();
            for (int j = 0; j < db2_CoordMatr.Count; j++)
            {
                db2_CoeffMatr.Add(db2_CoordMatr[j]);
            }
            db2_CoeffMatr.AddRange(db2_SupportMatr);
            db2a_C = To2DArray(db2_CoeffMatr);
            db2M_C = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.SparseOfArray(db2a_C);

            db2M_C.CoerceZero(db0_T);
        }
        public void Create_Equilibrium_MatrixA(
            Matrix<double> db2M_C,
            List<List<double>> db2_SupportMatr,
            List<Line> ln1_Edge, List<Point3d> pt1_Node,
            bool rank,
            ref string report,
            out int in0_Null,
            out int in0_Mech,
            out Matrix<double> db2M_A
            )
        {
            db2M_A = db2M_C.Transpose();

            int in0_Row = db2M_A.RowCount;
            int in0_Col = db2M_A.ColumnCount;
            report += "EQUILIBRIUM MATRIX " + in0_Row.ToString() + "x" + in0_Col.ToString() + "\n";
            int in0_Rank;
            if (rank) { in0_Rank = db2M_A.Rank(); }
            else { in0_Rank = in0_Col; }

            in0_Null = (in0_Col - in0_Rank);
            report += "S (Self Stresses) = " + in0_Null.ToString() + "\n";
            in0_Mech = in0_Null - ln1_Edge.Count + 3 * pt1_Node.Count - db2_SupportMatr.Count; //String.Join("", st1_Support).Length;
            report += "M (Internal Mechanisms) = " + in0_Mech.ToString() + "\n";

            if (in0_Null > 0 && in0_Mech == 0)
            {
                report += "The structure is statically indeterminate\n";
                report += "\n";
            }
            else if (in0_Null > 0 && in0_Mech > 0)
            {
                report += "The structure is statically indeterminate\n";
                report += "The structure is kinematically unstable: " + in0_Mech.ToString()
                  + " bracing edges or support force variables have to be introduced\n";
                report += "\n";
            }
            else if (in0_Null == 0 && in0_Mech > 0)
            {
                report += "The structure is kinematically unstable: " + in0_Mech.ToString()
                  + " bracing edges or support force variables have to be introduced\n";
                report += "\n";
            }
            else if (in0_Null == 0 && in0_Mech == 0)
            {
                report += "The structure is statically determinate\n";
                report += "\n";
            }
        }
        public void API_Initialize_InnerForces_Reactions(
            List<Point3d> pt1_Node,
            List<double> db1_Force,
            List<Line> ln1_Edge,
            List<Line> ln1_Load,
            List<int> in1_SupportNode,
            List<List<int>> in2_NodeNode,
            List<List<int>> in2_NodeEdge,
            List<int> in1_LoadNode,
            ref string report,
            out List<double> db1_EdgeForce,
            out List<Vector3d> vc1_Reac
            )
        {
            db1_EdgeForce = new List<double>();
            vc1_Reac = new List<Vector3d>();

            if (db1_Force.Count == ln1_Edge.Count)
            {

                report += "EXTERNAL SOLVER\n";
                report += db1_Force.Count.ToString() + " inner force values have been imported\n";
                report += "\n";

                // Overwrite List of Inner Forces
                db1_EdgeForce = db1_Force;

                // Create List of Reactions
                vc1_Reac = new List<Vector3d>();
                for (int i = 0; i < in1_SupportNode.Count; i++)
                {
                    int k = in1_SupportNode[i];
                    Vector3d vc0_Reac = new Vector3d(0, 0, 0);
                    for (int j = 0; j < in2_NodeNode[k].Count; j++)
                    {
                        Vector3d vc0_Force = pt1_Node[in2_NodeNode[k][j]] - pt1_Node[k];
                        vc0_Force.Unitize();
                        vc0_Force *= db1_EdgeForce[in2_NodeEdge[k][j]];
                        vc0_Reac -= vc0_Force;
                    }
                    //Here we have to consider all the Qn in the loadnode
                    //if (in1_LoadNode.Contains(k)) vc0_Reac -= (ln1_Load[in1_LoadNode.IndexOf(k)].To - ln1_Load[in1_LoadNode.IndexOf(k)].From);
                    for (int q = 0; q < in1_LoadNode.Count; q++)
                    {
                        if (in1_LoadNode[q] == k) { vc0_Reac -= ln1_Load[q].To - ln1_Load[q].From; }
                    }

                    vc1_Reac.Add(vc0_Reac);
                }

            }
        }

        public void PLASTIC_Initialize_InnerForces_Reactions(
            int in0_T,
            int in0_Null,
            int in0_Mech,
            List<int> in1_EdgeSelfStress,
            List<double> db1_EdgeSelfStress,
            List<string> st1_Support,
            List<Point3d> pt1_Node,
            List<Line> ln1_Edge,
            List<Line> ln1_Load,
            List<int> in1_LoadNode,
            List<List<double>> db2_CoordMatr,
            List<List<double>> db2_CoeffMatr,
            List<List<double>> db2_SupportMatr,
            double[,] db2a_C,
            Matrix<double> db2M_C,
            Matrix<double> db2M_A,
            double db0_T,
            ref string report,
            out List<double> db1_EdgeForce,
            out List<Vector3d> vc1_Reac
            )
        {
            db1_EdgeForce = new List<double>();
            vc1_Reac = new List<Vector3d>();
            if (in0_Mech == 0)
            {

                report += "PLASTIC SOLUTION\n";
                report += in0_Null.ToString() + " self-stresses have to be assigned as parameters\n";
                report += "\n";


                // Check List of self-stressed edges

                if (in0_Null > 0)
                {
                    if (in1_EdgeSelfStress.Count != db1_EdgeSelfStress.Count)
                    {

                        if (db1_EdgeSelfStress.Count == 0)
                        {
                            db1_EdgeSelfStress = new List<double>();
                            for (int i = 0; i < in1_EdgeSelfStress.Count; i++) db1_EdgeSelfStress.Add(0.0);
                            report += "A default value of 0.0 kN has been assigned to all the self-stresses\n";
                        }
                        else
                        {
                            for (int i = 1; i < in1_EdgeSelfStress.Count; i++) db1_EdgeSelfStress.Add(db1_EdgeSelfStress[0]);
                            report += "The value of " + db1_EdgeSelfStress[0].ToString() + " kN has been assigned to all the self-stresses\n";
                        }
                    }

                    List<int> in1_EdgeSelfStressClean = new List<int>();
                    List<double> db1_EdgeSelfStressClean = new List<double>();
                    for (int i = 0; i < in1_EdgeSelfStress.Count; i++)
                    {
                        if (in1_EdgeSelfStress[i] < ln1_Edge.Count && i < in0_Null)
                        {
                            in1_EdgeSelfStressClean.Add(in1_EdgeSelfStress[i]);
                            db1_EdgeSelfStressClean.Add(db1_EdgeSelfStress[i]);
                        }
                    }

                    in1_EdgeSelfStress = new List<int>(in1_EdgeSelfStressClean);
                    db1_EdgeSelfStress = new List<double>(db1_EdgeSelfStressClean);

                }
                else
                {
                    in1_EdgeSelfStress = new List<int>();
                    db1_EdgeSelfStress = new List<double>();
                }


                // Create List of Edges with unknown inner Force
                List<int> in1_Edge = new List<int>();
                for (int i = 0; i < ln1_Edge.Count; i++) in1_Edge.Add(i);
                List<int> in1_EdgeStress = in1_Edge.Except(in1_EdgeSelfStress).ToList();

                // Create new Kinematic Transformation Matrix
                db2_CoeffMatr = new List<List<double>>();
                for (int j = 0; j < db2_CoordMatr.Count; j++)
                {
                    if (!(in1_EdgeSelfStress.Contains(j))) db2_CoeffMatr.Add(db2_CoordMatr[j]);
                }
                db2_CoeffMatr.AddRange(db2_SupportMatr);
                db2a_C = To2DArray(db2_CoeffMatr);
                db2M_C = Matrix<double>.Build.SparseOfArray(db2a_C);

                db2M_C.CoerceZero(db0_T);

                // Create new Equilibrium Matrix A (Transpose of Kinematic Transformation Matrix)
                db2M_A = db2M_C.Transpose();

                // Prepare Dependent Variables Array b (Vector of Applied Loads and Self-Stresses)
                List<double> db1_DepVar = new List<double>(3 * pt1_Node.Count);
                for (int i = 0; i < 3 * pt1_Node.Count; i++) db1_DepVar.Add(0);

                for (int i = 0; i < in1_EdgeSelfStress.Count; i++)
                {
                    for (int j = 0; j < db1_DepVar.Count; j++) db1_DepVar[j] += -db2_CoordMatr[in1_EdgeSelfStress[i]][j] * db1_EdgeSelfStress[i];
                }

                for (int i = 0; i < ln1_Load.Count; i++)
                {
                    Vector3d vc0_Load = new Vector3d(ln1_Load[i].To - ln1_Load[i].From);
                    db1_DepVar[3 * in1_LoadNode[i]] += -vc0_Load.X;
                    db1_DepVar[3 * in1_LoadNode[i] + 1] += -vc0_Load.Y;
                    db1_DepVar[3 * in1_LoadNode[i] + 2] += -vc0_Load.Z;
                }

                // Create Vector b (Vector of Applied Loads and Self-Stresses)
                double[] db1a_b = db1_DepVar.ToArray();
                var db1V_b = Vector<double>.Build.SparseOfArray(db1a_b);

                // Check Matrix and Array Dimensions
                int in0_EqMatrRow = db2M_A.RowCount;
                int in0_EqMatrCol = 0;
                if (in0_EqMatrRow != 0) in0_EqMatrCol = db2M_A.ColumnCount;
                int in0_DepVarRow = db1_DepVar.Count;

                //in0_Rank = db2M_A.Rank();

                if (in0_EqMatrRow != 0 && in0_EqMatrRow == in0_EqMatrCol && in0_EqMatrRow == in0_DepVarRow) //&& in0_EqMatrRow == in0_Rank)
                {

                    // Solve system of linear equations to find Inner Force Values
                    var db1a_x = db2M_A.Solve(db1V_b);
                    List<double> db1_x = db1a_x.ToList();

                    int in0_CountF = 0;
                    db1_EdgeForce = new List<double>();
                    for (int i = 0; i < ln1_Edge.Count; i++)
                    {
                        if (in1_EdgeSelfStress.Contains(i))
                        {
                            db1_EdgeForce.Add(db1_EdgeSelfStress[in1_EdgeSelfStress.IndexOf(i)]);
                        }
                        else
                        {
                            db1_EdgeForce.Add(Math.Round(db1_x[in0_CountF], in0_T));
                            in0_CountF += 1;
                        }
                    }

                    // Create List of Reactions
                    List<double> db1_Reac = new List<double>();
                    for (int i = in1_EdgeStress.Count; i < db1_x.Count; i++)
                    {
                        db1_Reac.Add(db1_x[i]);
                    }
                    int in0_CountS = 0;
                    foreach (string st0_Support in st1_Support)
                    {
                        switch (st0_Support)
                        {
                            case "XYZ":
                                vc1_Reac.Add(new Vector3d(db1_Reac[in0_CountS], db1_Reac[in0_CountS + 1], db1_Reac[in0_CountS + 2]));
                                in0_CountS += 3;
                                break;
                            case "XZ":
                                vc1_Reac.Add(new Vector3d(db1_Reac[in0_CountS], 0, db1_Reac[in0_CountS + 1]));
                                in0_CountS += 2;
                                break;
                            case "XY":
                                vc1_Reac.Add(new Vector3d(db1_Reac[in0_CountS], db1_Reac[in0_CountS + 1], 0));
                                in0_CountS += 2;
                                break;
                            case "YZ":
                                vc1_Reac.Add(new Vector3d(0, db1_Reac[in0_CountS], db1_Reac[in0_CountS + 1]));
                                in0_CountS += 2;
                                break;
                            case "X":
                                vc1_Reac.Add(new Vector3d(db1_Reac[in0_CountS], 0, 0));
                                in0_CountS += 1;
                                break;
                            case "Y":
                                vc1_Reac.Add(new Vector3d(0, db1_Reac[in0_CountS], 0));
                                in0_CountS += 1;
                                break;
                            case "Z":
                                vc1_Reac.Add(new Vector3d(0, 0, db1_Reac[in0_CountS]));
                                in0_CountS += 1;
                                break;
                        }
                    }

                }
            }
        }

        public void ELASTIC_Initialize_InnerForces_Reactions(
            int in0_T,
            int in0_Mech,
            List<double> db1_SectionArea,
            List<double> db1_ElasticModule,
            double db0_ScaleDispl,
            string st0_Unit,
            List<Point3d> pt1_Support,
            List<int> in1_SupportNode,
            List<List<int>> in2_EdgeNode,
            List<List<int>> in2_NodeNode,
            List<List<int>> in2_NodeEdge,
            List<Point3d> pt1_Node,
            List<Line> ln1_Edge,
            List<Line> ln1_Load,
            List<int> in1_LoadNode,
            List<List<double>> db2_SupportMatr,
            Matrix<double> db2M_C,
            Matrix<double> db2M_A,
            double db0_T,
            ref string report,
            out List<double> db1_EdgeForce,
            out List<Vector3d> vc1_Reac
            )
        {
            db1_EdgeForce = new List<double>();
            vc1_Reac = new List<Vector3d>();
            if (in0_Mech == 0)
            {
                // Create Global Stiffness Matrix K

                report += "ELASTIC SOLUTION\n";
                report += ln1_Edge.Count.ToString() + " cross-section areas and elastic modules have to be assigned\n";

                // Check List of cross-section and material properties
                if (ln1_Edge.Count != db1_SectionArea.Count)
                {
                    if (db1_SectionArea.Count != 1)
                    {
                        db1_SectionArea = new List<double>();
                        for (int i = 0; i < ln1_Edge.Count; i++) db1_SectionArea.Add(0.01);
                        report += "A default cross-section area of 1.0 " + st0_Unit.ToString() + "2 has been assigned to all the edges\n";
                    }
                    else
                    {
                        for (int i = 1; i < ln1_Edge.Count; i++) db1_SectionArea.Add(db1_SectionArea[0]);
                        report += "A cross-section area of " + db1_SectionArea[0].ToString() + " " + st0_Unit + "2 has been assigned to all the edges\n";
                    }
                }

                if (ln1_Edge.Count != db1_ElasticModule.Count)
                {
                    if (db1_ElasticModule.Count != 1)
                    {
                        db1_ElasticModule = new List<double>();
                        for (int i = 0; i < ln1_Edge.Count; i++) db1_ElasticModule.Add(210 * 1E6);
                        report += "A default elastic module of 210000.0MPa has been assigned to all the edges\n";
                    }
                    else
                    {
                        report += "An elastic module of " + db1_ElasticModule[0].ToString() + "MPa has been assigned to all the edges\n";
                        db1_ElasticModule[0] *= 1E3;
                        for (int i = 1; i < ln1_Edge.Count; i++) db1_ElasticModule.Add(db1_ElasticModule[0]);

                    }
                }
                report += "\n";

                List<double> db1_k = new List<double>();
                for (int i = 0; i < ln1_Edge.Count; i++) db1_k.Add(db1_SectionArea[i] * db1_ElasticModule[i] / ln1_Edge[i].Length);
                for (int i = 0; i < db2_SupportMatr.Count; i++) db1_k.Add(1E50);
                double[] db1a_k = db1_k.ToArray();
                var db2M_k = Matrix<double>.Build.SparseOfDiagonalArray(db1a_k);
                var db2M_K = db2M_A * db2M_k * db2M_C;

                // Prepare Dependent Variables Array b (Vector of Applied Loads)
                List<double> db1_DepVar = new List<double>(3 * pt1_Node.Count);
                for (int i = 0; i < 3 * pt1_Node.Count; i++) db1_DepVar.Add(0);

                for (int i = 0; i < ln1_Load.Count; i++)
                {
                    Vector3d vc0_Load = new Vector3d(ln1_Load[i].To - ln1_Load[i].From);
                    db1_DepVar[3 * in1_LoadNode[i]] += -vc0_Load.X;
                    db1_DepVar[3 * in1_LoadNode[i] + 1] += -vc0_Load.Y;
                    db1_DepVar[3 * in1_LoadNode[i] + 2] += -vc0_Load.Z;
                }

                // Create Vector b (Vector of Applied Loads and Self-Stresses)
                double[] db1a_b = db1_DepVar.ToArray();
                var db1V_b = Vector<double>.Build.SparseOfArray(db1a_b);


                // Solve system of linear equations to find Degrees of Freedom
                var db1V_V = db2M_K.Solve(db1V_b);
                db1V_V.CoerceZero(db0_T);
                List<double> db1_V = db1V_V.ToList();
                List<Vector3d> vc1_V = new List<Vector3d>();
                for (int i = 0; i < db1_V.Count; i += 3)
                {
                    Vector3d vc0_V = new Vector3d(-db1_V[i], -db1_V[i + 1], -db1_V[i + 2]);
                    vc0_V *= db0_ScaleDispl;
                    vc1_V.Add(vc0_V);
                }
                //Print(string.Join<double>("\n", db1_V));

                // Find Inner Force Values
                var db1V_v = db2M_C * db1V_V;
                var db1V_xx = db2M_k * db1V_v;
                db1V_xx.CoerceZero(db0_T);
                List<double> db1_xx = db1V_xx.ToList();
                //Print(string.Join<double>("\n", db1_xx));

                int in0_CountF = 0;
                db1_EdgeForce = new List<double>();
                for (int i = 0; i < ln1_Edge.Count; i++)
                {
                    db1_EdgeForce.Add(Math.Round(db1_xx[in0_CountF], in0_T));
                    in0_CountF += 1;
                }

                // Update Nodes
                List<Point3d> pt1_NodeCalc = new List<Point3d>();
                for (int i = 0; i < pt1_Node.Count; i++) pt1_NodeCalc.Add(pt1_Node[i] + vc1_V[i] / db0_ScaleDispl);
                for (int i = 0; i < pt1_Node.Count; i++) pt1_Node[i] += vc1_V[i];

                // Update Supports
                for (int i = 0; i < pt1_Support.Count; i++) pt1_Support[i] += vc1_V[in1_SupportNode[i]];

                // Update Edges
                for (int i = 0; i < ln1_Edge.Count; i++) ln1_Edge[i] = new Line(pt1_Node[in2_EdgeNode[i][0]], pt1_Node[in2_EdgeNode[i][1]]);

                // Update Loads
                for (int i = 0; i < ln1_Load.Count; i++) ln1_Load[i] = new Line(ln1_Load[i].From + vc1_V[in1_LoadNode[i]], ln1_Load[i].To + vc1_V[in1_LoadNode[i]]);

                // Create List of Reactions
                vc1_Reac = new List<Vector3d>();
                for (int i = 0; i < in1_SupportNode.Count; i++)
                {
                    int k = in1_SupportNode[i];
                    Vector3d vc0_Reac = new Vector3d(0, 0, 0);
                    for (int j = 0; j < in2_NodeNode[k].Count; j++)
                    {
                        Vector3d vc0_Force = pt1_NodeCalc[in2_NodeNode[k][j]] - pt1_NodeCalc[k];
                        vc0_Force.Unitize();
                        vc0_Force *= db1_EdgeForce[in2_NodeEdge[k][j]];
                        vc0_Reac -= vc0_Force;
                    }
                    if (in1_LoadNode.Contains(k)) vc0_Reac -= (ln1_Load[in1_LoadNode.IndexOf(k)].To - ln1_Load[in1_LoadNode.IndexOf(k)].From);
                    vc1_Reac.Add(vc0_Reac);
                }

            }
        }

        #region  output
        public void AssembleData(
            int int0_NodeSelect,
            List<Point3d> pt1_Node,
            List<Line> ln1_Edge,
            List<double> db1_EdgeForce,
            List<Line> ln1_Load,
            List<int> in1_LoadNode,
            List<Point3d> pt1_Support,
            List<int> in1_SupportNode,
            List<string> st1_Support,
            List<Vector3d> vc1_Reac,
            Vector3d vc0_R_Resultant,
            Vector3d vc0_M0_Resultant,
            Point3d pt0_CentralAxis,
            List<List<string>> st2_NodeEdge,
            List<List<string>> st2_NodeNode,
            double db0_T,
            ref DIAGRAM Form_Diagram,
            ref string report
            )
        {
            // Nodes
            List<string> st1_Node = new List<string>(pt1_Node.Count);
            for (int i = 0; i < pt1_Node.Count; i++) st1_Node.Add("v" + i.ToString());
            List<Color> cl1_Node = new List<Color>(pt1_Node.Count);
            List<string> st1_NodeSelectEdge = new List<string>();

            if (int0_NodeSelect < pt1_Node.Count)
            {
                report += "VISUALIZATION\n";
                report += "The visualized node is v" + int0_NodeSelect.ToString();
            }
            else
            {
                report += "VISUALIZATION\n";
                report += "The entire structure is visualized\n";
            }

            for (int i = 0; i < pt1_Node.Count; i++)
            {
                if (i == int0_NodeSelect)
                {
                    st1_NodeSelectEdge = st2_NodeEdge[i];
                    cl1_Node.Add(System.Drawing.Color.Black);
                }

                else cl1_Node.Add(System.Drawing.Color.Gray);
            }
            //import diagram_Nodes
            for (int i = 0; i < pt1_Node.Count; i++)
            { Form_Diagram.Nodes.Add(new NODE(st1_Node[i], st2_NodeNode[i], st2_NodeEdge[i], cl1_Node[i], pt1_Node[i])); }

            // Edges
            List<Point3d> pt1_Edge = new List<Point3d>(ln1_Edge.Count);
            List<string> st1_Edge = new List<string>(ln1_Edge.Count);
            for (int i = 0; i < ln1_Edge.Count; i++) st1_Edge.Add("e" + i.ToString());
            List<Color> cl1_Edge = new List<Color>(ln1_Edge.Count);
            for (int i = 0; i < ln1_Edge.Count; i++)
            {
                if (db1_EdgeForce.Count == ln1_Edge.Count)
                {
                    if (st1_NodeSelectEdge.Count == 0 || st1_NodeSelectEdge.Contains(st1_Edge[i]))
                    {
                        if (db1_EdgeForce[i] > db0_T) cl1_Edge.Add(System.Drawing.Color.FromArgb(228, 7, 20));
                        else if (db1_EdgeForce[i] < -db0_T) cl1_Edge.Add(System.Drawing.Color.FromArgb(5, 120, 191));
                        else cl1_Edge.Add(System.Drawing.Color.White);
                    }
                    else cl1_Edge.Add(System.Drawing.Color.DarkGray);
                }
                else cl1_Edge.Add(System.Drawing.Color.DarkGray);
            }

            //import diagram_Edges
            for (int i = 0; i < ln1_Edge.Count; i++)
            {
                double force;
                if (db1_EdgeForce.Count == ln1_Edge.Count)
                { force = db1_EdgeForce[i]; }
                else { force = 0.0; }
                Form_Diagram.Edges.Add(new EDGE(st1_Edge[i], force, cl1_Edge[i], ln1_Edge[i]));
            }

            // Loads
            List<Point3d> pt1_Load = new List<Point3d>(ln1_Load.Count);
            List<string> st1_Load = new List<string>(ln1_Load.Count);
            for (int i = 0; i < ln1_Load.Count; i++) st1_Load.Add("Q" + i.ToString());
            List<Color> cl1_Load = new List<Color>(ln1_Load.Count);
            for (int i = 0; i < ln1_Load.Count; i++) cl1_Load.Add(System.Drawing.Color.DarkGreen);
            List<string> st1_NodeLoad = new List<string>();
            foreach (int in0_LoadNode in in1_LoadNode) st1_NodeLoad.Add("v" + in0_LoadNode);
            //import diagram_Loads
            for (int i = 0; i < ln1_Load.Count; i++)
            {
                string actPt;
                if (st1_NodeLoad.Count != 0) { actPt = st1_NodeLoad[i]; } else { actPt = ""; }
                Form_Diagram.Loads.Add(new LOAD(st1_Load[i], cl1_Load[i], ln1_Load[i], actPt));
            }

            // Resultants
            if (vc0_R_Resultant.Length > db0_T)
            { Form_Diagram.Result.Add(new RESULT("Rs", Color.DarkGreen, new Line(pt0_CentralAxis, vc0_R_Resultant, vc0_R_Resultant.Length))); }
            if (vc0_M0_Resultant.Length > db0_T)
            { Form_Diagram.Result.Add(new RESULT("M0", Color.Purple, new Line(pt0_CentralAxis, vc0_M0_Resultant, vc0_M0_Resultant.Length))); }

            // Reactions
            if (pt1_Support.Count == vc1_Reac.Count)
            {
                List<Line> ln1_Reac = new List<Line>();
                for (int i = 0; i < pt1_Support.Count; i++) ln1_Reac.Add(new Line(pt1_Support[i], vc1_Reac[i]));
                List<Point3d> pt1_Reac = new List<Point3d>();
                List<string> st1_Reac = new List<string>();
                for (int i = 0; i < pt1_Support.Count; i++) st1_Reac.Add("R" + (i + 1).ToString());
                List<Color> cl1_Reac = new List<Color>();
                for (int i = 0; i < pt1_Support.Count; i++) cl1_Reac.Add(System.Drawing.Color.DarkGreen);
                List<string> st1_NodeSupport = new List<string>();
                foreach (int in0_SupportNode in in1_SupportNode) st1_NodeSupport.Add("v" + in0_SupportNode);

                for (int i = 0; i < ln1_Reac.Count; i++)
                { Form_Diagram.Reacts.Add(new REACT(st1_Reac[i], st1_Support[i], ln1_Reac[i], cl1_Reac[i], st1_NodeSupport[i])); }
            }

            //NodeSelection
            Form_Diagram.int0_NodeSelect = int0_NodeSelect;

            //Wirte out 
        }


        //[211206] Organize and export the data for planarization
        public void ExportDataForPlanarize(List<List<int>> in2_EdgeNodePlan, DIAGRAM Form_Diagram, double Tol, out List<List<string>> Vnms, out List<List<string>> eNs)
        {
            List<string> exsiting_edgeIDs = Form_Diagram.Edges.Select(p => p.ID).ToList(); //In case the bar is eliminated because of zero bar;

            // eN Data for Planarity eN|[vn,vm]__
            List<List<string>> st2_EdgeNode = new List<List<string>>();//[vn,vm]
            List<List<string>> st2_Edge = new List<List<string>>();//[eN]

            foreach (List<int> i in in2_EdgeNodePlan)
            {
                List<string> sub = new List<string>();
                foreach (int j in i)
                { sub.Add("v" + j.ToString()); }

                string eN = "e" + in2_EdgeNodePlan.IndexOf(i).ToString();
                if (exsiting_edgeIDs.Contains(eN))
                {
                    st2_EdgeNode.Add(sub);
                    st2_Edge.Add(new List<string>() { eN });
                }
            }
            //__

            //QN Data for Planarity QN[vn,ve]__
            List<List<string>> st2_QnNode = new List<List<string>>();
            List<List<string>> st2_Qn = new List<List<string>>();
            foreach (LOAD load in Form_Diagram.Loads)
            {
                if (load.ln.Length > Tol)
                {
                    List<string> QnVnm = new List<string>() { load.ActionPt_ID, "ve" };
                    List<string> Qn = new List<string>() { load.ID };
                    st2_QnNode.Add(QnVnm);
                    st2_Qn.Add(Qn);
                }
            }

            st2_EdgeNode.AddRange(st2_QnNode);
            st2_Edge.AddRange(st2_Qn);
            //__

            //RN Data for Planarity RN[vn,ve]__
            List<List<string>> st2_RnNode = new List<List<string>>();
            List<List<string>> st2_Rn = new List<List<string>>();
            foreach (REACT react in Form_Diagram.Reacts)
            {
                if (react.ln.Length > Tol)
                {
                    List<string> RnVnm = new List<string>() { react.ActionPt_ID, "ve" };
                    List<string> Rn = new List<string>() { react.ID };
                    st2_RnNode.Add(RnVnm);
                    st2_Rn.Add(Rn);
                }
            }

            st2_EdgeNode.AddRange(st2_RnNode);
            st2_Edge.AddRange(st2_Rn);
            //__

            //Check the duplicate for eN|[Vn,Vm] => [eN,eM]|[Vn,Vm]__
            //Import:st2_EdgeNode[vn,vm] & st2_Edge[eN]
            Vnms = new List<List<string>>(); //algin with Lib
            eNs = new List<List<string>>();//algin with Vnms
            List<string> Lib = new List<string>();

            for (int i = 0; i < st2_EdgeNode.Count; i++)
            {
                List<string> Vnm = st2_EdgeNode[i];
                List<string> eN = st2_Edge[i];
                string Key0 = string.Format("[{0},{1}]", Vnm[0], Vnm[1]);
                string Key1 = string.Format("[{0},{1}]", Vnm[1], Vnm[0]);
                if (Lib.Contains(Key0))
                {
                    int index = Lib.IndexOf(Key0);
                    eNs[index].Add(eN[0]);
                }
                else if (Lib.Contains(Key1))
                {
                    int index = Lib.IndexOf(Key1);
                    eNs[index].Add(eN[0]);
                }
                else
                {
                    Lib.Add(Key0);
                    Vnms.Add(Vnm);
                    eNs.Add(eN);
                }
            }
            //__
        }

        #endregion output end

    }
}
