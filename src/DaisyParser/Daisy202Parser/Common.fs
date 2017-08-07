namespace DaisyParser.Daisy202Parser

open FParsec

module Common =
  open NodaTime.Text
  open NodaTime

  let parseDuration (input: string) : Duration option = 
    let parser = DurationPattern.CreateWithCurrentCulture("S.fff")
    if parser.Parse(input.Split('s').[0]).Success
    then parser.Parse(input.Split('s').[0]).Value |> Some
    else None

  let commentsParser<'b> : Parser<string, 'b> = 
    between
      (pstring "<!--")
      (pstring "-->")
      (charsTillString "-->" false System.Int32.MaxValue)

  let rec nestedCommentParser o = 
    let ign x = charsTillString x false System.Int32.MaxValue
    between
      (pstring "<!--")
      (pstring "-->")
      (attempt (ign "<!--" >>. nestedCommentParser >>. ign "-->") <|> ign "-->") <| o
  

  let BetweenTags str = 
    let startTag tagName =
      sprintf "<%s>" tagName
    let endTag tagName =
      sprintf "</%s>" tagName
    between (pstring <| startTag str) (pstring <| endTag str)
