// <copyright file="CellSetTextCommand.cs" company="Martin Hundrup">
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
    /// Represents the change of a cell's text.
    /// </summary>
    public class CellSetTextCommand : ICommand
    {
        /// <summary>
        /// The cell involved.
        /// </summary>
        private Cell cell;

        /// <summary>
        /// The text to set the cell text to.
        /// </summary>
        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSetTextCommand"/> class.
        /// </summary>
        /// <param name="nCell">The cell invloved.</param>
        /// <param name="nText">The text to set the cell text to.</param>
        public CellSetTextCommand(Cell nCell, string nText)
        {
            this.text = nText;
            this.cell = nCell;
        }

        /// <summary>
        /// Gets the cell involved.
        /// </summary>
        public Cell Cell
        {
            get { return this.cell; }
        }

        /// <summary>
        /// Exectutes the command.
        /// </summary>
        public void Execute()
        {
            this.cell.Text = this.text;
        }
    }
}
