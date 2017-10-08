namespace MyStem.Sharp
{
    public struct Defenition
    {
        public string Gr { get; }
        public string Lex { get; }

        public Defenition(string gr, string lex)
        {
            Gr = gr;
            Lex = lex;
        }
    }
}