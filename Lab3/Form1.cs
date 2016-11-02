using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form1 : Form
    {
        // Global Variable Declaration
        MSScriptControl.ScriptControl msc = new MSScriptControl.ScriptControl();
        private bool calculated = false;

        // Form Constructor
        public Form1()
        {
            msc.Language = "JavaScript";
            InitializeComponent();
        }

        // All button events for main input
        private void btnClick(object sender, EventArgs e)
        {
            Button obj = (Button)sender;
            inputEvent(obj.Text[0]);
        }

        // Keypress events (to add alternate input method)
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char a = e.KeyChar;
            inputEvent(a);
        }

        // Handle all input
        private void inputEvent(char a)
        {
            checkCalculated(a);
            switch (a)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '^':
                    addOperator(a);
                    break;
                case '(':
                case ')':
                    addParen(a);
                    break;
                case '=':
                case (char)Keys.Enter:
                    if (txtBox.TextLength > 0 && checkOperator(txtBox.TextLength - 1))
                    {
                        string answer = calculate(txtBox.Text);
                        txtBox.Text = answer;
                        calculated = true;
                    }
                    break;
                case '<':
                case (char)Keys.Back:
                    if (txtBox.TextLength > 0)
                        txtBox.Text = txtBox.Text.Remove(txtBox.Text.Length - 1);
                    break;
                case '.':
                    // Make sure only one point is in current number
                    int count = 0, i = txtBox.TextLength - 1;
                    for (; i > 0 && checkOperator(i); i--)
                    {
                        if (txtBox.Text[i] == '.')
                        {
                            count++;
                            break;
                        }
                    }
                    if (count == 0) txtBox.Text += '.';
                    break;
                default:
                    if (Char.IsDigit(a)) txtBox.Text += a;
                    break;
            }
            validateExpression();
        }

        // Adds operator to expression only if valid
        private void addOperator(char input)
        {
            if (input == '-' || txtBox.TextLength > 0 && checkOperator(txtBox.TextLength - 1)) 
                txtBox.Text += input;
        }

        // Adds parentheses to expression if valid to do so
        private void addParen(char input)
        {
            if (input == '(') txtBox.Text += input;
            else if (input == ')' && txtBox.Text[txtBox.TextLength - 1] != '(' &&
                Regex.Matches(txtBox.Text, @"\(").Count > Regex.Matches(txtBox.Text, @"\)").Count)
                txtBox.Text += input;
        }

        // Checks if the char at index is an operator
        private bool checkOperator(int index)
        {
            char prev = txtBox.Text[index];
            return prev != '+' && prev != '-' && prev != '/' && prev != '*' && prev != '^';
        }

        // Overload checkOperator function
        private bool checkOperator(char a)
        {
            return a != '+' && a != '-' && a != '/' && a!= '*' && a != '^';
        }

        // If last action was calculate, acts on text accordingly with input
        private void checkCalculated(char a)
        {
            if (calculated && a != (char)Keys.Enter)
            {
                if (checkOperator(a)) txtBox.Text = "";
                calculated = false;
            }
        }

        // Validate expression
        private void validateExpression()
        {
            // Change TextBox back color for if expression is valid, invalid, or calculated
            if (txtBox.TextLength > 0 && !checkOperator(txtBox.TextLength - 1) ||
                Regex.Matches(txtBox.Text, @"\(").Count != Regex.Matches(txtBox.Text, @"\)").Count)
                // Set to a light shade of red
                txtBox.BackColor = Color.FromArgb(255, 235, 235);
            else if (!calculated)
                // Set to a light shade of green
                txtBox.BackColor = Color.FromArgb(235, 255, 235);
            else
                // Set to white
                txtBox.BackColor = Color.White;
            // Set caret at end of text and scroll
            txtBox.Select(txtBox.TextLength, 0);
            txtBox.ScrollToCaret();
        }

        // Calculate the expression
        private string calculate(string expression)
        {
            // Add a space so eval calculatues correctly
            while (expression.Contains("--"))
                expression = expression.Replace(@"--", "- -");
            // Add * symbol for eval parentheses multiplication
            expression = Regex.Replace(expression, @"(\d)(\()", "$1*$2");
            // Calculate inside parentheses first
            while (expression.Contains("("))
            {
                Match m = Regex.Match(expression, @"\(");
                int count = 1, i = m.Index + 1;
                string exp = "";
                for (; i < expression.Length; i++)
                {
                    if (expression[i] == '(') count++;
                    else if (expression[i] == ')')
                    {
                        count--;
                        if (count == 0) break;
                    }
                    exp += expression[i];
                }
                expression = expression.Substring(0, m.Index) + calculate(exp) + expression.Substring(i + 1);
            }
            // Math pow instead of bitwise or ^
            while (expression.Contains("^"))
            {
                expression = Regex.Replace(expression, @"([\.\d(E+)]+)\^([\.\d]+)",
                    m => Math.Pow(Double.Parse(m.Groups[1].Value), Double.Parse(m.Groups[2].Value)).ToString(), RegexOptions.RightToLeft);
            }
            // Calculate value
            object answer;
            try
            {
                answer = msc.Eval(expression);
            }
            catch (Exception ex)
            {
                answer = "Err";
            }
            return answer.ToString();
        }
    }
}
