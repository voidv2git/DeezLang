namespace DeezLang
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "C:\\Users\\nicka\\OneDrive\\Documents\\Programming Projects\\C# Projects\\Console Apps\\DeezLang\\DeezLang\\main.deez";
            string outputFile = "C:\\Users\\nicka\\OneDrive\\Documents\\Programming Projects\\C# Projects\\Console Apps\\DeezLang\\DeezLang\\main.asm";
            string source = "";

            try
            {
                source = File.ReadAllText(inputFile) + '\n';
            }
            catch
            {
                Output.Error(inputFile, 0, Output.ErrorType.FileError, "Could Not Read File.");
            }

            Output.CompilationTimerStart();

            Lexer lexer = new Lexer(source);
            lexer.Lex();

            Compiler compiler = new Compiler(lexer.tokens, inputFile);
            compiler.Compile();

            Output.CompilationTimerEnd();

            File.WriteAllText(outputFile, compiler.outputASM);
        }
    }
}