namespace DaisyParser.Tests.Daisy202Parser
open NUnit.Framework

[<TestFixture>]
module BodyParserTests = 
  open DaisyParser.Daisy202Parser.BodyParser
  open FSharp.Data
  open NUnit.Framework
  open DaisyParser.Daisy202Parser.Domain

  [<Test>]
  let ``Can Parse Body`` () = 
    let testBody = 
      """
      <body>
        <h1 class="title" id="econ_0001"><a href="econ0001.smil#ec1a_0001">Economics by Richard G. Lipsey et al..</a></h1>
        <h1 id="econ_0002"><a href="econ0002.smil#ec2a0001">Information about the talking book</a></h1>
        <h1 id="econ_0003"><a href="econ0003.smil#ec3a0001">Contents in Brief</a></h1>
        <span class="page-front" id="econ_0004"><a href="econ0003.smil#ec3a0002">iv</a></span>
        <span class="page-front" id="econ_0005"><a href="econ0003.smil#ec3a0003">v</a></span>
        <span class="page-front" id="econ_0006"><a href="econ0003.smil#ec3a0004">vi</a></span>
        <span class="page-normal" id="econ_0021"><a href="econ0008.smil#ec8a0003">1</a></span>
        <h1 class="part" id="econ_0022"><a href="econ0009.smil#ec9a0001">Part 1. The nature of economics</a></h1>
        <h2 class="chapter" id="econ_0023"><a href="econ0010.smil#ec100001">1. The economic problem</a></h2>
        <span class="page-normal" id="econ_0024"><a href="econ0010.smil#ec100002">2</a></span>
        <span class="page-normal" id="econ_0025"><a href="econ0010.smil#ec100003">3</a></span>
        <h3 id="econ_0026"><a href="econ0010.smil#ec100004">What is economics?</a></h3>
        <h4 id="econ_0027"><a href="econ0010.smil#ec100005">Resources and commodities</a></h4>
        <h4 id="econ_0028"><a href="econ0010.smil#ec100006">Scarcity</a></h4>
        <h4 id="econ_0029"><a href="econ0010.smil#ec100007">Choice</a></h4>
        <span class="page-normal" id="econ_0031"><a href="econ0010.smil#ec100008">4</a></span>
        <span class="page-special" id="econ_0934"><a href="econ0085.smil#ec850027">G-1</a></span>
        <h1 class="glossary" id="econ_0935"><a href="econ0086.smil#ec860001">Glossary</a></h1>
        <div id="econ_0936" class="group"><a href="econ0086.smil#ec860002">Glossary item 1</a></div>
        <div id="econ_0937" class="group"><a href="econ0086.smil#ec860003">Glossary item 2</a></div>
        <div id="econ_0938" class="group"><a href="econ0086.smil#ec860004">Glossary item 3</a></div>
        <h1 id="econ_0946"><a href="econ0088.smil#ec880001">Ending announcement</a></h1>
      </body>
      """
    let nccBody = 
      HtmlDocument.Parse(testBody)
      |> toBodyElements
    
    Assert.AreEqual(19, Seq.length nccBody)
    Assert.AreEqual( H1 { Heading.Id = "econ_0001"
                          Anchor = { Text = "Economics by Richard G. Lipsey et al.."
                                     File = "econ0001.smil"
                                     Fragment = "ec1a_0001" } }
                                 , Seq.head nccBody)
    ()