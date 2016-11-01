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
        private bool calculated = false;
        public Form1()
        {
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
            calculate(txtBox.Text);
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
            else if (a == (char)Keys.Enter)
            {
                calculate(txtBox.Text);
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
        private void calculate(String expression)
        {
            MSScriptControl.ScriptControl msc = new MSScriptControl.ScriptControl();
            msc.Language = "JavaScript";
            // Add a space so eval calculatues correctly
            while (expression.Contains("--"))
            {
                expression = expression.Replace(@"--", "- -");
                
            }
            // Add * symbol for eval parentheses multiplication
            expression = Regex.Replace(expression, @"(\d)(\()", "$1*$2");
            object answer;
            try
            {
                answer = msc.Eval(expression);
            }
            catch
            {
                answer = "Err";
            }
            txtBox.Text = answer.ToString();
            calculated = true;
        }
    }
}
