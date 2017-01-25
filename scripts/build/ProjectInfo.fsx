[<AutoOpen>]
module ProjectInfo

#r @"../../packages/build/FAKE/tools/FakeLib.dll"

open Fake
open Fake.ReleaseNotesHelper

// --------------------------------------------------------------------------------------
// START TODO: Provide project-specific details below
// --------------------------------------------------------------------------------------

// Information about the project are used
//  - for version and project name in generated AssemblyInfo file
//  - by the generated NuGet package
//  - to run tests and to publish documentation on GitHub gh-pages
//  - for documentation, you also need to edit info in "docs/tools/generate.fsx"

// The name of the project
// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
let project = "fsfbl"

// Short summary of the project
// (used as description in AssemblyInfo and as a short summary for NuGet package)
let summary = "Fantasy baseball analysis in F#"

// Longer description of the project
// (used as a description for NuGet package; line breaks are automatically cleaned up)
let description = "Fantasy baseball analysis in F#"

// List of author names (for NuGet package)
let authors = [ "Sean Kell" ]

// Tags for your project (for NuGet package)
let tags = "F# fantasy baseball"

// File system information
let solutionFile  = "fsfbl.sln"

// Default target configuration
let configuration = "Release"

// Pattern specifying assemblies to be tested using NUnit
let testAssemblies = "tests/**/bin" </> configuration </> "*Tests*.dll"

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "smkell"
let gitHome = sprintf "%s/%s" "https://github.com" gitOwner

// The name of the project on GitHub
let gitName = "fsfbl"

// The url for the raw files hosted
let gitRaw = environVarOrDefault "gitRaw" "https://raw.githubusercontent.com/smkell"

// --------------------------------------------------------------------------------------
// END TODO: The rest of the file includes standard build steps
// --------------------------------------------------------------------------------------

// Read additional information from the release notes document
let release = LoadReleaseNotes "RELEASE_NOTES.md"