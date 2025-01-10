using System.Collections.Generic;
using Rhino.Geometry;
using System.Drawing;

namespace VGS_Main
{

    public class NODE : VgsCommon
    {
        public string ID;
        public List<string> adj_Node_ID;
        public List<string> adj_Edge_ID;
        public Color col;
        public Point3d pt;
        public NODE()
        { }
        public NODE(string id, List<string> adj_node_id, List<string> adj_edge_id, Color color, Point3d poi)
        {
            this.ID = id;
            this.adj_Node_ID = adj_node_id;
            this.adj_Edge_ID = adj_edge_id;
            this.col = color;
            this.pt = poi;
        }
        public NODE CopySelf()
        {
            NODE temp = new NODE();
            temp.ID = this.ID;
            temp.adj_Edge_ID = CopyStringList(this.adj_Edge_ID);
            temp.adj_Node_ID = CopyStringList(this.adj_Node_ID);
            temp.col = this.col;
            temp.pt = this.pt;
            return temp;
        }
    }
    public class EDGE : VgsCommon
    {
        public string ID;
        public double force;
        public Color col;
        public Line ln;
        public EDGE()
        { }
        public EDGE(string id, double db_force, Color color, Line line)
        {
            this.ID = id;
            this.force = db_force;
            this.col = color;
            this.ln = line;
        }
        public EDGE CopySelf()
        {
            EDGE temp = new EDGE();
            temp.ID = this.ID;
            temp.force = this.force;
            temp.col = this.col;
            temp.ln = this.ln;
            return temp;
        }
    }
    public class LOAD : VgsCommon
    {
        public string ID;
        public Color col;
        public Line ln;
        public string ActionPt_ID;

        public LOAD()
        { }
        public LOAD(string id, Color color, Line line, string actPt_id)
        {
            this.ID = id;
            this.col = color;
            this.ln = line;
            this.ActionPt_ID = actPt_id;
        }

        public LOAD CopySelf()
        {
            LOAD temp = new LOAD();
            temp.ID = this.ID;
            temp.col = this.col;
            temp.ln = this.ln;
            temp.ActionPt_ID = this.ActionPt_ID;
            return temp;
        }
    }
    public class REACT
    {
        public string ID;
        public string XYZ;
        public Line ln;
        public Color col;
        public string ActionPt_ID;
        public REACT() { }
        public REACT(string id, string xyz, Line line, Color color, string actPt_id)
        {
            this.ID = id;
            this.XYZ = xyz;
            this.ln = line;
            this.col = color;
            this.ActionPt_ID = actPt_id;
        }

        public REACT CopySelf()
        {
            return new REACT(this.ID, this.XYZ, this.ln, this.col, this.ActionPt_ID);
        }
    }
    public class RESULT
    {
        public string ID;
        public Color col;
        public Line ln;
        public RESULT() { }
        public RESULT(string id, Color color, Line line)
        {
            this.ID = id;
            this.col = color;
            this.ln = line;
        }
        public RESULT CopySelf()
        {
            return new RESULT(this.ID, this.col, this.ln);
        }
    }

}
