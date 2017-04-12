namespace DaisyParser.Tests

open DaisyParser.Library
open NUnit.Framework
module DoStuff = 
  [<Test>]
  let ``hello returns 42`` () =
    Assert.AreEqual(42,42)
  [<Test>]
  let ``more test`` () = 
    Assert.AreEqual(42,42)
