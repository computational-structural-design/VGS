using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGS_Main
{
    //Shortest path Graph
    public class ShortPathGraph
    {

        /// <summary>
        /// The JointMatrix is not a square format:
        /// 1 4 3 5
        /// 0 2
        /// 1
        /// 4 0 5 6
        /// </summary>
        public List<List<int>> JointMetrix = new List<List<int>>();
        public List<List<double>> Edge_Values = new List<List<double>>();

        public List<List<int>> shortestPath;
        public List<double> shortestLength;

        public void ClearSelf()
        {
            this.JointMetrix = new List<List<int>>();
            this.Edge_Values = new List<List<double>>();
            this.shortestPath = new List<List<int>>();
            this.shortestLength = new List<double>();
        }

        public void NoneRights()//Graph with no weights
        {
            for (int i = 0; i < this.JointMetrix.Count; i++)//initialize the entire matrix
            {
                List<double> list = new List<double>();
                for (int j = 0; j < this.JointMetrix.Count; j++)
                { list.Add(double.MaxValue); }
                this.Edge_Values.Add(list);
            }
            foreach (List<double> sublist in this.Edge_Values)//set equal length at the adjacent positions
            {
                foreach (int id in this.JointMetrix[this.Edge_Values.IndexOf(sublist)]) { sublist[id] = 1.0; }
            }
        }

        public List<int> Dijkstra(int start, int end)
        {

            List<int> Soloved = new List<int>();
            List<int> Unsoloved = new List<int>();
            for (int i = 0; i < this.JointMetrix.Count; i++) { Unsoloved.Add(i); }//Can not find the extreme point
            Soloved.Add(start);
            Unsoloved.Remove(start);

            this.shortestLength = new List<double>(new double[JointMetrix.Count]);
            this.shortestLength[start] = 0.0;
            foreach (int i in Unsoloved) { this.shortestLength[i] = double.MaxValue; }

            this.shortestPath = new List<List<int>>();
            foreach (double i in this.shortestLength)
            { shortestPath.Add(new List<int>()); }

            int Sub = start;
            this.shortestPath[Sub].Add(start);

            while (Unsoloved.Count > 0)
            {
                List<int> checklist = new List<int>();
                foreach (int i in this.JointMetrix[Sub]) { if (Unsoloved.Contains(i)) { checklist.Add(i); } }//Find the point that is adjacent to the sub point and has not obtained the shortest path
                foreach (int i in checklist)//Update the shortest path information of adjacent points of all target points
                {
                    if ((this.shortestLength[Sub] + this.Edge_Values[Sub][i]) < this.shortestLength[i])//If the step size of adjacent points (starting point step size plus connection length) is smaller than the step size of adjacent points (the path is better)
                    {
                        this.shortestLength[i] = this.shortestLength[Sub] + this.Edge_Values[Sub][i];//Update the step size of adjacent points
                        this.shortestPath[i] = CopyintList(this.shortestPath[Sub]); this.shortestPath[i].Add(i);//Update the shortest path of adjacent points
                    }
                }

                List<int> Findmin = CopyintList(Unsoloved);
                Findmin.Sort((p1, p2) => this.shortestLength[p1].CompareTo(this.shortestLength[p2]));
                int shortestID = Findmin[0];//Find the point with the shortest step in unsolved as the next starting point, and determine the shortest path of the point

                //Find the next point
                Soloved.Add(shortestID);
                Unsoloved.Remove(shortestID);
                Sub = shortestID;
            }
            return this.shortestPath[end];
        }

        //DeepCopyInt
        public List<int> CopyintList(List<int> list)
        {
            List<int> newlist = new List<int>();
            foreach (int i in list) { newlist.Add(i); }
            return newlist;
        }
    }
}
