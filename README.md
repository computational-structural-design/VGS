# VGS - Vector-based Graphic Statics

Vector-based Graphic Static (VGS) is a direct extension of traditional 2D graphic statics to the third dimension. VGS introduced a generalized procedure for the construction of a 3D vector-based force diagram for any given 3D form diagram of a spatial network in static equilibrium. By establishing an interdependency between form and force diagrams, VGS allows users to transform one of the diagrams and evaluate directly the resulting transformation of the other diagram. This property allows for a quick and interactive exploration of possible equilibrium solutions in the early design phase. VGS is implemented as a plug-in for the CAD environment [McNeel Rhino/Grasshopper](https://www.rhino3d.com/).
<br>
<br>

VGS is developed and mantained by:
- __Pierluigi D'Acunto__ [Technical University of Munich, Professorship of Structural Design](https://www.arc.ed.tum.de/sd/startseite/)
- __Yuchi Shen__ [Southeast University of Nanjing, School of Architecture](http://arch.seu.edu.cn/jz_en/main.htm)
- __Jean-Philippe Jasienski__ [UCLouvain, Structures & Technologies LAB](https://sites.uclouvain.be/structech_loci/)
- __Patrick Ole Ohlbrock__ [ETH Zurich, Chair of Structural Design](https://schwartz.arch.ethz.ch/)
<br>

If you use the VGS library, please refer to the official GitHub repository: <br>
@Misc{vgs2021, <br>
author = {D'Acunto, Pierluigi and Shen, Yuchi and Jasienski, Jean-Philippe and Ohbrock, Patrick Ole}, <br>
title = {{VGS: Vector-based Graphic Statics}}, <br>
year = {2021}, <br>
note = {Release 1.00}, <br>
url = {https://github.com/pierluigidacunto/VGS}, <br>
}
<br>
<br>

VGS makes use of the following libraries: 
- the C# implementation of the Boyer-Myrvold algorithm for [Planarity Testing by Ondrej Nepozitek](https://github.com/OndrejNepozitek/GraphPlanarityTesting) - MIT license; 
- the library [Math.NET Numerics](https://www.nuget.org/packages/MathNet.Numerics/) - MIT license.
<br>
<br>

To install  VGS, please copy the folder "VGS1.00beta" in the "Libraries" folder of Grasshopper. Please make sure that the files are unlocked. VGS requires [Rhino7 SR13 for Windows](https://www.rhino3d.com/download/rhino-for-windows/evaluation) or later.
<br>
<br>

Publications related to the VGS project include:
- __D'Acunto Pierluigi, Jasienski Jean-Philippe, Ohlbrock Patrick Ole, Fivet Corentin, Schwartz Joseph, Zastavni Denis__: Vector-based 3D graphic statics: A framework for the design of spatial structures based on the relation between form and forces, International Journal of Solids and Structures, Volume 167, pp. 58-70, 2019
- __D'Acunto Pierluigi, Jasienski Jean-Philippe, Ohlbrock Patrick Ole, Fivet Corentin__: Vector-based 3D Graphic Statics: Transformations of Force Diagrams. Proceedings of the IASS Symposium 2017 - Interfaces: Architecture Engineering Science, Hamburg, 2017
- __D'Acunto Pierluigi, Ohlbrock Patrick Ole, Jasienski Jean-Philippe, Fivet Corentin__: Vector-based 3D Graphic Statics (Part I): Evaluation of Global Equilibrium. Proceedings of the IASS Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016
- __Jasienski Jean-Philippe, D'Acunto Pierluigi, Ohlbrock Patrick Ole, Fivet Corentin__: Vector-based 3D Graphic Statics (Part II): Construction of Force Diagrams. Proceedings of the IASS Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016
- __Ohlbrock Patrick Ole, D'Acunto Pierluigi, Jasienski Jean-Philippe, Fivet Corentin__: Vector-Based 3D Graphic Statics (Part III): Designing with Combinatorial Equilibrium Modelling, Proceedings of the IASS Annual Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016




