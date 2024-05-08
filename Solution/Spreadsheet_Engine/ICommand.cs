// <copyright file="ICommand.cs" company="Martin Hundrup">
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
    /// Represents a command to be executed.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Runs the method contained.
        /// </summary>
        public void Execute();
    }
}
