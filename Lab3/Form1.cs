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
        MSScriptControl.ScriptControl msc = new MSScriptControl.ScriptControl();
        private bool calculated = false;

        public Form1()
        {
            msc.Language = "JavaScript";
            InitializeComponent();
        }

        public void btnNumberClick(object sender, EventArgs e)
        {
            checkCalculated();
            Button obj = (Button)sender;
            txtBox.Text += obj.Text;
        }

        public void btnOperatorClick(object sender, EventArgs e)
        {
            checkCalculated();
            Button obj = (Button)sender;
            addOperator(obj.Text[0]);
        }

        private void btnEqualsClick(object sender, EventArgs e)
        {
            string answer = calculate(txtBox.Text);
            txtBox.Text = answer;
            calculated = true;
        }

        private void btnBackClick(object sender, EventArgs e)
        {
            checkCalculated();
            if (txtBox.TextLength > 0)
            {
                txtBox.Text = txtBox.Text.Remove(txtBox.Text.Length - 1);
            }
        }

        private void btnParenClick(object sender, EventArgs e)
        {
            checkCalculated();
            Button obj = (Button)sender;
            addParen(obj.Text[0]);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            checkCalculated();
            char a = e.KeyChar;
            if (Char.IsDigit(a))
            {
                txtBox.Text += a;
            }
            else if (a == '+' || a == '-' || a == '*' || a == '/' || a == '^')
            {
                addOperator(a);  
            }
            else if (a == '(' || a == ')')
            {
                addParen(a);
            }
            else if (a == (char)Keys.Enter || a == 13)
            {
                btnEquals.PerformClick();
            }
            else if (a == (char)Keys.Back)
            {
                btnBack.PerformClick();
            }
            else if (a == '.')
            {
                int count = 0, i = txtBox.TextLength - 1;
                for (; i > 0 && checkOperator(i); i--)
                {
                    if (txtBox.Text[i] == '.')
                    {
                        count++;
                        break;
                    }
                }
                if (count == 0)
                {
                    txtBox.Text += a;
                }
            }
            else
            {
                Console.WriteLine(a);
            }
        }

        private void addOperator(char input)
        {
            switch (input)
            {
                case '^':
                case '/':
                case '*':
                case '+':
                    if (txtBox.TextLength > 0 && checkOperator(txtBox.TextLength - 1))
                    {
                        txtBox.Text += input;
                    }
                    break;
                case '-':
                    txtBox.Text += input;
                    break;
            }
        }

        private void addParen(char input)
        {
            if (input == '(')
            {
                txtBox.Text += input;
            }
            else if (input == ')')
            {
                if (txtBox.Text[txtBox.TextLength - 1] != '(')
                {
                    txtBox.Text += input;
                }
            }
        }

        private bool checkOperator(int index)
        {
            char prev = txtBox.Text[index];
            return prev != '+' && prev != '-' && prev != '/' && prev != '*' && prev != '^';
        }

        private void checkCalculated()
        {
            if (calculated)
            {
                txtBox.Text = "";
                calculated = false;
            }
        }

        private string calculate(string expression)
        {
            // Add a space so eval calculatues correctly
            while (expression.Contains("--"))
            {
                expression = expression.Replace(@"--", "- -");
            }
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
                expression = Regex.Replace(expression, @"(\d+)\^(\d+)",
                    m => Math.Pow(Double.Parse(m.Groups[1].Value), Double.Parse(m.Groups[2].Value)).ToString());
            }
            // Calculate value
            object answer;
            try
            {
                answer = msc.Eval(expression);
            }
            catch (Exception ex)
            {
                answer = "Err: " + ex.ToString();
            }
            return answer.ToString();
        }
    }
}
