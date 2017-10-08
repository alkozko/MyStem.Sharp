namespace MyStem.Sharp.Tests

open NUnit.Framework
open MyStem.Sharp
open System.Threading.Tasks
open System

[<TestFixture>]
type Tests() = 
    let myStemPath = MyStemDownloader().GetLocalPath()

    [<Test>]
    member  this.SimpleMyStemTest() =
        let wrapper = new Lemmatizer(myStemPath) 
        let lemmas = wrapper.Lemmatize "Съешь ещё этих мягких булочек и выпей чаю"
        Assert.AreEqual("съедать",lemmas.[0].GetText())
        ()
    
    [<Test>]
    member  this.MultiThreadingTest() =
        let wrapper = new Lemmatizer(myStemPath) 
        let t1 = Task.Run(fun () ->
            let lemmas = wrapper.Lemmatize @"Программа mystem производит морфологический анализ текста на русском языке. 
            Она умеет строить гипотетические разборы для слов, не входящих в словарь. 
            Первую версию программы написали Илья Сегалович и Виталий Титов."
            Assert.AreEqual("программа",lemmas.[0].GetText())
            ()
        )

        let lemmas = wrapper.Lemmatize "Облака - белокрылые лошадки"
        t1.Wait()
        Assert.AreEqual("облако",lemmas.[0].GetText())
        ()

    [<Test>]
    member  this.``Don't throw NRE when Null Aanalysis``() =
        let wrapper = new Lemmatizer(myStemPath) 
        let text = wrapper.Lemmatize ("Облака - белокрылые лошадки") 
                |> Seq.map (fun a -> a.GetText()) 
                |> String.concat " "
        Assert.AreEqual ("облако  -  белокрылый   лошадка ", text)
        ()

    [<Test>]
    member  this.``SuperTest``() =
        let wrapper = new Lemmatizer(myStemPath) 
        let text = wrapper.Lemmatize ("Программа mystem производит морфологический анализ текста на русском языке") 
                |> Seq.map (fun a -> if a.Analysis.Length > 0 then (sprintf "%s {%s}" a.Analysis.[0].Lex a.Analysis.[0].Gr) else a.GetText()) 
                |> String.concat " "
        Console.WriteLine text
        ()