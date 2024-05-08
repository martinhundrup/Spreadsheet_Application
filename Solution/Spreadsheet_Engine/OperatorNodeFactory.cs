// <copyright file="OperatorNodeFactory.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet_Engine.Expression;

namespace Spreadsheet_Engine.Expression
{
    /// <summary>
    /// Responsible for creating an operator node of the correct type.
    /// </summary>
    internal class OperatorNodeFactory
    {
        /// <summary>
        /// The object responsible for instantiating operator nodes.
        /// </summary>
        private static OperatorNodeFactory? factory;

        /// <summary>
        /// The dictionary that stores the operator types and their operator characters.
        /// </summary>
        private Dictionary<char, Type> operators = new Dictionary<char, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNodeFactory"/> class.
        /// </summary>
        private OperatorNodeFactory()
        {
            this.TraverseAvailableOperators((op, type) => this.operators.Add(op, type));
        }

        /// <summary>
        /// The delegate responsible for operator lookups.
        /// </summary>
        /// <param name="op">The operator character.</param>
        /// <param name="type">The type associated with the character.</param>
        private delegate void OnOperator(char op, Type type);

        /// <summary>
        /// Gets the operators dictionary.
        /// </summary>
        public Dictionary<char, Type> Operators
        {
            get { return this.operators; }
        }

        /// <summary>
        /// Used to implement the singleton pattern.
        /// </summary>
        /// <returns>The only instance of this class.</returns>
        public static OperatorNodeFactory GetInstance()
        {
            if (factory == null)
            {
                factory = new OperatorNodeFactory();
            }

            return factory;
        }

        /// <summary>
        /// Creates an operator node of the correct type.
        /// </summary>
        /// <param name="op">The operator to lookup.</param>
        /// <returns>An operator node that represents the operation.</returns>
        public OperatorNode CreateOperatorNode(char op)
        {
            if (this.operators.ContainsKey(op))
            {
                object operatorNodeObject = Activator.CreateInstance(this.operators[op]);
                if (operatorNodeObject is OperatorNode)
                {
                    return (OperatorNode)operatorNodeObject;
                }
            }

            throw new Exception("Unhandled operator");
        }

        /// <summary>
        /// Determines if a character represents an operator node.
        /// </summary>
        /// <param name="op">The operator to check.</param>
        /// <returns>True if operator; false otherwise.</returns>
        public bool IsOperator(char op)
        {
            return this.operators.ContainsKey(op);
        }

        /// <summary>
        /// Determines if a character represents an operator node.
        /// </summary>
        /// <param name="op">The operator to check.</param>
        /// <returns>True if operator; false otherwise.</returns>
        public bool IsOperator(string op)
        {
            return op.Length == 1 && this.operators.ContainsKey(op[0]);
        }

        /// <summary>
        /// Gets the precedence of an OperatorNode.
        /// </summary>
        /// <param name="op">The operator attached to the node.</param>
        /// <returns>The precendence of that operator.</returns>
        public int CheckPrecedence(char op)
        {
            if (this.operators.ContainsKey(op))
            {
                PropertyInfo operatorField = this.operators[op].GetProperty("Precedence");

                if (operatorField != null)
                {
                    // Get the int representing the precendence
                    object value = operatorField.GetValue(this.operators[op]);

                    if (value is int)
                    {
                        return (int)value;
                    }
                }
            }

            return -1; // should throw execption in the future
        }

        /// <summary>
        /// Gets the precedence of an OperatorNode.
        /// </summary>
        /// <param name="op">The operator attached to the node.</param>
        /// <returns>The precendence of that operator.</returns>
        public int CheckPrecedence(string op)
        {
            if (op.Length == 1)
            {
                return this.CheckPrecedence(op[0]);
            }

            return -1; // should throw execption in the future
        }

        /// <summary>
        /// Gets the associativity of an OperatorNode.
        /// </summary>
        /// <param name="op">The operator attached to the node.</param>
        /// <returns>The associativity of that operator.</returns>
        public Associativities CheckAssociativity(char op)
        {
            if (this.operators.ContainsKey(op))
            {
                PropertyInfo operatorField = this.operators[op].GetProperty("Associativity");

                if (operatorField != null)
                {
                    // Get the int representing the precendence
                    object value = operatorField.GetValue(this.operators[op]);

                    if (value is Associativities)
                    {
                        return (Associativities)value;
                    }
                }
            }

            return Associativities.Left; // should throw execption in the future
        }

        /// <summary>
        /// Gets the associativity of an OperatorNode.
        /// </summary>
        /// <param name="op">The operator attached to the node.</param>
        /// <returns>The associativity of that operator.</returns>
        public Associativities CheckAssociativity(string op)
        {
            if (op.Length == 1)
            {
                return this.CheckAssociativity(op[0]);
            }

            return Associativities.Left; // should throw execption in the future
        }

        /// <summary>
        /// Uses reflection to populate a dictionary with all operators and types of operator nodes.
        /// </summary>
        /// <param name="onOperator">The delegate responsible for lookups.</param>
        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            // get the type declaration of OperatorNode
            Type operatorNodeType = typeof(OperatorNode);

            // Iterate over all loaded assemblies:
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types that inherit from our OperatorNode class using LINQ
                IEnumerable<Type> operatorTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));

                // Iterate over the subclasses of OperatorNode
                foreach (var type in operatorTypes)
                {
                    // retrieve the Operator property of each subclass
                    PropertyInfo operatorField = type.GetProperty("Operation");

                    if (operatorField != null)
                    {
                        // Get the character of the Operator
                        object value = operatorField.GetValue(type);

                        if (value is char)
                        {
                            char operatorSymbol = (char)value;

                            // And invoke the function passed as parameter
                            // with the operator symbol and the operator class
                            onOperator(operatorSymbol, type);
                        }
                    }
                }
            }
        }
    }
}