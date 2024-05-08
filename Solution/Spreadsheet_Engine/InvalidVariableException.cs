// <copyright file="InvalidVariableException.cs" company="Martin Hundrup">
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
    /// An exception for the spreadsheet to be thrown when a variable has not been associated with a cell.
    /// </summary>
    internal class InvalidVariableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVariableException"/> class.
        /// Default constructor.
        /// </summary>
        public InvalidVariableException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVariableException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        public InvalidVariableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVariableException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        /// <param name="inner">The exceptin that threw this one.</param>
        public InvalidVariableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
