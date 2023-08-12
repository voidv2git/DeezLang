using System.Text;

namespace DeezLang
{
    internal class Compiler
    {
        public List<Lexer.Token> tokens;
        public string fileName;

        public Compiler(List<Lexer.Token> tokens, string fileName)
        {
            this.tokens = tokens;
            this.fileName = fileName;
        }

        private int i = 0;
        private int variableID = 0;

        private List<string> definedVariables = new List<string>();
        private Stack<string> statementStack = new Stack<string>();

        private Dictionary<string, string> definedMacros = new Dictionary<string, string>();
        private Dictionary<string, string> definedConstants = new Dictionary<string, string>();

        public string codeSection = "";
        public string dataSection = "";
        public string outputASM = "";

        public void Compile()
        {
            dataSection += "include \\masm32\\include\\masm32rt.inc\n";
            dataSection += ".data\n";

            codeSection += ".code\n";
            codeSection += "start:\n";
            codeSection += "push 0\n";

            while (i < tokens.Count)
            {
                if (tokens[i].type == Lexer.TokenTypes.DefineKeyword)
                {
                    i++;
                    if (tokens[i].type != Lexer.TokenTypes.Identifier)
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'Identifier', Not '{tokens[i].type}'.");
                    }

                    string name = tokens[i].value;
                    definedVariables.Add(name);
                    i++;

                    if (tokens[i].type == Lexer.TokenTypes.Int)
                    {
                        string value = tokens[i].value;
                        dataSection += $"{name} dd {value}\n";
                    }
                    else if (tokens[i].type == Lexer.TokenTypes.String)
                    {
                        string value = tokens[i].value;
                        DefineString(name, value);
                    }
                    else if (tokens[i].type == Lexer.TokenTypes.BufferKeyword)
                    {
                        i++;
                        if (tokens[i].type != Lexer.TokenTypes.Int && !definedConstants.ContainsKey(tokens[i].value))
                        {
                            Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'Int' Or 'Constant', Not '{tokens[i].type}'.");
                        }

                        string value = tokens[i].value;
                        
                        if (definedConstants.ContainsKey(tokens[i].value))
                        {
                            dataSection += $"{name} dd {definedConstants[value]} dup(?)\n";
                        }
                        else
                        {
                            dataSection += $"{name} dd {value} dup(?)\n";
                        }
                    }
                    else
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'Int', 'String', Or 'Buffer', Not '{tokens[i].type}'.");
                    }
                }

                else if (tokens[i].type == Lexer.TokenTypes.ConstKeyword)
                {
                    i++;
                    if (tokens[i].type != Lexer.TokenTypes.Identifier)
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'Identifier', Not '{tokens[i].type}'.");
                    }

                    string name = tokens[i].value;
                    i++;

                    if (tokens[i].type != Lexer.TokenTypes.Int)
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'Int', Not '{tokens[i].type}'.");
                    }

