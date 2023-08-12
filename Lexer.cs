namespace DeezLang
{
    internal class Lexer
    {
        public string source;

        public Lexer(string source)
        {
            this.source = source;
        }

        public enum TokenTypes
        {
            Int,
            String,
            Identifier,

            DefineKeyword,
            BufferKeyword,
            ConstKeyword,
            UsingKeyword,
            MacroKeyword,
            EndKeyword,
            IfKeyword,
            ElseKeyword,
            WhileKeyword,
            PtrKeyword,
            DupKeyword,

            AddOperator,
            SubOperator,
            MulOperator,
            DivOperator,
            EqualsOperator,
        }

        public class Token
        {
            public string value;
            public TokenTypes type;
            public int line;

            public Token(string value, TokenTypes type, int line)
            {
                this.value = value;
                this.type = type;
                this.line = line;
            }
        }

        public List<Token> tokens = new List<Token>();

        private string token = "";
        private int line = 1;

        public void Lex()
        {
            bool isString = false;
            int i = 0;

            while (i < source.Length)
            {
                if (source[i] == '#' && !isString)
                {
                    while (source[i] != '\n')
                    {
                        i++;
                    }

                    line++;
                }
                else if (source[i] == ' ' && !isString)
                {
                    AddToken();
                }
                else if (source[i] == '"')
                {
                    if (isString)
                    {
                        tokens.Add(new Token(token, TokenTypes.String, line));
                        token = "";
                    }

                    isString = !isString;
                }
                else if (source[i] == '\n' && !isString)
                {
                    AddToken();

                    line++;
                }
                else if (source[i] == '\r' && !isString)
                {
                    AddToken();
                }
                else if (source[i] == '\t' && !isString)
                {
                    AddToken();
                }
                else
                {
                    token += source[i];
                }

                i++;
            }
        }

        public void AddToken()
        {
            if (token != "")
            {
                if (int.TryParse(token, out _))
                {
                    tokens.Add(new Token(token, TokenTypes.Int, line));
                }
                else if (token == "define")
                {
                    tokens.Add(new Token(token, TokenTypes.DefineKeyword, line));
                }
                else if (token == "buffer")
                {
                    tokens.Add(new Token(token, TokenTypes.BufferKeyword, line));
                }
                else if (token == "using")
                {
                    tokens.Add(new Token(token, TokenTypes.UsingKeyword, line));
                }
                else if (token == "const")
                {
                    tokens.Add(new Token(token, TokenTypes.ConstKeyword, line));
                }
                else if (token == "macro")
                {
                    tokens.Add(new Token(token, TokenTypes.MacroKeyword, line));
                }
                else if (token == "if")
                {
                    tokens.Add(new Token(token, TokenTypes.IfKeyword, line));
                }
                else if (token == "else")
                {
                    tokens.Add(new Token(token, TokenTypes.ElseKeyword, line));
                }
                else if (token == "while")
                {
                    tokens.Add(new Token(token, TokenTypes.WhileKeyword, line));
                }
                else if (token == "end")
                {
                    tokens.Add(new Token(token, TokenTypes.EndKeyword, line));
                }
                else if (token == "ptr")
                {
                    tokens.Add(new Token(token, TokenTypes.PtrKeyword, line));
                }
                else if (token == "dup")
                {
                    tokens.Add(new Token(token, TokenTypes.DupKeyword, line));
                }
                else if (token == "+")
                {
                    tokens.Add(new Token(token, TokenTypes.AddOperator, line));
                }
                else if (token == "-")
                {
                    tokens.Add(new Token(token, TokenTypes.SubOperator, line));
                }
                else if (token == "*")
                {
                    tokens.Add(new Token(token, TokenTypes.MulOperator, line));
                }
                else if (token == "/")
                {
                    tokens.Add(new Token(token, TokenTypes.DivOperator, line));
                }
                else if (token == "=")
                {
                    tokens.Add(new Token(token, TokenTypes.EqualsOperator, line));
                }
                else
                {
                    tokens.Add(new Token(token, TokenTypes.Identifier, line));
                }

                token = "";
            }
        }
    }
}
