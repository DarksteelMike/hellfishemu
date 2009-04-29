using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sharp8_V3
{
    public partial class InputMapperForm : Form
    {
        private int TargetKeyIndex;

        private InputHandler AttachedInput;

        public InputMapperForm(InputHandler NewInput)
        {
            InitializeComponent();
            AttachedInput = NewInput;
            TargetKeyIndex = -1;
            UpdateInfo();
        }

        private void InputMapperForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (TargetKeyIndex != -1)
            {
                AttachedInput.KeyMappings[TargetKeyIndex] = e.KeyCode;
                TargetKeyIndex = -1;
                lblNowConfiguring.Text = "";
                UpdateInfo();
            }
        }

        private void bZero_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 0;
            lblNowConfiguring.Text = "0";
        }

        private void bOne_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 1;
            lblNowConfiguring.Text = "1";
        }

        private void bTwo_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 2;
            lblNowConfiguring.Text = "2";
        }

        private void bThree_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 3;
            lblNowConfiguring.Text = "3";
        }

        private void bFour_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 4;
            lblNowConfiguring.Text = "4";
        }

        private void bFive_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 5;
            lblNowConfiguring.Text = "5";
        }

        private void bSix_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 6;
            lblNowConfiguring.Text = "6";
        }

        private void bSeven_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 7;
            lblNowConfiguring.Text = "7";
        }

        private void bEight_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 8;
            lblNowConfiguring.Text = "8";
        }

        private void bNine_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 9;
            lblNowConfiguring.Text = "9";
        }

        private void bA_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 0xA;
            lblNowConfiguring.Text = "A";
        }

        private void bB_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 0xB;
            lblNowConfiguring.Text = "B";
        }

        private void bC_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 0xC;
            lblNowConfiguring.Text = "C";
        }

        private void bD_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 0xD;
            lblNowConfiguring.Text = "D";
        }

        private void bE_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 0xE;
            lblNowConfiguring.Text = "E";
        }

        private void bF_Click(object sender, EventArgs e)
        {
            TargetKeyIndex = 0xF;
            lblNowConfiguring.Text = "F";
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            AttachedInput.SaveMappings();
            DialogResult = DialogResult.OK;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            AttachedInput.LoadMappings();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            bZero.Text = "0: " + AttachedInput.KeyMappings[0].ToString();
            bOne.Text = "1: " + AttachedInput.KeyMappings[1].ToString();
            bTwo.Text = "2: " + AttachedInput.KeyMappings[2].ToString();
            bThree.Text = "3: " + AttachedInput.KeyMappings[3].ToString();
            bFour.Text = "4: " + AttachedInput.KeyMappings[4].ToString();
            bFive.Text = "5: " + AttachedInput.KeyMappings[5].ToString();
            bSix.Text = "6: " + AttachedInput.KeyMappings[6].ToString();
            bSeven.Text = "7: " + AttachedInput.KeyMappings[7].ToString();
            bEight.Text = "8: " + AttachedInput.KeyMappings[8].ToString();
            bNine.Text = "9: " + AttachedInput.KeyMappings[9].ToString();
            bA.Text = "A: " + AttachedInput.KeyMappings[0xA].ToString();
            bB.Text = "B: " + AttachedInput.KeyMappings[0xB].ToString();
            bC.Text = "C: " + AttachedInput.KeyMappings[0xC].ToString();
            bD.Text = "D: " + AttachedInput.KeyMappings[0xD].ToString();
            bE.Text = "E: " + AttachedInput.KeyMappings[0xE].ToString();
            bF.Text = "F: " + AttachedInput.KeyMappings[0xF].ToString();
        }

        private void bSetDefault_Click(object sender, EventArgs e)
        {
            AttachedInput.SetDefaultMappings();
            UpdateInfo();
        }
    }
}