// <copyright file="Node.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SA1401 // FieldsMustBePrivate

namespace Spreadsheet_Engine.Expression
{
    /// <summary>
    /// An abstract class representing the nodes used in the expression tree.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// The value being stored in this node.
        /// </summary>
        protected string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            this.value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="nValue">The value to initialize this node with.</param>
        public Node(string nValue)
        {
            this.value = nValue;
        }

        /// <summary>
        /// Gets the value field.
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Evaluates the subtree of the node.
        /// </summary>
        /// <returns>The result of the subtree evaluation.</returns>
        public abstract double Evaluate();
    }
}
