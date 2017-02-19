namespace MyStem.Sharp.Tests

open NUnit.Framework
open MyStem.Sharp.Wrapper
open System.Threading.Tasks

[<TestFixture>]
type Tests() = 
    let myStemPath = @"E:\Systems\alkozko\Downloads\mystem\mystem.exe"

    [<Test>]
    member  this.SimpleMyStemTest() =
        let wrapper = new MyStemWrapper(myStemPath) 
        let lemmas = wrapper.Lemmatize "Съешь ещё этих мягких булочек и выпей чаю"
        ()
    
    [<Test>]
    member  this.MultiThreadingTest() =
        let wrapper = new MyStemWrapper(myStemPath) 
        let t1 = Task.Run(fun () ->
            let lemmas = wrapper.Lemmatize @"Программа mystem производит морфологический анализ текста на русском языке. 
            Она умеет строить гипотетические разборы для слов, не входящих в словарь. 
            Первую версию программы написали Илья Сегалович и Виталий Титов."
            Assert.AreEqual("программа",lemmas.Head.GetText())
            ()
        )

        let lemmas = wrapper.Lemmatize "Облака - белокрылые лошадки"
        
        t1.Wait()

        Assert.AreEqual("облако",lemmas.Head.GetText())
       
        ()