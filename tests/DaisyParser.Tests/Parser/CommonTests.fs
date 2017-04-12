namespace DaisyParser.Tests

open DaisyParser.Parser.Common.Common
open NUnit.Framework
open DaisyParser.Library.DoStuff
open FParsec
module CommonTests =
  [<Test>]
  let ``Skip comment Works`` () = 
    test commentsParser """<!-- <!-- -->
      -->"""
    ()
  