using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeezLang
{
    internal class Output
    {
        public enum ErrorType
        {
            SyntaxError,
            FileError,
            UndefinedError,
        }

        private static Stopwatch timer = new Stopwatch();

        public static void Error(string fileName, int line, ErrorType type, string msg)
        {
            Console.Error.WriteLine($"\u001b[30;1m{fileName}\u001b[0m:\u001b[33;1m{line}\u001b[0m: \u001b[31;1m{type}\u001b[0m: {msg}");
            Environment.Exit(1);
        }

        public static void CompilationTimerStart()
        {
            timer.Start();
        }

        public static void CompilationTimerEnd()
        {
            timer.Stop();
            Console.WriteLine($"Compilation \u001b[33;1mFinished\u001b[0m In \u001b[33;1m{timer.Elapsed.TotalMilliseconds}\u001b[0m ms.");
        }
    }
}
