// <copyright file="Form1.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Spreadsheet_Engine;

namespace Spreadsheet_Martin_Hundrup
{
    /// <summary>
    /// The form class for the spreadsheet.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// The spreadsheet engine object that handles the logic.
        /// </summary>
        private Spreadsheet_Engine.Spreadsheet spreadsheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();

            // adds columns A-Z
            for (int i = 1; i <= 26; i++)
            {
                // set title of cell to letter A-Z
                this.dataGridView1.Columns.Add("Column" + i, ((char)(i + 64)).ToString());
            }

            // adds 50 rows
            for (int i = 0; i < 50; i++)
            {
                this.dataGridView1.Rows.Add(); // adds a new cell
                this.dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString(); // sets the header name of the cell
            }

            this.spreadsheet = new Spreadsheet_Engine.Spreadsheet(50, 26);
            this.spreadsheet.CellValueChanged += this.SpreadsheetCellValueChanged;
            this.spreadsheet.CellColorChanged += this.SpreadsheetCellColorChanged;
        }

        /// <summary>
        /// The event called when the value property of a cell in the Spreadsheet class has been changed.
        /// </summary>
        /// <param name="sender">The spreadsheet object that was changed.</param>
        /// <param name="e">The property that was changed.</param>
        private void SpreadsheetCellValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                Cell cell = (Cell)sender;
                this.dataGridView1[cell.ColIndex, cell.RowIndex].Value = cell.Value;
            }
        }

        /// <summary>
        /// The event called when the background color property of a cell in the Spreadsheet class has been changed.
        /// </summary>
        /// <param name="sender">The spreadsheet object that was changed.</param>
        /// <param name="e">The property that was changed.</param>
        private void SpreadsheetCellColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                Cell cell = (Cell)sender;
                this.dataGridView1[cell.ColIndex, cell.RowIndex].Style.BackColor = Color.FromArgb((int)cell.BGColor);
            }
        }

        /// <summary>
        /// Called when a cell edit starts.
        /// </summary>
        /// <param name="sender">The spreadsheet object that was changed.</param>
        /// <param name="e">The property that was changed.</param>
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (this.spreadsheet != null)
            {
                if (this.dataGridView1[e.ColumnIndex, e.RowIndex].Value == null)
                {
                    this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text = string.Empty;
                }
                else
                {
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text;
                }
            }
        }

        /// <summary>
        /// Called when a cell edit ends.
        /// </summary>
        /// <param name="sender">The spreadsheet object that was changed.</param>
        /// <param name="e">The property that was changed.</param>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.spreadsheet != null)
            {
                if (this.dataGridView1[e.ColumnIndex, e.RowIndex].Value == null)
                {
                    // add undo command to revert to now previous state
                    this.spreadsheet.AddUndo(new CellSetTextCommand(
                        this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex),
                        this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text));

                    this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text = string.Empty;
                }
                else
                {
                    // add undo command to revert to now previous state
                    this.spreadsheet.AddUndo(new CellSetTextCommand(
                        this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex),
                        this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text));

                    this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text = this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Value;
                }
            }
        }

        /// <summary>
        /// The event called when the set color button has been pressed.
        /// </summary>
        /// <param name="sender">The button object that was pressed.</param>
        /// <param name="e">EventArgs containing information about the button press.</param>
        private void SelectColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorPicker = new ColorDialog();
            List<(Cell, uint)> cells = new List<(Cell, uint)>();

            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
                {
                    cells.Add((this.spreadsheet.GetCell(cell.RowIndex, cell.ColumnIndex),
                        this.spreadsheet.GetCell(cell.RowIndex, cell.ColumnIndex).BGColor));
                }
            }

            this.spreadsheet.AddUndo(new CellSetColorCommand(cells));

            foreach ((Cell, uint) cell in cells)
            {
                cell.Item1.BGColor = (uint)colorPicker.Color.ToArgb();
            }

            this.dataGridView1.ClearSelection();
        }

        /// <summary>
        /// The method called when the undo button has been clicked.
        /// </summary>
        /// <param name="sender">The object calling.</param>
        /// <param name="e">Contains info about the event.</param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Undo();
        }

        /// <summary>
        /// The method called when the redo button has been clicked.
        /// </summary>
        /// <param name="sender">The object calling.</param>
        /// <param name="e">Contains info about the event.</param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Redo();
        }

        /// <summary>
        /// The method called when the undo button has been clicked.
        /// </summary>
        /// <param name="sender">The object calling.</param>
        /// <param name="e">Contains info about the event.</param>
        private void UndoContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Undo();
        }

        /// <summary>
        /// The method called when the redo button has been clicked.
        /// </summary>
        /// <param name="sender">The object calling.</param>
        /// <param name="e">Contains info about the event.</param>
        private void RedoContextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Redo();
        }

        /// <summary>
        /// The event called when the set color button has been pressed.
        /// </summary>
        /// <param name="sender">The button object that was pressed.</param>
        /// <param name="e">EventArgs containing information about the button press.</param>
        private void SetColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectColorToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// The event called when the save button has been pressed.
        /// </summary>
        /// <param name="sender">The button object that was pressed.</param>
        /// <param name="e">EventArgs containing information about the button press.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "xml files (*.xml)|*.xml"; // Only lets user save as a .xml file

            saveFile.FilterIndex = 2;
            saveFile.RestoreDirectory = true;

            // Check if the file was chosen or created successfully
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                // Creates a new StreamWriter object to write to the newly opened file
                using (StreamWriter outStream = new StreamWriter(saveFile.FileName))
                {
                    this.spreadsheet.Save(outStream);
                }
            }
        }

        /// <summary>
        /// The event called when the load button has been pressed.
        /// </summary>
        /// <param name="sender">The button object that was pressed.</param>
        /// <param name="e">EventArgs containing information about the button press.</param>
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog loadFile = new OpenFileDialog())
            {
                loadFile.InitialDirectory = "c:\\";
                loadFile.Filter = "xml files (*.xml)|*.xml"; // Only reads from a .mxl file
                loadFile.FilterIndex = 2;
                loadFile.RestoreDirectory = true;

                // Check that the file was opened successfully
                if (loadFile.ShowDialog() == DialogResult.OK)
                {
                    // Creates a new StreamReader object to read from the newly opened file
                    using (StreamReader inStream = new StreamReader(loadFile.OpenFile()))
                    {
                        this.spreadsheet.Load(inStream);
                    }
                }
            }
        }

        /// <summary>
        /// The event called when the clear button has been pressed.
        /// </summary>
        /// <param name="sender">The button object that was pressed.</param>
        /// <param name="e">EventArgs containing information about the button press.</param>
        private void ClearStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.spreadsheet.ClearSpreadsheet();
        }
    }
}