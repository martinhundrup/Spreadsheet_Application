// <copyright file="AdditionNode.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet_Engine.Expression
{
    /// <summary>
    /// Represents the addition operator in the expression tree.
    /// </summary>
    public class AdditionNode : OperatorNode
    {
        /// <summary>
        /// Initializes static members of the <see cref="AdditionNode"/> class.
        /// </summary>
        static AdditionNode()
        {
            Operation = '+';
            Precedence = 2;
            Associativity = Associativities.Left;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionNode"/> class.
        /// </summary>
        public AdditionNode()
        {
            this.value = "+";
        }

        /// <summary>
        /// Gets or sets the character representing the operation to be performed.
        /// </summary>
        public static char Operation
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the associativity.
        /// </summary>
        public static new Associativities Associativity
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the precedence.
        /// </summary>
        public static new int Precedence
        {
            get; set;
        }

        /// <summary>
        /// Evaluates the value of this node.
        /// </summary>
        /// <returns>The results of the left subtree added to the results of the right.</returns>
        public override double Evaluate()
        {
            if (this.nLeft != null && this.nRight != null)
            {
                return this.nLeft.Evaluate() + this.nRight.Evaluate();
            }
            else
            {
                // return 0 if the left and right nodes are null
                return 0.0;
            }
        }
    }
}
