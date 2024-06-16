namespace AnalizadorSintactico
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = expressionTextBox.Text;
            try
            {
                var tokens = Tokenize(input);
                var parser = new Parser(tokens);
                var result = parser.Parse();
                resultTextBox.Text = $"AST: {result}";
            }
            catch (Exception ex)
            {
                resultTextBox.Text = $"Error: {ex.Message}";
            }
        }

        private List<string> Tokenize(string input)
        {
            List<string> tokens = new List<string>();
            int i = 0;
            while (i < input.Length)
            {
                if (char.IsDigit(input[i]))
                {
                    string num = string.Empty;
                    while (i < input.Length && char.IsDigit(input[i]))
                    {
                        num += input[i];
                        i++;
                    }
                    tokens.Add(num);
                }
                else if ("+*()".Contains(input[i]))
                {
                    tokens.Add(input[i].ToString());
                    i++;
                }
                else if (char.IsWhiteSpace(input[i]))
                {
                    i++;
                }
                else
                {
                    throw new Exception($"Invalid character: {input[i]}");
                }
            }
            return tokens;
        }

        public class Parser
        {
            private List<string> _tokens;
            private int _currentTokenIndex;
            private string _currentToken;

            public Parser(List<string> tokens)
            {
                _tokens = tokens;
                _currentTokenIndex = 0;
                _currentToken = _tokens[_currentTokenIndex];
            }

            private void Eat(string tokenType)
            {
                if (_currentToken == tokenType)
                {
                    _currentTokenIndex++;
                    if (_currentTokenIndex < _tokens.Count)
                    {
                        _currentToken = _tokens[_currentTokenIndex];
                    }
                }
                else
                {
                    throw new Exception($"Unexpected token: {_currentToken}");
                }
            }

            public string Parse()
            {
                return Expr();
            }

            private string Expr()
            {
                // E -> T E'
                string node = Term();
                node = ExprPrime(node);
                return node;
            }

            private string ExprPrime(string node)
            {
                // E' -> + T E' | ?
                if (_currentToken == "+")
                {
                    Eat("+");
                    node = $"(+ {node} {Term()})";
                    node = ExprPrime(node);
                }
                return node;
            }

            private string Term()
            {
                // T -> F T'
                string node = Factor();
                node = TermPrime(node);
                return node;
            }

            private string TermPrime(string node)
            {
                // T' -> * F T' | ?
                if (_currentToken == "*")
                {
                    Eat("*");
                    node = $"(* {node} {Factor()})";
                    node = TermPrime(node);
                }
                return node;
            }

            private string Factor()
            {
                // F -> ( E ) | num
                if (_currentToken == "(")
                {
                    Eat("(");
                    string node = Expr();
                    Eat(")");
                    return node;
                }
                else if (char.IsDigit(_currentToken[0]))
                {
                    string node = _currentToken;
                    Eat(_currentToken);
                    return node;
                }
                else
                {
                    throw new Exception($"Unexpected token: {_currentToken}");
                }
            }
        }
    }
}

