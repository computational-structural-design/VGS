/*
@Misc{vgs2021,
author = {D'Acunto, Pierluigi and Shen, Yuchi and Jasienski, Jean-Philippe and Ohlbrock, Patrick Ole},
title = {{VGS Tool: Vector-based Graphic Statics}},
year = {2021},
note = {Release 1.00 Beta},
url = { https://github.com/computational-structural-design/VGS.git },
}
 */


namespace VGS_Main
{
    public class System_Configuration
    {
        static public double Sys_Tor = 0.0001;
        static public int in0_T = 10;
        static public double Scale = 10;
        static public double Text_scale = 0.2;
        static public int maxiteration = 1000;

        #region path
        public static class PATH
        {
            static public string Version { get { return "VGS1.00beta"; } }
            static public string MenuName_01 { get { return "0. System Config"; } }
            static public string MenuName_02 { get { return "1. Assemble Structural Model"; } }
            static public string MenuName_03 { get { return "2. Generate Diagrams"; } }
            static public string MenuName_04 { get { return "3. Transformation"; } }
            static public string MenuName_05 { get { return "4. Visualization"; } }
            static public string MenuName_06 { get { return "5. Surface Structure"; } }
            static public string MenuName_07 { get { return "6. WIP"; } }

            static private string current_path = Grasshopper.Folders.AppDataFolder;
        }
        #endregion

        public delegate void change();
        public event change ifchange;
        public void changing() { ifchange(); }
    }

    public class System_dynamic
    {
        static public System_Configuration temp = new System_Configuration();
    }
}
