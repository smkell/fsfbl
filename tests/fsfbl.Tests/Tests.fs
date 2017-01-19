namespace fsfbl.Tests

module ParseEventsTest =
    open fsfbl
    open NUnit.Framework
    open FParsec

    [<Test>]
    let ``parseIdEvent successfully parses Id`` () =
      let line = @"id,ANA201604040"
      let expect = Some (fsfbl.Retrosheets.IdEvent {teamCode = "ANA"; year = 2016L; month = 04L; day = 04L; gameNum = 0L})
      let actual = 
        match run fsfbl.Retrosheets.parseIdEvent line with
        | Success(id,_,_) -> Some id
        | _ -> None

      printfn "%A" actual

      Assert.AreEqual(expect,actual)

    [<Test>]
    let ``parseVersionEvent successfully parses version`` () =
      let line = @"version,2"
      let expect = Some (fsfbl.Retrosheets.VersionEvent 2L)
      let actual = 
        match run fsfbl.Retrosheets.parseVersionEvent line with
        | Success(id,_,_) -> Some id
        | _ -> None

      printfn "%A" actual

      Assert.AreEqual(expect,actual)

    [<Test>]
    let ``parseInfoEvent successfully parses info key-value pair`` () =
      let line = @"info,visteam,CHN"
      let expect = Some (fsfbl.Retrosheets.InfoEvent ("visteam", "CHN"))
      let actual = 
        match run fsfbl.Retrosheets.parseInfoEvent line with
        | Success(id,_,_) -> Some id
        | _ -> None

      printfn "%A" actual

      Assert.AreEqual(expect,actual)

    [<Test>]
    let ``parsePlayEvent successfully parses a play event``() =
        let line = @"play,1,0,fowld001,10,BX,D9/G+"

        let expectRecord = 
            {
                Retrosheets.PlayRecord.empty 
                with
                    inning = 1L;
                    homeOrAway = Retrosheets.Away;
                    playerId = "fowld001";
                    count = "10";
                    pitches = "BX";
                    description = Retrosheets.PlayDescription.empty
            }
        let expect = Some(Retrosheets.PlayEvent(expectRecord))
        let actual =
            match run Retrosheets.parsePlayEvent line with
            | Success(e,_,_) -> Some e
            | _ -> None
    
        printfn "%A" actual
        Assert.AreEqual(expect, actual)