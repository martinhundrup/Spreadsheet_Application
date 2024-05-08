// <copyright file="CircularReferenceException.cs" company="Martin Hundrup">
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
    /// An exception for the spreadsheet to be thrown when an a series of cells reference eachother circularly.
    /// </summary>
    internal class CircularReferenceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularReferenceException"/> class.
        /// Default constructor.
        /// </summary>
        public CircularReferenceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularReferenceException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        public CircularReferenceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularReferenceException"/> class.
        /// </summary>
        /// <param name="message">Contains details about the exception.</param>
        /// <param name="inner">The exceptin that threw this one.</param>
        public CircularReferenceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
