namespace Sharp8_V3
{
    partial class NumericInputFrom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nudTargetFPS = new System.Windows.Forms.NumericUpDown();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.lblUITargetFPS = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetFPS)).BeginInit();
            this.SuspendLayout();
            // 
            // nudTargetFPS
            // 
            this.nudTargetFPS.Location = new System.Drawing.Point(93, 12);
            this.nudTargetFPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTargetFPS.Name = "nudTargetFPS";
            this.nudTargetFPS.Size = new System.Drawing.Size(43, 20);
            this.nudTargetFPS.TabIndex = 0;
            this.nudTargetFPS.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(12, 38);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 1;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(93, 38);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 2;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // lblUITargetFPS
            // 
            this.lblUITargetFPS.AutoSize = true;
            this.lblUITargetFPS.Location = new System.Drawing.Point(23, 14);
            this.lblUITargetFPS.Name = "lblUITargetFPS";
            this.lblUITargetFPS.Size = new System.Drawing.Size(64, 13);
            this.lblUITargetFPS.TabIndex = 3;
            this.lblUITargetFPS.Text = "Target FPS:";
            // 
            // NumericInputFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(175, 69);
            this.ControlBox = false;
            this.Controls.Add(this.lblUITargetFPS);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.nudTargetFPS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NumericInputFrom";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "NumericInputFrom";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetFPS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudTargetFPS;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Label lblUITargetFPS;
    }
}