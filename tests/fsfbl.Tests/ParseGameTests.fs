namespace fsfbl.Tests

module ``Parsing a game's event log`` =
    open fsfbl
    open Xunit
    open FsUnit.Xunit
    open FParsec

    module ``when a game with starting rosters but no plays is given`` =
        let input = @"id,ANA201604040
info,visteam,CHN
info,hometeam,ANA
start,fowld001,""Dexter Fowler"",0,1,8
start,heywj001,""Jason Heyward"",0,2,9
start,zobrb001,""Ben Zobrist"",0,3,4
start,rizza001,""Anthony Rizzo"",0,4,3
start,bryak001,""Kris Bryant"",0,5,5
start,schwk001,""Kyle Schwarber"",0,6,7
start,solej001,""Jorge Soler"",0,7,10
start,montm001,""Miguel Montero"",0,8,2
start,russa002,""Addison Russell"",0,9,6
start,arrij001,""Jake Arrieta"",0,0,1
start,escoy001,""Yunel Escobar"",1,1,5
start,navad002,""Daniel Nava"",1,2,7
start,troum001,""Mike Trout"",1,3,8
start,pujoa001,""Albert Pujols"",1,4,10
start,calhk001,""Kole Calhoun"",1,5,9
start,cronc002,""C.J. Cron"",1,6,3
start,simma001,""Andrelton Simmons"",1,7,6
start,perec003,""Carlos Perez"",1,8,2
start,giavj001,""Johnny Giavotella"",1,9,4
start,richg002,""Garrett Richards"",1,0,1"

        let game = match run Retrosheets.parseGame input with
                         | Success(g,_,_) -> g
                         | Failure(s,_,_) -> failwith s

        [<Fact>]
        let ``then it should have the home team fielders on the field`` () =
            let expect : Map<Retrosheets.Fielder, string> =
                [ (Retrosheets.Pitcher, "richg002" );
                  (Retrosheets.Catcher, "perec003" );
                  (Retrosheets.FirstBaseman, "cronc002" );
                  (Retrosheets.SecondBaseman, "giavj001" );
                  (Retrosheets.ThirdBaseman, "escoy001" );
                  (Retrosheets.ShortStop, "simma001" );
                  (Retrosheets.LeftFielder, "navad002" );
                  (Retrosheets.CenterFielder, "troum001" ); 
                  (Retrosheets.RightFielder, "calhk001" ); ]
                |> Map.ofList
            let fielders = game.state.fielders
            fielders |> should equal expect

    module ``when a game is given with the home team batting`` =
        let input = @"id,ANA201604040
info,visteam,CHN
info,hometeam,ANA
start,fowld001,""Dexter Fowler"",0,1,8
start,heywj001,""Jason Heyward"",0,2,9
start,zobrb001,""Ben Zobrist"",0,3,4
start,rizza001,""Anthony Rizzo"",0,4,3
start,bryak001,""Kris Bryant"",0,5,5
start,schwk001,""Kyle Schwarber"",0,6,7
start,solej001,""Jorge Soler"",0,7,10
start,montm001,""Miguel Montero"",0,8,2
start,russa002,""Addison Russell"",0,9,6
start,arrij001,""Jake Arrieta"",0,0,1
start,escoy001,""Yunel Escobar"",1,1,5
start,navad002,""Daniel Nava"",1,2,7
start,troum001,""Mike Trout"",1,3,8
start,pujoa001,""Albert Pujols"",1,4,10
start,calhk001,""Kole Calhoun"",1,5,9
start,cronc002,""C.J. Cron"",1,6,3
start,simma001,""Andrelton Simmons"",1,7,6
start,perec003,""Carlos Perez"",1,8,2
start,giavj001,""Johnny Giavotella"",1,9,4
start,richg002,""Garrett Richards"",1,0,1
play,1,1,escoy001,20,BBX,13/G-"
        
        let game = match run Retrosheets.parseGame input with
                   | Success(g,_,_) -> g
                   | Failure(f,_,_) -> failwith f

        [<Fact(Skip="Just getting docs working for now")>]
        let ``then the away team should be on the field`` () =
            let expect : Map<Retrosheets.Fielder, string> =
                [ (Retrosheets.Pitcher, "arrij001" );
                  (Retrosheets.Catcher, "montm001" );
                  (Retrosheets.FirstBaseman, "rizza001" );
                  (Retrosheets.SecondBaseman, "zobrb001" );
                  (Retrosheets.ThirdBaseman, "bryak001" );
                  (Retrosheets.ShortStop, "russa002" );
                  (Retrosheets.LeftFielder, "schwk001" );
                  (Retrosheets.CenterFielder, "fowld001" ); 
                  (Retrosheets.RightFielder, "heywj001s" ); ]
                |> Map.ofList
            let fielders = game.state.fielders
            fielders |> should equal expect

    module ``when a game with one hit play is given`` =
        let input = @"id,ANA201604040
        play,1,0,fowld001,10,BX,D9/G+"

        let game = match run Retrosheets.parseGame input with
                         | Success(g,_,_) -> g
                         | Failure(s,_,_) -> failwith s

        [<Fact>]
        let ``then it should be the first inning`` () =
            game.state.inning |> should equal 1L

        [<Fact>]
        let ``then the away team should be batting`` () =
            game.state.battingSide |> should equal Retrosheets.Away

    
