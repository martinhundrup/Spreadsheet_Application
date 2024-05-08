// <copyright file="ExpressionTreeTest.cs" company="Martin Hundrup">
// Copyright (c) Martin Hundrup. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet_Engine.Expression;

namespace Spreadsheet_Engine.Testing
{
    /// <summary>
    /// The testing class for the ExpressionTree type.
    /// </summary>
    internal class ExpressionTreeTest
    {
        /// <summary>
        /// Used for testing private methods of the ExpressionTree class.
        /// </summary>
        private Expression.ExpressionTree expressionTree;

        /// <summary>
        /// Used for testing methods of the OperatorNodeFactory class.
        /// </summary>
        private Expression.OperatorNodeFactory factory;

        /// <summary>
        /// The block of code ran prior to each test.
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            Assembly.Load("NodeLibrary");
            this.expressionTree = new Expression.ExpressionTree();
            this.factory = OperatorNodeFactory.GetInstance();
        }

        /// <summary>
        /// Tests a variety of normal inputs against expected outputs.
        /// </summary>
        /// <param name="expression">The expression to test.</param>
        /// <returns>The evaluation of the expression.</returns>
        [Test]
        [TestCase("1+2+3", ExpectedResult = 6.0)]
        [TestCase("1+20+300", ExpectedResult = 321.0)]
        [TestCase("7", ExpectedResult = 7.0)]
        [TestCase("1+2", ExpectedResult = 3.0)]
        [TestCase("1-2", ExpectedResult = -1.0)]
        [TestCase("1+2+3+4+5+6+7+8+9", ExpectedResult = 45.0)]
        [TestCase("1*4*7", ExpectedResult = 28.0)]
        [TestCase("5/1/2", ExpectedResult = 2.5)]
        [TestCase("5+5*1/2", ExpectedResult = 7.5)]
        [TestCase("2 * 2 ^ 3 ^ 2", ExpectedResult = 1024.0)]
        public double TestEvaluateNormalCases(string expression)
        {
            Expression.ExpressionTree expressionTree = new Expression.ExpressionTree(expression);
            return expressionTree.Evaluate();
        }

        /// <summary>
        /// Tests a variety of boundary inputs against expected outputs.
        /// </summary>
        /// <param name="expression">The expression to test.</param>
        /// <returns>The evaluation of the expression.</returns>
        [Test]
        [TestCase("10/0", ExpectedResult = double.PositiveInfinity)]
        [TestCase("0/100", ExpectedResult = 0.0)]
        public double TestEvaluateEdgeCases(string expression)
        {
            Expression.ExpressionTree expressionTree = new Expression.ExpressionTree(expression);
            return expressionTree.Evaluate();
        }

        /// <summary>
        /// Tests cases which contain variables against expected outputs.
        /// </summary>
        /// <param name="expression">The expression to test.</param>
        /// <returns>The evaluation of the expression.</returns>
        [Test]
        [TestCase("A1+A2", ExpectedResult = 3.0)]
        [TestCase("A1+5", ExpectedResult = 6.0)]
        [TestCase("A3-5-A2", ExpectedResult = 4.0)]
        [TestCase("A1*5*A2", ExpectedResult = 10.0)]
        [TestCase("A4/A3", ExpectedResult = 11.0)]
        [TestCase("(B1) * (4 / B2 + 1) - ((16 + 8))", ExpectedResult = -18.0)]
        [TestCase("((B3 + 4 / 2) * (4 / (B4 + 1)) - (16 + 8))", ExpectedResult = -16.0)]
        public double TestEvaluateVariableCases(string expression)
        {
            Expression.ExpressionTree expressionTree = new Expression.ExpressionTree(expression);
            expressionTree.SetVariable("A1", 11.0);
            expressionTree.SetVariable("A1", 1.0);
            expressionTree.SetVariable("A2", 2.0);
            expressionTree.SetVariable("A3", 11.0);
            expressionTree.SetVariable("A4", 121.0);
            expressionTree.SetVariable("B1", 3.0);
            expressionTree.SetVariable("B2", 4.0);
            expressionTree.SetVariable("B3", 2.0);
            expressionTree.SetVariable("B4", 1.0);
            return expressionTree.Evaluate();
        }

