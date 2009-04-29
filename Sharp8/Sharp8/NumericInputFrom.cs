using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sharp8_V3
{
    public partial class NumericInputFrom : Form
    {
        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public int Value
        {
            get { return (int)nudTargetFPS.Value; }
            set { nudTargetFPS.Value = (decimal)value; }
        }

        public NumericInputFrom()
        {
            InitializeComponent();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}