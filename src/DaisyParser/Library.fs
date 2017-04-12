namespace DaisyParser.Library

open FParsec

module DoStuff = 
  let valToParse = "50"
  let test p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg 
    
  test pfloat "1.25"
  test pfloat "1.25E 2"
  
  let betweenSqrBrackets<'a> : (Parser<'a,unit> -> Parser<'a,unit>) = 
    between ((pstring "[") .>> spaces) ((pstring "]") .>> spaces)
  test (betweenSqrBrackets pfloat) "[ 1] "
  test (betweenSqrBrackets pfloat) "[]"
  test (betweenSqrBrackets pfloat) "[1.0]"
  test (many (betweenSqrBrackets pfloat)) "[10] [10]"

  // comma separted inside brackets
  let separatedByComma : (Parser<float list, unit>) =
    sepBy pfloat (pstring ",")
  
  let ``betweenSquaredBrackets and SeparatedbyCommas`` =
    (betweenSqrBrackets separatedByComma)
  test ``betweenSquaredBrackets and SeparatedbyCommas`` "[10,10]"
  