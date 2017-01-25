[<AutoOpen>]
module TestTasks

#r @"../../packages/build/FAKE/tools/FakeLib.dll"
#load "ProjectInfo.fsx"

open Fake
open Fake.Testing.XUnit2
open System

// --------------------------------------------------------------------------------------
// Run the unit tests using test runner

Target "RunTests" (fun _ ->
    !! testAssemblies
    |> xUnit2 (fun p ->
        { p with
            ShadowCopy = true
            TimeOut = TimeSpan.FromMinutes 20.
            XmlOutputPath = Some "TestResults.xml" })
)

"Build" ==> "RunTests"

Target "Test" DoNothing

"RunTests" ==> "Test"