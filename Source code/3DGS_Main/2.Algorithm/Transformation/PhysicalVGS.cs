using Grasshopper.Kernel.Types;
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
    #region VGS physical[221115]
    // new PhysicalSystem inherited from kangaroo2
    public class PhysicalVGS
    {

        private List<KangarooSolver.Particle> m_particles;

        private Stopwatch m_stopwatch;

        private double vSum;

        private int Iterations;

        public PhysicalVGS()
        {
            m_particles = new List<KangarooSolver.Particle>();
            m_stopwatch = new Stopwatch();
            vSum = 0.0;
            Iterations = 0;
        }

        public void AddParticle(Point3d p, double m)
        {
            m_particles.Add(new KangarooSolver.Particle(p, m));
        }

        public void DeleteParticle(int i)
        {
            m_particles.RemoveAt(i);
        }

        public void SetParticleList(List<Point3d> p)
        {
            ClearParticles();
            foreach (Point3d item in p)
            {
                m_particles.Add(new KangarooSolver.Particle(item, 1.0));
            }
        }

        public void Restart()
        {
            foreach (KangarooSolver.Particle particle in m_particles)
            {
                particle.Position = particle.StartPosition;
                particle.Velocity = Vector3d.Zero;
                particle.Orientation = particle.StartOrientation;
                particle.AngularVelocity = Vector3d.Zero;
            }

            Iterations = 0;
        }

        public void ClearParticles()
        {
            m_particles.Clear();
            Iterations = 0;
        }

        public int ParticleCount()
        {
            return m_particles.Count;
        }

        public double GetvSum()
        {
            return vSum;
        }

        public int GetIterations()
        {
            return Iterations;
        }

        public Point3d GetPosition(int index)
        {
            return m_particles[index].Position;
        }

        public IEnumerable<Point3d> GetPositions()
        {
            for (int i = 0; i < ParticleCount(); i++)
            {
                yield return m_particles[i].Position;
            }
        }

        public Point3d[] GetPositionArray()
        {
            Point3d[] array = new Point3d[ParticleCount()];
            for (int i = 0; i < ParticleCount(); i++)
            {
                array[i] = m_particles[i].Position;
            }

            return array;
        }

        public Point3d[] GetSomePositions(int[] indexes)
        {
            Point3d[] array = new Point3d[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                array[i] = m_particles[indexes[i]].Position;
            }

            return array;
        }

        public IEnumerable<GH_Point> GetPositionsGH()
        {
            for (int i = 0; i < ParticleCount(); i++)
            {
                yield return new GH_Point(m_particles[i].Position);
            }
        }

        public int FindParticleIndex(Point3d Pos, double tol, bool ByCurrent)
        {
            if (ByCurrent)
            {
                return m_particles.FindIndex((KangarooSolver.Particle x) => Util.OrthoClose(x.Position, Pos, tol));
            }

            return m_particles.FindIndex((KangarooSolver.Particle x) => Util.OrthoClose(x.StartPosition, Pos, tol));
        }

        public int FindOrientedParticleIndex(Plane P, double tol, bool ByCurrent)
        {
            return m_particles.FindIndex((KangarooSolver.Particle x) => Util.OrthoClose(x.StartPosition, P.Origin, tol) && x.StartOrientation.XAxis.IsParallelTo(P.XAxis) == 1 && x.StartOrientation.YAxis.IsParallelTo(P.YAxis) == 1);
        }

        public void AssignPIndex(IGoal Goal, double Tolerance)
        {
            AssignPIndex(Goal, Tolerance, ByCurrent: false);
        }

        public void AssignPIndex(IGoal Goal, double Tolerance, bool ByCurrent)
        {
            Goal.PIndex = new int[Goal.PPos.Length];
            for (int i = 0; i < Goal.PPos.Length; i++)
            {
                int num = FindParticleIndex(Goal.PPos[i], Tolerance, ByCurrent);
                if (num == -1)
                {
                    AddParticle(Goal.PPos[i], 1.0);
                    Goal.PIndex[i] = ParticleCount() - 1;
                }
                else
                {
                    Goal.PIndex[i] = num;
                }

                if (Goal.InitialOrientation == null || !Goal.InitialOrientation[i].IsValid)
                {
                    continue;
                }

                _ = m_particles[Goal.PIndex[i]].Orientation;
                if (!m_particles[Goal.PIndex[i]].Orientation.IsValid)
                {
                    m_particles[Goal.PIndex[i]].Orientation = Goal.InitialOrientation[i];
                    m_particles[Goal.PIndex[i]].StartOrientation = Goal.InitialOrientation[i];
                    continue;
                }

                num = FindOrientedParticleIndex(Goal.InitialOrientation[i], Tolerance, ByCurrent);
                if (num == -1)
                {
                    AddParticle(Goal.PPos[i], 1.0);
                    Goal.PIndex[i] = ParticleCount() - 1;
                    m_particles[Goal.PIndex[i]].Orientation = Goal.InitialOrientation[i];
                    m_particles[Goal.PIndex[i]].StartOrientation = Goal.InitialOrientation[i];
                }
                else
                {
                    Goal.PIndex[i] = num;
                }
            }
        }

        public List<object> GetOutput(List<IGoal> goals)
        {
            List<object> list = new List<object>();
            foreach (IGoal goal in goals)
            {
                list.Add(goal.Output(m_particles));
            }

            return list;
        }

        public void Step(List<IGoal> goals, bool parallel, int miniIteration)
        {
            if (parallel)
            {
                bool flag = false;
                m_stopwatch.Restart();
                while (!flag)
                {
                    for (int i = 0; i < miniIteration; i++)//changed i
                    {
                        foreach (KangarooSolver.Particle particle in m_particles)
                        {
                            particle.Position += particle.Velocity;
                            if (particle.Orientation.IsValid)
                            {
                                particle.Orientation.Origin = particle.Position;
                                particle.Orientation.Rotate(particle.AngularVelocity.Length, particle.AngularVelocity);
                            }
                        }

                        Parallel.ForEach(goals, delegate (IGoal C)
                        {
                            C.Calculate(m_particles);
                        });
                        foreach (IGoal goal in goals)
                        {
                            for (int j = 0; j < goal.PIndex.Length; j++)
                            {
                                    m_particles[goal.PIndex[j]].MoveSum += goal.Move[j] * goal.Weighting[j];
                                    m_particles[goal.PIndex[j]].WeightSum += goal.Weighting[j];
                                
                                if (goal.Torque != null)
                                {
                                    m_particles[goal.PIndex[j]].TorqueSum += goal.Torque[j] * goal.TorqueWeighting[j];
                                    m_particles[goal.PIndex[j]].TorqueWeightSum += goal.TorqueWeighting[j];
                                }
                            }
                        }

                        foreach (KangarooSolver.Particle particle2 in m_particles)
                        {
                            if (!particle2.MoveSum.IsZero)
                            {
                                Vector3d vector3d = particle2.MoveSum / particle2.WeightSum;

                                particle2.Position += vector3d;
                                particle2.Velocity += vector3d;
                                if (vector3d * particle2.Velocity < 0.0)
                                {
                                    particle2.Velocity *= 0.9;
                                }

                                if (particle2.Orientation.IsValid)
                                {
                                    particle2.Orientation.Origin = particle2.Position;
                                }
                            }
                            else
                            {
                                particle2.Velocity = Vector3d.Zero;
                            }

                            if (!particle2.TorqueSum.IsZero)
                            {
                                Vector3d vector3d2 = particle2.TorqueSum / particle2.TorqueWeightSum;
                                
                                particle2.Orientation.Origin = particle2.Position;
                                particle2.Orientation.Rotate(vector3d2.Length, vector3d2);
                                particle2.AngularVelocity += vector3d2;
                                if (vector3d2 * particle2.AngularVelocity < 0.0)
                                {
                                    particle2.AngularVelocity *= 0.9;
                                }
                            }
                            else
                            {
                                particle2.AngularVelocity = Vector3d.Zero;
                            }

                            particle2.ClearForces();
                        }

                        Iterations++;
                    }

                    vSum = 0.0;
                    foreach (KangarooSolver.Particle particle3 in m_particles)
                    {
                        vSum += particle3.Velocity.SquareLength;
                    }

                    vSum /= ParticleCount();
                    if (m_stopwatch.ElapsedMilliseconds > 15 || vSum < 0.00000000001)
                    {
                        flag = true;
                    }
                }

                return;
            }

            bool flag2 = false;
            m_stopwatch.Restart();
            while (!flag2)
            {
                for (int k = 0; k < miniIteration; k++)
                {
                    foreach (KangarooSolver.Particle particle4 in m_particles)
                    {
                        particle4.Position += particle4.Velocity;
                    }

                    foreach (IGoal goal2 in goals)
                    {
                        goal2.Calculate(m_particles);
                    }

                    foreach (IGoal goal3 in goals)
                    {
                        for (int l = 0; l < goal3.PIndex.Length; l++)
                        {
                            m_particles[goal3.PIndex[l]].MoveSum += goal3.Move[l] * goal3.Weighting[l];
                            m_particles[goal3.PIndex[l]].WeightSum += goal3.Weighting[l];
                        }
                    }

                    foreach (KangarooSolver.Particle particle5 in m_particles)
                    {
                        if (!particle5.MoveSum.IsZero)
                        {
                            Vector3d vector3d3 = particle5.MoveSum / particle5.WeightSum;
                            particle5.Position += vector3d3;
                            particle5.Velocity += vector3d3;
                            if (vector3d3 * particle5.Velocity < 0.0)
                            {
                                particle5.Velocity *= 0.9;
                            }
                        }
                        else
                        {
                            particle5.Velocity = Vector3d.Zero;
                        }

                        particle5.ClearForces();
                    }

                    Iterations += 10;
                }

                vSum = 0.0;
                foreach (KangarooSolver.Particle particle6 in m_particles)
                {
                    vSum += particle6.Velocity.SquareLength;
                }

                vSum /= ParticleCount();
                if (m_stopwatch.ElapsedMilliseconds > 15 || vSum < 0.00000000001)
                {
                    flag2 = true;
                }
            }
        }

        public void SimpleStep(List<IGoal> goals, bool momentum)
        {
            foreach (KangarooSolver.Particle particle in m_particles)
            {
                if (momentum)
                {
                    particle.Position += particle.Velocity;
                }

                if (particle.Orientation.IsValid)
                {
                    if (momentum)
                    {
                        particle.Orientation.Rotate(particle.AngularVelocity.Length, particle.AngularVelocity);
                    }

                    particle.Orientation.Origin = particle.Position;
                }
            }

            Parallel.ForEach(goals, delegate (IGoal C)
            {
                C.Calculate(m_particles);
            });
            foreach (IGoal goal in goals)
            {
                for (int i = 0; i < goal.PIndex.Length; i++)
                {
                    m_particles[goal.PIndex[i]].MoveSum += goal.Move[i] * goal.Weighting[i];
                    m_particles[goal.PIndex[i]].WeightSum += goal.Weighting[i];
                    if (goal.Torque != null)
                    {
                        m_particles[goal.PIndex[i]].TorqueSum += goal.Torque[i] * goal.TorqueWeighting[i];
                        m_particles[goal.PIndex[i]].TorqueWeightSum += goal.TorqueWeighting[i];
                    }
                }
            }

            foreach (KangarooSolver.Particle particle2 in m_particles)
            {
                if (!particle2.MoveSum.IsZero)
                {
                    Vector3d vector3d = particle2.MoveSum / particle2.WeightSum;
                    particle2.Position += vector3d;
                    if (momentum)
                    {
                        particle2.Velocity += vector3d;
                        if (vector3d * particle2.Velocity < 0.0)
                        {
                            particle2.Velocity *= 0.9;
                        }
                    }
                }
                else
                {
                    particle2.Velocity = Vector3d.Zero;
                }

                if (!particle2.TorqueSum.IsZero)
                {
                    Vector3d vector3d2 = particle2.TorqueSum / particle2.TorqueWeightSum;
                    particle2.Orientation.Rotate(vector3d2.Length, vector3d2);
                    particle2.Orientation.Origin = particle2.Position;
                    if (momentum)
                    {
                        particle2.AngularVelocity += vector3d2;
                        if (vector3d2 * particle2.AngularVelocity < 0.0)
                        {
                            particle2.AngularVelocity *= 0.9;
                        }
                    }
                }
                else
                {
                    particle2.AngularVelocity = Vector3d.Zero;
                }

                particle2.ClearForces();
            }

            Iterations++;
            if (!momentum)
            {
                return;
            }

            vSum = 0.0;
            foreach (KangarooSolver.Particle particle3 in m_particles)
            {
                vSum += particle3.Velocity.SquareLength;
            }

            vSum /= ParticleCount();
        }

        public void SimpleStep(List<IGoal> goals)
        {
            SimpleStep(goals, momentum: true);
        }

        public void SoftHardStep(List<IGoal> SoftGoals, List<IGoal> HardGoals, int HardIterations, double SoftMultiplier)
        {
            foreach (KangarooSolver.Particle particle in m_particles)
            {
                particle.Position += particle.Velocity;
                if (particle.Orientation.IsValid)
                {
                    particle.Orientation.Rotate(particle.AngularVelocity.Length, particle.AngularVelocity);
                    particle.Orientation.Origin = particle.Position;
                }
            }

            Parallel.ForEach(SoftGoals, delegate (IGoal C)
            {
                C.Calculate(m_particles);
            });
            foreach (IGoal SoftGoal in SoftGoals)
            {
                for (int i = 0; i < SoftGoal.PIndex.Length; i++)
                {
                    m_particles[SoftGoal.PIndex[i]].MoveSum += SoftGoal.Move[i] * SoftGoal.Weighting[i];
                    m_particles[SoftGoal.PIndex[i]].WeightSum += SoftGoal.Weighting[i];
                    if (SoftGoal.Torque != null)
                    {
                        m_particles[SoftGoal.PIndex[i]].TorqueSum += SoftGoal.Torque[i] * SoftGoal.TorqueWeighting[i];
                        m_particles[SoftGoal.PIndex[i]].TorqueWeightSum += SoftGoal.TorqueWeighting[i];
                    }
                }
            }

            foreach (KangarooSolver.Particle particle2 in m_particles)
            {
                Vector3d vector3d = particle2.MoveSum / particle2.WeightSum;
                vector3d *= SoftMultiplier;
                particle2.Position += vector3d;
                particle2.Velocity += vector3d;
                if (vector3d * particle2.Velocity < 0.0)
                {
                    particle2.Velocity *= 0.9;
                }

                Vector3d vector3d2 = particle2.TorqueSum / particle2.TorqueWeightSum;
                vector3d2 *= SoftMultiplier;
                particle2.Orientation.Rotate(vector3d2.Length, vector3d2);
                particle2.Orientation.Origin = particle2.Position;
                particle2.AngularVelocity += vector3d2;
                if (vector3d2 * particle2.AngularVelocity < 0.0)
                {
                    particle2.AngularVelocity *= 0.9;
                }

                particle2.ClearForces();
            }

            for (int j = 0; j < HardIterations; j++)
            {
                Parallel.ForEach(HardGoals, delegate (IGoal C)
                {
                    C.Calculate(m_particles);
                });
                foreach (IGoal HardGoal in HardGoals)
                {
                    for (int k = 0; k < HardGoal.PIndex.Length; k++)
                    {
                        m_particles[HardGoal.PIndex[k]].MoveSum += HardGoal.Move[k] * HardGoal.Weighting[k];
                        m_particles[HardGoal.PIndex[k]].WeightSum += HardGoal.Weighting[k];
                        if (HardGoal.Torque != null)
                        {
                            m_particles[HardGoal.PIndex[k]].TorqueSum += HardGoal.Torque[k] * HardGoal.TorqueWeighting[k];
                            m_particles[HardGoal.PIndex[k]].TorqueWeightSum += HardGoal.TorqueWeighting[k];
                        }
                    }
                }

                foreach (KangarooSolver.Particle particle3 in m_particles)
                {
                    if (particle3.WeightSum != 0.0)
                    {
                        Vector3d vector3d3 = particle3.MoveSum / particle3.WeightSum;
                        particle3.Position += vector3d3;
                        particle3.Velocity += vector3d3;
                        Vector3d vector3d4 = particle3.TorqueSum / particle3.TorqueWeightSum;
                        particle3.Orientation.Rotate(vector3d4.Length, vector3d4);
                        particle3.Orientation.Origin = particle3.Position;
                        particle3.AngularVelocity += vector3d4;
                        particle3.ClearForces();
                    }
                }
            }

            Iterations++;
            vSum = 0.0;
            foreach (KangarooSolver.Particle particle4 in m_particles)
            {
                vSum += particle4.Velocity.SquareLength;
            }

            vSum /= ParticleCount();
        }

        public void MomentumStep(List<IGoal> goals, double damping, int Iters)
        {
            for (int i = 0; i < Iters; i++)
            {
                foreach (KangarooSolver.Particle particle in m_particles)
                {
                    particle.Position += particle.Velocity;
                    if (particle.Orientation.IsValid)
                    {
                        particle.Orientation.Rotate(particle.AngularVelocity.Length, particle.AngularVelocity);
                        particle.Orientation.Origin = particle.Position;
                    }
                }

                Parallel.ForEach(goals, delegate (IGoal C)
                {
                    C.Calculate(m_particles);
                });
                foreach (IGoal goal in goals)
                {
                    for (int j = 0; j < goal.PIndex.Length; j++)
                    {
                        m_particles[goal.PIndex[j]].MoveSum += goal.Move[j] * goal.Weighting[j];
                        m_particles[goal.PIndex[j]].WeightSum += goal.Weighting[j];
                        if (goal.Torque != null)
                        {
                            m_particles[goal.PIndex[j]].TorqueSum += goal.Torque[j] * goal.TorqueWeighting[j];
                            m_particles[goal.PIndex[j]].TorqueWeightSum += goal.TorqueWeighting[j];
                        }
                    }
                }

                foreach (KangarooSolver.Particle particle2 in m_particles)
                {
                    if (!particle2.MoveSum.IsZero)
                    {
                        Vector3d vector3d = particle2.MoveSum / (particle2.WeightSum + particle2.Mass);
                        particle2.Position += vector3d;
                        particle2.Velocity += vector3d;
                    }

                    particle2.Velocity *= damping;
                    if (!particle2.TorqueSum.IsZero)
                    {
                        Vector3d vector3d2 = particle2.TorqueSum / (particle2.TorqueWeightSum + particle2.Mass);
                        particle2.Orientation.Rotate(vector3d2.Length, vector3d2);
                        particle2.Orientation.Origin = particle2.Position;
                        particle2.AngularVelocity += vector3d2;
                    }

                    particle2.AngularVelocity *= damping;
                    particle2.ClearForces();
                }
            }

            Iterations += Iters;
            vSum = 0.0;
            foreach (KangarooSolver.Particle particle3 in m_particles)
            {
                vSum += particle3.Velocity.SquareLength;
            }

            vSum /= ParticleCount();
        }

        public List<List<Vector3d>> GetAllMoves(List<IGoal> goals)
        {
            Parallel.ForEach(goals, delegate (IGoal C)
            {
                C.Calculate(m_particles);
            });
            List<List<Vector3d>> list = new List<List<Vector3d>>();
            foreach (KangarooSolver.Particle particle in m_particles)
            {
                _ = particle;
                List<Vector3d> item = new List<Vector3d>();
                list.Add(item);
            }

            foreach (IGoal goal in goals)
            {
                for (int i = 0; i < goal.PIndex.Length; i++)
                {
                    list[goal.PIndex[i]].Add(goal.Move[i]);
                }
            }

            return list;
        }

        public List<List<double>> GetAllWeightings(List<IGoal> goals)
        {
            Parallel.ForEach(goals, delegate (IGoal C)
            {
                C.Calculate(m_particles);
            });
            List<List<double>> list = new List<List<double>>();
            foreach (KangarooSolver.Particle particle in m_particles)
            {
                _ = particle;
                List<double> item = new List<double>();
                list.Add(item);
            }

            foreach (IGoal goal in goals)
            {
                for (int i = 0; i < goal.PIndex.Length; i++)
                {
                    list[goal.PIndex[i]].Add(goal.Weighting[i]);
                }
            }

            return list;
        }
    }


    #endregion

}
