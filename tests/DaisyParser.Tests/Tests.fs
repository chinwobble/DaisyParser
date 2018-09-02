namespace DaisyParser.Tests

open Expecto
open FsCheck
open GeneratorsCode

module Tests =
    let config10k = { FsCheckConfig.defaultConfig with maxTest = 10000}
    // bug somewhere:  registering arbitrary generators causes Expecto VS test adapter not to work
    //let config10k = { FsCheckConfig.defaultConfig with maxTest = 10000; arbitrary = [typeof<Generators>] }
    let configReplay = { FsCheckConfig.defaultConfig with maxTest = 10000 ; replay = Some <| (1940624926, 296296394) }

    [<Tests>]
    let testSimpleTests =
        testList "DomainTypes.Tag" [
            testCase "equality test" <| fun () -> 
                let result = 41 
                Expect.isTrue (result = 41) "Expected true"
            testCase "equality" <| fun () ->
                let result = 42
                Expect.isTrue (result = 42) "Expected True"

            testPropertyWithConfig config10k "whitespace" <|
                fun  () ->
                    Prop.forAll (Arb.fromGen <| whitespaceString())
                        (fun (x : string) -> 
                            x = x)
        ]

