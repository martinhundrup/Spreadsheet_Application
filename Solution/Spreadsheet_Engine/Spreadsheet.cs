// <copyright file="Spreadsheet.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Spreadsheet_Engine.Expression;

namespace Spreadsheet_Engine
{
    /// <summary>
    /// Manages the entire spreadsheet application logic.
    /// </summary>
    public class Spreadsheet
    {
        // --FIELDS-- //

        /// <summary>
        /// A 2D array that stores all the cells of the spreadsheet.
        /// </summary>
        private SpreadsheetCell[,] cells;

        /// <summary>
        /// The expression tree in charge of evaluating any expressions in the spreadsheet.
        /// </summary>
        private ExpressionTree tree;

        /// <summary>
        /// The stack in which undo commands are stored.
        /// </summary>
        private Stack<ICommand> undoStack;

        /// <summary>
        /// The stack in which redo commands are stored.
        /// </summary>
        private Stack<ICommand> redoStack;

        // --CONSTRUCTORS-- //

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows">The amount of rows the spreadsheet will contain.</param>
        /// <param name="columns">The amount of columns the spreadsheet will contain.</param>
        public Spreadsheet(int rows = 0, int columns = 0)
        {
            if (rows <= 0 || columns <= 0)
            {
                throw new IndexOutOfRangeException("invalid value");
            }

            this.tree = new ExpressionTree();
            this.cells = new SpreadsheetCell[rows, columns];
            this.undoStack = new Stack<ICommand>();
            this.redoStack = new Stack<ICommand>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this.cells[i, j] = new SpreadsheetCell(i, j);
                    this.cells[i, j].PropertyChanged += this.CellPropertyChanged;
                }
            }
        }

        // --EVENTS-- //

        /// <summary>
        /// The event to be called when a cell value is changed.
        /// </summary>
        /// <param name="cell">A reference to the cell that was changed.</param>
        public event PropertyChangedEventHandler? CellValueChanged;

        /// <summary>
        /// The event to be called when a cell color is changed.
        /// </summary>
        /// <param name="cell">A reference to the cell that was changed.</param>
        public event PropertyChangedEventHandler? CellColorChanged;

        // --PROPERTIES-- //

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int RowCount
        {
            get { return this.cells.GetLength(0); }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int ColumnCount
        {
            get { return this.cells.GetLength(1); }
        }

        // --METHODS-- //

        /// <summary>
        /// Finds a reference to the cell at the specified index.
        /// </summary>
        /// <param name="rowIndex">The row index of the queried cell.</param>
        /// <param name="colIndex">The column index of the queried cell.</param>
        /// <returns>A reference to the cell at the location.</returns>
        /// <exception cref="IndexOutOfRangeException">Throws if quieried out of bounds of array.</exception>
        public Cell GetCell(int rowIndex, int colIndex)
        {
            // Make sure the row index and column index exist within the cells 2D array.
            if (rowIndex >= 0 && rowIndex < this.cells.GetLength(0) && colIndex >= 0 && colIndex < this.cells.GetLength(1))
            {
                return this.cells[rowIndex, colIndex];
            }
            else
            {
                throw new IndexOutOfRangeException("Cell index is out of bounds.");
            }
        }

        /// <summary>
        /// The event called when a property of the Cell class has been changed.
        /// </summary>
        /// <param name="sender">The cell object that was changed.</param>
        /// <param name="e">The property that was changed.</param>
        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Check for null before accessing properties of sender and e
            if (sender != null && e != null && e.PropertyName != null)
            {
                if (e.PropertyName == "Color")
                {
                    SpreadsheetCell tempCell = (SpreadsheetCell)sender;

                    if (this.CellValueChanged != null)
                    {
                        // Call property changed event.
                        this.CellColorChanged(tempCell, new PropertyChangedEventArgs(e.PropertyName));
                    }
                }
                else
                {
                    // Typecast the sender to its actual (Cell) type to access information
                    string newText = e.PropertyName;

                    // Remove any white space for analyzing
                    string cleanedText = newText.Replace(" ", string.Empty);

                    SpreadsheetCell tempCell = (SpreadsheetCell)sender;

                    int row = tempCell.RowIndex;
                    int col = tempCell.ColIndex;

                    // Check first character of the new text to determine the course of action.
                    if (cleanedText.Length != 0 && cleanedText[0] == '=')
                    {
                        // remove the equal sign
                        string expression = cleanedText.Replace("=", string.Empty);

                        this.tree.Expression = expression;

                        // supports dynamic updating
                        try
                        {
                            this.SubscribeCellToCellsReferenced(this.cells[row, col]);

                            // set value to evaluated expression
                            try
                            {
                                this.cells[row, col].SetValue(this.tree.Evaluate().ToString());

                                // update the variable representing this cell
                                this.UpdateVariable(col, row);
                            }
                            catch (VariableNotSetException)
                            {
                                this.cells[row, col].SetValue("Variable not set");
                                this.UpdateVariable(col, row);
                            }
                            catch (InvalidExpressionFormattingException)
                            {
                                this.cells[row, col].SetValue("Invalid expression format");
                                this.UpdateVariable(col, row);
                            }
                            catch
                            {
                                this.cells[row, col].SetValue("Unown error");
                                this.UpdateVariable(col, row);
                            }
                            finally
                            {
                                if (this.CellValueChanged != null)
                                {
                                    // Call property changed event.
                                    this.CellValueChanged(tempCell, new PropertyChangedEventArgs(e.PropertyName));
                                }
                            }
                        }
                        catch (InvalidVariableException)
                        {
                            this.cells[row, col].SetValue("Variable not found");
                            this.UpdateVariable(col, row);
                        }
                        catch (SelfReferenceException)
                        {
                            this.cells[row, col].SetValue("Self reference");
                            this.UpdateVariable(col, row);
                        }
                    }
                    else
                    {
                        // unsub from all cells
                        this.cells[row, col].UnsubscribeFromAllReferencedCells();

                        // Set value equal to the inputted value.
                        this.cells[row, col].SetValue(newText);

                        // update the variable representing this cell
                        if (double.TryParse(newText, out var x))
                        {
                            this.UpdateVariable(col, row);
                        }
                    }

                    if (this.CellValueChanged != null)
                    {
                        // Call property changed event.
                        this.CellValueChanged(tempCell, new PropertyChangedEventArgs(e.PropertyName));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the expression tree's dictionary of variables.
        /// </summary>
        /// <param name="col">The column index of the cell variable.</param>
        /// <param name="row">The row index of the cell variable.</param>
        public void UpdateVariable(int col, int row)
        {
            char letter = (char)(col + 65);
            string number = (row + 1).ToString();
            double value = 0.0; // responsible for default value
            double.TryParse(this.cells[row, col].Value, out value);
            this.tree.SetVariable(letter + number, value);
        }

        /// <summary>
        /// Executes the top undo command and pushes the prior state to the redo stack.
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count > 0)
            {
                // I need to see what command is at the top of the undo stack and
                // push the current state to the redo stack
                object command = this.undoStack.Pop();

                if (command.GetType() == typeof(CellSetTextCommand))
                {
                    Cell cell = ((CellSetTextCommand)command).Cell;

                    this.AddRedo(new CellSetTextCommand(cell, cell.Text));
                }
                else if (command.GetType() == typeof(CellSetColorCommand))
                {
                    List<(Cell, uint)> cells = ((CellSetColorCommand)command).Cells;
                    List<(Cell, uint)> newCells = new List<(Cell, uint)>();

                    foreach ((Cell, uint) cell in cells)
                    {
                        newCells.Add((cell.Item1, cell.Item1.BGColor));
                    }

                    this.AddRedo(new CellSetColorCommand(newCells));
                }

                ((ICommand)command).Execute();
            }
        }

        /// <summary>
        /// Executes the top redo command and pushes the prior state to the undo stack.
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count > 0)
            {
                // I need to see what command is at the top of the undo stack and
                // push the current state to the redo stack
                object command = this.redoStack.Pop();

                // case 1: text command
                if (command.GetType() == typeof(CellSetTextCommand))
                {
                    Cell cell = ((CellSetTextCommand)command).Cell;

                    this.AddUndo(new CellSetTextCommand(cell, cell.Text));
                }
                else if (command.GetType() == typeof(CellSetColorCommand)) // case 2: color command
                {
                    List<(Cell, uint)> cells = ((CellSetColorCommand)command).Cells;
                    List<(Cell, uint)> newCells = new List<(Cell, uint)>();

                    foreach ((Cell, uint) cell in cells)
                    {
                        newCells.Add((cell.Item1, cell.Item1.BGColor));
                    }

                    this.AddUndo(new CellSetColorCommand(newCells));
                }

                ((ICommand)command).Execute();
            }
        }

        /// <summary>
        /// Pushes a command to the Undo Stack.
        /// </summary>
        /// <param name="command">The command to push.</param>
        public void AddUndo(ICommand command)
        {
            this.undoStack.Push(command);
        }

        /// <summary>
        /// Pushes a command to the Redo Stack.
        /// </summary>
        /// <param name="command">The command to push.</param>
        public void AddRedo(ICommand command)
        {
            this.redoStack.Push(command);
        }

        /// <summary>
        /// Saves all relavent information about the spreadsheet to an xml doc.
        /// </summary>
        /// <param name="fileStream">A pointer to the file to write to.</param>
        public void Save(StreamWriter fileStream)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    ",
                NewLineOnAttributes = true,
            };

            XmlWriter writer = XmlWriter.Create(fileStream, settings);

            writer.WriteStartElement("spreadsheet"); // start spreadsheet element

            foreach (Cell cell in this.cells)
            {
                // only write cells with changed values
                if (cell.Text != cell.DefaultText || cell.BGColor != cell.DefaultColor)
                {
                    writer.WriteStartElement("cell"); // start cell element

                    char letter = (char)(cell.ColIndex + 65);
                    string number = (cell.RowIndex + 1).ToString();

                    writer.WriteStartElement("name"); // start name element
                    writer.WriteString(letter + number);
                    writer.WriteEndElement(); // end name element

                    writer.WriteStartElement("text"); // start text element
                    writer.WriteString(cell.Text);
                    writer.WriteEndElement(); // end text element

                    writer.WriteStartElement("color"); // start color element
                    writer.WriteString(cell.BGColor.ToString("X8"));
                    writer.WriteEndElement(); // end color element

                    writer.WriteEndElement(); // end cell elememnt
                }
            }

            writer.WriteEndElement(); // end spreadsheet element
            writer.Close();
        }

        /// <summary>
        /// Loads a spreadsheet state from an XML doc.
        /// </summary>
        /// <param name="fileStream">A pointer to the file to read from.</param>
        public void Load(StreamReader fileStream)
        {
            this.ClearSpreadsheet();

            XDocument doc = XDocument.Load(fileStream);

            foreach (XElement element in doc.Descendants("cell"))
            {
                XElement? name = element.Element("name");
                XElement? color = element.Element("color");
                XElement? text = element.Element("text");

                if (name != null) // only change things if the name was found
                {
                    int row = int.Parse(name.Value.Substring(1)) - 1;
                    int col = name.Value[0] - 65;

                    if (text != null)
                    {
                        this.GetCell(row, col).Text = text.Value;
                    }

                    if (color != null)
                    {
                        this.GetCell(row, col).BGColor = Convert.ToUInt32(color.Value, 16);
                    }
                }
            }
        }

        /// <summary>
        /// Resets all cells members back to their default ones.
        /// </summary>
        public void ClearSpreadsheet()
        {
            // clear cells
            foreach (Cell cell in this.cells)
            {
                cell.Text = cell.DefaultText;
                cell.BGColor = cell.DefaultColor;
            }

            // clear redo and undo stacks
            this.redoStack.Clear();
            this.undoStack.Clear();
        }

        /// <summary>
        /// Subscribes a cell to all referenced cells' property changed event.
        /// </summary>
        /// <param name="cell">The cell to update.</param>
        private void SubscribeCellToCellsReferenced(Cell cell)
        {
            cell.SubscribeToCellsReferenced(this.GetCellsFromExpression(cell.Text));
        }

        /// <summary>
        /// Gets a reference to each cell referenced by name in an expression.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <returns>An array of all cells referenced.</returns>
