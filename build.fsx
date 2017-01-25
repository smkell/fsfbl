// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------
#I "scripts/build"
#load "FakeHelper.fsx"
#load "ProjectInfo.fsx"
#load "BuildTasks.fsx"
#load "DocsTasks.fsx"
#load "NotebooksTasks.fsx"
#load "PublishTasks.fsx"
#load "TestTasks.fsx"

#r @"packages/build/FAKE/tools/FakeLib.dll"
open Fake

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing


"Build" ==> "All"
"Test"  ==> "All"
"GenerateDocs" ==> "All"
"PublishNotebooks" ==> "All"
"BuildPackage" ==> "All"

"All"
  =?> ("ReleaseDocs",isLocalBuild)
"Clean"
  ==> "Release"

"BuildPackage"
  ==> "PublishNuget"
  ==> "Release"

"ReleaseDocs"
  ==> "Release"

RunTargetOrDefault "All"
