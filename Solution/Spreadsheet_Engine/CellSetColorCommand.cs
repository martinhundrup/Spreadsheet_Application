// <copyright file="CellSetColorCommand.cs" company="Martin Hundrup">
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
    /// Represents the command of changing the color of several cells.
    /// </summary>
    public class CellSetColorCommand : ICommand
    {
        /// <summary>
        /// A list of cells and corresponding colors.
        /// </summary>
        private List<(Cell, uint)> cells;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSetColorCommand"/> class.
        /// </summary>
        /// <param name="nCells">The list of cells and corresponding colors.</param>
        public CellSetColorCommand(List<(Cell, uint)> nCells)
        {
            this.cells = nCells;
        }

        /// <summary>
        /// Gets the cells and corresponding colors involved.
        /// </summary>
        public List<(Cell, uint)> Cells
        {
            get { return this.cells; }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
            foreach ((Cell, uint) cell in this.cells)
            {
                cell.Item1.BGColor = cell.Item2;
            }
        }
    }
}
