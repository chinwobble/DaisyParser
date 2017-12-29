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
    let (|TextRef|SequenceRef|Audio|) (node: HtmlNode) =
      if node.HasName "audio"  then Audio 
      elif node.HasName "seq"  then SequenceRef 
      elif node.HasName "text" then TextRef
      else invalidArg "" ""

    let toSmilText (textNode: HtmlNode) : SmilTextReference = 
      let fileReference = textNode.AttributeValue("src").Split('#')
      { Id           = textNode.AttributeValue("value")
        File         = fileReference.[0]
        Fragment     = fileReference.[1] }
      
    let toSmilAudioReference (audioNode: HtmlNode) : SmilAudioReference =
      { Id          = audioNode.AttributeValue("id")
        File        = audioNode.AttributeValue("src")
        ClipEnd     = None // todo fix 
        ClipStart   = None } // todo fix
    
    // todo handle seq with par (used for notes)
    let toNestedSeq (seqNodes: HtmlNode List) =
        seqNodes
        |> Seq.filter ((fun x -> x.Elements "audio") >> Seq.isEmpty >> not)
        |> Seq.map (fun sn -> 
          sn.Elements "audio"
          |> Seq.map toSmilAudioReference)
        |> Seq.map SmilNestedSeq.Audio
      
    {
      Id = par.AttributeValue("id")
      Text = par.Elements("text") |> Seq.last |> toSmilText
      Audio = None
      Seqs = toNestedSeq (par.Elements "seq") |> Some
    } |> Some
    
      

  let smilBodyOuterSeq (node:HtmlNode) = 
    parseDuration (node.AttributeValue("dur"))
    |> Option.map (fun dur ->
      Some { SmilBody.Duration = dur
             Par = node.Elements "par" 
                    |> Seq.map toPar 
                    |> Seq.choose id 
             Seq = None })