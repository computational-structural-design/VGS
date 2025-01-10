using System.Collections.Generic;
using Grasshopper.Kernel.Data;
using Grasshopper;


namespace VGS_Main
{

    public class FcDiagramAssembleData : VgsCommon
    {
        public string error = "";
        public DataTree<string> nN;
        public DataTree<string> st2d_s;
        public List<string> st1_v;
        public List<string> st1_c;
        public List<string> st1_e;
        public List<string> ve_list;

        public override string ToString()
        {
            return error;
        }
        public FcDiagramAssembleData(DataTree<string> Input_st2d_s, List<string> Input_st1_v, List<string> Input_st1_c, List<string> Input_st1_e, DataTree<string> Input_nN, List<string> ve_list_input)
        {
            this.st1_v = CopyStringList(Input_st1_v);
            this.st1_c = CopyStringList(Input_st1_c);
            this.st1_e = CopyStringList(Input_st1_e);
            this.nN = CopyDataTreeString_nN(Input_nN);
            this.st2d_s = CopyDataTreeString(Input_st2d_s);
            this.ve_list = new List<string>(ve_list_input);
        }
        public FcDiagramAssembleData()
        {
            nN = new DataTree<string>();
            st2d_s = new DataTree<string>();
            st1_v = new List<string>();
            st1_c = new List<string>();
            st1_e = new List<string>();
            this.ve_list = new List<string>();
        }
        public FcDiagramAssembleData CopySelf()
        {
            FcDiagramAssembleData temp = new FcDiagramAssembleData(this.st2d_s, this.st1_v, this.st1_c, this.st1_e, this.nN, this.ve_list);
            return temp;
        }
        private DataTree<string> CopyDataTreeString(DataTree<string> tree)
        {
            DataTree<string> temp = new DataTree<string>();
            for (int i = 0; i < tree.BranchCount; i++)
            {
                temp.AddRange(tree.Branch(i), new GH_Path(1, i));
            }
            return temp;
        }
        private DataTree<string> CopyDataTreeString_nN(DataTree<string> tree)
        {
            DataTree<string> temp = new DataTree<string>();
            for (int i = 0; i < tree.BranchCount / 2; i++)
            {
                temp.AddRange(tree.Branch(i * 2), new GH_Path(i, 0));
                temp.AddRange(tree.Branch(i * 2 + 1), new GH_Path(i, 1));
            }
            return temp;
        }
    }
}

