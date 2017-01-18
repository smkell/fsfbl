module fsfbl.Tests

open fsfbl
open NUnit.Framework
open FParsec

//[<Test>]
//let ``parseIdEvent successfully parses Id`` () =
//  let line = @"id,ANA201604040"
//  let expect = Some (fsfbl.Library.IdEvent "ANA201604040")
//  let actual = 
//    match run fsfbl.Library.parseIdEvent line with
//    | Success(id,_,_) -> Some id
//    | _ -> None
//
//  printfn "%A" actual
//
//  Assert.AreEqual(expect,actual)

[<Test>]
let ``parseVersionEvent successfully parses version`` () =
  let line = @"version,2"
  let expect = Some (fsfbl.Library.VersionEvent 2L)
  let actual = 
    match run fsfbl.Library.parseVersionEvent line with
    | Success(id,_,_) -> Some id
    | _ -> None

  printfn "%A" actual

  Assert.AreEqual(expect,actual)

[<Test>]
let ``parseInfoEvent successfully parses info key-value pair`` () =
  let line = @"info,visteam,CHN"
  let expect = Some (fsfbl.Library.InfoEvent ("visteam", "CHN"))
  let actual = 
    match run fsfbl.Library.parseInfoEvent line with
    | Success(id,_,_) -> Some id
    | _ -> None

  printfn "%A" actual

  Assert.AreEqual(expect,actual)