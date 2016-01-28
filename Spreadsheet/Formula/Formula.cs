// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016
// Finished by Wolfgang Andris, January 27, 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        public string[] tokens;
        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            // Define the possible patterns for tokens
            String lpPattern = @"^\($";
            String rpPattern = @"^\)$";
            String opPattern = @"^[\+\-*/]$";
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*$";
            String doublePattern = @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:e[\+-]?\d+)?$";

            // Keep track of number of left and right paren
            int nOpenParen = 0;
            int nCloseParen = 0;

            // Empty formula is invalid
            if (formula.Length == 0)
            {
                throw new FormulaFormatException("The formula cannot be empty");
            }

            // Split the input string into tokens and store them in an array
            var tokensList = new List<string>(GetTokens(formula));
            var tokensArray = tokensList.ToArray();

            // Check if the formula contains an invalid token
            for (int i = 0; i < tokensArray.Length - 1; i++)
            {
                if (!Regex.IsMatch(tokensArray[i], lpPattern) && !Regex.IsMatch(tokensArray[i], rpPattern) && !Regex.IsMatch(tokensArray[i], varPattern) &&
                   !Regex.IsMatch(tokensArray[i], doublePattern) && !Regex.IsMatch(tokensArray[i], opPattern))
                {
                    throw new FormulaFormatException("Token number " + i + " is an invalid token");
                }
            }

            // Check if first token is left paren, variable or double
            if (!Regex.IsMatch(tokensArray[0], lpPattern) && !Regex.IsMatch(tokensArray[0], varPattern) && !Regex.IsMatch(tokensArray[0], doublePattern))
            {
                throw new FormulaFormatException("First token has to be a number, a variable or an opening parenthesis");
            }

            // Check if last token is right paren, variable or double
            if (!Regex.IsMatch(tokensArray[tokensArray.Length - 1], rpPattern) && !Regex.IsMatch(tokensArray[tokensArray.Length - 1], varPattern) &&
                !Regex.IsMatch(tokensArray[tokensArray.Length - 1], doublePattern))
            {
                throw new FormulaFormatException("Last token has to be a number, a variable or an opening parenthesis");
            }

            // Loop throgh the whole formula to check if its valid
            for (int i = 0; i < tokensArray.Length - 1; i++)
            {   
                // Increase counter for parenthesis when one is seen            
                if (Regex.IsMatch(tokensArray[i], lpPattern))
                {
                    nOpenParen++;
                }
                else if (Regex.IsMatch(tokensArray[i], rpPattern))
                {
                    nCloseParen++;

                    // If at some point, there are more right than left parenthesis, throw an exception
                    if (nCloseParen > nOpenParen)
                    {
                        throw new FormulaFormatException("There cant be more closing than opening parenthesis at any time");
                    }
                }

                // If token is left paren or operator and next token is not a double, a variable or a left paren, throw an exception
                if (Regex.IsMatch(tokensArray[i], lpPattern) || Regex.IsMatch(tokensArray[i], opPattern))
                {
                    if (!Regex.IsMatch(tokensArray[i + 1], doublePattern) && !Regex.IsMatch(tokensArray[i + 1], varPattern) &&
                       !Regex.IsMatch(tokensArray[i + 1], lpPattern))
                    {
                        throw new FormulaFormatException("Opening parenthesis and operator can only be succeeded by a number," +
                            " a variable or another opening parenthesis");
                    }
                }

                // If token is double, variable or right paren and next token is not an operator or  a right paren, throw exception.
                else if (Regex.IsMatch(tokensArray[i], doublePattern) || Regex.IsMatch(tokensArray[i], varPattern) ||
                    Regex.IsMatch(tokensArray[i], rpPattern))
                {
                    if(!Regex.IsMatch(tokensArray[i + 1], opPattern) && !Regex.IsMatch(tokensArray[i + 1], rpPattern))
                    {
                        throw new FormulaFormatException("Numbers, variables and closing parenthesis can only be succeeded by" +
                            " an operator or another closing parenthesis");
                    }
                }
            }

            // Check if last token is right paren, because loop only continued until next to last token
            if (tokensArray[tokensArray.Length - 1] == ")")
            {
                nCloseParen++;
            }

            // If total number of left and right parentheses is not the same, throw exception
            if (nCloseParen != nOpenParen)
            {
                throw new FormulaFormatException("In total, the formula must contain the same amount of opening parenthesis" +
                    " and closing parenthesis");
            }

            // If everything is fine, store the array of tokens in the created object
            tokens = tokensArray;
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            // Create two stacks, one containing the values, one containing the operators
            var valueStack = new Stack<String>();
            var operatorStack = new Stack<String>();

            // Define patterns for variable and double tokens
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*$";
            String doublePattern = @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:e[\+-]?\d+)?$";

            // Loop through tokens to evaluate the expression
            for (int i = 0; i < tokens.Length; i++)
            {
                // Case token is a double
                if (Regex.IsMatch(tokens[i], doublePattern))
                {
                    // If a multiplication operator is on top of the operator stack, do the mulitplication
                    if (operatorStack.Count != 0 && operatorStack.Peek() == "*")
                    {
                        valueStack.Push(Convert.ToString(Convert.ToDouble(valueStack.Pop()) * Convert.ToDouble(tokens[i])));
                        operatorStack.Pop();
                    }
                    // If a division operator is on top of the operator stack, do the division
                    else if (operatorStack.Count != 0 && operatorStack.Peek() == "/")
                    {
                        // Check if its a division by zero
                        if (Convert.ToDouble(tokens[i]) == 0)
                        {
                            throw new FormulaEvaluationException("Division by zero is impossible");
                        }
                        else
                        {
                            valueStack.Push(Convert.ToString(Convert.ToDouble(valueStack.Pop()) / Convert.ToDouble(tokens[i])));
                            operatorStack.Pop();
                        }
                    }

                    // If another token is on top of the operator stack or its empty, push double
                    else
                    {
                        valueStack.Push(tokens[i]);
                    }
                }

                // Case token is a variable
                else if (Regex.IsMatch(tokens[i], varPattern))
                {

                    // Double to store the value of the variable
                    double value = 0;

                    // Look up the value of the variable. If it is undefined, throw an exception
                    try
                    {
                        value = lookup(tokens[i]);
                    }
                    catch (UndefinedVariableException e)
                    {
                        throw new FormulaEvaluationException("An undefined variable was used in the formula");
                    }

                    // Check if operator stack is zero
                    if (operatorStack.Count != 0)
                    {
                        // If a multiplication operator is on top of the stack, do the multiplication
                        if (operatorStack.Peek() == "*")
                        {
                            valueStack.Push(Convert.ToString(Convert.ToDouble(valueStack.Pop()) * value));
                            operatorStack.Pop();
                        }

                        // If a division operator is on top of the stack, do the division
                        else if (operatorStack.Peek() == "/")
                        {
                            // If its a division by zero, throw an exception
                            if (value == 0)
                            {
                                throw new FormulaEvaluationException("Division by zero is impossible");
                            }
                            else
                            {
                                valueStack.Push(Convert.ToString(Convert.ToDouble(valueStack.Pop()) / value));
                                operatorStack.Pop();
                            }
                        }

                        // If another token is on top of the operator stack, push the value of the variable
                        else
                        {
                            valueStack.Push(Convert.ToString(value));
                        }
                    }

                    // If the operator stack is empty, push the value of the variable
                    else
                    {
                        valueStack.Push(Convert.ToString(value));
                    }
                }

                // Case token is a + or a -
                else if (tokens[i] == "+" || tokens[i] == "-")
                {
                    // Check if operator stack is empty
                    if (operatorStack.Count != 0)
                    {
                        // If there is another plus or minus on top of the operator stack, apply it to
                        // the two values on top of the value stack
                        if (operatorStack.Peek() == "+" || operatorStack.Peek() == "-")
                        {
                            double secondOperand = Convert.ToDouble(valueStack.Pop());
                            double firstOperand = Convert.ToDouble(valueStack.Pop());

                            if (operatorStack.Peek() == "+")
                            {
                                valueStack.Push(Convert.ToString(firstOperand + secondOperand));
                            }
                            if (operatorStack.Peek() == "-")
                            {
                                valueStack.Push(Convert.ToString(firstOperand - secondOperand));
                            }
                            operatorStack.Pop();
                        }
                    }

                    // If operator stack is empty, push the plus or minus
                    operatorStack.Push(tokens[i]);
                }

                // Case token is a multiplication sign, a division sign or a left paren, just push it
                else if (tokens[i] == "*" || tokens[i] == "/" || tokens[i] == "(")
                {
                    operatorStack.Push(tokens[i]);
                }

                // Case token is a right paren
                else
                {
                    // If there is another plus or minus on top of the operator stack, apply it to
                    // the two values on top of the value stack
                    if (operatorStack.Peek() == "+" || operatorStack.Peek() == "-")
                    {
                        double secondOperand = Convert.ToDouble(valueStack.Pop());
                        double firstOperand = Convert.ToDouble(valueStack.Pop());

                        if (operatorStack.Peek() == "+")
                        {
                            valueStack.Push(Convert.ToString(firstOperand + secondOperand));
                        }
                        if (operatorStack.Peek() == "-")
                        {
                            valueStack.Push(Convert.ToString(firstOperand - secondOperand));
                        }
                        operatorStack.Pop();
                    }

                    // Now a left paren is on top of the operator stack, push it
                    operatorStack.Pop();

                    // Check if operator stack is empty
                    if (operatorStack.Count != 0)
                    {
                        // If a multiplication sign is on top of the operator stack, apply
                        // it to the two values on top of the stack
                        if (operatorStack.Peek() == "*")
                        {
                            Double secondOperand = Convert.ToDouble(valueStack.Pop());
                            Double firstOperand = Convert.ToDouble(valueStack.Pop());

                            valueStack.Push(Convert.ToString(firstOperand * secondOperand));
                            operatorStack.Pop();
                        }

                        // If a division sign is on top of the operator stack, apply
                        // it to the two values on top of the stack
                        else if (operatorStack.Peek() == "/")
                        {
                            // If its a division by zero, throw an exception
                            if (Convert.ToDouble(valueStack.Peek()) == 0)
                            {
                                throw new FormulaEvaluationException("Division by zero is impossible");
                            }
                            else
                            {
                                Double secondOperand = Convert.ToDouble(valueStack.Pop());
                                Double firstOperand = Convert.ToDouble(valueStack.Pop());

                                valueStack.Push(Convert.ToString(firstOperand / secondOperand));
                                operatorStack.Pop();
                            }
                        }
                    }
                }
            }

            // After every token is processed, if the operator stack is empty, the value on the 
            // value stack is the result of the formula
            if (operatorStack.Count == 0)
            {
                return Convert.ToDouble(valueStack.Pop());
            }

            // If there is an operator left on the operator stack, apply it to the two values, that
            // are left on the value stack. That result is the result of the formula
            else
            {
                double secondOperand = Convert.ToDouble(valueStack.Pop());
                double firstOperand = Convert.ToDouble(valueStack.Pop());
                if (operatorStack.Peek() == "+")
                {
                    return firstOperand + secondOperand;
                }
                else
                {
                    return firstOperand - secondOperand;
                }
            }
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}

