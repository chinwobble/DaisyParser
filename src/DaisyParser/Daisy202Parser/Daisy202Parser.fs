namespace DaisyParser.Parser.Daisy202

open FParsec
open FSharp.Data
open DaisyParser.Parser.Common.Common
module HeadParsers =
  let headParser<'a> = 
    BetweenTags "head"
  
  
  // let headParser' =
  //   headParser <| manyChars
    
