namespace DaisyParser.Daisy202Parser

module Common = 
  open NodaTime
  open NodaTime.Text
  
  let toOption (input: ParseResult<'T>) =
    if input.Success 
    then Some input.Value
    else None
    
  let parseDuration (input : string) : Duration option = 
    let parser = DurationPattern.CreateWithCurrentCulture("S.fff")
    parser.Parse(input.Split('s').[0])
    |> toOption
