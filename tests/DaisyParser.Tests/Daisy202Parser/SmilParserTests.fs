namespace DaisyParser.Tests.Daisy202Parser
open DaisyParser.Daisy202Parser

module SmilParserTests = 
  open DaisyParser.Daisy202Parser.BodyParser
  open DaisyParser.Daisy202Parser.Domain
  open FSharp.Data
  open NodaTime.Text
  open System.Linq
  open Expecto

  [<Tests>]
  let Tests =
    testList "" [
      testCase "dfd" <| fun _ ->
        let expected = true
        Expect.equal true expected "passed"
    ]

  [<Tests>]
  let ``Can Parse Body`` () = 
    let testBody = """
      <!DOCTYPE SMIL PUBLIC "-//W3C//DTD SMIL 1.0//EN" "http://www.w3.org/TR/REC-SMIL/SMIL10.dtd">
        <smil>
          <head>
            <meta name="ncc:generator" content="LpStudioGen v1.6" />
            <meta name="dc:identifier" content="DTB00345" />
            <meta name="dc:format" content="Daisy 2.02" />
            <meta name="dc:title" content="Economics" />
            <meta name="title" content="13. Monopoly" />
            <meta name="ncc:totalElapsedTime" content="14:04:48" />
            <meta name="ncc:timeInThisSmil" content="0:02:38" />
            <layout>
                <region id="txtView" />
            </layout>
          </head>
          <body>
            <seq dur="158.485s">
              <par endsync="last" id="ec79_0002">
                <text src="ncc.html#econ_0346" id="ec79_0003" />
                <audio src="econ25000c.mp3" id="ec79_0004" />
              </par>
              <par endsync="last" id="ec79_0005">
                <text src="ncc.html#econ_0348" id="ec79_0006" />
                <seq id="ec79_0007">
                    <audio src="econ25000d.mp3" clip-begin="npt=72.200s" clip-end="npt=74.659s" id="phrs_0011" />
                    <audio src="econ25000d.mp3" clip-begin="npt=74.659s" clip-end="npt=81.269s" id="phrs_0012" />
                    <audio src="econ25000d.mp3" clip-begin="npt=81.269s" clip-end="npt=91.691s" id="phrs_0013" />
                    <audio src="econ25000d.mp3" clip-begin="npt=91.691s" clip-end="npt=95.477s" id="phrs_0014" />
                    <audio src="econ25000d.mp3" clip-begin="npt=95.477s" clip-end="npt=110.575s" id="phrs_0015" />
                    <audio src="econ25000d.mp3" clip-begin="npt=110.575s" clip-end="npt=115.777s" id="phrs_0016" />
                    <audio src="econ25000d.mp3" clip-begin="npt=115.777s" clip-end="npt=119.980s" id="phrs_0017" />
                    <audio src="econ25000d.mp3" clip-begin="npt=119.980s" clip-end="npt=127.816s" id="phrs_0018" />
                    <audio src="econ25000d.mp3" clip-begin="npt=127.816s" clip-end="npt=135.288s" id="phrs_0019" />
                    <audio src="econ25000d.mp3" clip-begin="npt=135.288s" clip-end="npt=141.507s" id="phrs_0020" />
                    <audio src="econ25000d.mp3" clip-begin="npt=141.507s" clip-end="npt=158.485s" id="phrs_0021" />
                </seq>
              </par>
            </seq>
          </body>
        </smil>
      """
    let smilBody = 
      SmilParser.toSmilBody testBody
    
    let unwrappedSmilBody =
      smilBody.Value
    Expect.isTrue unwrappedSmilBody.Seq.IsNone "should be true"

    let firstAudio =
      unwrappedSmilBody.Par.First().Audio
      
    Expect.equal "econ25000c.mp3" firstAudio.Value.File "true"

    let secondAudio =
      unwrappedSmilBody.Par.Last().Seqs.Value.First()

    match secondAudio with 
    | SmilNestedSeq.Audio audioRefs ->
      audioRefs
      |> Seq.iter (fun x -> Expect.equal "econ25000d.mp3"  x.File "")
      
      let ref = audioRefs.First()
      Expect.equal 72.200 ref.ClipStart.Value.TotalSeconds ""
      Expect.equal 74.659 ref.ClipEnd.Value.TotalSeconds ""

      let lastref = audioRefs.Last()
      Expect.equal 141.507 lastref.ClipStart.Value.TotalSeconds ""
    | _ -> invalidArg "" "should be audio"
    ()