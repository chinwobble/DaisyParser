namespace DaisyParser.Daisy202Parser

module BodyParser =
  open DaisyParser.Daisy202Parser.Domain
  open FSharp.Data
  open System
  open NodaTime.Text
  

  let bodyNodes (body: HtmlDocument) = 
    body
    |> HtmlDocumentExtensions.Body
    |> HtmlNodeExtensions.Descendants
  let doStuff =
    ()