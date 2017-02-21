module MyStem.Sharp.Wrapper

open MyStem.Sharp.Dtos
open System.Diagnostics
open System.Text
open System.IO
open Newtonsoft.Json.Linq

type MyStemWrapper (path: string) = 
    
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
               
    let cast(array:JArray) : list<WordDefention> =
        array.ToObject<list<WordDefention>>()
                    
    member this.Lemmatize (text:string) : list<WordDefention> =
        lock lockObject (fun () -> getProcessOutput text mystemProc reader)
            |> JArray.Parse
            |> cast