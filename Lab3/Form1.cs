﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void btnNumberClick(object sender, EventArgs e)
        {
            Button obj = (Button)sender;
            txtBox.Text += obj.Text;
        }

        public void btnOperatorClick(object sender, EventArgs e)
        {
            Button obj = (Button)sender;
        }

        private void btnEqualsClick(object sender, EventArgs e)
        {

        }

        private void btnParenClick(object sender, EventArgs e)
        {

        }
    }
}
