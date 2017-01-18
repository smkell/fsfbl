// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r @"..\..\packages\FParsec-Big-Data-Edition\lib\net45\FParsecCS.dll"
#r @"..\..\packages\FParsec-Big-Data-Edition\lib\net45\FParsec.dll"

#load "Retrosheets.fs"
open System
open fsfbl
open FParsec

let exampleGame = @"id,ANA201604040
version,2
info,visteam,CHN
info,hometeam,ANA
info,site,ANA01
info,date,2016/04/04
info,number,0
info,starttime,7:08PM
info,daynight,night
info,usedh,true
info,umphome,barrt901
info,ump1b,herna901
info,ump2b,barkl901
info,ump3b,littw901
info,howscored,park
info,pitches,pitches
info,oscorer,munse701
info,temp,75
info,winddir,torf
info,windspeed,6
info,fieldcond,unknown
info,precip,unknown
info,sky,sunny
info,timeofgame,188
info,attendance,44020
info,wp,arrij001
info,lp,richg002
info,save,
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
play,1,0,fowld001,10,BX,D9/G+
play,1,0,heywj001,00,X,63/G.2-3
play,1,0,zobrb001,02,SFS,K
play,1,0,rizza001,11,BCX,S8/G.3-H
play,1,0,bryak001,22,BSS*B1S,K23
play,1,1,escoy001,20,BBX,13/G-
play,1,1,navad002,11,BCX,8/F
play,1,1,troum001,01,CX,4/L
play,2,0,schwk001,12,BCCS,K
play,2,0,solej001,12,SBFX,13/G-
play,2,0,montm001,32,SFBBBS,K
play,2,1,pujoa001,21,FBBX,7/L
play,2,1,calhk001,12,FBSX,S9/L+
play,2,1,cronc002,01,C1X,9/F
play,2,1,simma001,02,CCC,K
play,3,0,russa002,32,CCBBBB,W
play,3,0,fowld001,01,CX,S9/L+.1-2
play,3,0,heywj001,12,CS*BS,K
play,3,0,zobrb001,00,X,46(1)3/GDP
play,3,1,perec003,30,BBBB,W
play,3,1,giavj001,02,CFX,64(1)/FO/G
play,3,1,escoy001,00,1X,4/L/DP.1X1(43)
play,4,0,rizza001,31,SBBBB,W
play,4,0,bryak001,32,F1FFB*BB1B,W.1-2
play,4,0,schwk001,32,B*BSFF*BX,3/G.2-3;1-2
play,4,0,solej001,01,SX,S7/G+.3-H;2-3
play,4,0,montm001,32,BFFFFBFBX,S6/G.3-H;1-2
play,4,0,russa002,32,STBBBS,K
play,4,0,fowld001,22,C*BBFC,K
play,4,1,navad002,12,CCFBFFFFS,K
play,4,1,troum001,22,BBCFFS,K
play,4,1,pujoa001,00,X,3/P3F
play,5,0,heywj001,21,BFBX,3/G
play,5,0,zobrb001,20,BBX,S7/L-
play,5,0,rizza001,12,FBC+1,PO1(23)
play,5,0,rizza001,22,FBC+1.FFFBX,8/F
play,5,1,calhk001,22,FBFBFX,43/G
play,5,1,cronc002,12,CBSX,3/G
play,5,1,simma001,02,CFX,43/G
play,6,0,bryak001,00,,NP
sub,salaf001,""Fernando Salas"",1,0,1
play,6,0,bryak001,12,.BCCFS,K
play,6,0,schwk001,32,SSBBBC,K
play,6,0,solej001,32,CBBBSFFB,W
play,6,0,montm001,11,BFX,HR/9/L.1-H
play,6,0,russa002,01,FX,8/F
play,6,1,perec003,12,CFBS,K
play,6,1,giavj001,11,CBX,13/G
play,6,1,escoy001,12,CSBX,43/G
play,7,0,fowld001,00,,NP
sub,bedrc001,""Cam Bedrosian"",1,0,1
play,7,0,fowld001,00,.X,S9/G+
play,7,0,heywj001,22,CBBFX,D7/F.1-3
play,7,0,zobrb001,02,CCS,K
play,7,0,rizza001,30,IIII,IW
play,7,0,bryak001,22,CBC*BX,64(1)/FO/G.3-H;2-3
play,7,0,schwk001,11,BCX,43/G+
play,7,1,navad002,00,,NP
sub,szczm001,""Matt Szczur"",0,6,7
play,7,1,navad002,11,.BCX,S8/F
play,7,1,troum001,12,CBCS,K
play,7,1,pujoa001,22,CCF*BBS,K
play,7,1,calhk001,22,SF*B*BFFX,3/G
play,8,0,solej001,00,,NP
sub,morim002,""Mike Morin"",1,0,1
play,8,0,solej001,22,.SCBFBS,K
play,8,0,montm001,00,X,9/F
play,8,0,russa002,02,CFX,9/F
play,8,1,cronc002,00,,NP
sub,grimj002,""Justin Grimm"",0,0,1
play,8,1,cronc002,00,.X,D9/F
play,8,1,simma001,00,X,3/G.2-3
play,8,1,perec003,12,FS*BX,5/L+
play,8,1,giavj001,01,FX,63/G
play,9,0,fowld001,00,,NP
sub,rasmc002,""Cory Rasmus"",1,0,1
play,9,0,fowld001,32,.SCBBFBB,W
play,9,0,heywj001,00,X,8/F
play,9,0,zobrb001,02,CFFFX,S9/G.1-3
play,9,0,rizza001,12,FCBX,6/P
play,9,0,bryak001,32,BSC*BB>B,W.1-2
play,9,0,szczm001,22,CTBBX,D7/L+.3-H;2-H;1-H
play,9,0,solej001,12,*BFSX,E5/G
play,9,0,montm001,12,CFBT,K
play,9,1,escoy001,00,,NP
sub,woodt004,""Travis Wood"",0,0,1
play,9,1,escoy001,02,.TFX,63/G
play,9,1,navad002,00,,NP
sub,gentc001,""Craig Gentry"",1,2,11
play,9,1,gentc001,12,.CBFC,K
play,9,1,troum001,22,CFBBX,9/F
data,er,arrij001,0
data,er,grimj002,0
data,er,woodt004,0
data,er,richg002,3
data,er,salaf001,2
data,er,bedrc001,1
data,er,morim002,0
data,er,rasmc002,30
id,ANA201604050"

let event = run Retrosheets.parseIdEvent exampleGame
printfn "%A" event

let game = run Retrosheets.parseGame exampleGame
printfn "%A" game

let games = run Retrosheets.parseGames exampleGame