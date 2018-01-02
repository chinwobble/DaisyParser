namespace DaisyParser.Daisy202Parser

open Domain
open System.Linq

module Processor =
  let getAudio (refId:string) (smilBody:SmilBody) =
    let getFileFromMatchingPar (par: SmilPar) =
      par.Audio
      |> Option.orElse(
        par.Seqs
        |> Option.map (fun x -> x.First())
        |> Option.bind(function 
          | SmilNestedSeq.Audio a ->
            a.First() |> Some
          | _ -> None
        )
      )

    let tryFindByParId () = 
      smilBody.Par
      |> Seq.tryFind(fun x -> x.Id = refId)

    let tryFindByTextId () =
      smilBody.Par
      |> Seq.tryFind (fun p -> p.Text.Id = refId)
    
    tryFindByTextId ()
    |> Option.orElseWith tryFindByParId 
    |> Option.bind getFileFromMatchingPar
    
    