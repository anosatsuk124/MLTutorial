// ---
// jupyter:
//   jupytext:
//     text_representation:
//       extension: .fsx
//       format_name: percent
//       format_version: '1.3'
//       jupytext_version: 1.14.0
//   kernelspec:
//     display_name: .NET (F#)
//     language: F#
//     name: .net-fsharp
// ---

// %%
#r "nuget: Deedle"
#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.12"
#r "nuget: FSharp.Data"

// %%
open FSharp.Stats
let factorial3 = SpecialFunctions.Factorial.factorial 3
factorial3

// %%
open FSharp.Data
open Deedle

let rawData =
    Http.RequestString @"https://raw.githubusercontent.com/dotnet/machinelearning/master/test/data/housing.txt"

// %%
let df = Frame.ReadCsvString(rawData,hasHeaders=true,separators="\t")
df.Print()

// %%
let housesNotAtRiver =
    df
    |> Frame.sliceCols ["RoomesPerDwlling";"MedianHomeValue";"CharlesRiver"]
    |> Frame.filterRowValues (fun s -> s.GetAs<bool> "CharlesRiver" |> not)

housesNotAtRiver.Print()

// %%
open Plotly.NET

let pricesNotAtRiver : seq<float> =
    housesNotAtRiver
    |> Frame.getCol "MedianHomeValue"
    |> Series.values
    
let h1 =
    Chart.Histogram pricesNotAtRiver
    |> Chart.withXAxisStyle "median value of owner occupied home in 1000s"
    |> Chart.withXAxisStyle "price distruibution"

h1

// %%
