// <copyright file="SelfReferenceException.cs" company="Martin Hundrup">
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
    /// An exception for the spreadsheet to be thrown when cell includes itself in it's text.
    /// </summary>
    internal class SelfReferenceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfReferenceException"/> class.
        /// Default constructor.
        /// </summary>
        public SelfReferenceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfReferenceException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        public SelfReferenceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfReferenceException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        /// <param name="inner">The exceptin that threw this one.</param>
        public SelfReferenceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}