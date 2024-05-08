// <copyright file="Cell.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System.ComponentModel;

#pragma warning disable SA1401 // FieldsMustBePrivate

namespace Spreadsheet_Engine
{
    /// <summary>
    /// The base class for all cells within the spreadsheet.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        // --FIELDS-- //

        /// <summary>
        /// Represents the display text of the cell.
        /// </summary>
        protected string? text;

        /// <summary>
        /// Represents the actual value of the cell.
        /// </summary>
        protected string? value;

        /// <summary>
        /// The background color of the cell.
        /// </summary>
        protected uint bgColor;

        /// <summary>
        /// The row index of the cell.
        /// </summary>
        private readonly int rowIndex;

        /// <summary>
        /// The column index of the cell.
        /// </summary>
        private readonly int colIndex;

        /// <summary>
        /// A list of all cells currently referenced by this one.
        /// </summary>
        private List<Cell> referencedCells;

        /// <summary>
        /// The default value for bg color.
        /// </summary>
        private uint defaultColor = 0xFFFFFFFF;

        /// <summary>
        /// The default value for text.
        /// </summary>
        private string defaultText = string.Empty;

        // --CONSTRUCTORS-- //

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="nRowIndex">The row index to intialize this cell with.</param>
        /// <param name="nColIndex">The column index to intialize this cell with.</param>
        protected Cell(int nRowIndex, int nColIndex)
        {
            this.rowIndex = nRowIndex;
            this.colIndex = nColIndex;
            this.referencedCells = new List<Cell>();
            this.bgColor = this.defaultColor; // hex value for white
            this.text = this.defaultText;
        }

        // --EVENTS-- //

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged = (sender, e) => { };

        // --PROPERTIES-- //

        /// <summary>
        /// Gets property for the row index.
        /// </summary>
        public int RowIndex
        {
            get { return this.rowIndex; }
        }

        /// <summary>
        /// Gets property for the column index.
        /// </summary>
        public int ColIndex
        {
            get { return this.colIndex; }
        }

        /// <summary>
        /// Gets or sets property for the text.
        /// </summary>
        public string Text
        {
            get // Returns the empty string if the text value is null.
            {
                if (this.text != null)
                {
                    return this.text;
                }

                return string.Empty;
            }

            set
            {
                if (value != this.text)
                {
                    this.text = value;

                    if (this.PropertyChanged != null)
                    {
                        try
                        {
                            // Call property changed event whenever the text is changed to a new value.
                            this.PropertyChanged(this, new PropertyChangedEventArgs(value));
                        }
                        catch (CircularReferenceException)
                        {
                            this.value = "Circular reference";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets property for the value.
        /// </summary>
        public string Value
        {
            get // returns the empty string if the text value is null.
            {
                if (this.value != null)
                {
                    return this.value;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the background color of this cell.
        /// </summary>
        public uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                this.bgColor = value;

                if (this.PropertyChanged != null)
                {
                    // Call property changed event whenever the text is changed to a new value.
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }

        /// <summary>
        /// Gets the default color for this cell.
        /// </summary>
        public uint DefaultColor
        {
            get { return this.defaultColor; }
        }

        /// <summary>
        /// Gets the default text for this cell.
        /// </summary>
        public string DefaultText
        {
            get { return this.defaultText; }
        }

        // --METHODS-- //

        /// <summary>
        /// Subscribes to all referenced cell's propery changed event.
        /// </summary>
        /// <param name="cells">The list of cells being referenced by this one.</param>
        public void SubscribeToCellsReferenced(Cell[] cells)
        {
            this.UnsubscribeFromAllReferencedCells();

            if (cells != null)
            {
                foreach (Cell cell in cells)
                {
                    if (cell == this)
                    {
                        throw new SelfReferenceException();
                    }

                    this.referencedCells.Add(cell);
                    cell.PropertyChanged += this.ReferencedCellChanged;
                }

                this.CheckCircularReference(this);
            }
        }

        /// <summary>
        /// Unsubscribes from all cell property changed events currently subscribed to.
        /// </summary>
        public void UnsubscribeFromAllReferencedCells()
        {
            if (this.referencedCells != null && this.referencedCells.Count > 0)
            {
                foreach (Cell cell in this.referencedCells)
                {
                    cell.PropertyChanged -= this.ReferencedCellChanged;
                    if (this.referencedCells.Count == 0)
                    {
                        break;
                    }
                }

                this.referencedCells = new List<Cell>(); // clear existing list after unsubbing
            }
        }

        /// <summary>
        /// Recursive helper function that determines if a circular reference exists whenever a new link is made.
        /// </summary>
        /// <param name="original">A reference to the cell with the most recent change to referenced cells.</param>
        /// <exception cref="CircularReferenceException">Throws the respective exception.</exception>
        protected void CheckCircularReference(Cell original)
        {
            if (this.referencedCells.Contains(original))
            {
                throw new CircularReferenceException();
            }
            else
            {
                foreach (Cell cell in this.referencedCells)
                {
                    cell.CheckCircularReference(original);
                }
            }
        }

        /// <summary>
        /// The event called when a referenced cell property changed event is triggered.
        /// </summary>
        /// <param name="sender">The cell changed.</param>
        /// <param name="e">Information about the change.</param>
        protected void ReferencedCellChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(this.text));
            }
            catch (CircularReferenceException)
            {
                this.value = "Circular reference";
            }
        }
    }
}