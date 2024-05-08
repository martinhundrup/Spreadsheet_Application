// <copyright file="Program.cs" company="Martin Hundrup">
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
    /// Handles the start of the ExprssionTree demo.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The entry point upons starting the program.
        /// </summary>
        private static void Main()
        {
            Menu menu = new Menu();
            menu.RunMenu();
        }
    }
}
