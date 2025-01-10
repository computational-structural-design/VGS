using KangarooSolver;
using KangarooSolver.Goals;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace VGS_Main
{
    public class Diagram_Transformation_Kangaroo : VgsCommon
    {
        public class Reciprocal_force_config
        {

            //force setting

            private double pararell_force = 10;
            public double Pararell_force { get { return pararell_force; } set { if (value < 0) { pararell_force = 0; } else { pararell_force = value; } } }

            private double duplicate_force = 10;
            public double Duplicate_force { get { return duplicate_force; } set { if (value < 0) { duplicate_force = 0; } else { duplicate_force = value; } } }

            private double coincidentN_force = 0.1;
            public double CoincidentN_force { get { return coincidentN_force; } set { if (value < 0) { coincidentN_force = 0; } else { coincidentN_force = value; } } }

            private double cPolygon_force = 0.1;
            public double CPolygon_force { get { return cPolygon_force; } set { if (value < 0) { cPolygon_force = 0; } else { cPolygon_force = value; } } }

            public Reciprocal_force_config() { SetValues(10, 10, 0.1, 0.1); }

            public void SetValues(double pf, double df,  double cN,double cp)
            {
                Pararell_force = pf; Duplicate_force = df; CoincidentN_force =cN; CPolygon_force = cp;
            }

            public override string ToString()
            {
                return string.Format("Interdependency Config：\n" +    
                    "The strength keep the lines pararell: {0}\n" +
                    "The strength keep the duplicate edge in same length: {1}\n" +
                    "The strength CoincidentN_force: {2}\n" +
                    "The strength closing the force polygon:{3}",
                    Pararell_force.ToString(), Duplicate_force.ToString(), CoincidentN_force.ToString(), CPolygon_force.ToString());
            }
        }
        public class PhysicalSystem_config
        {
            //PS setting
            private int Step_MiniIteration = 10;
            public int Step_MiniIter { get { return Step_MiniIteration; } set { if (value > 1) { Step_MiniIteration = value; } else if (value < 0) { Step_MiniIteration = 1; } else { Step_MiniIteration = value; } } }

            private int animation_gap = 50;
            public int GAP_animation { get { return animation_gap; } set { if (value > 1000) { animation_gap = 1000; } else if (value < 10) { animation_gap = 10; } else { animation_gap = value; } } }

            private double DR_threshold = 0.0000000001;
            public double DR_th { get { return DR_threshold; } set { if (value > 0.001) { DR_threshold = 0.001; } else if (value < 0.00000000001) { DR_threshold = 0.00000000001; } else { DR_threshold = value; } } }

            private double distortionFm = 50.0;
            public double DistortionFm { get { return distortionFm; } set { if (value > 500) { distortionFm = 500; } else if (value < 1) { distortionFm = 1; } else { distortionFm = value; } } }

            private double distortionFc = 50.0;
            public double DistortionFc { get { return distortionFc; } set { if (value > 500) { distortionFc = 500; } else if (value < 1) { distortionFc = 1; } else { distortionFc = value; } } }

            private double distortion_force = 0.1;
            public double Distortion_force { get { return distortion_force; } set { if (value < 0) { distortion_force = 0; } else { distortion_force = value; } } }

            public PhysicalSystem_config()
            { }

            public override string ToString()
            {
                return string.Format("Simulation Config：\n" +
                    "The mini iteration number in each Physical Step: {0}\n" +
                    "Animate gap between iteration: {1}\n" +
                    "Threshold to stop iterating: {2}\n" +
                    "Distortion range for the transformation[Form diagram]: {3}",
                    "Distortion range for the transformation[Force diagram]: {4}",
                    Step_MiniIter.ToString(), GAP_animation.ToString(), DR_th.ToString(), DistortionFm.ToString(), DistortionFc.ToString());
            }
            public void SetValues(int stepIter, int ani_gap, double threshold, double distortionFm,double distortionFc,double distortionStrength)
            {
                Step_MiniIter = stepIter;
                GAP_animation = ani_gap;
                DR_th = threshold;
                DistortionFm = distortionFm;
                DistortionFc = distortionFc;
                Distortion_force = distortionStrength;
            }
        }
        public class lock_config
        {
            public List<string> RQ_lock;
            public List<bool[]> RQ_lock_ADLI;
            public List<double> RQ_lock_s;//strength of the external force config

            public List<string> eN_lock;
            public List<bool[]> eN_lock_DLM;
            public List<double> eN_lock_s;//

            public List<string> vN_lock;
            public List<bool[]> vN_lock_xyz;
            public List<double> vN_lock_s;

            public lock_config()
            {
                RQ_lock = new List<string>();
                RQ_lock_ADLI = new List<bool[]>();
                RQ_lock_s = new List<double>();

                eN_lock = new List<string>();
                eN_lock_DLM = new List<bool[]>();
                eN_lock_s = new List<double>();

                vN_lock = new List<string>();
                vN_lock_xyz = new List<bool[]>();
                vN_lock_s = new List<double>();
            }

            public void ReadData(lock_config config)
            {
                foreach (string id in config.RQ_lock)
                {
                    if (this.RQ_lock.Contains(id)) 
                    { 
                        this.RQ_lock_ADLI[RQ_lock.IndexOf(id)] = config.RQ_lock_ADLI[config.RQ_lock.IndexOf(id)]; 
                        this.RQ_lock_s[RQ_lock.IndexOf(id)] = config.RQ_lock_s[config.RQ_lock.IndexOf(id)]; 
                    }
                    else 
                    {
                        this.RQ_lock.Add(id);
                        this.RQ_lock_ADLI.Add(config.RQ_lock_ADLI[config.RQ_lock.IndexOf(id)]);
                        this.RQ_lock_s.Add(config.RQ_lock_s[config.RQ_lock.IndexOf(id)]);
                    }
                }

                foreach (string id in config.eN_lock)
                {
                    if (this.eN_lock.Contains(id)) 
                    {
                        this.eN_lock_DLM[eN_lock.IndexOf(id)] = config.eN_lock_DLM[config.eN_lock.IndexOf(id)];
                        this.eN_lock_s[eN_lock.IndexOf(id)] = config.eN_lock_s[config.eN_lock.IndexOf(id)];
                    }
                    else 
                    {
                        this.eN_lock.Add(id);
                        this.eN_lock_DLM.Add(config.eN_lock_DLM[config.eN_lock.IndexOf(id)]);
                        this.eN_lock_s.Add(config.eN_lock_s[config.eN_lock.IndexOf(id)]);
                    }
                }

                foreach (string id in config.vN_lock)
                {
                    if (this.vN_lock.Contains(id))
                    {
                        this.vN_lock_xyz[vN_lock.IndexOf(id)] = config.vN_lock_xyz[config.vN_lock.IndexOf(id)];
                        this.vN_lock_s[vN_lock.IndexOf(id)] = config.vN_lock_s[config.vN_lock.IndexOf(id)];
                    }
                    else 
                    {
                        this.vN_lock.Add(id);
                        this.vN_lock_s.Add(config.vN_lock_s[config.vN_lock.IndexOf(id)]);
                        this.vN_lock_xyz.Add(config.vN_lock_xyz[config.vN_lock.IndexOf(id)]);
                    }
                }
            }
            public void default_setting(List<string> loads_id, List<string> reacts_id, List<string> edges_id)
            {
                List<string> rq_id = ListCopy(loads_id);
                rq_id.AddRange(reacts_id);
                foreach (string id in rq_id)
                {
                    if (!RQ_lock.Contains(id)) { RQ_lock.Add(id); RQ_lock_ADLI.Add(new bool[] { false, false, false, false }); RQ_lock_s.Add(0.0); }//[210901]增加第四个设定 在作用线上
                }
                foreach (string id in edges_id)
                {
                    if (!eN_lock.Contains(id)) { eN_lock.Add(id); eN_lock_DLM.Add(new bool[] { false, false,false }); eN_lock_s.Add(0.0); }
                }
            }//defaulty we add all the  loads supports reaction forces and the edge setting into the configuration.

            public override string ToString()
            {
                string report = "";
                if (RQ_lock.Count > 0) { report += "RQ_lock: " + RQ_lock.Count.ToString() + "\n"; }
                if (eN_lock.Count > 0) { report += "eN_lock: " + eN_lock.Count.ToString() + "\n"; }
                if (vN_lock.Count > 0) { report += "vN_lock: " + vN_lock.Count.ToString() + "\n"; }
                return report;
            }
        }

        public class TransfTempInfo
        {
            public TransfTempInfo() { }

            public lock_config lock_info;

            public List<string> RQ_fix;
            public List<string> RQ_changeable;

            public List<Point3d> Nodes;
            public List<string> Nodes_id;

            public List<Line> Fm_ln;
            public List<string> Fm_id;
            public List<double> Fm_db;

            public List<List<Line>> Fc_ln;
            public List<List<string[]>> Fc_vn;
            public List<List<string>> Fc_id;
            public List<List<double>> Fc_db;

            public List<List<bool>> Fc_reverse;//corresponds to Fc_ln;
            public List<List<int[]>> Fc_back_ind; //corresponds with Fc_ln（list algin with Fm_ln） back to the index of Fc_ln2(input);

            public List<Line> Fc_ln0 { get { return Flattern2D(this.Fc_ln); } }
        }
        public static class Transformation_model
        {

            public static List<IGoal> Transformation_LoadPath_Optimize(MODEL Fm, MODEL Fc, double Strength,ref TransfTempInfo tp, double tolerance)
            {
                List<Line> Fm_ln1_e = Fm.Diagram.Edges.Select(p => p.ln).ToList();
                List<string> Fm_st1_e = Fm.Diagram.Edges.Select(p => p.ID).ToList();
                List<double> Fm_db1_e = Fm.Diagram.Edges.Select(p => p.force).ToList();
                //Since the logic corresponding to shape and force is updated, a copy method is created here to continue the old version logic; [0618]
                FmFcMatch(ref tp,Fm_ln1_e, Fm_st1_e, Fm_db1_e, Fc.Diagram.ln2_Edge, Fc.Diagram.st2_Edge, Fc.Diagram.db2_EdgeForce, Fc.Diagram.st2_vn_Edge, tolerance);
               
                List<IGoal> goals = new List<IGoal>();

                for (int i = 0; i < tp.Fm_ln.Count; i++)
                {
                    for (int j = 0; j < tp.Fc_ln[i].Count; j++)
                    {
                        goals.Add(new GS_loadpathOPT(Strength, tp.Fm_ln[i], tp.Fc_ln[i][j]));
                    }
                }
                return goals;
            }
            public static List<IGoal> Transformation_Reciprocal(ref TransfTempInfo tp,MODEL Fm, MODEL Fc, lock_config input_lock_info, Reciprocal_force_config R_config,PhysicalSystem_config S_config, double tolerance)
            {
                List<Point3d> nodes = Fm.Diagram.Nodes.Select(p => p.pt).ToList();
                List<string> nodes_id = Fm.Diagram.Nodes.Select(p => p.ID).ToList();

                List<Line> Fm_ln1_e = Fm.Diagram.Edges.Select(p => p.ln).ToList();
                List<string> Fm_st1_e = Fm.Diagram.Edges.Select(p => p.ID).ToList();
                List<double> Fm_db1_e = Fm.Diagram.Edges.Select(p => p.force).ToList();

                List<Line> Fm_ln1_l = Fm.Diagram.Loads.Select(p => p.ln).ToList();
                List<string> Fm_st1_l = Fm.Diagram.Loads.Select(p => p.ID).ToList();
                List<double> Fm_db1_l = Fm.Diagram.Loads.Select(p => p.ln.Length).ToList();

                List<Line> Fm_ln1_r = Fm.Diagram.Reacts.Select(p => p.ln).ToList();
                List<string> Fm_st1_r = Fm.Diagram.Reacts.Select(p => p.ID).ToList();
                List<double> Fm_db1_r = Fm.Diagram.Reacts.Select(p => p.ln.Length).ToList();

                List<Line> Fm_ln1 = new List<Line>(); Fm_ln1.AddRange(Fm_ln1_e); Fm_ln1.AddRange(Fm_ln1_l); Fm_ln1.AddRange(Fm_ln1_r);
                List<string> Fm_st1 = new List<string>(); Fm_st1.AddRange(Fm_st1_e); Fm_st1.AddRange(Fm_st1_l); Fm_st1.AddRange(Fm_st1_r);
                List<double> Fm_db1 = new List<double>(); Fm_db1.AddRange(Fm_db1_e); Fm_db1.AddRange(Fm_db1_l); Fm_db1.AddRange(Fm_db1_r);

                tp.Nodes = ListCopy(nodes); tp.Nodes_id = ListCopy(nodes_id);

                FmFcMatch(ref tp,Fm_ln1, Fm_st1, Fm_db1, Fc.Diagram.ln2_Edge, Fc.Diagram.st2_Edge, Fc.Diagram.db2_EdgeForce, Fc.Diagram.st2_vn_Edge, tolerance);

                tp.lock_info = new lock_config();
                tp.lock_info.default_setting(Fm_st1_l, Fm_st1_r, Fm_st1_e);
                tp.lock_info.ReadData(input_lock_info);

                //return Interaction_setting_new(distort, recip_config);
                return Interaction_setting_TEST(tp, Fm, R_config, S_config);//[0611]
            }
            public static string WirteBackToModel(ref TransfTempInfo tp, MODEL Fm, MODEL Fc, List<Object> output)
            {
                List<Line> Output_lines = new List<Line>();
                for (int i = 0; i < output.Count; i++)
                {
                    if (output[i] == null) { continue; }
                    Output_lines.Add((Line)output[i]);
                }
                if (!UpdateValue(Output_lines,ref tp)) { return "Output Error: the number of edges are not matching"; } //Consuming!!

                if (!WriteBackToFormDiagram(tp, ref Fm.Diagram.Edges, ref Fm.Diagram.Loads, ref Fm.Diagram.Reacts)) { return "Write Back Error: the Form edges are not matching"; }

                if (!WriteBackToForceDiagram(tp, ref Fc.Diagram.ln2_Edge, ref Fc.Diagram.st2_Edge, ref Fc.Diagram.db2_EdgeForce)) { return "Write Back Error: the Force edges are not matching"; }

                return "Success";
            }
            public static bool UpdateValue(List<Line> Output_lines, ref TransfTempInfo tp)
            {
                List<Line> Fc0_ln = new List<Line>(tp.Fc_ln0);

                if ((tp.Fm_ln.Count + Fc0_ln.Count) > Output_lines.Count)
                { return false; }
                
               for (int i = 0; i < Output_lines.Count - Fc0_ln.Count; i++)
               {
                   tp.Fm_ln[i] = Output_lines[i];
               }
               
                        int k = Output_lines.Count - Fc0_ln.Count;
                        for (int i = 0; i < tp.Fc_ln.Count; i++)
                        {
                            double force_sum = 0;
                            for (int j = 0; j < tp.Fc_ln[i].Count; j++)
                            {
                                tp.Fc_ln[i][j] = Output_lines[k];
                                if (tp.Fc_db[i][j] > 0) { tp.Fc_db[i][j] = tp.Fc_ln[i][j].Length; }
                                else { tp.Fc_db[i][j] = -tp.Fc_ln[i][j].Length; }
                                force_sum += tp.Fc_db[i][j];
                                k++;
                            }
                            tp.Fm_db[i] = force_sum / tp.Fc_db[i].Count;
                        }

                return true;
            }
            private static bool WriteBackToForceDiagram(TransfTempInfo tp, ref List<List<Line>> Fc_ln2, ref List<List<string>> Fc_id2, ref List<List<double>> Fc_db2)
            {
                for (int i = 0; i < tp.Fc_ln.Count; i++)
                {
                    for (int j = 0; j < tp.Fc_ln[i].Count; j++)
                    {
                        int ind0 = tp.Fc_back_ind[i][j][0];
                        int ind1 = tp.Fc_back_ind[i][j][1];

                        if (tp.Fc_id[i][j] != Fc_id2[ind0][ind1]) { return false; }// if the name is not matched,return false

                        if (tp.Fc_reverse[ind0][ind1])
                        {
                            Fc_ln2[ind0][ind1] = new Line(tp.Fc_ln[i][j].To, tp.Fc_ln[i][j].From); Fc_db2[ind0][ind1] = tp.Fc_db[i][j];
                        }
                        else
                        {
                            Fc_ln2[ind0][ind1] = new Line(tp.Fc_ln[i][j].From, tp.Fc_ln[i][j].To); Fc_db2[ind0][ind1] = tp.Fc_db[i][j];
                        }

                    }
                }
                return true;
            }
            private static bool WriteBackToFormDiagram(TransfTempInfo tp, ref List<EDGE> Model_edges, ref List<LOAD> Model_loads, ref List<REACT> Model_reacts)
            {
                int k = 0;

                for (int i = 0; i < Model_edges.Count; i++)
                {
                    if (System.Math.Abs(Model_edges[i].force) < System_Configuration.Sys_Tor) { continue; }
                    if (Model_edges[i].ID != tp.Fm_id[k]) { return false; }
                    Model_edges[i].ln = tp.Fm_ln[k]; Model_edges[i].force = tp.Fm_db[k];
                    k++;
                }

                for (int i = 0; i < Model_loads.Count; i++)
                {
                    if (System.Math.Abs(Model_loads[i].ln.Length) < System_Configuration.Sys_Tor) { continue; }
                    if (Model_loads[i].ID != tp.Fm_id[k]) { return false; }
                    Model_loads[i].ln = tp.Fm_ln[k];
                    k++;
                }

                for (int i = 0; i < Model_reacts.Count; i++)
                {
                    if (System.Math.Abs(Model_reacts[i].ln.Length) < System_Configuration.Sys_Tor) { continue; }
                    if (Model_reacts[i].ID != tp.Fm_id[k]) { return false; }
                    Model_reacts[i].ln = tp.Fm_ln[k];
                    k++;
                }

                return true;
            }
            private static void FmFcMatch(ref TransfTempInfo tp,List<Line> Fm_ln1, List<string> Fm_id1, List<double> Fm_db1, List<List<Line>> Fc_ln2, List<List<string>> Fc_id2, List<List<double>> Fc_db2, List<List<string[]>> Fc_vn2, double tol)
            {
                tp.Fm_ln = new List<Line>();
                tp.Fm_id = new List<string>();
                tp.Fm_db = new List<double>();

                tp.Fc_ln = new List<List<Line>>();
                tp.Fc_vn = new List<List<string[]>>();
                tp.Fc_id = new List<List<string>>();
                tp.Fc_db = new List<List<double>>();

                tp.Fc_reverse = new List<List<bool>>(Fc_ln2.Count);//initialize the reverse signal of the fc_edges
                tp.Fc_back_ind = new List<List<int[]>>();//TM.Fc_reverse[][] is the reverse information
                for (int i = 0; i < Fc_ln2.Count; i++)
                {
                    List<bool> temp = new List<bool>(Fc_ln2[i].Count);
                    for (int j = 0; j < Fc_ln2[i].Count; j++) { temp.Add(false); }
                    tp.Fc_reverse.Add(temp);
                }

                for (int i = 0; i < Fm_ln1.Count; i++)
                {
                    if (Fm_ln1[i].Length < tol || System.Math.Abs(Fm_db1[i]) < tol) { continue; }

                    tp.Fm_ln.Add(Fm_ln1[i]);
                    tp.Fm_id.Add(Fm_id1[i]);
                    tp.Fm_db.Add(Fm_db1[i]);

                    //Try to find the matched edges in force diagram
                    List<Line> Fc_ln_sub = new List<Line>();
                    List<string> Fc_id_sub = new List<string>();
                    List<double> Fc_db_sub = new List<double>();
                    List<int[]> Fc_ind_sub = new List<int[]>();
                    List<string[]> Fc_vn_sub = new List<string[]>();

                    for (int j = 0; j < Fc_ln2.Count; j++)
                    {
                        for (int k = 0; k < Fc_ln2[j].Count; k++)
                        {
                            if (Fm_id1[i] == Fc_id2[j][k])//match
                            {
                                //[0611][0618]
                                //if (angle_value(Fm_ln1[i], Fc_ln2[j][k]) <1.414214) { Fc_ln2[j][k] = new Line(Fc_ln2[j][k].To, Fc_ln2[j][k].From); this.Fc_reverse[j][k] = true; }//Reverse the edge if oppositing[0611]
                                Fc_ln_sub.Add(Fc_ln2[j][k]);
                                Fc_id_sub.Add(Fc_id2[j][k]);
                                Fc_db_sub.Add(Fc_db2[j][k]);
                                Fc_ind_sub.Add(new int[2] { j, k });
                                Fc_vn_sub.Add(Fc_vn2[j][k]);
                            }
                        }
                    }
                    tp.Fc_ln.Add(Fc_ln_sub);
                    tp.Fc_id.Add(Fc_id_sub);
                    tp.Fc_db.Add(Fc_db_sub);
                    tp.Fc_back_ind.Add(Fc_ind_sub);
                    tp.Fc_vn.Add(Fc_vn_sub);
                }

            }

            /*
            private static List<IGoal> Interaction_setting(TransfTempInfo tp, MODEL Fm, Reciprocal_force_config recip_config, PhysicalSystem_config S_config)
            {
                //The distorion range of form and force
                double Fm_distort = S_config.DistortionFm;
                double Fc_distort = S_config.DistortionFc;
                double DistortionStrength = S_config.Distortion_force;
                //import values
                List<Line> Fm_ln = tp.Fm_ln;
                List<string> Fm_id = tp.Fm_id;
                
                List<List<Line>> Fc_ln = tp.Fc_ln;
                List<List<string>> Fc_id = tp.Fc_id;
                List<List<string[]>> Fc_vn = tp.Fc_vn;

                //RQxyz should be in the order of the ones in Fm_ln(the last lines usually)
                List<IGoal> Goal = new List<IGoal>();
                //define the para of transformation
                double Pararell_force = recip_config.Pararell_force;
                double Duplicate_force = recip_config.Duplicate_force;

                //define the goals and outputs
                List<IGoal> Angle = new List<IGoal>();
                List<KangarooSolver.Goals.AnchorXYZ> Anchor = new List<KangarooSolver.Goals.AnchorXYZ>();
                List<KangarooSolver.Goals.CoLinear> CoLinear = new List<KangarooSolver.Goals.CoLinear>();
                List<KangarooSolver.Goals.AnchorXYZ> CoLinear_FixPts = new List<KangarooSolver.Goals.AnchorXYZ>();

                List<KangarooSolver.Goals.ClampLength> Form_CL = new List<KangarooSolver.Goals.ClampLength>();
                List<KangarooSolver.Goals.ClampLength> Force_CL = new List<KangarooSolver.Goals.ClampLength>();
                List<KangarooSolver.Goals.Spring> Form_spring = new List<KangarooSolver.Goals.Spring>();
                List<KangarooSolver.Goals.Spring> Force_spring = new List<KangarooSolver.Goals.Spring>();

                List<GS_Direction> Ext_Dir = new List<GS_Direction>();//Reaction force and Load dosent change
                List<KangarooSolver.Goals.EqualLength> Dupli_edge = new List<EqualLength>();

                //Config the reciprocacity
                for (int i = 0; i < Fm_ln.Count; i++)
                {
                    //Chech if external
                    if ((Fm_id[i].Substring(0, 1) == "Q" || Fm_id[i].Substring(0, 1) == "R"))//IF EXTERNAL CASE
                    {
                        //
                        string id = Fm_id[i];
                        if (tp.lock_info.RQ_lock.Contains(id))//If the external force is setted
                        {
                            int index = tp.lock_info.RQ_lock.IndexOf(id);
                            bool[] setting = tp.lock_info.RQ_lock_ADLI[index];
                            double RQstrength = tp.lock_info.RQ_lock_s[index];
                            if (setting[0]) { Anchor.Add(new AnchorXYZ(Fm_ln[i].From, true, true, true, RQstrength)); }
                            if (setting[1]) { Ext_Dir.Add(new GS_Direction(Fm_ln[i].From, Fm_ln[i].To, new Vector3d(Fm_ln[i].To - Fm_ln[i].From), RQstrength)); }
                            if (setting[2]) { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, RQstrength)); }
                            else { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, 0.0)); }
                            if (setting[3])
                            {
                                List<Point3d> C_pts = Colinear_Pts(Fm_ln[i]);
                                CoLinear.Add(new KangarooSolver.Goals.CoLinear(C_pts, RQstrength));
                                CoLinear_FixPts.Add(new AnchorXYZ(C_pts[2], true, true, true, RQstrength));
                                CoLinear_FixPts.Add(new AnchorXYZ(C_pts[3], true, true, true, RQstrength));
                            }//[210901]

                            //Basic edge setting for FORM edges
                            Form_CL.Add(new KangarooSolver.Goals.ClampLength(Fm_ln[i].From, Fm_ln[i].To, Fm_ln[i].Length * Fm_distort, Fm_ln[i].Length / Fm_distort, DistortionStrength));
                            for (int j = 0; j < Fc_ln[i].Count; j++)//for the corresponding forces
                            {

                                Line fm = Fm_ln[i];//[210902]
                                if (Fc_vn[i][j][0] == "ve") { fm = new Line(Fm_ln[i].To, Fm_ln[i].From); }//[210902] If the serial number is inverted, it means that it needs to be reversed
                                //Keep pararell between the matched edges(form and force)
                                Angle.Add(new K_Angle2(fm, Fc_ln[i][j],0.0, Pararell_force));//[210902] In the case of splitting the external force, the direction needs to be judged again[C]
                                //Angle.Add(new Angle2(fm, Fc_ln[i][j], 0.0, Pararell_force));

                                //Basic edge setting for FORCE edges
                                Force_CL.Add(new KangarooSolver.Goals.ClampLength(Fc_ln[i][j].From, Fc_ln[i][j].To, Fc_ln[i][j].Length * Fc_distort, Fc_ln[i][j].Length / Fc_distort, DistortionStrength));
                                if (setting[2]) { Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, RQstrength)); }
                                else { Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, 0.0)); }

                                //Same length in form and foce for loads and reacts
                                List<Curve> same_len_lines = new List<Curve>() { Fm_ln[i].ToNurbsCurve(), Fc_ln[i][j].ToNurbsCurve() };
                                Dupli_edge.Add(new KangarooSolver.Goals.EqualLength(same_len_lines, Duplicate_force));
                            }
                        }
                    }
                    else//Not External
                    {
                        string id = Fm_id[i];
                        if (tp.lock_info.eN_lock.Contains(id))
                        {
                            int index = tp.lock_info.eN_lock.IndexOf(id);
                            bool[] setting = tp.lock_info.eN_lock_DLM[index];
                            double eNstrength= tp.lock_info.eN_lock_s[index];
                            if (setting[0]) { Ext_Dir.Add(new GS_Direction(Fm_ln[i].From, Fm_ln[i].To, new Vector3d(Fm_ln[i].To - Fm_ln[i].From), eNstrength)); }
                            //Basic edge setting for FORM edges
                            Form_CL.Add(new KangarooSolver.Goals.ClampLength(Fm_ln[i].From, Fm_ln[i].To, Fm_ln[i].Length * Fm_distort, Fm_ln[i].Length / Fm_distort, DistortionStrength));
                            if (setting[1]) { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, eNstrength)); }
                            else { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, 0.0)); }

                            //Keep the same length between the duplicate edges
                            if (Fc_ln[i].Count > 1)
                            {
                                List<Curve> pararell_lines = new List<Curve>();
                                foreach (Line ln in Fc_ln[i]) { pararell_lines.Add(ln.ToNurbsCurve()); }
                                Dupli_edge.Add(new KangarooSolver.Goals.EqualLength(pararell_lines, Duplicate_force));
                            }

                            for (int j = 0; j < Fc_ln[i].Count; j++)
                            {
                                //Keep pararell between the matched edges(form and force)
                                int in0 = Fm.Diagram.Nodes.Select(p => p.ID).ToList().IndexOf(Fc_vn[i][j][0]);
                                int in1 = Fm.Diagram.Nodes.Select(p => p.ID).ToList().IndexOf(Fc_vn[i][j][1]);
                                Point3d pt0 = Fm.Diagram.Nodes[in0].pt; Point3d pt1 = Fm.Diagram.Nodes[in1].pt;
                                Line fm = new Line(pt0, pt1);
                              
                                Angle.Add(new K_Angle2(fm, Fc_ln[i][j],0.0,Pararell_force));//[C]

                                //Basic edge setting for FORCE edges
                                Force_CL.Add(new KangarooSolver.Goals.ClampLength(Fc_ln[i][j].From, Fc_ln[i][j].To, Fc_ln[i][j].Length * Fc_distort, Fc_ln[i][j].Length / Fc_distort, DistortionStrength));
                                //[220402]if the magnitude is fixed, then the force spring is fixed
                                if (setting[2]) { Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, eNstrength)); }
                                else { Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, 0.0)); };
                                //[220402]__End change
                            }
                        }
                    }
                }

                //Config the globa anchors
                for (int i = 0; i < tp.lock_info.vN_lock.Count; i++)
                {
                    string vID = tp.lock_info.vN_lock[i];
                    bool[] setting = tp.lock_info.vN_lock_xyz[i];
                    double vNstrength = tp.lock_info.vN_lock_s[i];
                    if (vID.Substring(0, 1) == "v")
                    {
                        int index = tp.Nodes_id.IndexOf(vID);
                        Anchor.Add(new AnchorXYZ(tp.Nodes[index], setting[0], setting[1], setting[2], vNstrength));
                    }
                }
                //_End Config_

                Goal.AddRange(Form_spring);
                Goal.AddRange(Force_spring);
                Goal.AddRange(Form_CL);
                Goal.AddRange(Force_CL);

                Goal.AddRange(Angle);
                Goal.AddRange(Anchor);
                Goal.AddRange(Dupli_edge);
                Goal.AddRange(Ext_Dir);
                Goal.AddRange(CoLinear);
                Goal.AddRange(CoLinear_FixPts);
                return Goal;
            }
            */
            private static List<IGoal> Interaction_setting_TEST(TransfTempInfo tp,MODEL Fm, Reciprocal_force_config recip_config, PhysicalSystem_config S_config)
            {
                //The distorion range of form and force
                double Fm_distort = S_config.DistortionFm;
                double Fc_distort = S_config.DistortionFc;
                double DistortionStrength = S_config.Distortion_force;

                //import values
                List<Line> Fm_ln = tp.Fm_ln;
                List<string> Fm_id = tp.Fm_id;

                List<List<Line>> Fc_ln = tp.Fc_ln;
                List<List<string>> Fc_id = tp.Fc_id;
                List<List<string[]>> Fc_vn = tp.Fc_vn;

                //RQxyz should be in the order of the ones in Fm_ln(the last lines usually)
                List<IGoal> Goal = new List<IGoal>();
                //define the para of transformation
                double Pararell_force = recip_config.Pararell_force;
                double Duplicate_force = recip_config.Duplicate_force;

                //define the goals and outputs
                List<IGoal> Angle = new List<IGoal>();
                List<KangarooSolver.Goals.AnchorXYZ> Anchor = new List<KangarooSolver.Goals.AnchorXYZ>();
                List<KangarooSolver.Goals.CoLinear> CoLinear = new List<KangarooSolver.Goals.CoLinear>();
                List<KangarooSolver.Goals.AnchorXYZ> CoLinear_FixPts = new List<KangarooSolver.Goals.AnchorXYZ>();

                List<KangarooSolver.Goals.ClampLength> Form_CL = new List<KangarooSolver.Goals.ClampLength>();
                List<KangarooSolver.Goals.ClampLength> Force_CL = new List<KangarooSolver.Goals.ClampLength>();
                List<KangarooSolver.Goals.Spring> Form_spring = new List<KangarooSolver.Goals.Spring>();
                List<KangarooSolver.Goals.Spring> Force_spring = new List<KangarooSolver.Goals.Spring>();

                List<GS_Direction> Ext_Dir = new List<GS_Direction>();//Reaction force and Load dosent change
                List<KangarooSolver.Goals.EqualLength> Dupli_edge = new List<EqualLength>();
                
                //Config the reciprocacity
                for (int i = 0; i < Fm_ln.Count; i++)
                {
                    //Chech if external
                    if ((Fm_id[i].Substring(0, 1) == "Q" || Fm_id[i].Substring(0, 1) == "R"))//IF EXTERNAL CASE
                    {
                        //
                        string id = Fm_id[i];
                        if (tp.lock_info.RQ_lock.Contains(id))//If the external force is setted
                        {
                            int index = tp.lock_info.RQ_lock.IndexOf(id);
                            bool[] setting = tp.lock_info.RQ_lock_ADLI[index];
                            double RQstrength = tp.lock_info.RQ_lock_s[index];
                            if (setting[0]) { Anchor.Add(new AnchorXYZ(Fm_ln[i].From, true, true, true, RQstrength)); }
                            if (setting[1]) { Ext_Dir.Add(new GS_Direction(Fm_ln[i].From, Fm_ln[i].To, new Vector3d(Fm_ln[i].To - Fm_ln[i].From), RQstrength)); }
                            if (setting[2]) { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, RQstrength)); }
                            else { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, 0.0)); }
                            if (setting[3])
                            {
                                List<Point3d> C_pts = Colinear_Pts(Fm_ln[i]);
                                CoLinear.Add(new KangarooSolver.Goals.CoLinear(C_pts, RQstrength));
                                CoLinear_FixPts.Add(new AnchorXYZ(C_pts[2], true, true, true, RQstrength));
                                CoLinear_FixPts.Add(new AnchorXYZ(C_pts[3], true, true, true, RQstrength));
                            }//[210901]

                            //Basic edge setting for FORM edges
                            Form_CL.Add(new KangarooSolver.Goals.ClampLength(Fm_ln[i].From, Fm_ln[i].To, Fm_ln[i].Length * Fm_distort, Fm_ln[i].Length / Fm_distort, DistortionStrength));

                            for (int j = 0; j < Fc_ln[i].Count; j++)//for the corresponding forces
                            {
                                Line fm = Fm_ln[i];//[210902]
                                if (Fc_vn[i][j][0] == "ve") { fm = new Line(Fm_ln[i].To, Fm_ln[i].From); }//[210902] If the serial number is inverted, it means that it needs to be reversed
                                //Keep pararell between the matched edges(form and force)
                                //K_Angle2: Added two weights to different the shear force on form and force;
                                 Angle.Add(new K_Angle2(fm, Fc_ln[i][j], 0.0, Pararell_force/ Fc_ln[i].Count, Fm_distort, Fc_distort));//[ANGLE]
                                //Angle.Add(new Angle2(fm, Fc_ln[i][j], 0.0, Pararell_force));

                                //Basic edge setting for FORCE edges
                                Force_CL.Add(new KangarooSolver.Goals.ClampLength(Fc_ln[i][j].From, Fc_ln[i][j].To, Fc_ln[i][j].Length * Fc_distort, Fc_ln[i][j].Length / Fc_distort, DistortionStrength));
                                if (setting[2]) { Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, RQstrength)); }
                                else { Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, 0.0)); }

                                //Same length in form and foce for loads and reacts
                                List<Curve> same_len_lines = new List<Curve>() { Fm_ln[i].ToNurbsCurve(), Fc_ln[i][j].ToNurbsCurve() };
                                Dupli_edge.Add(new KangarooSolver.Goals.EqualLength(same_len_lines, Duplicate_force));
                            }
                        }
                    }
                    else//Not External
                    {
                        string id = Fm_id[i];
                        if (tp.lock_info.eN_lock.Contains(id))
                        {
                            int index = tp.lock_info.eN_lock.IndexOf(id);
                            bool[] setting = tp.lock_info.eN_lock_DLM[index];
                            double eNstrength = tp.lock_info.eN_lock_s[index];
                            if (setting[0]) { Ext_Dir.Add(new GS_Direction(Fm_ln[i].From, Fm_ln[i].To, new Vector3d(Fm_ln[i].To - Fm_ln[i].From), eNstrength)); }
                            //Basic edge setting for FORM edges
                            Form_CL.Add(new KangarooSolver.Goals.ClampLength(Fm_ln[i].From, Fm_ln[i].To, Fm_ln[i].Length * Fm_distort, Fm_ln[i].Length / Fm_distort, DistortionStrength));
                            if (setting[1]) { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, eNstrength)); }
                            else { Form_spring.Add(new KangarooSolver.Goals.Spring(Fm_ln[i], Fm_ln[i].Length, 0.0)); }

                            //Keep the same length between the duplicate edges
                            if (Fc_ln[i].Count > 1)
                            {
                                List<Curve> pararell_lines = new List<Curve>();
                                foreach (Line ln in Fc_ln[i]) { pararell_lines.Add(ln.ToNurbsCurve()); }
                                Dupli_edge.Add(new KangarooSolver.Goals.EqualLength(pararell_lines, Duplicate_force));
                            }

                            for (int j = 0; j < Fc_ln[i].Count; j++)
                            {
                                //Keep pararell between the matched edges(form and force)
                                int in0 = Fm.Diagram.Nodes.Select(p => p.ID).ToList().IndexOf(Fc_vn[i][j][0]);
                                int in1 = Fm.Diagram.Nodes.Select(p => p.ID).ToList().IndexOf(Fc_vn[i][j][1]);
                                Point3d pt0 = Fm.Diagram.Nodes[in0].pt; Point3d pt1 = Fm.Diagram.Nodes[in1].pt;
                                Line fm = new Line(pt0, pt1);

                                Angle.Add(new K_Angle2(fm, Fc_ln[i][j], 0.0, Pararell_force/ Fc_ln[i].Count, Fm_distort, Fc_distort)); //[ANGLE]
                                //[C]

                                //Basic edge setting for FORCE edges
                                Force_CL.Add(new KangarooSolver.Goals.ClampLength(Fc_ln[i][j].From, Fc_ln[i][j].To, Fc_ln[i][j].Length * Fc_distort, Fc_ln[i][j].Length / Fc_distort, DistortionStrength));
                                //[220402]if the magnitude is fixed, then the force spring is fixed
                                if (setting[2]) { Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, eNstrength)); }
                                else {Force_spring.Add(new KangarooSolver.Goals.Spring(Fc_ln[i][j], Fc_ln[i][j].Length, 0.0)); };
                                //[220402]__End change
                            }
                        }
                    }
                }

                //Config the globa anchors
                for (int i = 0; i < tp.lock_info.vN_lock.Count; i++)
                {
                    string vID = tp.lock_info.vN_lock[i];
                    bool[] setting = tp.lock_info.vN_lock_xyz[i];
                    double vNstrength = tp.lock_info.vN_lock_s[i];
                    if (vID.Substring(0, 1) == "v")
                    {
                        int index = tp.Nodes_id.IndexOf(vID);
                        Anchor.Add(new AnchorXYZ(tp.Nodes[index], setting[0], setting[1], setting[2], vNstrength));
                    }
                }
                //_End Config_

                Goal.AddRange(Form_spring);
                Goal.AddRange(Force_spring);
                Goal.AddRange(Form_CL);
                Goal.AddRange(Force_CL);

                Goal.AddRange(Angle);
                Goal.AddRange(Anchor);
                Goal.AddRange(Dupli_edge);
                Goal.AddRange(Ext_Dir);
                Goal.AddRange(CoLinear);
                Goal.AddRange(CoLinear_FixPts);
                return Goal;
            }
        }


        
        public class PhysicalSystem_K_test
        {
            public PhysicalVGS PS;
            public int iteration;
            public string status;
            private List<IGoal> calculate_goals;

            public PhysicalSystem_K_test() {this.Reset();}
            public List<Object> pip(bool reset, List<Object> Geometric_constraints, List<IGoal> Reciprocal, int gap)
            {
                if (reset || PS == null) { Reset(); }
                //PS.ClearParticles();[Debug]
                calculate_goals = Setup_Pindex(Geometric_constraints, Reciprocal, 0.00001);

                if (reset) { return PS.GetOutput(Reciprocal); }
                else
                {
                    return NormalIteration(calculate_goals, Reciprocal, gap);
                    //return SmoothIteration(calculate_goals, Reciprocal, gap);
                }
            }

            public List<Object> pip_(bool reset, List<Object> Geometric_constraints, List<IGoal> Reciprocal, int gap)
            {
                if (reset || PS == null) { Reset(); }

                if (reset) { return PS.GetOutput(Reciprocal); }
                else
                {
                    return NormalIteration(calculate_goals, Reciprocal, gap);
                    //return SmoothIteration(calculate_goals, Reciprocal, gap);
                }
            }

            public List<IGoal> Setup_Pindex(List<Object> Geometric_constraints, List<IGoal> Reciprocal, double tol_samepoint)
            {
                List<IGoal> Goals = new List<IGoal>();

                foreach (IGoal g in Reciprocal)
                {
                    PS.AssignPIndex(g, tol_samepoint);
                    Goals.Add(g);
                }

                foreach (IGoal g in Geometric_constraints)
                {
                    if (g.PPos == null) { Goals.Add(g); continue; }
                    PS.AssignPIndex(g, tol_samepoint);
                    Goals.Add(g);
                }

                return Goals;
            }
            public void Reset()
            { PS = new PhysicalVGS(); iteration = 0; status = "Initialized"; }
            public List<Object> NormalIteration(List<IGoal> Goals, List<IGoal> Reciprocal, int miniIteration)
            {
                PS.Step(Goals, true, miniIteration);
                iteration++;
                return PS.GetOutput(Reciprocal);
            }
            public List<Object> SmoothIteration(List<IGoal> Goals, List<IGoal> Reciprocal, int miniIteration)
            {
                List<List<Vector3d>> moves = ListListCopy(PS.GetAllMoves(Goals));
                double sum = 0.0; int count = 0;
                foreach (List<Vector3d> move in moves) { sum += sumVec(move); count += move.Count; }
                double ave_move = sum / count;

                List<IGoal> smoothedGoals = new List<IGoal>();

                for (int i = 0; i < Goals.Count; i++)
                {
                    if (moves.Count == Goals.Count)
                    {
                        if (moves[i] != null)
                        {
                            moves[i].Sort((a, b) => a.Length.CompareTo(b.Length)); moves[i].Reverse();
                            double theshold_move = ave_move;
                            double max_move = moves[i][0].Length;
                            if (max_move > theshold_move) { double multi = theshold_move / max_move; multipe_weight(Goals[i], multi); }
                        }
                    }
                    smoothedGoals.Add(Goals[i]);
                }

                PS.Step(smoothedGoals, true, miniIteration);//[should be changed if we are trying to test new stepVGS]
                iteration++;
                return PS.GetOutput(Reciprocal);
            }//BUG Not working
            private double sumVec(List<Vector3d> vecs)
            {
                double sum = 0.0;
                foreach (Vector3d vec in vecs) { sum += vec.Length; }
                return sum;
            }//For SmoothIteration BUG Not working
            private void multipe_weight(IGoal goal, double m)
            {
                if (m <= 0) { return; }
                for (int i = 0; i < goal.Weighting.Length; i++)
                {
                    goal.Weighting[i] *= m;
                }
            }//For SmoothIteration BUG Not working

        }
        /**/
        public class PhysicalSystem_K
        {
            public PhysicalSystem PS;
            public int iteration;
            public string status;
            private List<IGoal> calculate_goals;

            public PhysicalSystem_K() { this.Reset(); }
            public List<Object> pip(bool reset, List<Object> Geometric_constraints, List<IGoal> Reciprocal, double gap)
            {
                if (reset || PS == null) { Reset(); }
                calculate_goals = Setup_Pindex(Geometric_constraints, Reciprocal, 0.00001);

                if (reset) { return PS.GetOutput(Reciprocal); }
                else
                {
                    return NormalIteration(calculate_goals, Reciprocal, gap);
                    //return SmoothIteration(calculate_goals, Reciprocal, gap);
                }
            }
            public List<IGoal> Setup_Pindex(List<Object> Geometric_constraints, List<IGoal> Reciprocal, double tol_samepoint)
            {
                List<IGoal> Goals = new List<IGoal>();

                foreach (IGoal g in Reciprocal)
                {
                    PS.AssignPIndex(g, tol_samepoint);
                    Goals.Add(g);
                }

                foreach (IGoal g in Geometric_constraints)
                {
                    if (g.PPos == null) { Goals.Add(g); continue; }
                    PS.AssignPIndex(g, tol_samepoint);
                    Goals.Add(g);
                }

                return Goals;
            }
            public void Reset()
            { PS = new KangarooSolver.PhysicalSystem(); iteration = 0; status = "Initialized"; }
            public List<Object> NormalIteration(List<IGoal> Goals, List<IGoal> Reciprocal, double threshold)
            {
                PS.Step(Goals, true, threshold);
                iteration++;
                return PS.GetOutput(Reciprocal);
            }
            public List<Object> SmoothIteration(List<IGoal> Goals, List<IGoal> Reciprocal, double threshold)
            {
                List<List<Vector3d>> moves = ListListCopy(PS.GetAllMoves(Goals));
                double sum = 0.0; int count = 0;
                foreach (List<Vector3d> move in moves) { sum += sumVec(move); count += move.Count; }
                double ave_move = sum / count;

                List<IGoal> smoothedGoals = new List<IGoal>();

                for (int i = 0; i < Goals.Count; i++)
                {
                    if (moves.Count == Goals.Count)
                    {
                        if (moves[i] != null)
                        {
                            moves[i].Sort((a, b) => a.Length.CompareTo(b.Length)); moves[i].Reverse();
                            double theshold_move = ave_move;
                            double max_move = moves[i][0].Length;
                            if (max_move > theshold_move) { double multi = theshold_move / max_move; multipe_weight(Goals[i], multi); }
                        }
                    }
                    smoothedGoals.Add(Goals[i]);
                }

                PS.Step(smoothedGoals, true, threshold);
                iteration++;
                return PS.GetOutput(Reciprocal);
            }//BUG Not working
            private double sumVec(List<Vector3d> vecs)
            {
                double sum = 0.0;
                foreach (Vector3d vec in vecs) { sum += vec.Length; }
                return sum;
            }//For SmoothIteration BUG Not working
            private void multipe_weight(IGoal goal, double m)
            {
                if (m <= 0) { return; }
                for (int i = 0; i < goal.Weighting.Length; i++)
                {
                    goal.Weighting[i] *= m;
                }
            }//For SmoothIteration BUG Not working

        }

        public class GS_ClampLength : GoalObject
        {
            public double Upper;
            public double Lower;
            public double Stiffness;
            public bool fix;

            public GS_ClampLength()
            {
            }

            public GS_ClampLength(int S, int E, double U, double L, double k, bool isfixed)
            {
                PIndex = new int[2] { S, E };
                Move = new Vector3d[2];
                Weighting = new double[2];
                Upper = U;
                Lower = L;
                Stiffness = k;
                fix = isfixed;
            }

            public GS_ClampLength(Point3d S, Point3d E, double U, double L, double k, bool isfixed)
            {
                PPos = new Point3d[2] { S, E };
                Move = new Vector3d[2];
                Weighting = new double[2] { k, k };
                Upper = U;
                Lower = L;
                Stiffness = k;
                fix = isfixed;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Vector3d current = p[PIndex[1]].Position - p[PIndex[0]].Position;
                double LengthNow = current.Length;
                if (fix)
                {
                    double stretchfactor = 1.0 - Upper / LengthNow;
                    Vector3d SpringMove = 0.5 * current * stretchfactor;
                    Move[0] = SpringMove;
                    Move[1] = -SpringMove;
                    Weighting[0] = 10 * Stiffness;
                    Weighting[1] = 10 * Stiffness;
                }
                else
                {
                    if (LengthNow > Upper)
                    {
                        double stretchfactor = 1.0 - Upper / LengthNow;
                        Vector3d SpringMove = 0.5 * current * stretchfactor;
                        Move[0] = SpringMove;
                        Move[1] = -SpringMove;
                        Weighting[0] = Stiffness;
                        Weighting[1] = Stiffness;
                    }
                    else if (LengthNow <= Lower)
                    {
                        double stretchfactor = 1.0 - Lower / LengthNow;
                        Vector3d SpringMove = 0.5 * current * stretchfactor;

                        Move[0] = Vector3d.Zero;
                        Move[1] = Vector3d.Zero;

                        Point3d P0_loc = p[PIndex[0]].Position;
                        Point3d P1_loc = p[PIndex[1]].Position;

                        p[PIndex[0]].Position = P1_loc;
                        p[PIndex[1]].Position = P0_loc;
                    }
                    else
                    {
                        Move[0] = Vector3d.Zero;
                        Move[1] = Vector3d.Zero;
                    }
                }
            }

            public override object Output(List<KangarooSolver.Particle> p)
            {
                return new Line(p[PIndex[0]].Position, p[PIndex[1]].Position);
            }
        }
        public class GS_loadpathOPT_test : GoalObject
        {
            private double rate;
            public GS_loadpathOPT_test(double k, Line LA, Line LB, double rate)
            {
                PPos = new Point3d[4] { LA.From, LA.To, LB.From, LB.To };
                Move = new Vector3d[4];
                Weighting = new double[4] { k, k, k, k };
                this.rate = rate;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d[] pts = this.GetCurrentPositions(p);
                Vector3d vA = pts[1] - pts[0];
                Vector3d vB = pts[3] - pts[2];
                double a = vA.Length;
                double b = vB.Length;
                double ab = a * b;

                vA.Unitize();
                vB.Unitize();
                double x = 0.001;
                Move[0] = vA * b * x * rate;
                Move[1] = -Move[0];

                Move[2] = vB * a * x * rate;
                Move[3] = -Move[2];

            }
        }
        public class GS_loadpathOPT : GoalObject
        {
            public GS_loadpathOPT(double k, Line LA, Line LB)
            {
                PPos = new Point3d[4] { LA.From, LA.To, LB.From, LB.To };
                Move = new Vector3d[4];
                Weighting = new double[4] { k, k, k, k };
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d[] pts = this.GetCurrentPositions(p);
                Vector3d vA = pts[1] - pts[0];
                Vector3d vB = pts[3] - pts[2];
                double a = vA.Length;
                double b = vB.Length;

                vA.Unitize();
                vB.Unitize();
                double x = 0.001;
                Move[0] = vA * b * x;
                Move[1] = -Move[0];

                Move[2] = vB * a * x;
                Move[3] = -Move[2];

            }
        }//[0417 LoadPathBackToOld]
        public class GS_Direction : GoalObject
        {
            public Vector3d Dir;
            public double Strength;
            public double initial_len;
            public GS_Direction()
            {
            }

            public GS_Direction(int Start, int End, Vector3d Direction, double K)
            {
                PIndex = new int[2] { Start, End };
                Move = new Vector3d[2];
                Weighting = new double[2] { K, K };
                Dir = Direction;
                Dir.Unitize();
                Strength = K;
            }

            public GS_Direction(Point3d Start, Point3d End, Vector3d Direction, double K)
            {
                PPos = new Point3d[2] { Start, End };
                Move = new Vector3d[2];
                Weighting = new double[2] { K, K };
                Dir = Direction;
                Dir.Unitize();
                Strength = K;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d S = p[PIndex[0]].Position;
                Point3d E = p[PIndex[1]].Position;
                Vector3d V = E - S;
                Vector3d To = (Dir * V.Length - V) * 0.5;

                Move[0] = -To;
                Move[1] = To;

                Weighting[0] = Strength;
                Weighting[1] = Strength;
            }
        }//The stable version
        public class GS_Direction2 : GoalObject
        {
            public Vector3d Dir;
            public double Strength;
            public double initial_len;
            public GS_Direction2()
            {
            }

            public GS_Direction2(int Start, int End, Vector3d Direction, double K)
            {
                PIndex = new int[2] { Start, End };
                Move = new Vector3d[2];
                Weighting = new double[2] { K, K };
                Dir = Direction;
                Dir.Unitize();
                Strength = K;
            }

            public GS_Direction2(Point3d Start, Point3d End, Vector3d Direction, double K)
            {
                PPos = new Point3d[2] { Start, End };
                Move = new Vector3d[2];
                Weighting = new double[2] { K, K };
                Dir = Direction;
                Dir.Unitize();
                Strength = K;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d S = p[PIndex[0]].Position;
                Point3d E = p[PIndex[1]].Position;

                Vector3d vec0 = new Vector3d(E - S);
                Vector3d vec1 = Dir;
                Vector3d vec0T = new Vector3d(vec0); vec0T.Unitize();
                Vector3d vec1T = new Vector3d(vec1); vec1T.Unitize();
                Vector3d dir;
                double Len;
                if ((vec0T + vec1T).Length == 0) { dir = -vec0; Len = vec0.Length + vec1.Length; }

                else
                {
                    double R = (vec0 * vec1) / (vec0.Length * vec0.Length);
                    double a = (vec1.X - vec0.X * R);
                    double b = (vec1.Y - vec0.Y * R);
                    double c = (vec1.Z - vec0.Z * R);
                    Len = Math.Sin(Vector3d.VectorAngle(vec0, vec1)) * vec0.Length;
                    dir = new Vector3d(a, b, c);
                }
                dir.Unitize(); dir *= Len*0.5;

                Move[0] = -dir;
                Move[1] = dir;

                Weighting[0] = Strength;
                Weighting[1] = Strength;
            }
        }//[0519 Update]

        public class K_Angle2_test : GoalObject//@[231122] Back up with K_Angle2_
        {
            public double EI;
            public double RestAngle;
            public double w1;
            public double w2;

            public K_Angle2_test()
            {
            }

            public K_Angle2_test(double Strength, double RA, int P0, int P1, int P2, int P3)
            {
                this.PIndex = new int[4] { P0, P1, P2, P3 };
                this.Move = new Vector3d[4];
                this.Weighting = new double[4];
                this.EI = Strength;
                this.RestAngle = RA;
            }

            public K_Angle2_test(Line L0, Line L1, double RA, double Strength, double w1, double w2)
            {
                this.PPos = new Point3d[4]
                {
        L0.From,
        L0.To,
        L1.From,
        L1.To
                };
                this.Move = new Vector3d[4];
                this.Weighting = new double[4];
                this.EI = Strength;
                this.RestAngle = RA;
                this.w1 = w1;
                this.w2 = w2;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d position1 = p[this.PIndex[0]].Position;
                Point3d position2 = p[this.PIndex[1]].Position;
                Point3d position3 = p[this.PIndex[2]].Position;
                Point3d position4 = p[this.PIndex[3]].Position;
                Vector3d vec12 = position2 - position1;
                Point3d point3d = position3;
                Vector3d vec34 = position4 - point3d;
                double tan12 = Math.Tan((w1 * 2 / (w1 + w2)) * 0.5 * Vector3d.VectorAngle(vec12, vec34));
                double tan34 = Math.Tan((w2 * 2 / (w1 + w2)) * 0.5 * Vector3d.VectorAngle(vec12, vec34));

                Vector3d perp = Vector3d.CrossProduct(vec12, vec34);
                perp.Unitize();
                Vector3d shearF12 = Vector3d.CrossProduct(vec12, perp);
                Vector3d shearF34 = Vector3d.CrossProduct(perp, vec34);

                Vector3d vec12_ = (vec12 - tan12 * shearF12) * 0.5; vec12_.Unitize();
                Vector3d vec34_ = (vec34 - tan34 * shearF34) * 0.5; vec34_.Unitize();

                Point3d Ln0_F = 0.5 * position1 + 0.5 * position2 - vec12_ * vec12.Length * 0.5;
                //Point3d Ln0_T = 0.5 * position1 + 0.5 * position2 + vec12_ * vec12.Length * 0.5;

                Point3d Ln1_F = 0.5 * position3 + 0.5 * position4 - vec34_ * vec34.Length * 0.5;
                //Point3d Ln1_T = 0.5 * position3 + 0.5 * position4 + vec34_ * vec34.Length * 0.5;

                Vector3d Dir0 = Ln0_F - position1;
                Vector3d Dir1 = Ln1_F - position3;
                this.Move[0] = Dir0;
                this.Move[1] = -Dir0;
                this.Move[2] = Dir1;
                this.Move[3] = -Dir1;
                this.Weighting[0] = this.Weighting[1] = 4.0 * this.EI / (vec12.Length * vec12.Length * vec12.Length);
                this.Weighting[2] = this.Weighting[3] = 4.0 * this.EI / (vec34.Length * vec34.Length * vec34.Length);
            }
        }

        public class K_Angle2 : GoalObject
        {
            public double EI;
            public double RestAngle;
            public double w1;
            public double w2;

            public K_Angle2()
            {
            }

            public K_Angle2(double Strength, double RA, int P0, int P1, int P2, int P3)
            {
                this.PIndex = new int[4] { P0, P1, P2, P3 };
                this.Move = new Vector3d[4];
                this.Weighting = new double[4];
                this.EI = Strength;
                this.RestAngle = RA;
            }

            public K_Angle2(Line L0, Line L1, double RA, double Strength,double w1,double w2)
            {
                this.PPos = new Point3d[4]
                {
                L0.From,
                L0.To,
                L1.From,
                L1.To
                };
                this.Move = new Vector3d[4];
                this.Weighting = new double[4];
                this.EI = Strength;
                this.RestAngle = RA;
                this.w1 = w1;
                this.w2 = w2;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d position1 = p[this.PIndex[0]].Position;
                Point3d position2 = p[this.PIndex[1]].Position;
                Point3d position3 = p[this.PIndex[2]].Position;
                Point3d position4 = p[this.PIndex[3]].Position;
                Vector3d vec12 = position2 - position1;
                Point3d point3d = position3;
                Vector3d vec34 = position4 - point3d;
                double top = 2.0 * Math.Sin(Vector3d.VectorAngle(vec12, vec34) - this.RestAngle);
                double length = (vec12 + vec34).Length;
                double scaler12 = vec12.Length * top / length;
                double scaler34 = vec34.Length * top / length;
                Vector3d perp = Vector3d.CrossProduct(vec12, vec34);
                perp.Unitize();
                Vector3d shearF12 = Vector3d.CrossProduct(vec12, perp);
                Vector3d shearF34 = Vector3d.CrossProduct(perp, vec34);
                Vector3d Dir0 = shearF12 * (0.25 * scaler12)*(w1/(w1+w2));
                Vector3d DIr1 = shearF34 * (0.25 * scaler34)*(w2/(w1+w2));
                this.Move[0] = Dir0;
                this.Move[1] = -Dir0;
                this.Move[2] = DIr1;
                this.Move[3] = -DIr1;
                this.Weighting[0] = this.Weighting[1] = 4.0 * this.EI / (vec12.Length * vec12.Length * vec12.Length);
                this.Weighting[2] = this.Weighting[3] = 4.0 * this.EI / (vec34.Length * vec34.Length * vec34.Length);
            }
        }
        public class Single_Angle2 : GoalObject
        {
            public double EI;
            public double RestAngle;

            public Single_Angle2()
            {
            }

            public Single_Angle2(double Strength, double RA, int P0, int P1, int P2, int P3)
            {
                this.PIndex = new int[4] { P0, P1, P2, P3 };
                this.Move = new Vector3d[4];
                this.Weighting = new double[4];
                this.EI = Strength;
                this.RestAngle = RA;
            }

            public Single_Angle2(Line L0, Line L1, double RA, double Strength)//L0 should align to L1（L1 will not tranfrom）
            {
                this.PPos = new Point3d[4]
                {
                L0.From,
                L0.To,
                L1.From,
                L1.To
                };
                this.Move = new Vector3d[4];
                this.Weighting = new double[4];
                this.EI = Strength;
                this.RestAngle = RA;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d position1 = p[this.PIndex[0]].Position;
                Point3d position2 = p[this.PIndex[1]].Position;
                Point3d position3 = p[this.PIndex[2]].Position;
                Point3d position4 = p[this.PIndex[3]].Position;
                Vector3d vec12 = position2 - position1;
                Point3d point3d = position3;
                Vector3d vec34 = position4 - point3d;
                double top = 2.0 * Math.Sin(Vector3d.VectorAngle(vec12, vec34) - this.RestAngle);
                double length = (vec12 + vec34).Length;
                double scaler12 = vec12.Length * top / length;
                double scaler34 = vec34.Length * top / length;
                Vector3d perp = Vector3d.CrossProduct(vec12, vec34);
                perp.Unitize();
                Vector3d shearF12 = Vector3d.CrossProduct(vec12, perp);
                Vector3d shearF34 = Vector3d.CrossProduct(perp, vec34);
                Vector3d Dir0 = shearF12 * (0.25 * scaler12);
                Vector3d DIr1 = shearF34 * (0.25 * scaler34);
                this.Move[0] = Dir0;
                this.Move[1] = -Dir0;
                this.Move[2] = DIr1;
                this.Move[3] = -DIr1;
                this.Weighting[0] = this.Weighting[1] = 4.0 * this.EI / (vec12.Length * vec12.Length * vec12.Length);
                this.Weighting[2] = this.Weighting[3] =0;
            }
        }
        public class GS_Angle2 : GoalObject
        {
            public double Strength;
 
            public GS_Angle2()
            {
            }

            public GS_Angle2(Line ln0, Line ln1, double K)
            {
                PPos = new Point3d[4] { ln0.From, ln0.To, ln1.From, ln1.To };
                Move = new Vector3d[4];
                Weighting = new double[4] { K, K, K, K };
                Strength = K;
            }
            public GS_Angle2(int p0, int p1, int p2, int p3, Vector3d Direction, double K)
            {
                PIndex = new int[4] { p0, p1, p2, p3 };
                Move = new Vector3d[4];
                Weighting = new double[4] { K, K, K, K };
                Strength = K;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d S0 = p[PIndex[0]].Position;
                Point3d E0 = p[PIndex[1]].Position;
                Point3d S1 = p[PIndex[2]].Position;
                Point3d E1 = p[PIndex[3]].Position;

                Vector3d vec0 = new Vector3d(E0- S0);
                Vector3d vec1 = new Vector3d(E1 - S1);

                Vector3d vec0T = new Vector3d(vec0); vec0T.Unitize();
                Vector3d vec1T = new Vector3d(vec1); vec1T.Unitize();

                Vector3d dir0;
                double Len0;
                double Len1;
                if ((vec0T + vec1T).Length == 0) { dir0 =new Vector3d(0,0,0);Len0 = 1.0;}
                else
                {
                    double R = (vec0 * vec1) / (vec0.Length * vec0.Length);
                    double a = (vec1.X - vec0.X * R);
                    double b = (vec1.Y - vec0.Y * R);
                    double c = (vec1.Z - vec0.Z * R);
                    Len0 = vec0.Length * Vector3d.VectorAngle(vec0, vec1) / Math.PI;
                    dir0 = new Vector3d(a, b, c);
                }
                dir0.Unitize(); 

                Move[0] = -dir0* 0.25*Len0;
                Move[1] = dir0* 0.25*Len0;

                Vector3d dir1;
                if ((vec0T + vec1T).Length == 0) { dir1 = new Vector3d(0, 0, 0); Len1 = 1.0; }
                else
                {
                    double R = (vec1 * vec0) / (vec1.Length * vec1.Length);
                    double a = (vec0.X - vec1.X * R);
                    double b = (vec0.Y - vec1.Y * R);
                    double c = (vec0.Z - vec1.Z * R);
                    Len1 = vec1.Length * Vector3d.VectorAngle(vec0, vec1)/Math.PI;
                    dir1 = new Vector3d(a, b, c);
                }
                dir1.Unitize();

                Move[2] = -dir1 * 0.25*Len1;
                Move[3] = dir1 *  0.25 * Len1;

                Weighting[0] = Strength/(Math.Pow(vec0.Length,3));
                Weighting[1] = Strength/(Math.Pow(vec0.Length, 3));
                Weighting[2] = Strength/(Math.Pow(vec1.Length, 3));
                Weighting[3] = Strength/(Math.Pow(vec1.Length, 3));
            }
        }//[0519 Update Stable version: Angle2 (In lib)]
        public class Angle_O : GoalObject
        {
            public double EI;
            public double RestAngle;

            public Angle_O()
            {
            }

            /// <summary>
            /// Construct a new Angle goal by particle index.
            /// </summary>
            /// <param name="Strength">Strength of this goal.</param>
            /// <param name="RA">Rest Angle.</param>
            /// <param name="P0">Start of the first line segment.</param>
            /// <param name="P1">End of the first line segment. This can be identical to P2 if the line segments are connected.</param>
            /// <param name="P2">Start of the second line segment. This can be identical to P1 if the line segments are connected.</param>
            /// <param name="P3">End of the second line segment.</param>
            public Angle_O(double Strength, double RA, int P0, int P1, int P2, int P3)
            {
                PIndex = new int[4] { P0, P1, P2, P3 };
                Move = new Vector3d[4];
                Weighting = new double[4];
                EI = Strength;
                RestAngle = RA;
            }

            public Angle_O(Line L0, Line L1, double RA, double Strength)
            {
                PPos = new Point3d[4] { L0.From, L0.To, L1.From, L1.To };
                Move = new Vector3d[4];
                Weighting = new double[4];
                EI = Strength;
                RestAngle = RA;
            }

            public override void Calculate(List<KangarooSolver.Particle> p)
            {
                Point3d P0 = p[PIndex[0]].Position;
                Point3d P1 = p[PIndex[1]].Position;
                Point3d P2 = p[PIndex[2]].Position;
                Point3d P3 = p[PIndex[3]].Position;

                Vector3d V01 = P1 - P0;
                Vector3d V23 = P3 - P2;
                double top = 2 * Math.Sin(Vector3d.VectorAngle(V01, V23) - RestAngle);
                double Lc = (V01 + V23).Length;
                double Sa = top / (V01.Length * Lc);
                double Sb = top / (V23.Length * Lc);

                Vector3d Perp = Vector3d.CrossProduct(V01, V23);
                Vector3d ShearA = Vector3d.CrossProduct(V01, Perp);
                Vector3d ShearB = Vector3d.CrossProduct(Perp, V23);

                ShearA.Unitize();
                ShearB.Unitize();

                ShearA *= Sa;
                ShearB *= Sb;

                Move[0] = ShearA;
                Move[1] = -ShearA;
                Move[2] = ShearB;
                Move[3] = -ShearB;

                Weighting[0] = EI;
                Weighting[1] = EI;
                Weighting[2] = EI;
                Weighting[3] = EI;
            }
        }//Daniel Piker
        private static List<Point3d> Colinear_Pts(Line ln)
        {
            Point3d P0 = ln.From;
            Point3d P1 = ln.To;
            Vector3d V01 = new Vector3d(P1 - P0); V01.Unitize();
            Vector3d V10 = new Vector3d(P0 - P1); V10.Unitize();
            Point3d P2 = ln.From + V10 * ln.Length * 30;
            Point3d P3 = ln.To + V01 * ln.Length * 30;
            return new List<Point3d>() { P0, P1, P2, P3 };
        }
    }
}
