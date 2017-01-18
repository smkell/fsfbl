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
module Library = 
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

    type GameId = 
        {
            teamCode : string
            year : int64
            month : int64
            day : int64
            gameNum : int64
        }

    let emptyGameId = { teamCode = ""; year = 0L; month = 0L; day = 0L; gameNum = 0L }

    /// Represents an event in a Retrosheets event log.
    type Event =
        /// An `Event` which provides the game id.
        | IdEvent of GameId
        /// An `Event` which provides the event log version.
        | VersionEvent of int64
        /// An `Event` which provides info records.
        | InfoEvent of string * string
        /// An `Event` which provides a start or sub record.
        | StartEvent of playerId : string * playerName : string * homeOrAway : Team * battingOrder : int64 * fieldingPosition : int64
        | PlayEvent of PlayRecord
        | DataEvent of key : string * playerId : string * value : int64
    
    let parseGameId : Parser<GameId,unit> = 
        preturn {teamCode = ""; year = 0L; month = 0L; day = 0L; gameNum = 0L }
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
        (pstring "start," <|> pstring "sub,")
        >>. tuple5 (charsTillString "," true 100) (charsTillString "," true 100) (pint64 .>> pstring "," |>> fun (i) -> if i = 0L then Away else Home) (pint64 .>> pstring ",") (pint64)
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

    type Game = 
        {
            gameId : GameId
            homeTeamId : string
            awayTeamId : string
        }

    let emptyGame = { gameId = emptyGameId; homeTeamId = ""; awayTeamId = ""}

    /// Parses a game from a series of events.
    let parseGame = 
        let rec loop acc events =
            // Iterate over the events and update the acc with each pass
            match events with 
            | [] -> acc
            | hd::tl -> 
                match hd with 
                | IdEvent gameId -> loop { acc with gameId = gameId } tl
                | InfoEvent("visteam", awayTeamId) -> loop {acc with awayTeamId = awayTeamId} tl
                | InfoEvent("hometeam", homeTeamId) -> loop {acc with homeTeamId = homeTeamId} tl
                | _ -> loop acc tl

        parseIdEvent .>>. many parseNonIdEvent
        |>> fun (id, rest) -> id::rest
        |>> loop emptyGame

    let parseGames = many parseGame