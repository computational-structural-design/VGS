using System;
using System.Drawing;
using Grasshopper.Kernel;
using VGS_Main;

namespace GraphicStatic
{
    public class GS : GH_AssemblyInfo
  {
    public override string Name
    {
        get
        {
                return System_Configuration.PATH.Version;
        }
    }
    public override Bitmap Icon
    {
        get
        {
            //Return a 24x24 pixel bitmap to represent this GHA library.
            return null;
        }
    }
    public override string Description
    {
        get
        {
            return "Vector-based Graphic Static (VGS Tool)";
        }
    }
    public override Guid Id
    {
        get
        {
                return new Guid("4dd5105d-5e17-49dd-802b-5968e7c7b09d");
        }
    }

    public override string AuthorName
    {
        get
        {
            return "D'Acunto, Pierluigi and Shen, Yuchi and Jasienski, Jean-Philippe and Ohbrock, Patrick Ole";
        }
    }
    public override string AuthorContact
    {
        get
        {
            return "yuchi_shen@seu.edu.cn";
        }
    }
}
}
