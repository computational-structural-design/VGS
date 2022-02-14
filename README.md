# VGS - Vector-based Graphic Statics

Vector-based Graphic Static (VGS) is a direct extension of traditional 2D graphic statics to the third dimension. VGS introduced a generalized procedure for the construction of a 3D vector-based force diagram for any given 3D form diagram of a spatial network in static equilibrium. By establishing an interdependency between form and force diagrams, VGS allows users to transform one of the diagrams and evaluate directly the resulting transformation of the other diagram. This property allows for a quick and interactive exploration of possible equilibrium solutions in the early design phase. VGS is implemented as a plug-in for the CAD environment [McNeel Rhino/Grasshopper](https://www.rhino3d.com/).
<br>
<br>

VGS is developed by:
- __Pierluigi D'Acunto__ (Technical University of Munich)
- __Yuchi Shen__ (Southeast University of Nanjing)
- __Jean-Philippe Jasienski__ (UCLouvain)
- __Patrick Ole Ohlbrock__ (ETH Zurich)
<br>

VGS makes use of the following libraries: 
- the C# implementation of the Boyer-Myrvold algorithm for [Planarity Testing by Ondrej Nepozitek](https://github.com/OndrejNepozitek/GraphPlanarityTesting) - MIT license; 
- the library [Math.NET Numerics](https://www.nuget.org/packages/MathNet.Numerics/) - MIT license.
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

Publications related to the VGS project include:
- __D'Acunto Pierluigi, Jasienski Jean-Philippe, Ohlbrock Patrick Ole, Fivet Corentin, Schwartz Joseph, Zastavni Denis__: Vector-based 3D graphic statics: A framework for the design of spatial structures based on the relation between form and forces, International Journal of Solids and Structures, Volume 167, pp. 58-70, 2019
- __D'Acunto Pierluigi, Jasienski Jean-Philippe, Ohlbrock Patrick Ole, Fivet Corentin__: Vector-based 3D Graphic Statics: Transformations of Force Diagrams. Proceedings of the IASS Symposium 2017 - Interfaces: Architecture Engineering Science, Hamburg, 2017
- __D'Acunto Pierluigi, Ohlbrock Patrick Ole, Jasienski Jean-Philippe, Fivet Corentin__: Vector-based 3D Graphic Statics (Part I): Evaluation of Global Equilibrium. Proceedings of the IASS Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016
- __Jasienski Jean-Philippe, D'Acunto Pierluigi, Ohlbrock Patrick Ole, Fivet Corentin__: Vector-based 3D Graphic Statics (Part II): Construction of Force Diagrams. Proceedings of the IASS Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016
- __Ohlbrock Patrick Ole, D'Acunto Pierluigi, Jasienski Jean-Philippe, Fivet Corentin__: Vector-Based 3D Graphic Statics (Part III): Designing with Combinatorial Equilibrium Modelling, Proceedings of the IASS Annual Symposium 2016 - Spatial Structures in the 21st Century, Tokyo, 2016




