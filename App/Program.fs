open System
open System.IO
open Microsoft.ML
open Microsoft.ML.Data

// %%
let _dataPath =
    Path.Combine(Environment.CurrentDirectory, "Data", "yelp_labelled.txt")

// %%
type SentimentData =
    { [<LoadColumn 0>]
      SentimentText: string
      [<LoadColumn 1; ColumnName "Label">]
      Sentiment: bool }

[<CLIMutable>]
type SentimentPrediction =
    { [<ColumnName "PredictedLabel">]
      Prediction: bool
      Probability: float
      Score: float }

// %%
let mlcontext = MLContext()

// %%
let LoadData (mlContext: MLContext) =
    let dataView =
        mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader = false)

    mlContext.Data.TrainTestSplit(dataView, testFraction = 0.2)

// %%
let splitDataView = LoadData(mlcontext)

// %%
let BuildAndTrainModel (mlContext: MLContext) splitTrainSet =
    mlContext
        .Transforms
        .Text
        .FeaturizeText(
            outputColumnName = "Features",
            inputColumnName = nameof Unchecked.defaultof<SentimentData>.SentimentText
        )
        .Append(
            mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName = "Label",
                featureColumnName = "Features"
            )
        )
        .Fit(splitTrainSet)


// %%
let model = BuildAndTrainModel mlcontext splitDataView.TrainSet
