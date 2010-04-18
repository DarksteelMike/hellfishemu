namespace SharpBoy
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.scDisplay = new SdlDotNet.Windows.SurfaceControl();
            this.msMenus = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAndDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdGetROM = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.scDisplay)).BeginInit();
            this.msMenus.SuspendLayout();
            this.SuspendLayout();
            // 
            // scDisplay
            // 
            this.scDisplay.AccessibleDescription = "SdlDotNet SurfaceControl";
            this.scDisplay.AccessibleName = "SurfaceControl";
            this.scDisplay.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.scDisplay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.scDisplay.Image = ((System.Drawing.Image)(resources.GetObject("scDisplay.Image")));
            this.scDisplay.InitialImage = ((System.Drawing.Image)(resources.GetObject("scDisplay.InitialImage")));
            this.scDisplay.Location = new System.Drawing.Point(1, 25);
            this.scDisplay.Name = "scDisplay";
            this.scDisplay.Size = new System.Drawing.Size(320, 288);
            this.scDisplay.TabIndex = 0;
            this.scDisplay.TabStop = false;
            // 
            // msMenus
            // 
            this.msMenus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.msMenus.Location = new System.Drawing.Point(0, 0);
            this.msMenus.Name = "msMenus";
            this.msMenus.Size = new System.Drawing.Size(322, 24);
            this.msMenus.TabIndex = 1;
            this.msMenus.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem,
            this.resetAndDebugToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Enabled = false;
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debuggerToolStripMenuItem_Click);
            // 
            // resetAndDebugToolStripMenuItem
            // 
            this.resetAndDebugToolStripMenuItem.Enabled = false;
            this.resetAndDebugToolStripMenuItem.Name = "resetAndDebugToolStripMenuItem";
            this.resetAndDebugToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.resetAndDebugToolStripMenuItem.Text = "Reset and Debug";
            this.resetAndDebugToolStripMenuItem.Click += new System.EventHandler(this.resetAndDebugToolStripMenuItem_Click);
            // 
            // ofdGetROM
            // 
            this.ofdGetROM.Filter = "Gameboy ROMS|*.gb";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 311);
            this.Controls.Add(this.scDisplay);
            this.Controls.Add(this.msMenus);
            this.MainMenuStrip = this.msMenus;
            this.Name = "frmMain";
            this.Text = "SharpBoy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.scDisplay)).EndInit();
            this.msMenus.ResumeLayout(false);
            this.msMenus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SdlDotNet.Windows.SurfaceControl scDisplay;
        private System.Windows.Forms.MenuStrip msMenus;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofdGetROM;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetAndDebugToolStripMenuItem;
    }
}