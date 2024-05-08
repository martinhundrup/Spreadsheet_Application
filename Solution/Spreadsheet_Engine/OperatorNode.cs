// <copyright file="OperatorNode.cs" company="Martin Hundrup">
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
    /// Denotes the associativity of the operator node.
    /// </summary>
    public enum Associativities
    {
        /// <summary>
        /// Left associativity.
        /// </summary>
        Left,

        /// <summary>
        /// Right associativity.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Abstract node class that holds a operator value.
    /// </summary>
    public abstract class OperatorNode : Node
    {
        /// <summary>
        /// The left node of this subtree.
        /// </summary>
        protected Node? nLeft;

        /// <summary>
        /// The right node of this subtree.
        /// </summary>
        protected Node? nRight;

        /// <summary>
        /// Gets or sets the associativity.
        /// </summary>
        public static Associativities Associativity
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the precedence.
        /// </summary>
        public static int Precedence
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the left node.
        /// </summary>
        public Node NLeft
        {
            get { return this.nLeft; }
            set { this.nLeft = value; }
        }

        /// <summary>
        /// Gets or sets the right node.
        /// </summary>
        public Node NRight
        {
            get { return this.nRight; }
            set { this.nRight = value; }
        }
    }
}
