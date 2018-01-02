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

  let toPar (par: HtmlNode) : (SmilPar Option) = 
    let toSmilText (textNode: HtmlNode) : SmilTextReference = 
      let fileReference = textNode.AttributeValue("src").Split('#')
      { Id           = textNode.AttributeValue("id")
        File         = fileReference.[0]
        Fragment     = fileReference.[1] }
      
    let toSmilAudioReference (audioNode: HtmlNode) :SmilAudioReference =
      let parseBeginEnd (clipString:string) =
        let parser = DurationPattern.CreateWithInvariantCulture("SS.FFFFFFFFF")
        parser.Parse(clipString.Replace("npt=", "").Replace("s", ""))
        |> toOption
      { Id          = audioNode.AttributeValue("id")
        File        = audioNode.AttributeValue("src")
        ClipEnd     = audioNode.AttributeValue("clip-end") |> parseBeginEnd
        ClipStart   = audioNode.AttributeValue("clip-begin")   |> parseBeginEnd } 
    
    // todo handle seq with par (used for notes)
    let toNestedSeq (seqNodes: HtmlNode List) =
        seqNodes
        |> Seq.filter ((fun x -> x.Elements "audio") >> Seq.isEmpty >> not)
        |> Seq.map (fun sn -> 
          sn.Elements "audio"
          |> Seq.map toSmilAudioReference)
        |> Seq.map SmilNestedSeq.Audio
    
    let tryGetAudioRef (nodes:HtmlNode List) = 
      if nodes.Length = 1 
      then Seq.last nodes 
        |> toSmilAudioReference 
        |> Some
      else None 
    {
      Id = par.AttributeValue("id")
      Text = par.Elements("text") |> Seq.last |> toSmilText
      Audio = tryGetAudioRef <| par.Elements "audio"
      Seqs =  if (par.Elements "seq").Length > 0 
              then toNestedSeq (par.Elements "seq") |> Some
              else None
    } |> Some
    
  let smilBodyOuterSeq (node:HtmlNode) = 
    parseDuration (node.AttributeValue("dur"))
    |> Option.map (fun dur ->
           { SmilBody.Duration = dur
             Par = node.Elements "par" 
                    |> Seq.map toPar 
                    |> Seq.choose id 
             Seq = None })