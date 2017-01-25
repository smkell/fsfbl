(*** hide ***)
#I "../../bin/"

(**
Deedle in 10 minutes using F#
=============================
This document is a quick overview of the most important features of F# data frame library.
You can also get this page as an [F# script file](https://github.com/BlueMountainCapital/Deedle/blob/master/docs/content/tutorial.fsx)
from GitHub and run the samples interactively.
The first step is to install `Deedle.dll` [from NuGet](https://www.nuget.org/packages/Deedle).
Next, we need to load the library - in F# Interactive, this is done by loading 
an `.fsx` file that loads the actual `.dll` with the library and registers 
pretty printers for types representing data frame and series. In this sample, 
we also need  [F# Charting](http://fsharp.github.io/FSharp.Charting), which 
works similarly:
*)
#I "../../packages/notebook/FSharp.Charting"
#I "../../packages/notebook/Deedle"
#load "FSharp.Charting.fsx"
#load "Deedle.fsx"

open System
open Deedle
open FSharp.Charting

(**
<a name="creating"></a>
Creating series and frames
--------------------------
A data frame is a collection of series with unique column names (although these
do not actually have to be strings). So, to create a data frame, we first need
to create a series:
*)

(*** define-output: create1 ***)
// Create from sequence of keys and sequence of values
let dates  = 
  [ DateTime(2013,1,1); 
    DateTime(2013,1,4); 
    DateTime(2013,1,8) ]
let values = 
  [ 10.0; 20.0; 30.0 ]
let first = Series(dates, values)

// Create from a single list of observations
Series.ofObservations
  [ DateTime(2013,1,1) => 10.0
    DateTime(2013,1,4) => 20.0
    DateTime(2013,1,8) => 30.0 ]

(*** include-it: create1 ***)

(*** define-output: create2 ***)
// Shorter alternative to 'Series.ofObservations'
series [ 1 => 1.0; 2 => 2.0 ]

// Create series with implicit (ordinal) keys
Series.ofValues [ 10.0; 20.0; 30.0 ]
(*** include-it: create2 ***)