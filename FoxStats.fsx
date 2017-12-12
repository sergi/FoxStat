// Before running any code, invoke Paket to get the dependencies.
//
// You can either build the project (Ctrl + Alt + B in VS) or run
// '.paket/paket.bootstrap.exe' and then '.paket/paket.exe install'
// (if you are on a Mac or Linux, run the 'exe' files using 'mono')
//
// Once you have packages, use Alt+Enter (in VS) or Ctrl+Enter to
// run the following in F# Interactive. You can ignore the project
// (running it doesn't do anything, it just contains this script)
#load "packages/FsLab/FsLab.fsx"
#r @"packages/SQLProvider/lib/net451/FSharp.Data.SqlProvider.dll"
#r @"packages/System.Data.SQLite.Core/lib/net451/System.Data.SQLite.dll"

open Deedle
open FSharp.Data
open FSharp.Data.Sql
open System.Data.SQLite
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

let [<Literal>] connectionString = 
    "Data Source=" + __SOURCE_DIRECTORY__ + @"/database.db;Version=3;foreign keys=true"

let [<Literal>] resolutionPath = "packages/System.Data.SQLite.Core/lib/net451"
type Sql = 
    SqlDataProvider<
        ConnectionString = connectionString,
        DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
        ResolutionPath=resolutionPath,
        SQLiteLibrary = Common.SQLiteLibrary.SystemDataSQLite>

let ctx = Sql.GetDataContext()

// Connect to the WorldBank and access indicators EU and CZ
// Try changing the code to look at stats for your country!
let wb = WorldBankData.GetDataContext()
let es = wb.Countries.Spain.Indicators
let eu = wb.Countries.``European Union``.Indicators

// Use Deedle to get time-series with school enrollment data
let czschool = series es.``School enrollment, tertiary, female (% gross)``
let euschool = series eu.``School enrollment, tertiary, female (% gross)``

// Get 5 years with the largest difference between EU and CZ
abs (czschool - euschool)
|> Series.sort
|> Series.rev
|> Series.take 5

// Plot a line chart comparing the two data sets
// (Opens a web browser window with the chart)
[ czschool.[1975 .. 2010]; euschool.[1975 .. 2010] ]
|> Chart.Line
|> Chart.WithOptions (Options(legend=Legend(position="bottom")))
|> Chart.WithLabels ["ES"; "EU"]
