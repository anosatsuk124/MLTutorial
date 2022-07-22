// %%
#r "nuget: SixLabors.ImageSharp"

// %%
#r "nuget: Plotly.NET, 2.0.0"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.12"

// %%
open SixLabors.ImageSharp
open SixLabors.ImageSharp.Processing

// %%
open System
open System.IO

// %%
let _dataPath =
    Path.Combine(Environment.CurrentDirectory, "..", "Data/dataset/images")

// %%
let image: Image<PixelFormats.Rgba32> =
    Image.Load(Path.Combine(_dataPath, "imori_256x256.png"))

// %%
open Plotly.NET

// %%
let ImageShow (img: Image) =
    let ms = new MemoryStream()
    let b64 = Convert.ToBase64String(img.SaveAsPng ms |> ms.ToArray)
    Chart.Image(
        Source=($"data:image/jpg;base64,{b64}")
    )

// %%
imageShow image

// %%
let image2 = image.Clone(ignore)
image2.Mutate (fun x -> x.Resize (image2.Width/2, image2.Height/2)  |> ignore)

// %%
ImageShow image2

// %%
[
ImageShow image
ImageShow image2
] |> Chart.Grid (1,2)

// %%
let image3 = image.Clone(ignore)

// %%
image3.ProcessPixelRows (fun accessor ->
    printfn $"{accessor.GetRowSpan(0).Length}" 
    |> ignore)

// %%
