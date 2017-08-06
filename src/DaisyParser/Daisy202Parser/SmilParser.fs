namespace DaisyParser.Daisy202Parser

module SmilParser =
  open DaisyParser.Daisy202Parser.Domain
  open FSharp.Data
  open System
  open NodaTime.Text

  let toBodyElements (doc: HtmlDocument) = 
    doc
    |> HtmlDocumentExtensions.Body
    |> HtmlNodeExtensions.Descendants