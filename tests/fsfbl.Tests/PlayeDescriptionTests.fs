module fsfbl.Tests.PlayDescription

open fsfbl
open NUnit.Framework
open FParsec

let runTestCases testCases =
    let test (line, expect) =
        let actual = 
            match run Retrosheets.playDescription line with 
            | Success(e,_,_) -> Some e
            | _ -> None
    
        printfn "Expect: %A" expect
        printfn "Actual: %A" actual
        printfn ""

        Assert.AreEqual(expect, actual)

    testCases
    |> List.iter test

[<Test>]
let ``The fielder should be correctly attributed for an unassited out`` () =
    [ 
        (@"8/F78", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.CenterFielder]; runner = Retrosheets.Batter } ] });
        (@"9/SF.3-H", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.RightFielder]; runner = Retrosheets.Batter } ] });
        (@"3/G.2-3", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.FirstBaseman]; runner = Retrosheets.Batter } ] });
    ]
    |> runTestCases 

[<Test>]
let ``The fielders should be correctly attributed for a ground ball with an assist out`` () =
    [ 
        (@"63/G6M", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ShortStop; Retrosheets.FirstBaseman]; runner = Retrosheets.Batter } ] });
        (@"143/G1", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.Pitcher; Retrosheets.SecondBaseman; Retrosheets.FirstBaseman]; runner = Retrosheets.Batter } ] });
        (@"54(B)/BG25/SH.1-2", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ThirdBaseman; Retrosheets.SecondBaseman]; runner = Retrosheets.Batter } ] });
        (@"54/BG25/SH.1-2", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ThirdBaseman; Retrosheets.SecondBaseman]; runner = Retrosheets.FirstBase } ] });
    ]
    |> runTestCases

[<Test>]
let ``Double plays should be correctly handled`` () =
    [
        (@"64(1)3/GDP/G6", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ShortStop; Retrosheets.SecondBaseman]; runner = Retrosheets.FirstBase
         }; { fielders = [Retrosheets.FirstBaseman]; runner = Retrosheets.Batter} ] });
    ]
    |> runTestCases
        
