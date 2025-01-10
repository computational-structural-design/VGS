
using System.Collections.Generic;
using Rhino.Geometry;

namespace VGS_Main
{

    public class Edge_line
    {
        public List<Line> lines;
        public List<double> forces;

        public Edge_line()
        {
            lines = new List<Line>();
            forces = new List<double>();
        }

        public void AddData(List<Line> line_set, List<double> force_set)
        {
            lines.AddRange(line_set);
            forces.AddRange(force_set);
        }

        public override string ToString()
        {
            if (lines.Count == forces.Count && lines.Count == 0) { return "[Edges] No element"; }
            else if (lines.Count > 0 && forces.Count == 0) { return string.Format("[Edges] Pure Lines [{0}]", lines.Count.ToString()); }
            else if (lines.Count == forces.Count) { return string.Format("[Edges] Forced Lines [{0}]", lines.Count.ToString()); }
            else { return "[Edges]:error"; }
        }
    }
    public class Load_line : VgsCommon
    {
        public List<Line> lines;
        public string report;

        public Load_line()
        {
            lines = new List<Line>();
            report = "[Load]";
        }
        public Load_line(List<Line> lns, List<Point3d> pts, List<Vector3d> vec)
        {
            if (lns.Count > 0 && pts.Count == 0 && vec.Count == 0) { lines = lns; report = string.Format("[Load] LineLoads {0}", lines.Count.ToString()); }
            else if (lns.Count == 0 && pts.Count == vec.Count && pts.Count > 0)
            {
                List<Line> temp = new List<Line>();
                for (int i = 0; i < pts.Count; i++) { temp.Add(new Line(pts[i], new Point3d(pts[i] + vec[i]))); }
                lines = temp;
                report = string.Format("[Load] VecLoads {0}", lines.Count.ToString());
            }
            else if (lns.Count == pts.Count && lns.Count > 0)
            {
                List<Line> temp = new List<Line>();
                for (int i = 0; i < lns.Count; i++)
                {
                    if (ComparePts(lns[i].To, pts[i], System_Configuration.Sys_Tor)) { temp.Add(new Line(lns[i].To, lns[i].From)); }
                    else { temp.Add(lns[i]); }
                }
                lines = temp;
                report = string.Format("[Load] LineLoads {0}", lines.Count.ToString());
            }
            else if (lns.Count == 0 && vec.Count == 1 && pts.Count > 0)
            {
                List<Line> temp = new List<Line>();
                for (int i = 0; i < pts.Count; i++) { temp.Add(new Line(pts[i], new Point3d(pts[i] + vec[0]))); }
                lines = temp;
                report = string.Format("[Load] VecLoads {0}", lines.Count.ToString());
            }
            else
            {
                lines = new List<Line>();
                report = "[Load] None";
            }
        }

        public void AddData(List<Line> line_set)
        { lines.AddRange(line_set); }

        public override string ToString()
        {
            return report;
        }
    }
    public class Support_point
    {
        public List<Point3d> locate;
        public List<string> status;

        public Support_point()
        {
            locate = new List<Point3d>();
            status = new List<string>();
        }
        public Support_point(List<Point3d> pts, string str)
        {
            locate = pts;
            status = new List<string>();
            foreach (Point3d pt in pts)
            { status.Add(str); }
        }

        public void AddData(List<Point3d> pts, List<string> strs)
        {
            locate.AddRange(pts);
            status.AddRange(strs);
        }
        public override string ToString()
        {
            if (locate.Count == status.Count && locate.Count > 0)
            { return string.Format("[Supports] {0}", locate.Count.ToString()); }
            else { return "invalid setting"; }
        }
    }
    public class SelfStress_index
    {
        public List<int> index;
        public List<double> forces;

        public SelfStress_index()
        {
            index = new List<int>();
            forces = new List<double>();
        }
        public SelfStress_index(List<int> ind, List<double> force)
        {

            if (ind.Count > 1 && force.Count == 1)
            {
                for (int i = 0; i < ind.Count - 1; i++)
                {
                    force.Add(force[0]);
                }
            }
            index = ind;
            forces = force;
        }

        public override string ToString()
        {
            if (index.Count == forces.Count && index.Count > 0) { return string.Format("[SelfStress] {0}", index.Count.ToString()); }
            else { return "invalid setting"; }
        }

        public void AddData(List<int> ind, List<double> fors)
        {
            index.AddRange(ind);
            forces.AddRange(fors);
        }
    }

}
