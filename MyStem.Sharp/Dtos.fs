module MyStem.Sharp.Dtos

type Defention(gr,lex) =
    member this.gr = gr
    member this.lex = lex

type WordDefention(text, analysis:list<Defention>) = 
    member this.text = text
    member this.analysis = analysis
    member this.GetText() : string = 
        match this.analysis with
        | [] -> this.text
        | _ -> analysis.Head.lex