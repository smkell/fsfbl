[<AutoOpen>]
module NotebooksTasks

#r @"../../packages/build/FAKE/tools/FakeLib.dll"
#load "FakeHelper.fsx"

open Fake
open System

// --------------------------------------------------------------------------------------
// Notebook Scripts

let buildNotebookTarget fsiargs notebookName target =
    trace (sprintf "Building report (%s), this could take some time, please wait..." notebookName)
    let notebookDir = sprintf "notebooks/%s" notebookName
    let exit = executeFAKEWithOutput notebookDir "build.fsx" "" ["target", target]
    if exit <> 0 then
        failwith (sprintf "Running target %s for %s notebook failed" target notebookName)
    ()

Target "RunExploratoryNotebook" (fun _ ->
    trace (sprintf "Running exploratory notebook")
    buildNotebookTarget "" "exploratory" "run"
)

Target "BuildExploratoryNotebook" (fun _ ->
    trace (sprintf "Building exploratory notebook")
    buildNotebookTarget "" "exploratory" "html"
)

Target "RunReportsNotebook" (fun _ ->
    trace (sprintf "Running reports notebook")
    buildNotebookTarget "" "reports" "run"
)

Target "BuildReportsNotebook" (fun _ ->
    trace (sprintf "Building reports notebook")
    buildNotebookTarget "" "reports" "html"
)

Target "PublishNotebooks" (fun _ ->
    CopyRecursive "notebooks/exploratory/output" "docs/output/notebooks/exploratory" true |> tracefn "%A"
    CopyRecursive "notebooks/reports/output" "docs/output/notebooks/reports" true |> tracefn "%A"
)

"CopyBinaries"
    ==> "BuildExploratoryNotebook"
    ==> "PublishNotebooks"
    
"CopyBinaries"
    ==> "BuildReportsNotebook"
    ==> "PublishNotebooks"