namespace fsfbl

/// Documentation for my library
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///
/// ## Retrosheets grammer
///     
///     digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
///     team code = "ANA" | "BAL" | "BOS" | "CHA" | "CLE" | "DET" | "HOU" | "KCA" | "MIN" | "NYA" | 
///                 "OAK" | "SEA" | "TBA" | "TEX" | "TOR" | "ARI" | "ATL" | "CHN" | "CIN" | "COL" |
///                 "LAN" | "MIA" | "MIL" | "NYN" | "PHI" | "SDN" | "SFN" | "SLN" | "WAS" ;
///     year = digit, digit, digit, digit ;
///     month = digit, digit ;
///     day = digit, digit ;
///     game number = digit ;
///     game id = team code, year, month, day, game number ;
///     id event = "id", ",", game id, "\n" ;
///     fielder = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
///     fielders = fielder { fielder } ;
///     single = "S", { fielders } ;
///     double = "D", { fielders } ;
///     triple = "T", { fielders } ;
///     home run = "H" | "HR", { fielders } ;
///     hit = single | double | triple | home run ;
///     base runner = "B" | "1" | "2" | "3" ;
///     runner out = "(", base runner, ")" ;
///     out = fielders, { runner out } ;
///     outs = out, { out } ;
///     interference = "C/E", fielder ;
///     ground rule double = "DGR" ;
///     error = "E", fielder ;
///     fielders coice = "FC", fielder ;
///     error on fly ball = "FLE", fielder ;
///     basic description = hit 
///                       | outs
///                       | interference
///                       | ground rule double
///                       | error 
///                       | fielders choice 
///                       | error on fly ball ;
///     play event = basic description { modifiers } { runner advances } ;
module Retrosheets = 
    open FParsec
    open System

    type Team = Home | Away

    /// A type which represents the valid fielder positions.
    type Fielder = 
        | Pitcher
        | Catcher
        | FirstBaseman
        | SecondBaseman
        | ThirdBaseman
        | ShortStop
        | LeftFielder
        | CenterFielder
        | RightFielder
        | Unknown
        with 
            /// Returns the `Fielder` type which corresponds with the position code given by `c`.
            ///
            /// ## Parameters
            ///  - `c` - The position code.
            static member OfChar c =
                match c with
                | '1' -> Pitcher
                | '2' -> Catcher
                | '3' -> FirstBaseman
                | '4' -> SecondBaseman
                | '5' -> ThirdBaseman
                | '6' -> ShortStop
                | '7' -> LeftFielder
                | '8' -> CenterFielder
                | '9' -> RightFielder
                | _   -> Unknown
    
    /// A type which enumerates the possible base runners.
    type BaseRunner =
        | Batter
        | FirstBase
        | SecondBase
        | ThirdBase
        | Unknown
        with
            /// Returns the `BaseRunner` type which corresponds with the base runner code given.
            ///
            /// ## Parameters
            ///  - `c` (implicit) - The base runner code.
            static member OfChar = 
                function
                | 'B' -> Batter
                | '1' -> FirstBase
                | '2' -> SecondBase
                | '3' -> ThirdBase     
                | _   -> Unknown
                
    /// A type which represents an out that occured during a play.
    type Out = 
        { fielders: Fielder list; runner: BaseRunner } 
        with
            static member empty =
                { fielders = []; runner = Unknown }

    /// A type which represents a hit.
    type Hit =
        /// Represents a single hit.
        | Single of Fielder list
        /// Represents a doube hit.
        | Double of Fielder list
        /// Represents a triple hit.
        | Triple of Fielder list
        /// Represents a home run hit.
        | HomeRun of Fielder list

    /// A type which represents interference that occured in a play.
    type Interference =
        | Interference of Fielder

    // A type which represents the description of a play.
    type PlayDescription = 
        {
            outs  : Out list
            hit   : Hit option
            interference : Interference option
        }
        with 
            static member empty =
                { outs = []; hit = None; interference = None; }

    type PlayRecord = 
        { 
            inning : int64
            homeOrAway : Team
            playerId : string
            count : string
            pitches : string
            description : PlayDescription 
        } with
            static member empty =
                { inning = 0L; homeOrAway = Away; playerId = ""; count = ""; pitches = ""; description = PlayDescription.empty }

    /// Type used to represent a "Start" or "Sub" event's data.
    type StartRecord =
        {
            playerId : string;
            playerName : string;
            homeOrAway : Team;
            battingOrder : int64;
            fieldingPosition : Fielder;
        } with
            /// An empty StartRecord, useful for constructing new records.
            static member empty =
                { playerId = ""; playerName = ""; homeOrAway = Away; battingOrder = 0L; fieldingPosition = Fielder.Unknown }

    type GameId = 
        {
            teamCode : string
            year : int64
            month : int64
            day : int64
            gameNum : int64
        } with
            static member empty =
                { teamCode = ""; year = 0L; month = 0L; day = 0L; gameNum = 0L }

    /// Represents an event in a Retrosheets event log.
    type Event =
        /// An `Event` which provides the game id.
        | IdEvent of GameId
        /// An `Event` which provides the event log version.
        | VersionEvent of int64
        /// An `Event` which provides info records.
        | InfoEvent of string * string
        /// An `Event` which provides a start or sub record.
        | StartEvent of StartRecord
        /// An `Event` which provides a play record.
        | PlayEvent of PlayRecord
        /// An `Event` which provides additional data.
        | DataEvent of key : string * playerId : string * value : int64
    
    /// Parses a game id.
    let parseGameId : Parser<GameId,unit> = 
        preturn GameId.empty
        .>>. anyString 3 |>> fun (a, i) -> { a with teamCode = i }
        .>>. anyString 4 |>> fun (a, i) -> { a with year = Int64.Parse(i) }
        .>>. anyString 2 |>> fun (a, i) -> { a with month = Int64.Parse(i) }
        .>>. anyString 2 |>> fun (a, i) -> { a with day = Int64.Parse(i) }
        .>>. anyString 1 |>> fun (a, i) -> { a with gameNum = Int64.Parse(i) }

    /// Parses an Id event record.  
    let parseIdEvent : Parser<Event,unit> = pstring "id," >>. parseGameId .>> restOfLine true |>> IdEvent

    /// Parses a Version event record.
    let parseVersionEvent : Parser<Event,unit> = pstring "version," >>. pint64 .>> restOfLine true|>> VersionEvent

    /// Parses an Info event record.
    let parseInfoEvent : Parser<Event,unit> = pstring "info," >>. charsTillString "," true 100 .>>. restOfLine true |>> InfoEvent

    /// Parses a `Fielder` from the input.
    let fielder : Parser<Fielder,unit> = digit |>> Fielder.OfChar

    /// Parses a Start event record.
    let parseStartEvent : Parser<Event,unit> = 
        preturn StartRecord.empty
        .>> (pstring "start," <|> pstring "sub,")
        .>>. (charsTillString "," true 100) |>> fun (acc, i) -> { acc with playerId = i } 
        .>>. (charsTillString "," true 100) |>> fun (acc, i) -> { acc with playerName = i } 
        .>>. (pint64 .>> pstring "," |>> fun (i) -> if i = 0L then Away else Home) |>> fun (acc, i) -> { acc with homeOrAway = i }
        .>>. (pint64 .>> pstring ",") |>> fun (acc, i) -> { acc with battingOrder = i }
        .>>. (fielder) |>> fun (acc, i) -> { acc with fieldingPosition = i }
        .>> restOfLine true
        |>> StartEvent

    /// Parses one or more `Fielder`s from the input.
    let fielders = many1 fielder

    /// Parses a `BaseRunner` from the input.
    let baseRunner = anyOf "B123" |>> BaseRunner.OfChar

    /// Parses a runner who was put out during a play.
    let runnerOut = between (pchar '(') (pchar ')') baseRunner
    
    /// Parses an `Out` from the input.
    let out =
        let pickRunnerOut (fielders:Fielder list) runnerOut =
            match runnerOut with 
            | Some(br) -> br
            | None -> match fielders with
                      | []    -> Unknown
                      | _::[] -> Batter
                      | _     -> match List.tryLast fielders with
                                 | Some(FirstBaseman) -> Batter
                                 | Some(SecondBaseman) -> FirstBase
                                 | Some(ThirdBaseman) -> SecondBase
                                 | Some(Catcher) -> ThirdBase
                                 | _ -> Unknown
                      

        fielders .>>. opt runnerOut
        |>> fun (f, b) -> { fielders = f; runner = pickRunnerOut f b}
    
    /// Parses one or more `Out`s from the input.
    let outs = many1 out

    let someOrEmptyList =
        function
        | Some(x) -> x
        | None    -> []

    /// Parses a `Single` from the input.
    let single =
        pchar 'S' >>. (opt fielders |>> someOrEmptyList)
        |>> Single

    /// Parses a `Double` from the input.
    let double =
        pchar 'D' >>. (opt fielders |>> someOrEmptyList)
        |>> Double

    /// Parses a `Triple` from the input.
    let triple =
        pchar 'T' >>. (opt fielders |>> someOrEmptyList)
        |>> Triple

    /// Parses a `HomeRun` from the input.
    let homeRun =
        (pstring "HR" <|> pstring "H")
        >>. (opt fielders |>> someOrEmptyList)
        |>> HomeRun


    /// Parses a `Hit` from the input.
    let hit = choice [ single
                       double
                       triple
                       homeRun ]

    /// Parses an `Interference` from the input.
    let interference =
        pstring "C/E" >>. fielder
        |>> Interference

    let groundRuleDouble : Parser<PlayDescription,unit> =
            pstring "DGR" >>.
            preturn { outs = []; hit = None; interference = None; }

    /// Parses a `PlayDescription` from the input.
    let playDescription =
        let outPlay =
            preturn PlayDescription.empty
            .>>. outs |>> fun (acc, o) -> { acc with outs = o }
        
        let hitPlay =
            preturn PlayDescription.empty
            .>>. hit |>> fun (acc, h) -> { acc with hit = Some h }

        let interferencePlay =
            preturn PlayDescription.empty
            .>>. interference |>> fun (acc, i) -> { acc with interference = Some i }

        choice [ outPlay
                 hitPlay
                 interferencePlay
                 groundRuleDouble ]
    
    /// Parses a Play event record.
    let parsePlayEvent : Parser<Event,unit> =
        (preturn PlayRecord.empty .>> pstring "play,")
        .>>. (pint64 .>> pstring ",") |>> (fun (a, i) -> {a with inning = i})
        .>>. (pint64 .>> pstring ",") |>> (fun (a, i) -> {a with homeOrAway = if i = 0L then Away else Home})
        .>>. (charsTillString "," true 100) |>> (fun (a, i) -> {a with playerId = i})
        .>>. (charsTillString "," true 100) |>> (fun (a, i) -> {a with count = i})
        .>>. (charsTillString "," true 100) |>> (fun (a, i) -> {a with pitches = i})
        .>>. playDescription |>> (fun (a, i) -> {a with description = i})
        |>> PlayEvent

    /// Parses a Data event record.
    let parseDataEvent : Parser<Event,unit> =
        pstring "data,"
        >>. tuple3 (charsTillString "," true 100) (charsTillString "," true 100) (pint64)
        .>> restOfLine true
        |>> DataEvent

    /// Parses an event record.
    let parseEvent = choice [ parseIdEvent
                              parseVersionEvent
                              parseInfoEvent
                              parseStartEvent
                              parsePlayEvent
                              parseDataEvent ]

    /// Parses an event record.
    let parseNonIdEvent = choice [ parseVersionEvent
                                   parseInfoEvent
                                   parseStartEvent
                                   parsePlayEvent
                                   parseDataEvent ]
    
    type Player = 
        {
            playerId : string;
            playerName : string;
            playerTeamId : string;
            fieldPosition : Fielder
        } with
            static member empty =
                { playerId = ""; playerName = ""; playerTeamId = ""; fieldPosition = Fielder.Unknown }
    
    type GameState = 
        {
            inning : int64
            battingSide : Team
            fieldingSide : Team
            fielders : Map<Fielder, string>
        }
        with
            static member empty =
                { inning = 1L; battingSide = Away; fieldingSide = Home; fielders = Map.empty}

    type Game = 
        {
            gameId : GameId
            homeTeamId : string
            homeRoster : Map<string, Player>
            awayTeamId : string
            awayRoster : Map<string, Player>
            state : GameState
        } with
            static member empty =
                { 
                    gameId = GameId.empty;
                    homeTeamId = "";
                    homeRoster = Map.empty;
                    awayTeamId = "";
                    awayRoster = Map.empty;
                    state = GameState.empty
                }

            static member addFielder game player team =
                let game = match team with
                           | Home -> 
                                let player = { player with playerTeamId = game.homeTeamId }
                                { game with homeRoster = game.homeRoster.Add(player.playerId, player) }
                           | Away -> 
                                let player = { player with playerTeamId = game.awayTeamId }
                                { game with homeRoster = game.homeRoster.Add(player.playerId, player) }                    
                if team = game.state.fieldingSide then
                    let state = { game.state with fielders = game.state.fielders.Add(player.fieldPosition, player.playerId) }
                    { game with state = state }
                else
                    game             
                    
            member game.swapSides inning battingSide =
                let rec loop (state : GameState) (players: Player list) = 
                    match players with
                    | [] -> state
                    | hd::tl -> 
                        loop { state with fielders = state.fielders.Add(hd.fieldPosition, hd.playerId) } tl
                    
                match game.state.battingSide with
                | Home -> 
                    let players = game.homeRoster
                                  |> Map.toList
                                  |> List.map (fun (_, p) -> p)
                    let state = loop game.state players
                    { game with state = { state with fieldingSide = Away; battingSide = Home } }
                | Away -> 
                    let players = game.homeRoster
                                  |> Map.toList
                                  |> List.map (fun (_, p) -> p)
                    let state = loop game.state players
                    { game with state = { state with fieldingSide = Home; battingSide = Away } }
                

    /// Parses a game from a series of events.
    let parseGame = 
        let rec loop acc events =
            // Iterate over the events and update the acc with each pass
            match events with 
            | [] -> acc
            | hd::tl -> 
                match hd with 
                | IdEvent gameId -> 
                    loop { acc with gameId = gameId } tl
                | InfoEvent("visteam", awayTeamId) -> 
                    loop {acc with awayTeamId = awayTeamId} tl
                | InfoEvent("hometeam", homeTeamId) -> 
                    loop {acc with homeTeamId = homeTeamId} tl
                | StartEvent(startRecord) ->
                    let player = 
                        { 
                            playerId = startRecord.playerId; 
                            playerName = startRecord.playerName; 
                            playerTeamId = ""; 
                            fieldPosition = startRecord.fieldingPosition 
                        }
                      
                    loop (Game.addFielder acc player startRecord.homeOrAway) tl

                | PlayEvent(playRecord) ->
                    let newGame =
                        acc.swapSides playRecord.inning playRecord.homeOrAway
                    loop newGame tl
                | _ -> loop acc tl

        parseIdEvent .>>. many parseNonIdEvent
        |>> fun (id, rest) -> id::rest
        |>> loop Game.empty

    let parseGames = many parseGame