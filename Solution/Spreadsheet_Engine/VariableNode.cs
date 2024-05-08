// <copyright file="VariableNode.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

namespace Spreadsheet_Engine.Expression
{
    /// <summary>
    /// Concrete node class that holds a variable value.
    /// </summary>
    public class VariableNode : Node
    {
        /// <summary>
        /// The value the variable of the node stores.
        /// </summary>
        private double variableValue;

        /// <summary>
        /// A dictionary that stores all variables and their values in the current expression.
        /// </summary>
        private Dictionary<string, double> variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="nValue">The value to initialize this node with.</param>
        /// <param name="nVariables">A reference to the variables dictionary.</param>
        public VariableNode(string nValue, Dictionary<string, double> nVariables)
        {
            this.value = nValue;

            this.variables = nVariables;
        }

        /// <summary>
        /// Evaluates the subtree of the node.
        /// </summary>
        /// <returns>The stored value of the tree as a double.</returns>
        public override double Evaluate()
        {
            if (this.variables.ContainsKey(this.value))
            {
                return this.variables[this.value];
            }
            else
            {
                throw new VariableNotSetException();
            }
        }
    }
}
