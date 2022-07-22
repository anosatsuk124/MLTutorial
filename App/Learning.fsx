#r "nuget: Microsoft.ML"

open System
open System.IO

open Microsoft.ML
open Microsoft.ML.Data

let _dataPath =
    Path.Combine(Environment.CurrentDirectory, "../..", "Data", "yelp_labelled.txt")

// +
type SentimentData =
    { [<LoadColumn 0>]
      SentimentText: string
      [<LoadColumn 1; ColumnName "Label">]
      Sentiment: bool }

[<CLIMutable>]
type SentimentPrediction =
    { [<ColumnName "PredictedLabel">]
      Prediction: bool
      Probability: single
      Score: single }
// -

let mlcontext = MLContext()

let LoadData (mlContext: MLContext) =
    let dataView =
        mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader = false)

    mlContext.Data.TrainTestSplit(dataView, testFraction = 0.2)

let splitDataView = LoadData(mlcontext)

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


// +
printfn "=============== Create and Train the Model ==============="

let model = BuildAndTrainModel mlcontext splitDataView.TrainSet

printfn "=============== End of training ==============="
// -

let Evaluate (mlContext: MLContext) (model: ITransformer) (splitTestSet: IDataView) =
    let predictions = model.Transform(splitTestSet)
    mlContext.BinaryClassification.Evaluate(predictions, "Label")

// +
printfn "=============== Evaluating Model accuracy with Test data==============="

let metrics = Evaluate mlcontext model splitDataView.TestSet

printfn "Model quality metrics evaluation"
printfn "--------------------------------"
printfn $"Accuracy: {metrics.Accuracy:P2}"
printfn $"Auc: {metrics.AreaUnderRocCurve:P2}"
printfn $"F1Score: {metrics.F1Score:P2}"
printfn "=============== End of model evaluation ==============="
// -

let UseModelWithSingleItem (mlContext: MLContext) (model: ITransformer) =
    let predictionFunction =
        mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model)
    { SentimentText = "This was a very bad steak"; Sentiment = false; }
    |> predictionFunction.Predict

// +
printfn "=============== Prediction Test of model with a single sample and test dataset ==============="

let resultPrediction = UseModelWithSingleItem mlcontext model
printfn $"""Prediction: {if resultPrediction.Prediction then "Positive" else "Negative"} | Probability: {resultPrediction.Probability} """

printfn "=============== End of Predictions ==============="
// -


