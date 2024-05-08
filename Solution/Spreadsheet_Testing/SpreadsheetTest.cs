// <copyright file="SpreadsheetTest.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Spreadsheet_Engine.Testing
{
    /// <summary>
    /// A testing class for the Spreadsheet class.
    /// </summary>
    public class SpreadsheetTest
    {
        /// <summary>
        /// Used for testing private methods of the Spreadsheet class.
        /// </summary>
        private Spreadsheet spreadsheet = new Spreadsheet(50, 26);

        /// <summary>
        /// The block of code ran prior to each test.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            Assembly.Load("NodeLibrary");
        }

        /// <summary>
        /// Tests the GetCell() function within the Spreadsheet class.
        /// </summary>
        [Test]
        public void TestGetCell()
        {
            Spreadsheet testSheet = new Spreadsheet(5, 5);

            Assert.IsTrue(testSheet.GetCell(3, 3).RowIndex == 3 && testSheet.GetCell(4, 4).ColIndex == 4);
        }

        /// <summary>
        /// Tests the RowCount property within the Spreadsheet class.
        /// </summary>
        [Test]
        public void TestRowCount()
        {
            Spreadsheet testSheet = new Spreadsheet(5, 5);

            Assert.IsTrue(testSheet.RowCount == 5);
        }

        /// <summary>
        /// Tests the ColumnCount property within the Spreadsheet class.
        /// </summary>
        [Test]
        public void TestColumnCount()
        {
            Spreadsheet testSheet = new Spreadsheet(5, 5);

            Assert.IsTrue(testSheet.ColumnCount == 5);
        }

        /// <summary>
        /// Tests the CellPropertyChanged event functionality.
        /// </summary>
        [Test]
        public void TestCellPropertyChanged()
        {
            Spreadsheet testSheet = new Spreadsheet(5, 5);

            testSheet.GetCell(0, 0).Text = "5";
            testSheet.GetCell(1, 1).Text = "=A1";

            Assert.That(testSheet.GetCell(1, 1).Value, Is.EqualTo(testSheet.GetCell(0, 0).Value));
        }

        /// <summary>
        /// Exceptional Case: a spreadsheet with negative bounds is declared.
        /// </summary>
        [Test]
        public void TestNegativeValues()
        {
            try
            {
                Spreadsheet testSheet = new Spreadsheet(-5, -5);
                Assert.Fail();
            }
            catch
            {
                // if an exception is encounted, we successfully checked for invalid values for the spreadsheet creation
                Assert.Pass();
            }
        }

        /// <summary>
        /// Edge Case: a spreadsheet with a 0 bound is declared.
        /// </summary>
        [Test]
        public void TestZeroValue()
        {
            try
            {
                Spreadsheet testSheet = new Spreadsheet(5, 0);
                Spreadsheet testSheet2 = new Spreadsheet(0, 5);
                Assert.Fail();
            }
            catch
            {
                // if an exception is encounted, we successfully checked for invalid values for the spreadsheet creation
                Assert.Pass();
            }
        }

        /// <summary>
        /// Ensures the when a cell's value is changed, all cells references that cell also get updated.
        /// </summary>
        [Test]
        public void TestDynamicCellUpdates()
        {
            Spreadsheet testSheet = new Spreadsheet(5, 5);

            testSheet.GetCell(0, 0).Text = "5";
            testSheet.GetCell(1, 1).Text = "=A1";
            testSheet.GetCell(0, 0).Text = "4";

            Assert.That(testSheet.GetCell(1, 1).Value, Is.EqualTo("4"));
        }

        /// <summary>
        /// Test the IsCellFromVariable method.
        /// </summary>
        [Test]
        public void TestIsCellFromVariable()
        {
            MethodInfo methodInfo = this.GetMethod("IsCellFromVariable", new Type[] { typeof(string) });

            bool case1 = (bool)methodInfo.Invoke(this.spreadsheet, new object[] { "A1" }); // should be true
            bool case2 = (bool)methodInfo.Invoke(this.spreadsheet, new object[] { "A0" }); // should be false
            bool case3 = (bool)methodInfo.Invoke(this.spreadsheet, new object[] { "Z50" }); // should be true

            Assert.That(case1 && !case2 && case3, Is.EqualTo(true));
        }

        /// <summary>
        /// Tests the GetCellFromName method.
        /// </summary>
        [Test]
        public void TestGetCellFromName()
        {
            MethodInfo methodInfo = this.GetMethod("GetCellFromName", new Type[] { typeof(string) });

            bool case1 = this.spreadsheet.GetCell(0, 0) == (Cell)methodInfo.Invoke(this.spreadsheet, new object[] { "A1" }); // should be true
            bool case2 = this.spreadsheet.GetCell(49, 25) == (Cell)methodInfo.Invoke(this.spreadsheet, new object[] { "Z50" }); // should be true

            Assert.That(case1 && case2, Is.EqualTo(true));
        }

        /// <summary>
        /// Tests the GetCellsFromExpression method.
        /// </summary>
        [Test]
        public void TestGetCellsFromExpression()
        {
            MethodInfo methodInfo = this.GetMethod("GetCellsFromExpression", new Type[] { typeof(string) });

            Cell[] cellArr = { this.spreadsheet.GetCell(0, 0), this.spreadsheet.GetCell(49, 25), this.spreadsheet.GetCell(11, 1) };

            Assert.That(methodInfo.Invoke(this.spreadsheet, new object[] { "=A1 * Z50 / (25 - B12)" }), Is.EqualTo(cellArr));
        }

        /// <summary>
        /// Tests the edge cases of GetCellsFromExpression method.
        /// </summary>
        [Test]
        public void TestGetCellsFromExpressionEdgeCase()
        {
            MethodInfo methodInfo = this.GetMethod("GetCellsFromExpression", new Type[] { typeof(string) });

            try
            {
                methodInfo.Invoke(this.spreadsheet, new object[] { "=A1 * Z51 / (25 - [12) - hello" });
                Assert.Fail();
            }
            catch
            {
                Assert.Pass();
            }
        }

        /// <summary>
        /// Tests the loading capabilites of the spreadsheet; covers edge cases and regular cases.
        /// </summary>
        [Test]
        public void TestLoading()
        {
            StreamReader reader = new StreamReader("testoutput.xml");
            this.spreadsheet.Load(reader);

            bool case1 = this.spreadsheet.GetCell(0, 0).BGColor.ToString("X8") == "FFFF0000" &&
                this.spreadsheet.GetCell(0, 0).Text == "1";
            bool case2 = this.spreadsheet.GetCell(1, 1).BGColor.ToString("X8") == "FF00FF00" &&
                this.spreadsheet.GetCell(1, 1).Text == "=A1";
            bool case3 = this.spreadsheet.GetCell(2, 2).BGColor.ToString("X8") == "FF0000FF" &&
                this.spreadsheet.GetCell(2, 2).Text == "=A1+B2";

            Assert.IsTrue(case1 && case2 && case3);
        }

        /// <summary>
        /// Tests that a cell displays the correct error message when a self reference is present.
        /// </summary>
        [Test]
        public void TestSelfReference()
        {
            Spreadsheet testSheet = new Spreadsheet(5, 5);

            testSheet.GetCell(1, 2).Text = "=12 + C2 * 45";

            Assert.That(testSheet.GetCell(1, 2).Value, Is.EqualTo("Self reference"));
        }

        /// <summary>
        /// Tests that a cell displays the correct error message when a circular reference is present.
        /// </summary>
        [Test]
        public void TestCircularReference()
        {
            Spreadsheet testSheet = new Spreadsheet(5, 5);

            testSheet.GetCell(0, 0).Text = "=D4"; // A1
            testSheet.GetCell(1, 1).Text = "=A1 + 45"; // B2
            testSheet.GetCell(2, 2).Text = "=B2 / 8"; // C3
            testSheet.GetCell(3, 3).Text = "=A1 - B2 + C3"; // D4

            Assert.That(testSheet.GetCell(3, 3).Value, Is.EqualTo("Circular reference"));
        }

        /// <summary>
        /// Used for testing private methods of classes.
        /// </summary>
        /// <param name="methodName">The name of the method to test.</param>
        /// <param name="parameterTypes">The list of parameters in the method.</param>
        /// <returns>A reference to the method to test.</returns>
        private MethodInfo GetMethod(string methodName, Type[] parameterTypes)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                Assert.Fail("methodName cannot be null or whitespace");
            }

            var method = this.spreadsheet.GetType().GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance,
                null,
                parameterTypes,
                null);

            if (method == null)
            {
                Assert.Fail(string.Format("{0} method not found", methodName));
            }

            return method;
        }
    }
}