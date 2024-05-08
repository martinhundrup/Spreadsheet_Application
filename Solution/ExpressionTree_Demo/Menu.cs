// <copyright file="Menu.cs" company="Martin Hundrup">
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
    /// The main class that handles the menu of the ExpressionTree_Demo menu.
    /// </summary>
    internal class Menu
    {
        /// <summary>
        /// The current expression to evaluate.
        /// </summary>
        private ExpressionTree? currentExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
        {
            this.currentExpression = new ExpressionTree("A1+B1+C1");
        }

        /// <summary>
        /// Starts a new demo.
        /// </summary>
        public void RunMenu()
        {
            while (true)
            {
                Console.WriteLine("current expression: " + this.currentExpression.Expression);
                Console.WriteLine("1 => enter a new expression");
                Console.WriteLine("2 => set a variable value");
                Console.WriteLine("3 => evaluate");
                Console.WriteLine("4 => quit");

                string? readLine = Console.ReadLine();
                int input = 0;

                int.TryParse(readLine, out input); // clean input

                switch (input)
                {
                    case 1: // set to new expression

                        Console.Write("enter a new expression: ");
                        string? s = Console.ReadLine();
                        Console.WriteLine();
                        if (s != null)
                        {
                            this.currentExpression = new ExpressionTree(s);
                        }

                        break;

                    case 2: // set variable

                        Console.Write("enter variable name: ");
                        string? s1 = Console.ReadLine();

                        if (s1 != null)
                        {
                            int t = 0;
                            if (int.TryParse(s1[0].ToString(), out t))
                            {
                                // check to see if valid var name
                                Console.WriteLine("error: invalid variable name");
                                Console.WriteLine();
                                break;
                            }
                        }

                        Console.Write("enter variable value: ");
                        string? s2 = Console.ReadLine();
                        double t2 = 0.0;

                        // make sure its a valid number value
                        if (!double.TryParse(s2, out t2))
                        {
                            Console.WriteLine("error: invalid variable value");
                            Console.WriteLine();
                            break;
                        }

                        this.currentExpression.SetVariable(s1, t2);
                        Console.WriteLine();
                        break;

                    case 3: // print evaluation result

                        Console.WriteLine("result: " + this.currentExpression.Evaluate());
                        Console.WriteLine();
                        break;

                    case 4: // exit application

                        Console.WriteLine("exiting");
                        Environment.Exit(0);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
