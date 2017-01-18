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
///     id event = "id", ",", game id, "\n"
///     game id = team code, year, month, day, game number
///     team code = "ANA" | "BAL" | "BOS" | "CHA" | "CLE" | "DET" | "HOU" | "KCA" | "MIN" | "NYA" |
///                 "OAK" | "SEA" | "TBA" | "TEX" | "TOR" | "ARI" | "ATL" | "CHN" | "CIN" | "COL" |
///                 "LAN" | "MIA" | "MIL" | "NYN" | "PHI" | "SDN" | "SFN" | "SLN" | "WAS"
module Retrosheets = 
    open FParsec
    open System

    type Team = Home | Away

    type PlayRecord = 
        { 
            inning : int64
            homeOrAway : Team
            playerId : string
            count : string
            pitches : string
            description : string 
        }

    /// Type used to represent a "Start" or "Sub" event's data.
    type StartRecord =
        {
            playerId : string;
            playerName : string;
            homeOrAway : Team;
            battingOrder : int64;
            fieldingPosition : int64;
        } with
            /// An empty StartRecord, useful for constructing new records.
            static member empty =
                { playerId = ""; playerName = ""; homeOrAway = Away; battingOrder = 0L; fieldingPosition = 0L }

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

    /// Parses a Start event record.
    let parseStartEvent : Parser<Event,unit> = 
        preturn StartRecord.empty
        .>> (pstring "start," <|> pstring "sub,")
        .>>. (charsTillString "," true 100) |>> fun (acc, i) -> { acc with playerId = i } 
        .>>. (charsTillString "," true 100) |>> fun (acc, i) -> { acc with playerName = i } 
        .>>. (pint64 .>> pstring "," |>> fun (i) -> if i = 0L then Away else Home) |>> fun (acc, i) -> { acc with homeOrAway = i }
        .>>. (pint64 .>> pstring ",") |>> fun (acc, i) -> { acc with battingOrder = i }
        .>>. (pint64) |>> fun (acc, i) -> { acc with fieldingPosition = i }
        .>> restOfLine true
        |>> StartEvent

    /// Parses a Play event record.
    let parsePlayEvent : Parser<Event,unit> =
        (preturn {inning=0L; homeOrAway = Home; playerId=""; count=""; pitches=""; description=""} .>> pstring "play,")
        .>>. (pint64 .>> pstring ",") |>> (fun (a, i) -> {a with inning = i})
        .>>. (pint64 .>> pstring ",") |>> (fun (a, i) -> {a with homeOrAway = if i = 0L then Away else Home})
        .>>. (charsTillString "," true 100) |>> (fun (a, i) -> {a with playerId = i})
        .>>. (charsTillString "," true 100) |>> (fun (a, i) -> {a with count = i})
        .>>. (charsTillString "," true 100) |>> (fun (a, i) -> {a with pitches = i})
        .>>. (restOfLine true) |>> (fun (a, i) -> {a with description = i})
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
        } with
            static member empty =
                { playerId = ""; playerName = ""; playerTeamId = ""; }

    type Game = 
        {
            gameId : GameId
            homeTeamId : string
            homeRoster : Map<string, Player>
            awayTeamId : string
            awayRoster : Map<string, Player>
        } with
            static member empty =
                { gameId = GameId.empty; homeTeamId = ""; homeRoster = Map.empty; awayTeamId = ""; awayRoster = Map.empty }

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
                        { playerId = startRecord.playerId; playerName = startRecord.playerName; playerTeamId = ""; }
                    match startRecord.homeOrAway with
                    | Home -> loop { acc with homeRoster = acc.homeRoster.Add(startRecord.playerId, { player with playerTeamId = acc.homeTeamId }) } tl
                    | Away -> loop { acc with awayRoster = acc.awayRoster.Add(startRecord.playerId, { player with playerTeamId = acc.awayTeamId }) } tl
                | _ -> loop acc tl

        parseIdEvent .>>. many parseNonIdEvent
        |>> fun (id, rest) -> id::rest
        |>> loop Game.empty

    let parseGames = many parseGame