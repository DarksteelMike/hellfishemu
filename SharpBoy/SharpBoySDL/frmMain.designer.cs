namespace SharpBoy2
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
            this.surfaceControl1 = new SdlDotNet.Windows.SurfaceControl();
            this.msMenus = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdGetROM = new System.Windows.Forms.OpenFileDialog();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAndDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.surfaceControl1)).BeginInit();
            this.msMenus.SuspendLayout();
            this.SuspendLayout();
            // 
            // surfaceControl1
            // 
            this.surfaceControl1.AccessibleDescription = "SdlDotNet SurfaceControl";
            this.surfaceControl1.AccessibleName = "SurfaceControl";
            this.surfaceControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.surfaceControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.surfaceControl1.Image = ((System.Drawing.Image)(resources.GetObject("surfaceControl1.Image")));
            this.surfaceControl1.InitialImage = ((System.Drawing.Image)(resources.GetObject("surfaceControl1.InitialImage")));
            this.surfaceControl1.Location = new System.Drawing.Point(1, 25);
            this.surfaceControl1.Name = "surfaceControl1";
            this.surfaceControl1.Size = new System.Drawing.Size(320, 288);
            this.surfaceControl1.TabIndex = 0;
            this.surfaceControl1.TabStop = false;
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ofdGetROM
            // 
            this.ofdGetROM.Filter = "Gameboy ROMS|*.gb";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem,
            this.resetAndDebugToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Enabled = false;
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debuggerToolStripMenuItem_Click);
            // 
            // resetAndDebugToolStripMenuItem
            // 
            this.resetAndDebugToolStripMenuItem.Enabled = false;
            this.resetAndDebugToolStripMenuItem.Name = "resetAndDebugToolStripMenuItem";
            this.resetAndDebugToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.resetAndDebugToolStripMenuItem.Text = "Reset and Debug";
            this.resetAndDebugToolStripMenuItem.Click += new System.EventHandler(this.resetAndDebugToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 311);
            this.Controls.Add(this.surfaceControl1);
            this.Controls.Add(this.msMenus);
            this.MainMenuStrip = this.msMenus;
            this.Name = "frmMain";
            this.Text = "SharpBoy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.surfaceControl1)).EndInit();
            this.msMenus.ResumeLayout(false);
            this.msMenus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SdlDotNet.Windows.SurfaceControl surfaceControl1;
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