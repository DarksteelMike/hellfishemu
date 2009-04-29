namespace Sharp8_V3
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.scDisplay = new SdlDotNet.Windows.SurfaceControl();
            this.msMainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetWithDebuggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseAndOpenDebuggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapinputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetFPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playSoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdGetROM = new System.Windows.Forms.OpenFileDialog();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.scDisplay)).BeginInit();
            this.msMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // scDisplay
            // 
            this.scDisplay.AccessibleDescription = "SdlDotNet SurfaceControl";
            this.scDisplay.AccessibleName = "SurfaceControl";
            this.scDisplay.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.scDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scDisplay.Image = ((System.Drawing.Image)(resources.GetObject("scDisplay.Image")));
            this.scDisplay.InitialImage = null;
            this.scDisplay.Location = new System.Drawing.Point(0, 27);
            this.scDisplay.Name = "scDisplay";
            this.scDisplay.Size = new System.Drawing.Size(640, 320);
            this.scDisplay.TabIndex = 0;
            this.scDisplay.TabStop = false;
            // 
            // msMainMenu
            // 
            this.msMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.configurationToolStripMenuItem});
            this.msMainMenu.Location = new System.Drawing.Point(0, 0);
            this.msMainMenu.Name = "msMainMenu";
            this.msMainMenu.Size = new System.Drawing.Size(644, 24);
            this.msMainMenu.TabIndex = 1;
            this.msMainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openROMToolStripMenuItem
            // 
            this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
            this.openROMToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openROMToolStripMenuItem.Text = "&Open ROM";
            this.openROMToolStripMenuItem.Click += new System.EventHandler(this.openROMToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetWithDebuggerToolStripMenuItem,
            this.pauseAndOpenDebuggerToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.debugToolStripMenuItem.Text = "&Debug";
            // 
            // resetWithDebuggerToolStripMenuItem
            // 
            this.resetWithDebuggerToolStripMenuItem.Enabled = false;
            this.resetWithDebuggerToolStripMenuItem.Name = "resetWithDebuggerToolStripMenuItem";
            this.resetWithDebuggerToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.resetWithDebuggerToolStripMenuItem.Text = "&Reset with Debugger";
            this.resetWithDebuggerToolStripMenuItem.Click += new System.EventHandler(this.resetWithDebuggerToolStripMenuItem_Click);
            // 
            // pauseAndOpenDebuggerToolStripMenuItem
            // 
            this.pauseAndOpenDebuggerToolStripMenuItem.Enabled = false;
            this.pauseAndOpenDebuggerToolStripMenuItem.Name = "pauseAndOpenDebuggerToolStripMenuItem";
            this.pauseAndOpenDebuggerToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.pauseAndOpenDebuggerToolStripMenuItem.Text = "&Pause and open Debugger";
            this.pauseAndOpenDebuggerToolStripMenuItem.Click += new System.EventHandler(this.pauseAndOpenDebuggerToolStripMenuItem_Click);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapinputToolStripMenuItem,
            this.targetFPSToolStripMenuItem,
            this.playSoundToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            // 
            // mapinputToolStripMenuItem
            // 
            this.mapinputToolStripMenuItem.Name = "mapinputToolStripMenuItem";
            this.mapinputToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mapinputToolStripMenuItem.Text = "Map &input";
            this.mapinputToolStripMenuItem.Click += new System.EventHandler(this.mapinputToolStripMenuItem_Click);
            // 
            // targetFPSToolStripMenuItem
            // 
            this.targetFPSToolStripMenuItem.Name = "targetFPSToolStripMenuItem";
            this.targetFPSToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.targetFPSToolStripMenuItem.Text = "Target FPS: 60";
            this.targetFPSToolStripMenuItem.Click += new System.EventHandler(this.targetFPSToolStripMenuItem_Click);
            // 
            // playSoundToolStripMenuItem
            // 
            this.playSoundToolStripMenuItem.Checked = true;
            this.playSoundToolStripMenuItem.CheckOnClick = true;
            this.playSoundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.playSoundToolStripMenuItem.Name = "playSoundToolStripMenuItem";
            this.playSoundToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.playSoundToolStripMenuItem.Text = "Play Sound";
            this.playSoundToolStripMenuItem.Click += new System.EventHandler(this.playSoundToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.resetToolStripMenuItem.Text = "&Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(644, 350);
            this.Controls.Add(this.scDisplay);
            this.Controls.Add(this.msMainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MainMenuStrip = this.msMainMenu;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(650, 382);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 382);
            this.Name = "MainForm";
            this.Text = "Sharp8";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.scDisplay)).EndInit();
            this.msMainMenu.ResumeLayout(false);
            this.msMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SdlDotNet.Windows.SurfaceControl scDisplay;
        private System.Windows.Forms.MenuStrip msMainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofdGetROM;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetWithDebuggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseAndOpenDebuggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapinputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem targetFPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playSoundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
    }
}