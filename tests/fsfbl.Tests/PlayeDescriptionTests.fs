namespace fsfbl.Tests

module ``Parsing play descriptions`` =
    open fsfbl
    open Xunit
    open FsUnit.Xunit
    open FParsec

    let runTestCases testCases =
        let test (line, expect) =
            let actual = 
                match run Retrosheets.playDescription line with 
                | Success(e,_,_) -> Some e
                | _ -> None

            actual |> should equal expect

        testCases
        |> List.iter test

    [<Fact>]
    let ``when the play resulted in an unassisted out, it attributes it to the correct fielder.`` () =
        [ 
            (@"8/F78", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.CenterFielder]; runner = Retrosheets.Batter } ] });
            (@"9/SF.3-H", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.RightFielder]; runner = Retrosheets.Batter } ] });
            (@"3/G.2-3", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.FirstBaseman]; runner = Retrosheets.Batter } ] });
        ]
        |> runTestCases 

    [<Fact>]
    let ``when the play resulted in an assisted out, it attributes it to the correct fielders.`` () =
        [ 
            (@"63/G6M", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ShortStop; Retrosheets.FirstBaseman]; runner = Retrosheets.Batter } ] });
            (@"143/G1", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.Pitcher; Retrosheets.SecondBaseman; Retrosheets.FirstBaseman]; runner = Retrosheets.Batter } ] });
            (@"54(B)/BG25/SH.1-2", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ThirdBaseman; Retrosheets.SecondBaseman]; runner = Retrosheets.Batter } ] });
            (@"54/BG25/SH.1-2", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ThirdBaseman; Retrosheets.SecondBaseman]; runner = Retrosheets.FirstBase } ] });
        ]
        |> runTestCases

    [<Fact>]
    let ``when the play resulted in a double play, it attributes it to the correct fielders, and puts out the correct runners.`` () =
        [
            (@"64(1)3/GDP/G6", Some { Retrosheets.PlayDescription.empty with outs = [ { fielders = [Retrosheets.ShortStop; Retrosheets.SecondBaseman]; runner = Retrosheets.FirstBase
             }; { fielders = [Retrosheets.FirstBaseman]; runner = Retrosheets.Batter} ] });
        ]
        |> runTestCases

    [<Fact>]
    let ``when the play resulted in a hit, it attributes stats correctly.`` () =
        [
            (@"S7", Some { Retrosheets.PlayDescription.empty with hit = Some (Retrosheets.Single [ Retrosheets.LeftFielder ] )});
            (@"S", Some { Retrosheets.PlayDescription.empty with hit = Some (Retrosheets.Single [] )});
            (@"D7/G5.3-H;2-H;1-H", Some { Retrosheets.PlayDescription.empty with hit = Some (Retrosheets.Double [ Retrosheets.LeftFielder ] )});
            (@"HR9/F9LS.3-H;1-H", Some { Retrosheets.PlayDescription.empty with hit = Some (Retrosheets.HomeRun [ Retrosheets.RightFielder ] )});
            (@"H9/F9LS.3-H;1-H", Some { Retrosheets.PlayDescription.empty with hit = Some (Retrosheets.HomeRun [ Retrosheets.RightFielder ] )});
        ]
        |> runTestCases

    [<Fact>]
    let ``when the play resulted in an interference call, it attributes it to the correct fielder.`` () =
        [
            (@"C/E2.1-2", Some { Retrosheets.PlayDescription.empty with interference = Some (Retrosheets.Interference Retrosheets.Catcher) });
            (@"C/E1.1-2", Some { Retrosheets.PlayDescription.empty with interference = Some (Retrosheets.Interference Retrosheets.Pitcher) });
        ]
        |> runTestCases
        
