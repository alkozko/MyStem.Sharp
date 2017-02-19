namespace MyStem.Sharp.Tests

open NUnit.Framework
open MyStem.Sharp.Wrapper

[<TestFixture>]
type Tests() = 
    let myStemPath = @"E:\Systems\alkozko\Downloads\mystem\mystem.exe"

    [<Test>]
    member  this.simpleMyStemTest() =
        let wrapper = new MyStemWrapper(myStemPath) 
        let lemmas = wrapper.Lemmatize "Съешь ещё этих мягких булочек и выпей чаю"
        ()