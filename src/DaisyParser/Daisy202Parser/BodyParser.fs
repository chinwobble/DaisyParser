namespace DaisyParser.Daisy202Parser

module BodyParser =
  open DaisyParser.Daisy202Parser.Domain
  open FSharp.Data
  open System
  open NodaTime.Text
  
  let mapToBodyElement (node: HtmlNode) : BodyElement option = 
    let allowedHeaderTags = ["h1"; "h2"; "h3"; "h4"; "h5"; "h6"]
    let hreference (nodeWithAnchor:HtmlNode) = 
      let anchor = nodeWithAnchor.Elements("a") |> Seq.head
      let href = anchor.AttributeValue("href")
      { Text = anchor.DirectInnerText() // will throw exception on malformed body
        File = href.Split('#').[0]
        Fragment = href.Split('#').[1] }

    match node.Name().ToLower() with
    | a when Seq.contains a allowedHeaderTags -> 
      { Heading.Id = node.AttributeValue "id"
        Anchor = hreference node }
      |> BodyElement.H1
      |> Some

    | "span" ->
      let decodeSpanClass = function 
        | "page-front" -> SpanClass.PageFront |> Some
        | "page-normal" -> SpanClass.PageNormal |> Some
        | "page-special" -> SpanClass.PageSpecial |> Some
        | _ -> None
      { Span.Id = node.AttributeValue "id" 
        Class = decodeSpanClass <| node.AttributeValue("class").ToLower()
        Anchor = hreference node }
      |> BodyElement.Span
      |> Some
    
    // | "div" -> 

    | _ -> None

  let toBodyElements (doc: HtmlDocument) = 
    doc
    |> HtmlDocumentExtensions.Body
    |> HtmlNodeExtensions.Descendants
    |> Seq.map mapToBodyElement
    |> Seq.choose id