#pragma warning disable SA1018 // Nullable type symbols should be spaced correctly - disable due to conflicting warnings
        private Cell[] ? GetCellsFromExpression(string expression)
#pragma warning restore SA1018 // Nullable type symbols should be spaced correctly
        {
            string[] varNames = this.tree.GetVariableNames(expression);
            List<Cell> cellList = new List<Cell>();

            foreach (string var in varNames)
            {
                if (this.IsCellFromVariable(var))
                {
                    cellList.Add(this.GetCellFromName(var));
                }
            }

            if (cellList.Count > 0)
            {
                return cellList.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a reference to a cell based on it's name.
        /// </summary>
        /// <param name="name">The name to search by.</param>
        /// <returns>A reference to the cell found.</returns>
        private Cell? GetCellFromName(string name)
        {
            if (this.IsCellFromVariable(name)) // should be a redundant check, but prevents from breaking
            {
                char colChar = name[0];
                string rowStr = name.Substring(1);

                return this.cells[int.Parse(name.Substring(1)) - 1, name[0] - 65];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if a variable name represents a valid cell.
        /// </summary>
        /// <param name="name">The name to search by.</param>
        /// <returns>True if a cell is found; false otherwise.</returns>
        private bool IsCellFromVariable(string name)
        {
            char colChar = name[0];
            string rowStr = name.Substring(1);

            int rowIndex = -1;
            try
            {
                rowIndex = int.Parse(rowStr) - 1;
            }
            catch
            {
                throw new InvalidVariableException();
            }
            finally // it won't let me skip the finally block
            {
            }

            return (colChar >= 65 && colChar < 65 + this.ColumnCount) && (rowIndex >= 0 && rowIndex < this.RowCount);
        }

        // --CLASSES-- //

        /// <summary>
        /// A concrete class to inherit the abstract Cell class.
        /// </summary>
        private class SpreadsheetCell : Cell
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SpreadsheetCell"/> class.
            /// </summary>
            /// <param name="nRowIndex">The row index to intialize this cell with.</param>
            /// <param name="nColIndex">The column index to intialize this cell with.</param>
            public SpreadsheetCell(int nRowIndex, int nColIndex)
                : base(nRowIndex, nColIndex)
            {
            }

            /// <summary>
            /// Sets the value of the cell.
            /// </summary>
            /// <param name="nValue">The new value for the node to store.</param>
            public void SetValue(string nValue)
            {
                this.value = nValue;
            }
        }
    }
}