        /// <summary>
        /// Tests the IsOperator() function on addition in the ExpressionTree class.
        /// </summary>
        [Test]
        public void TestIsOperatorAdd()
        {
            Assert.IsTrue(this.factory.IsOperator(Expression.AdditionNode.Operation));
        }

        /// <summary>
        /// Tests the IsOperator() function on subtraction in the ExpressionTree class.
        /// </summary>
        [Test]
        public void TestIsOperatorSub()
        {
            Assert.IsTrue(this.factory.IsOperator(Expression.SubtractionNode.Operation));
        }

        /// <summary>
        /// Tests the IsOperator() function on multiplication in the ExpressionTree class.
        /// </summary>
        [Test]
        public void TestIsOperatorMul()
        {
            Assert.IsTrue(this.factory.IsOperator(Expression.MultiplicationNode.Operation));
        }

        /// <summary>
        /// Tests the IsOperator() function on division in the ExpressionTree class.
        /// </summary>
        [Test]
        public void TestIsOperatorDiv()
        {
            Assert.IsTrue(this.factory.IsOperator(Expression.DivisionNode.Operation));
        }

        /// <summary>
        /// Tests the expression tokenizer.
        /// </summary>
        [Test]
        public void TestExpressionTokenize()
        {
            MethodInfo methodInfo = this.GetMethod("Tokenize", new Type[] { typeof(string) });
            string s = "10 + 4.0 *123 -(( 68 / 2))";
            string[] tokens = { "10", "+", "4.0", "*", "123", "-", "(", "(", "68", "/", "2", ")", ")" };
            Assert.That(methodInfo.Invoke(this.expressionTree, new object[] { s }), Is.EqualTo(tokens));
        }

        /// <summary>
        /// Tests the expression tokenizer.
        /// </summary>
        [Test]
        public void TestExpressionVarTokenize()
        {
            MethodInfo methodInfo = this.GetMethod("Tokenize", new Type[] { typeof(string) });
            string s = "Hello + A5 * 123.000 -( (68 / BB$5))";
            string[] tokens = { "Hello", "+", "A5", "*", "123.000", "-", "(", "(", "68", "/", "BB$5", ")", ")" };
            Assert.That(methodInfo.Invoke(this.expressionTree, new object[] { s }), Is.EqualTo(tokens));
        }

        /// <summary>
        /// Tests the postfix converter.
        /// </summary>
        [Test]
        public void TestPostfixConversion()
        {
            MethodInfo methodInfo = this.GetMethod("ConvertToPostfix", new Type[] { typeof(string) });
            string s = "10.00 + 4 * 123 - ((68.0 / 2.0))";
            string[] postfix = { "10.00", "4", "123", "*", "+", "68.0", "2.0", "/", "-" };
            Assert.That(methodInfo.Invoke(this.expressionTree, new object[] { s }), Is.EqualTo(postfix));
        }

        /// <summary>
        /// Tests a variety of inputs against expected outputs.
        /// </summary>
        /// <param name="name">The token to test.</param>
        /// <returns>The evaluation of the method.</returns>
        [Test]
        [TestCase("A1", ExpectedResult = true)]
        [TestCase("100", ExpectedResult = false)]
        [TestCase("*", ExpectedResult = false)]
        [TestCase("Zhello", ExpectedResult = true)]
        public bool TestIsVariableName(string name)
        {
            Expression.ExpressionTree expressionTree = new Expression.ExpressionTree();
            return expressionTree.IsVariableName(name);
        }

        /// <summary>
        /// Tests the GetVariableNames method.
        /// </summary>
        [Test]
        public void TestGetVariableNames()
        {
            string expression = "A50 + Zhello -+= G+I23";
            Expression.ExpressionTree expressionTree = new Expression.ExpressionTree(expression);
            string[] s = { "A50", "Zhello", "G", "I23" };
            Assert.That(expressionTree.GetVariableNames(expression), Is.EqualTo(s));
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

            var method = this.expressionTree.GetType().GetMethod(
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
