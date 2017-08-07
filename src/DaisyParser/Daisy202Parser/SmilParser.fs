namespace DaisyParser.Daisy202Parser

// this module attempts to implement section 2.3 and 2.4 of the daisy202 spec
// http://www.daisy.org/z3986/specifications/daisy_202.html?q=publications/specifications/daisy_202.html#textdoc
module SmilParser =
  open DaisyParser.Daisy202Parser.Domain
  open FSharp.Data
  open System
  open NodaTime.Text
  open Common

  let toBodyElements (doc: HtmlDocument) = 
    doc
    |> HtmlDocumentExtensions.Body
    |> HtmlNodeExtensions.Descendants

  let rec toPar (par: HtmlNode) : (SmilPar Option) = 
    let toSmilText (textNode: HtmlNode) : SmilTextReference = 
      let fileReference = textNode.AttributeValue("src").Split('#')
      { Id           = textNode.AttributeValue("value")
        File         = fileReference.[0]
        Fragment     = fileReference.[1] }
      
    let toSmilParChildren nestedSeqNodes : SmilParChildren Option = 
      match nestedSeqNodes with 
      | a when (Array.filter (HtmlNode.hasName "audio") a).Length = 1 ->
        let audioNode = Seq.find (HtmlNode.hasName "audio") a
        { File = audioNode.AttributeValue("src")
          ClipStart = None
          ClipEnd   = None
          Id        = audioNode.AttributeValue("id")}
        |> SmilParChildren.Audio
        |> Some

      | a when (Array.filter (HtmlNode.hasName "seq") a).Length = 1 ->
        let toRecord file id clipStart clipEnd = 
          { File      = file
            ClipStart = Some clipStart
            ClipEnd   = Some clipEnd
            Id        = id }
        a 
        |> Array.find (HtmlNode.hasName "seq")
        |> HtmlNodeExtensions.Elements 
        |> Seq.filter (HtmlNode.hasName "audio")
        |> Seq.map (fun audioNode -> 
          let toRecordClip = toRecord (audioNode.AttributeValue("src")) (audioNode.AttributeValue("id"))
          let durationResult attr = 
            parseDuration (audioNode.AttributeValue(attr).Split('=').[1])
          Option.map2 toRecordClip (durationResult "clip-begin") (durationResult "clip-begin"))
        |> Seq.choose id
        |> SmilNestedSeq.Audio
        |> SmilParChildren.Seqs
        |> Some
      | _ -> None
      
    match toSmilParChildren (HtmlNode.elements par |> Seq.toArray) with 
    | Some children -> 
      Some { SmilPar.Id = par.AttributeValue("id")
             Text = toSmilText (HtmlNode.elementsNamed ["text"] par |> Seq.head)
             Children = children } 
    | None -> None
      

  let smilBodyOuterSeq (node:HtmlNode) = 
    match parseDuration (node.AttributeValue("dur")) with 
    | Some dur ->   
      Some { SmilBody.Duration = dur
             Par = node.Elements "par" |> Seq.map toPar |> Seq.choose id 
             Seq = None }
    | _ -> None