// Written by Joe Zachary for CS 3500, January 2016.
// Reapired error in Evaluate5.  Added TestMethod Attribute
//    for Evaluate4 and Evaluate5 - JLZ January 25, 2016

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTestCases
{
    /// <summary>
    /// A class containing test cases for the Formula class
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1()
        {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// Another syntax error, more left than right paren.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct4()
        {
            Formula f = new Formula("(3+5)*7)+(/2");
        }

        /// <summary>
        /// Another syntax error, invalid token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5()
        {
            Formula f = new Formula("(3+#)*4");
        }

        /// <summary>
        /// Another syntax error, more right than left paren.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6()
        {
            Formula f = new Formula("(3+5)/12e4*(7+3))");
        }

        /// <summary>
        /// Another syntax error, invalid first token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct7()
        {
            Formula f = new Formula("*(3+4)");
        }

        /// <summary>
        /// Another syntax error, invalid last token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct8()
        {
            Formula f = new Formula("(7+2)/");
        }

        /// <summary>
        /// Another syntax error, operator after opening paren.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct9()
        {
            Formula f = new Formula("3*(/2)");
        }

        /// <summary>
        /// Another syntax error, operator after operator.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct10()
        {
            Formula f = new Formula("4/*");
        }

        /// <summary>
        /// Another syntax error, double after closing paren.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct11()
        {
            Formula f = new Formula("4*(3+5)6");
        }

        /// <summary>
        /// Another syntax error, opening paren after variable.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct12()
        {
            Formula f = new Formula("z(3+2)");
        }

        /// <summary>
        /// Another syntax error, empty formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct13()
        {
            Formula f = new Formula("");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// FormulaEvaluationException (meaning that no variables have
        /// values). The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// Testing UndefinedVariableException, which leads to a FormulaEvaluationException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate6 ()
        {
            Formula f = new Formula("(x + y) * q * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// Double after multiplication operator.
        /// </summary>
        [TestMethod]
        public void Evaluate7()
        {
            Formula f = new Formula("9*4");
            Assert.AreEqual(f.Evaluate(v => 0), 36.0, 1e-6);
        }
        /// <summary>
        /// Double that gets pushed and Double after multiplication operator.
        /// </summary>
        [TestMethod]
        public void Evaluate8()
        {
            Formula f = new Formula("(3 + 12) * 9");
            Assert.AreEqual(f.Evaluate(v => 0), 135.0, 1e-6);
        }
        /// <summary>
        /// Variable after multiplication.
        /// </summary>
        [TestMethod]
        public void Evaluate9()
        {
            Formula f = new Formula("x1 * x2");
            Assert.AreEqual(f.Evaluate(v => 5), 25.0, 1e-6);
        }
        /// <summary>
        /// Variable that gets pushed and variable after multiplication operator.
        /// </summary>
        [TestMethod]
        public void Evaluate10()
        {
            Formula f = new Formula("(4 - x1)*x2");
            Assert.AreEqual(f.Evaluate(v => 3), 3.0, 1e-6);
        }
        /// <summary>
        /// Double divided by zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate11()
        {
            Formula f = new Formula("7/0");
            Assert.AreEqual(f.Evaluate(v => 0), 20.0, 1e-6);
        }
        /// <summary>
        /// Variable divided by zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate12()
        {
            Formula f = new Formula("x1/0");
            Assert.AreEqual(f.Evaluate(v => 0), 20.0, 1e-6);
        }
        /// <summary>
        /// Only double and parenthesis.
        /// </summary>
        [TestMethod]
        public void Evaluate13()
        {
            Formula f = new Formula("(3)");
            Assert.AreEqual(f.Evaluate(Lookup4), 3.0, 1e-6);
        }
        /// <summary>
        /// Only variable and parenthesis.
        /// </summary>
        [TestMethod]
        public void Evaluate14()
        {
            Formula f = new Formula("(x1)");
            Assert.AreEqual(f.Evaluate(v => 10.55), 10.55, 1e-6);
        }
        /// <summary>
        /// Addition with + on operator stack.
        /// </summary>
        [TestMethod]
        public void Evaluate15()
        {
            Formula f = new Formula("3 + 1e-3 + 14");
            Assert.AreEqual(f.Evaluate(Lookup4), 17.001, 1e-6);
        }
        /// <summary>
        /// Subtraction with + on operator stack.
        /// </summary>
        [TestMethod]
        public void Evaluate16()
        {
            Formula f = new Formula("4.1 + 2.3e5 - 6");
            Assert.AreEqual(f.Evaluate(Lookup4), 229998.1, 1e-6);
        }
        /// <summary>
        /// Closing parenthesis with a division operator on top of stack.
        /// </summary>
        [TestMethod]
        public void Evaluate17()
        {
            Formula f = new Formula("(7 + 3)/(1 + 1)");
            Assert.AreEqual(f.Evaluate(Lookup4), 5.0, 1e-6);
        }
        /// <summary>
        /// No empty operator stack at the end
        /// </summary>
        [TestMethod]
        public void Evaluate18()
        {
            Formula f = new Formula("(4.3) - (x1-3)");
            Assert.AreEqual(f.Evaluate(v => 3), 4.3, 1e-6);
        }
        /// <summary>
        /// Just a long calculation.
        /// </summary>
        [TestMethod]
        public void Evaluate19()
        {
            Formula f = new Formula("3*(2e5-7)/100*x1-(x2+6)+2.2e3/10");
            Assert.AreEqual(f.Evaluate(v => 10), 60201.9, 1e-6);
        }

        /// <summary>
        /// Division by a variable which is zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate20()
        {
            Formula f = new Formula("8/x1");
            Assert.AreEqual(f.Evaluate(v => 0), 60201.9, 1e-6);
        }

        /// <summary>
        /// Division by zero in parentheses.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate21()
        {
            Formula f = new Formula("5/(3-3)");
            Assert.AreEqual(f.Evaluate(v => 10), 60201.9, 1e-6);
        }

        /// <summary>
        /// A subtraction that leads to a negative number.
        /// </summary>
        [TestMethod]
        public void Evaluate22()
        {
            Formula f = new Formula("2 + 3*(3 - 5)");
            Assert.AreEqual(f.Evaluate(v => 0), -4, 1e-6);
        }
        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
