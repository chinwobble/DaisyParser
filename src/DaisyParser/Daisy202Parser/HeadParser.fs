namespace DaisyParser.Daisy202Parser

module HeadParser =

    open DaisyParser.Daisy202Parser.Domain
    open FSharp.Data
    open System
    open NodaTime.Text
    
    let private metaTuples (document: HtmlDocument) = 
      document.Descendants "head"
      |> Seq.head
      |> HtmlNode.elementsNamed ["title" ; "meta"]
      |> Seq.filter (HtmlNode.hasName "meta")
      |> Seq.map (fun x -> 
        ( HtmlNode.attributeValue "name" x
        , HtmlNode.attributeValue "content" x
        , HtmlNode.attributeValue "scheme" x ))
    
    let private findByKey (key: string) (kvps: (string*string*string) seq): (string*string) =
        let (_, v, s) = 
          kvps
          |> Seq.find (fun (k, _, _) -> k.ToLower() = key.ToLower())
        (v, s)
            
    let createRecord (document:HtmlDocument)  = 
      let getValueFromNode (key: string) = 
        metaTuples document
        |> findByKey key
        |> fst

      let parseTotal = 
        let (value, scheme) =
          metaTuples document
          |> findByKey "ncc:totaltime"

        let totalTimeParser = 
          scheme
          |> (fun scheme -> 
              if scheme = "hh:mm:ss"
              then "H:mm:ss"
              else scheme)
          |> DurationPattern.CreateWithCurrentCulture

        let parserResult = totalTimeParser.Parse(value)
        parserResult.Value.TotalMinutes |> int

      { DaisyMetadata.Creator = getValueFromNode "dc:creator"
        Date = DateTime.Now
        Format = getValueFromNode "dc:format"
        Publisher = getValueFromNode "dc:publisher"
        Title = getValueFromNode "dc:title"
        Charset = getValueFromNode "ncc:charset"
        PageFront = getValueFromNode "ncc:pageFront" |> int
        PageNormal = getValueFromNode "ncc:pagenormal" |> int
        PageSpecial = getValueFromNode "ncc:pageSpecial" |> int
        TocItems = getValueFromNode "ncc:tocItems" |> int
        TotalTime = parseTotal
        OptionalMetaData = [] }
    
