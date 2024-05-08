// <copyright file="ConstantNode.cs" company="Martin Hundrup">
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
    /// Concrete node class that holds a constant value.
    /// </summary>
    public class ConstantNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNode"/> class.
        /// </summary>
        /// <param name="nValue">The string to initialize the node with.</param>
        public ConstantNode(string nValue)
        {
            this.value = nValue;
        }

        /// <summary>
        /// Evaluates the subtree of the node.
        /// </summary>
        /// <returns>The stored value of the tree as a double.</returns>
        public override double Evaluate()
        {
            if (this.value != null)
            {
                return double.Parse(this.value);
            }
            else
            {
                // if this.value is null, just return 0
                return 0.0;
            }
        }
    }
}
