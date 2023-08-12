namespace DeezLang
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Output.Error("null", 0, Output.ErrorType.UsageError, "[USAGE] DeezLang [inputFile] [outputFile]");
            }

            string inputFile = args[1];
            string outputFile = args[2];

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
