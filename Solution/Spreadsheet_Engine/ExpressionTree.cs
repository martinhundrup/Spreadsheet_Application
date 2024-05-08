// <copyright file="ExpressionTree.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

namespace Spreadsheet_Engine.Expression
{
    /// <summary>
    /// An arithmetic expression parser.
    /// </summary>
    public class ExpressionTree
    {
        // --FIELDS-- //

        /// <summary>
        /// The expression this object will parse.
        /// </summary>
        private string expression;

        /// <summary>
        /// A dictionary that stores all variables and their values in the current expression.
        /// </summary>
        private Dictionary<string, double> variables = new Dictionary<string, double>();

        /// <summary>
        /// All valid operators to consider when evaluating.
        /// </summary>
        // private char[] operators;
        private Dictionary<char, Type> operators = new Dictionary<char, Type>();

        /// <summary>
        /// The root node of the expression tree.
        /// </summary>
        private Node? nRoot;

        /// <summary>
        /// The object responsible for instantiating operator nodes.
        /// </summary>
        private OperatorNodeFactory factory;

        // --CONSTRUCTORS-- //

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        public ExpressionTree()
        {
            this.expression = string.Empty;
            this.nRoot = null;
            this.factory = OperatorNodeFactory.GetInstance();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression">The expression to initialize with.</param>
        public ExpressionTree(string expression)
        {
            this.expression = expression;
            this.nRoot = null;
            this.factory = OperatorNodeFactory.GetInstance();
        }

        // --PROPERTIES-- //

        /// <summary>
        /// Gets or sets the expression represented by this object.
        /// </summary>
        public string Expression
        {
            get { return this.expression; }
            set { this.expression = value; }
        }

        // -- METHODS -- //

        /// <summary>
        /// Sets the specified variable withint this objects variables dictionary.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="variableValue">The value to set the variable to.</param>
        public void SetVariable(string variableName, double variableValue)
        {
            if (this.variables.ContainsKey(variableName))
            {
                // update existing variable name
                this.variables[variableName] = variableValue;
            }
            else
            {
                // adding new variable
                this.variables.Add(variableName, variableValue);
            }
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <returns>The result of the expression.</returns>
        public double Evaluate()
        {
            try
            {
                this.nRoot = this.ConstructTree(this.ConvertToPostfix(this.expression));
            }
            catch
            {
                throw new InvalidExpressionFormattingException();
            }

            return this.nRoot.Evaluate();
        }

        /// <summary>
        /// Gets all variable names referenced in an expression.
        /// </summary>
        /// <param name="s">The expression string.</param>
        /// <returns>A tokenized list of all variable names found.</returns>
        public string[] GetVariableNames(string s)
        {
            string[] tokens = this.Tokenize(s);
            List<string> variables = new List<string>();

            foreach (string token in tokens)
            {
                // parenthesis aren't variables
                if (this.IsVariableName(token) && (token != "(" && token != ")"))
                {
                    variables.Add(token);
                }
            }

            return variables.ToArray();
        }

        /// <summary>
        /// Determines if a token represents a variable name.
        /// </summary>
        /// <param name="s">The token string to check.</param>
        /// <returns>True if it is a variable name; false otherwise.</returns>
        public bool IsVariableName(string s)
        {
            return !this.factory.IsOperator(s) && !double.TryParse(s, out double x);
        }

        /// <summary>
        /// Converts a valid infix expression to a postfix expression.
        /// </summary>
        /// <param name="s">The infix expression.</param>
        /// <returns>The postfix expression as tokens.</returns>
        private string[] ConvertToPostfix(string s)
        {
            // step one: clean string of any spaces
            string infix = s.Replace(" ", string.Empty);

            // step two: tokenize string
            string[] tokens = this.Tokenize(s);

            // step three: follow Dijkstra's Shunting Yard Algorithm
            Stack<string> operandStack = new Stack<string>();
            List<string> output = new List<string>();

            foreach (string token in tokens)
            {
                if (token == "(") // if the incoming symbol is left parenthesis, push it on the stack
                {
                    operandStack.Push("(");
                }
                else if (token == ")") // if the incoming symbol is a right parenthesis, discard, pop and print stack until left parenthesis; pop and discard it
                {
                    string temp = operandStack.Pop();
                    while (temp != "(")
                    {
                        if (temp != "(")
                        {
                            output.Add(temp);
                        }

                        temp = operandStack.Pop();
                    }
                }
                else if (this.factory.IsOperator(token))
                {
                    if (operandStack.Count == 0 || operandStack.Peek() == "(") // if operator and empty stack or left parenthesis on top, push onto stack
                    {
                        operandStack.Push(token);
                    }
                    else if (this.factory.CheckPrecedence(token) < this.factory.CheckPrecedence(operandStack.Peek()) ||
                            (this.factory.CheckPrecedence(token) == this.factory.CheckPrecedence(operandStack.Peek()) &&
                            this.factory.CheckAssociativity(token) == Associativities.Right)) // note: check for associativity in the future
                    { // we are assuming left associativity for all operators
                        operandStack.Push(token);
                    }
                    else
                    {
                        // while (operandStack.Count > 0 && this.factory.CheckPrecedence(operandStack.Peek()) <= this.factory.CheckPrecedence(token) && operandStack.Peek() != "(")
                        while (operandStack.Count > 0 && operandStack.Peek() != "(" &&
                            (this.factory.CheckPrecedence(operandStack.Peek()) < this.factory.CheckPrecedence(token) ||
                            (this.factory.CheckPrecedence(operandStack.Peek()) == this.factory.CheckPrecedence(token) &&
                            this.factory.CheckAssociativity(token) == Associativities.Left)))
                        {
                            output.Add(operandStack.Pop());
                        }

                        operandStack.Push(token);
                    }
                }
                else
                {
                    output.Add(token);
                }
            } // end of foreach loop

            foreach (string remains in operandStack)
            {
                output.Add(remains);
            }

            return output.ToArray();
        }

        /// <summary>
        /// Constructs an expression tree.
        /// </summary>
        /// <param name="tokens">The tokenized expression in postfix form.</param>
        /// <returns>The root node of the full tree.</returns>
        private Node ConstructTree(string[] tokens)
        {
            Stack<Node> nodeStack = new Stack<Node>();

            foreach (string token in tokens)
            {
                if (this.factory.IsOperator(token))
                {
                    OperatorNode op = this.factory.CreateOperatorNode(token[0]);
                    op.NRight = nodeStack.Pop();
                    op.NLeft = nodeStack.Pop();
                    nodeStack.Push(op);
                }
                else
                {
                    if (int.TryParse(token, out int t))
                    {
                        nodeStack.Push(new ConstantNode(token));
                    }
                    else
                    {
                        nodeStack.Push(new VariableNode(token, this.variables));
                    }
                }
            }

            return nodeStack.Pop();
        }

        /// <summary>
        /// Tokenizes a string using the list of operators. Keeps the operators as tokens.
        /// </summary>
        /// <param name="s">The string to tokenize.</param>
        /// <returns>An array containing every token in the string.</returns>
        private string[] Tokenize(string s)
        {
            // contains all tokens and delims
            List<string> tokens = new List<string>();
            s = s.Replace(" ", string.Empty); // clean any empty spaces
            s = s.Replace("=", string.Empty); // clean any = signs
            string value = string.Empty;
            char[] cString = s.ToCharArray();

            for (int i = 0; i < cString.Length; i++)
            {
                if (this.factory.IsOperator(cString[i]) || cString[i] == ')' || cString[i] == '(') // we found an operator or parenthesis
                {
                    if (value != string.Empty)
                    {
                        tokens.Add(value);
                        value = string.Empty;
                    }

                    tokens.Add(cString[i].ToString());
                }
                else // continue reading value
                {
                    value += cString[i];
                }
            }

            if (value != string.Empty)
            {
                tokens.Add(value);
            }

            return tokens.ToArray();
        }
    }
}
