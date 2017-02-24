module MyStem.Sharp.Wrapper

open System.Diagnostics
open System.Text
open System.IO
open Newtonsoft.Json.Linq

type Defention(gr:string,lex:string) =
    member this.gr = gr
    member this.lex = lex

type WordDefention(text, analysis:Defention[]) = 
    member this.text = text
    member this.analysis = analysis
    member this.GetText() : string = 
        match this.analysis |> Array.length with
        | 0 -> this.text
        | _ -> analysis.[0].lex

type Lemmatizer (path: string) = 
    
    let lockObject = ref 4

    let createStartInfo path =
        let startInfo = new ProcessStartInfo()
        do
            startInfo.UseShellExecute <- false
            startInfo.RedirectStandardOutput <- true
            startInfo.RedirectStandardError <- true
            startInfo.RedirectStandardInput <- true
            startInfo.CreateNoWindow <- true
            startInfo.WindowStyle <- ProcessWindowStyle.Hidden
            startInfo.FileName <- path
            startInfo.Arguments <- " -lcdi --format json"
        startInfo
    
    let createProcess path:Process =
        let proc = new Process (StartInfo = createStartInfo path)
        proc.Start()
        proc
    
    let mystemProc = createProcess path
    let reader = new StreamReader(mystemProc.StandardOutput.BaseStream, Encoding.UTF8)

    let getProcessOutput text (proc:Process) (reader:StreamReader) = 
        let buffer = Encoding.UTF8.GetBytes(text + "\r\n")
        proc.StandardInput.BaseStream.Write(buffer,0,buffer.Length)
        proc.StandardInput.BaseStream.Flush()
        reader.ReadLine()
                             
    let call (action: ('T -> 'R)) (obj:'T) : 'R =
        action obj

    member this.Lemmatize (text:string) : WordDefention[] =
        lock lockObject (fun () -> getProcessOutput text mystemProc reader)
            |> JArray.Parse
            |> call (fun a -> a.ToObject<list<WordDefention>>())
            |> List.toArray