                    string value = tokens[i].value;
                    definedConstants[name] = value;
                }

                else if (tokens[i].type == Lexer.TokenTypes.UsingKeyword)
                {
                    i++;
                    if (tokens[i].type != Lexer.TokenTypes.String)
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'String', Not '{tokens[i].type}'.");
                    }

                    string path = tokens[i].value;
                    string source = "";

                    try
                    {
                        source = File.ReadAllText(path) + '\n';
                    }
                    catch
                    {
                        Output.Error(path, 0, Output.ErrorType.FileError, "Could Not Read File.");
                    }

                    Lexer lexer = new Lexer(source);
                    lexer.Lex();

                    InsertTokensIntoTokens(lexer.tokens);
                }

                else if (tokens[i].type == Lexer.TokenTypes.IfKeyword)
                {
                    i++;
                    string _operator = tokens[i].value;
                    if (_operator != "==" && _operator != "!=" && _operator != ">=" && _operator != "<=" && _operator != ">" && _operator != "<")
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.UndefinedError, $"Invalid Operator '{tokens[i].value}'.");
                    }

                    codeSection += $"pop ebx\n";
                    codeSection += $"pop eax\n";
                    codeSection += $".if eax {_operator} ebx\n";
                    statementStack.Push("if");
                }

                else if (tokens[i].type == Lexer.TokenTypes.WhileKeyword)
                {
                    i++;
                    string _operator = tokens[i].value;
                    if (_operator != "==" && _operator != "!=" && _operator != ">=" && _operator != "<=" && _operator != ">" && _operator != "<")
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.UndefinedError, $"Invalid Operator '{tokens[i].value}'.");
                    }

                    codeSection += $"pop ebx\n";
                    codeSection += $"pop eax\n";
                    codeSection += $"push eax\n";
                    codeSection += $"push ebx\n";
                    codeSection += $".while eax {_operator} ebx\n";
                    statementStack.Push("while");
                }

                else if (tokens[i].type == Lexer.TokenTypes.ElseKeyword)
                {
                    string type = "";

                    try
                    {
                        type = statementStack.Pop();
                        statementStack.Push(type);
                    }
                    catch
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"'else' Keyword Missing Statement.");
                    }

                    if (type != "if")
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"'{type}' Statement Does Not Support 'else' Keyword.");
                        codeSection += ".else\n";
                    }

                    codeSection += ".else\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.EndKeyword)
                {
                    string type = "";

                    try
                    {
                        type = statementStack.Pop();
                    }
                    catch
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"No Statement To End.");
                    }

                    if (type == "if")
                    {
                        codeSection += ".endif\n";
                    }
                    else if (type == "while")
                    {
                        codeSection += $"pop ebx\n";
                        codeSection += $"pop eax\n";
                        codeSection += $"push eax\n";
                        codeSection += $"push ebx\n";
                        codeSection += ".endw\n";
                        codeSection += $"pop eax\n";
                        codeSection += $"pop eax\n";
                    }
                }

                else if (tokens[i].type == Lexer.TokenTypes.MacroKeyword)
                {
                    i++;
                    if (tokens[i].type != Lexer.TokenTypes.Identifier)
                    {
                        Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'Identifier', Not '{tokens[i].type}'.");
                    }

                    string name = tokens[i].value;
                    i++;
                    string asm = "";

                    while (tokens[i].type != Lexer.TokenTypes.EndKeyword)
                    {
                        if (tokens[i].type != Lexer.TokenTypes.String)
                        {
                            Output.Error(fileName, tokens[i].line, Output.ErrorType.SyntaxError, $"Expected Type 'String', Not '{tokens[i].type}'.");
                        }

                        asm += tokens[i].value + '\n';
                        i++;
                    }

                    definedMacros[name] = asm;
                }

                else if (tokens[i].type == Lexer.TokenTypes.PtrKeyword)
                {
                    codeSection += $"pop eax\n";
                    codeSection += $"push [eax]\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.DupKeyword)
                {
                    codeSection += $"pop eax\n";
                    codeSection += $"push eax\n";
                    codeSection += $"push eax\n";
                }

                else if (definedMacros.ContainsKey(tokens[i].value))
                {
                    codeSection += definedMacros[tokens[i].value];
                }

                else if (definedConstants.ContainsKey(tokens[i].value))
                {
                    codeSection += $"push {definedConstants[tokens[i].value]}\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.Int)
                {
                    codeSection += $"push {tokens[i].value}\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.String)
                {
                    string value = tokens[i].value;
                    DefineString($"variable{variableID}", value);

                    codeSection += $"push offset variable{variableID}\n";

                    variableID++;
                }

                else if (definedVariables.Contains(tokens[i].value))
                {
                    codeSection += $"push offset {tokens[i].value}\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.AddOperator)
                {
                    codeSection += $"pop ebx\n";
                    codeSection += $"pop eax\n";
                    codeSection += $"add eax, ebx\n";
                    codeSection += $"push eax\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.SubOperator)
                {
                    codeSection += $"pop ebx\n";
                    codeSection += $"pop eax\n";
                    codeSection += $"sub eax, ebx\n";
                    codeSection += $"push eax\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.MulOperator)
                {
                    codeSection += $"pop ebx\n";
                    codeSection += $"pop eax\n";
                    codeSection += $"mul eax, ebx\n";
                    codeSection += $"push eax\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.DivOperator)
                {
                    codeSection += $"pop ebx\n";
                    codeSection += $"pop eax\n";
                    codeSection += $"div eax, ebx\n";
                    codeSection += $"push eax\n";
                }

                else if (tokens[i].type == Lexer.TokenTypes.EqualsOperator)
                {
                    codeSection += $"pop ebx\n";
                    codeSection += $"pop eax\n";
                    codeSection += $"mov [eax], ebx\n";
                }

                else
                {
                    Output.Error(fileName, tokens[i].line, Output.ErrorType.UndefinedError, $"Undefined Identifier '{tokens[i].value}'.");
                }

                i++;
            }

            if (statementStack.Count > 0)
            {
                Output.Error(fileName, tokens[i-1].line, Output.ErrorType.SyntaxError, $"Statement Missing 'end' Keyword.");
            }

            codeSection += "end start\n";

            outputASM = dataSection + codeSection;
        }

        private void InsertTokensIntoTokens(List<Lexer.Token> tokensToInsert)
        {
            int _i = i;

            foreach (Lexer.Token token in tokensToInsert)
            {
                i++;
                tokens.Insert(i, token);
            }

            i = _i;
        }

        private void DefineString(string name, string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value.Replace("\\n", "\r\n"));
            string formatedBytes = string.Join(", ", bytes) + ", 0";

            dataSection += $"{name} db {formatedBytes}\n";
        }
    }
}
