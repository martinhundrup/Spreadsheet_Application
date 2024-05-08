// <copyright file="InvalidExpressionFormattingException.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet_Engine
{
    /// <summary>
    /// An exception for the spreadsheet to be thrown when an expression has not been formatted correctly.
    /// </summary>
    internal class InvalidExpressionFormattingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidExpressionFormattingException"/> class.
        /// Default constructor.
        /// </summary>
        public InvalidExpressionFormattingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidExpressionFormattingException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        public InvalidExpressionFormattingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidExpressionFormattingException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        /// <param name="inner">The exceptin that threw this one.</param>
        public InvalidExpressionFormattingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
