# VGS Tool - Vector-based Graphic Statics

Vector-based Graphic Static (VGS) is a direct extension of traditional 2D graphic statics to the third dimension. VGS introduced a generalized procedure for the construction of a 3D vector-based force diagram for any given 3D form diagram of a spatial network in static equilibrium. By establishing an interdependency between form and force diagrams, VGS allows users to transform one of the diagrams and evaluate directly the resulting transformation of the other diagram. This property allows for a quick and interactive exploration of possible equilibrium solutions in the early design phase. VGS Tool is implemented as a plug-in for the CAD environment [McNeel Rhino/Grasshopper](https://www.rhino3d.com/) for both Windows and MacOS.
<br>
<br>

VGS Tool is developed and mantained by:
- __Pierluigi D'Acunto__ [Technical University of Munich, Professorship of Structural Design](https://www.arc.ed.tum.de/sd/structural-design/)
- __Yuchi Shen__ [Southeast University of Nanjing, School of Architecture](http://arch.seu.edu.cn/jz_en/main.htm)
- __Jean-Philippe Jasienski__ [UCLouvain, Structures & Technologies LAB](https://sites.uclouvain.be/structech_loci/)
- __Patrick Ole Ohlbrock__ [ETH Zurich, Chair of Structural Design](https://schwartz.arch.ethz.ch/)
<br>

If you use the VGS Tool, please reference the official GitHub repository: <br>
@Misc{vgs2021, <br>
author = {D'Acunto, Pierluigi and Shen, Yuchi and Jasienski, Jean-Philippe and Ohlbrock, Patrick Ole}, <br>
title = {{VGS Tool: Vector-based Graphic Statics}}, <br>
year = {2021}, <br>
note = {Release 1.00 Beta}, <br>
url = {https://github.com/computational-structural-design/VGS.git}, <br>
}
<br>
<br>

VGS Tool makes use of the following libraries: 
- the C# implementation of the Boyer-Myrvold algorithm for [Planarity Testing by Ondrej Nepozitek](https://github.com/OndrejNepozitek/GraphPlanarityTesting) - MIT license; 
- [Math.NET Numerics](https://www.nuget.org/packages/MathNet.Numerics/) - MIT license;
- [Kangaroo2](https://www.rhino3d.com/) by Daniel Piker.
<br>
<br>

To install  VGS Tool, please copy the folder "VGS1.00beta" in the "Libraries" folder of Grasshopper. If you work on Windows, please make sure that the files are unlocked. VGS requires [Rhino7 SR13 for Windows](https://www.rhino3d.com/download/rhino-for-windows/evaluation) or [Rhino7 SR13 for MacOS](https://www.rhino3d.com/download/rhino-for-mac/evaluation) or later.
<br>
<br>

Publications related to the VGS project include:
- __D'Acunto Pierluigi, Jasienski Jean-Philippe, Ohlbrock Patrick Ole, Fivet Corentin, Schwartz Joseph, Zastavni Denis__: Vector-based 3D graphic statics: A framework for the design of spatial structures based on the relation between form and forces, International Journal of Solids and Structures, Volume 167, pp. 58-70, 2019
- __Shen Yuchi, D'Acunto Pierluigi, Jasienski Jean-Philippe, Ohlbrock Patrick Ole___: A new tool for the conceptual design of structures in equilibrium based on graphic statics, International fib Symposium on Conceptual Design of Structures, Attisholz, 2021
- __D'Acunto Pierluigi, Jasienski Jean-Philippe, Ohlbrock Patrick Ole, Fivet Corentin__: Vector-based 3D Graphic Statics: Transformations of Force Diagrams. Proceedings of the IASS Symposium 2017 - Interfaces: Architecture Engineering Science, Hamburg, 2017
- __D'Acunto Pierluigi, Ohlbrock Patrick Ole, Jasienski Jean-Philippe, Fivet Corentin__: Vector-based 3D Graphic Statics (Part I): Evaluation of Global Equilibrium. Proceedings of the IASS Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016
- __Jasienski Jean-Philippe, D'Acunto Pierluigi, Ohlbrock Patrick Ole, Fivet Corentin__: Vector-based 3D Graphic Statics (Part II): Construction of Force Diagrams. Proceedings of the IASS Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016
- __Ohlbrock Patrick Ole, D'Acunto Pierluigi, Jasienski Jean-Philippe, Fivet Corentin__: Vector-Based 3D Graphic Statics (Part III): Designing with Combinatorial Equilibrium Modelling, Proceedings of the IASS Annual Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016




