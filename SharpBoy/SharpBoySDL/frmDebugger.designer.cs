namespace SharpBoy
{
    partial class frmDebugger
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
            this.lblUIROM = new System.Windows.Forms.Label();
            this.lblUIMemory = new System.Windows.Forms.Label();
            this.bJumpToPC = new System.Windows.Forms.Button();
            this.grpRegisters = new System.Windows.Forms.GroupBox();
            this.tbSP = new System.Windows.Forms.TextBox();
            this.lblUISP = new System.Windows.Forms.Label();
            this.tbPC = new System.Windows.Forms.TextBox();
            this.lblUIPC = new System.Windows.Forms.Label();
            this.cbC = new System.Windows.Forms.CheckBox();
            this.cbH = new System.Windows.Forms.CheckBox();
            this.cbN = new System.Windows.Forms.CheckBox();
            this.cbZ = new System.Windows.Forms.CheckBox();
            this.tbE = new System.Windows.Forms.TextBox();
            this.lblUIE = new System.Windows.Forms.Label();
            this.tbC = new System.Windows.Forms.TextBox();
            this.lblUIC = new System.Windows.Forms.Label();
            this.tbF = new System.Windows.Forms.TextBox();
            this.lblUIF = new System.Windows.Forms.Label();
            this.tbHL = new System.Windows.Forms.TextBox();
            this.lblUIHL = new System.Windows.Forms.Label();
            this.tbD = new System.Windows.Forms.TextBox();
            this.lblUID = new System.Windows.Forms.Label();
            this.tbB = new System.Windows.Forms.TextBox();
            this.lblUIB = new System.Windows.Forms.Label();
            this.tbA = new System.Windows.Forms.TextBox();
            this.lblUIA = new System.Windows.Forms.Label();
            this.bEditRAM = new System.Windows.Forms.Button();
            this.lblUITIMAUpdate = new System.Windows.Forms.Label();
            this.lblUIDivUpdate = new System.Windows.Forms.Label();
            this.lblTIMAUpdate = new System.Windows.Forms.Label();
            this.lblDIVUpdate = new System.Windows.Forms.Label();
            this.lblCyclesRun = new System.Windows.Forms.Label();
            this.lblUICyclesRun = new System.Windows.Forms.Label();
            this.bStep = new System.Windows.Forms.Button();
            this.bReset = new System.Windows.Forms.Button();
            this.vlbRAM = new VirtualListBox.VListBox();
            this.vlbROM = new VirtualListBox.VListBox();
            this.lbBreakConditions = new System.Windows.Forms.ListBox();
            this.tbBreakCondition = new System.Windows.Forms.TextBox();
            this.bAddBreakCondition = new System.Windows.Forms.Button();
            this.bRemoveBreakCondition = new System.Windows.Forms.Button();
            this.bRun = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.grpRegisters.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUIROM
            // 
            this.lblUIROM.AutoSize = true;
            this.lblUIROM.Location = new System.Drawing.Point(12, 9);
            this.lblUIROM.Name = "lblUIROM";
            this.lblUIROM.Size = new System.Drawing.Size(32, 13);
            this.lblUIROM.TabIndex = 1;
            this.lblUIROM.Text = "ROM";
            // 
            // lblUIMemory
            // 
            this.lblUIMemory.AutoSize = true;
            this.lblUIMemory.Location = new System.Drawing.Point(596, 10);
            this.lblUIMemory.Name = "lblUIMemory";
            this.lblUIMemory.Size = new System.Drawing.Size(44, 13);
            this.lblUIMemory.TabIndex = 3;
            this.lblUIMemory.Text = "Memory";
            // 
            // bJumpToPC
            // 
            this.bJumpToPC.Location = new System.Drawing.Point(191, 81);
            this.bJumpToPC.Name = "bJumpToPC";
            this.bJumpToPC.Size = new System.Drawing.Size(75, 23);
            this.bJumpToPC.TabIndex = 4;
            this.bJumpToPC.Text = "Jump To PC";
            this.bJumpToPC.UseVisualStyleBackColor = true;
            // 
            // grpRegisters
            // 
            this.grpRegisters.Controls.Add(this.tbSP);
            this.grpRegisters.Controls.Add(this.lblUISP);
            this.grpRegisters.Controls.Add(this.tbPC);
            this.grpRegisters.Controls.Add(this.lblUIPC);
            this.grpRegisters.Controls.Add(this.cbC);
            this.grpRegisters.Controls.Add(this.cbH);
            this.grpRegisters.Controls.Add(this.cbN);
            this.grpRegisters.Controls.Add(this.cbZ);
            this.grpRegisters.Controls.Add(this.tbE);
            this.grpRegisters.Controls.Add(this.lblUIE);
            this.grpRegisters.Controls.Add(this.tbC);
            this.grpRegisters.Controls.Add(this.lblUIC);
            this.grpRegisters.Controls.Add(this.tbF);
            this.grpRegisters.Controls.Add(this.lblUIF);
            this.grpRegisters.Controls.Add(this.tbHL);
            this.grpRegisters.Controls.Add(this.lblUIHL);
            this.grpRegisters.Controls.Add(this.tbD);
            this.grpRegisters.Controls.Add(this.lblUID);
            this.grpRegisters.Controls.Add(this.tbB);
            this.grpRegisters.Controls.Add(this.lblUIB);
            this.grpRegisters.Controls.Add(this.tbA);
            this.grpRegisters.Controls.Add(this.lblUIA);
            this.grpRegisters.Location = new System.Drawing.Point(272, 26);
            this.grpRegisters.Name = "grpRegisters";
            this.grpRegisters.Size = new System.Drawing.Size(239, 146);
            this.grpRegisters.TabIndex = 5;
            this.grpRegisters.TabStop = false;
            this.grpRegisters.Text = "CPU Registers";
            // 
            // tbSP
            // 
            this.tbSP.Enabled = false;
            this.tbSP.Location = new System.Drawing.Point(117, 117);
            this.tbSP.Name = "tbSP";
            this.tbSP.Size = new System.Drawing.Size(62, 20);
            this.tbSP.TabIndex = 21;
            // 
            // lblUISP
            // 
            this.lblUISP.AutoSize = true;
            this.lblUISP.Location = new System.Drawing.Point(97, 120);
            this.lblUISP.Name = "lblUISP";
            this.lblUISP.Size = new System.Drawing.Size(21, 13);
            this.lblUISP.TabIndex = 20;
            this.lblUISP.Text = "SP";
            // 
            // tbPC
            // 
            this.tbPC.Enabled = false;
            this.tbPC.Location = new System.Drawing.Point(26, 117);
            this.tbPC.Name = "tbPC";
            this.tbPC.Size = new System.Drawing.Size(62, 20);
            this.tbPC.TabIndex = 19;
            // 
            // lblUIPC
            // 
            this.lblUIPC.AutoSize = true;
            this.lblUIPC.Location = new System.Drawing.Point(6, 120);
            this.lblUIPC.Name = "lblUIPC";
            this.lblUIPC.Size = new System.Drawing.Size(21, 13);
            this.lblUIPC.TabIndex = 18;
            this.lblUIPC.Text = "PC";
            // 
            // cbC
            // 
            this.cbC.AutoSize = true;
            this.cbC.Enabled = false;
            this.cbC.Location = new System.Drawing.Point(146, 84);
            this.cbC.Name = "cbC";
            this.cbC.Size = new System.Drawing.Size(33, 17);
            this.cbC.TabIndex = 17;
            this.cbC.Text = "C";
            this.cbC.UseVisualStyleBackColor = true;
            // 
            // cbH
            // 
            this.cbH.AutoSize = true;
            this.cbH.Enabled = false;
            this.cbH.Location = new System.Drawing.Point(146, 61);
            this.cbH.Name = "cbH";
            this.cbH.Size = new System.Drawing.Size(34, 17);
            this.cbH.TabIndex = 16;
            this.cbH.Text = "H";
            this.cbH.UseVisualStyleBackColor = true;
            // 
            // cbN
            // 
            this.cbN.AutoSize = true;
            this.cbN.Enabled = false;
            this.cbN.Location = new System.Drawing.Point(146, 38);
            this.cbN.Name = "cbN";
            this.cbN.Size = new System.Drawing.Size(34, 17);
            this.cbN.TabIndex = 15;
            this.cbN.Text = "N";
            this.cbN.UseVisualStyleBackColor = true;
            // 
            // cbZ
            // 
            this.cbZ.AutoSize = true;
            this.cbZ.Enabled = false;
            this.cbZ.Location = new System.Drawing.Point(146, 15);
            this.cbZ.Name = "cbZ";
            this.cbZ.Size = new System.Drawing.Size(33, 17);
            this.cbZ.TabIndex = 14;
            this.cbZ.Text = "Z";
            this.cbZ.UseVisualStyleBackColor = true;
            // 
            // tbE
            // 
            this.tbE.Enabled = false;
            this.tbE.Location = new System.Drawing.Point(95, 65);
            this.tbE.Name = "tbE";
            this.tbE.Size = new System.Drawing.Size(45, 20);
            this.tbE.TabIndex = 13;
            // 
            // lblUIE
            // 
            this.lblUIE.AutoSize = true;
            this.lblUIE.Location = new System.Drawing.Point(75, 68);
            this.lblUIE.Name = "lblUIE";
            this.lblUIE.Size = new System.Drawing.Size(14, 13);
            this.lblUIE.TabIndex = 12;
            this.lblUIE.Text = "E";
            // 
            // tbC
            // 
            this.tbC.Enabled = false;
            this.tbC.Location = new System.Drawing.Point(95, 39);
            this.tbC.Name = "tbC";
            this.tbC.Size = new System.Drawing.Size(45, 20);
            this.tbC.TabIndex = 11;
            // 
            // lblUIC
            // 
            this.lblUIC.AutoSize = true;
            this.lblUIC.Location = new System.Drawing.Point(75, 42);
            this.lblUIC.Name = "lblUIC";
            this.lblUIC.Size = new System.Drawing.Size(14, 13);
            this.lblUIC.TabIndex = 10;
            this.lblUIC.Text = "C";
            // 
            // tbF
            // 
            this.tbF.Enabled = false;
            this.tbF.Location = new System.Drawing.Point(95, 13);
            this.tbF.Name = "tbF";
            this.tbF.Size = new System.Drawing.Size(45, 20);
            this.tbF.TabIndex = 9;
            // 
            // lblUIF
            // 
            this.lblUIF.AutoSize = true;
            this.lblUIF.Location = new System.Drawing.Point(75, 16);
            this.lblUIF.Name = "lblUIF";
            this.lblUIF.Size = new System.Drawing.Size(13, 13);
            this.lblUIF.TabIndex = 8;
            this.lblUIF.Text = "F";
            // 
            // tbHL
            // 
            this.tbHL.Enabled = false;
            this.tbHL.Location = new System.Drawing.Point(26, 91);
            this.tbHL.Name = "tbHL";
            this.tbHL.Size = new System.Drawing.Size(62, 20);
            this.tbHL.TabIndex = 7;
            // 
            // lblUIHL
            // 
            this.lblUIHL.AutoSize = true;
            this.lblUIHL.Location = new System.Drawing.Point(6, 94);
            this.lblUIHL.Name = "lblUIHL";
            this.lblUIHL.Size = new System.Drawing.Size(21, 13);
            this.lblUIHL.TabIndex = 6;
            this.lblUIHL.Text = "HL";
            // 
            // tbD
            // 
            this.tbD.Enabled = false;
            this.tbD.Location = new System.Drawing.Point(26, 65);
            this.tbD.Name = "tbD";
            this.tbD.Size = new System.Drawing.Size(45, 20);
            this.tbD.TabIndex = 5;
            // 
            // lblUID
            // 
            this.lblUID.AutoSize = true;
            this.lblUID.Location = new System.Drawing.Point(6, 68);
            this.lblUID.Name = "lblUID";
            this.lblUID.Size = new System.Drawing.Size(15, 13);
            this.lblUID.TabIndex = 4;
            this.lblUID.Text = "D";
            // 
            // tbB
            // 
            this.tbB.Enabled = false;
            this.tbB.Location = new System.Drawing.Point(26, 39);
            this.tbB.Name = "tbB";
            this.tbB.Size = new System.Drawing.Size(45, 20);
            this.tbB.TabIndex = 3;
            // 
            // lblUIB
            // 
            this.lblUIB.AutoSize = true;
            this.lblUIB.Location = new System.Drawing.Point(6, 42);
            this.lblUIB.Name = "lblUIB";
            this.lblUIB.Size = new System.Drawing.Size(14, 13);
            this.lblUIB.TabIndex = 2;
            this.lblUIB.Text = "B";
            // 
            // tbA
            // 
            this.tbA.Enabled = false;
            this.tbA.Location = new System.Drawing.Point(26, 13);
            this.tbA.Name = "tbA";
            this.tbA.Size = new System.Drawing.Size(45, 20);
            this.tbA.TabIndex = 1;
            // 
            // lblUIA
            // 
            this.lblUIA.AutoSize = true;
            this.lblUIA.Location = new System.Drawing.Point(6, 16);
            this.lblUIA.Name = "lblUIA";
            this.lblUIA.Size = new System.Drawing.Size(14, 13);
            this.lblUIA.TabIndex = 0;
            this.lblUIA.Text = "A";
            // 
            // bEditRAM
            // 
            this.bEditRAM.Location = new System.Drawing.Point(518, 26);
            this.bEditRAM.Name = "bEditRAM";
            this.bEditRAM.Size = new System.Drawing.Size(75, 23);
            this.bEditRAM.TabIndex = 6;
            this.bEditRAM.Text = "button1";
            this.bEditRAM.UseVisualStyleBackColor = true;
            // 
            // lblUITIMAUpdate
            // 
            this.lblUITIMAUpdate.AutoSize = true;
            this.lblUITIMAUpdate.Location = new System.Drawing.Point(269, 183);
            this.lblUITIMAUpdate.Name = "lblUITIMAUpdate";
            this.lblUITIMAUpdate.Size = new System.Drawing.Size(120, 13);
            this.lblUITIMAUpdate.TabIndex = 7;
            this.lblUITIMAUpdate.Text = "Cycles until DIV update:";
            // 
            // lblUIDivUpdate
            // 
            this.lblUIDivUpdate.AutoSize = true;
            this.lblUIDivUpdate.Location = new System.Drawing.Point(269, 205);
            this.lblUIDivUpdate.Name = "lblUIDivUpdate";
            this.lblUIDivUpdate.Size = new System.Drawing.Size(128, 13);
            this.lblUIDivUpdate.TabIndex = 8;
            this.lblUIDivUpdate.Text = "Cycles until TIMA update:";
            // 
            // lblTIMAUpdate
            // 
            this.lblTIMAUpdate.AutoSize = true;
            this.lblTIMAUpdate.Location = new System.Drawing.Point(403, 205);
            this.lblTIMAUpdate.Name = "lblTIMAUpdate";
            this.lblTIMAUpdate.Size = new System.Drawing.Size(35, 13);
            this.lblTIMAUpdate.TabIndex = 9;
            this.lblTIMAUpdate.Text = "label1";
            // 
            // lblDIVUpdate
            // 
            this.lblDIVUpdate.AutoSize = true;
            this.lblDIVUpdate.Location = new System.Drawing.Point(403, 183);
            this.lblDIVUpdate.Name = "lblDIVUpdate";
            this.lblDIVUpdate.Size = new System.Drawing.Size(35, 13);
            this.lblDIVUpdate.TabIndex = 10;
            this.lblDIVUpdate.Text = "label2";
            // 
            // lblCyclesRun
            // 
            this.lblCyclesRun.AutoSize = true;
            this.lblCyclesRun.Location = new System.Drawing.Point(403, 228);
            this.lblCyclesRun.Name = "lblCyclesRun";
            this.lblCyclesRun.Size = new System.Drawing.Size(35, 13);
            this.lblCyclesRun.TabIndex = 12;
            this.lblCyclesRun.Text = "label1";
            // 
            // lblUICyclesRun
            // 
            this.lblUICyclesRun.AutoSize = true;
            this.lblUICyclesRun.Location = new System.Drawing.Point(269, 228);
            this.lblUICyclesRun.Name = "lblUICyclesRun";
            this.lblUICyclesRun.Size = new System.Drawing.Size(107, 13);
            this.lblUICyclesRun.TabIndex = 11;
            this.lblUICyclesRun.Text = "Cycles run this frame:";
            // 
            // bStep
            // 
            this.bStep.Location = new System.Drawing.Point(272, 255);
            this.bStep.Name = "bStep";
            this.bStep.Size = new System.Drawing.Size(75, 23);
            this.bStep.TabIndex = 13;
            this.bStep.Text = "Step";
            this.bStep.UseVisualStyleBackColor = true;
            this.bStep.Click += new System.EventHandler(this.bStep_Click);
            // 
            // bReset
            // 
            this.bReset.Location = new System.Drawing.Point(191, 26);
            this.bReset.Name = "bReset";
            this.bReset.Size = new System.Drawing.Size(75, 23);
            this.bReset.TabIndex = 14;
            this.bReset.Text = "Reset";
            this.bReset.UseVisualStyleBackColor = true;
            this.bReset.Click += new System.EventHandler(this.bReset_Click);
            // 
            // vlbRAM
            // 
            this.vlbRAM.Count = 1;
            this.vlbRAM.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.vlbRAM.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vlbRAM.FormattingEnabled = true;
            this.vlbRAM.ItemHeight = 16;
            this.vlbRAM.Location = new System.Drawing.Point(599, 26);
            this.vlbRAM.Name = "vlbRAM";
            this.vlbRAM.Size = new System.Drawing.Size(173, 452);
            this.vlbRAM.TabIndex = 2;
            this.vlbRAM.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.vlbRAM_DrawItem);
            // 
            // vlbROM
            // 
            this.vlbROM.Count = 1;
            this.vlbROM.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.vlbROM.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vlbROM.FormattingEnabled = true;
            this.vlbROM.ItemHeight = 16;
            this.vlbROM.Location = new System.Drawing.Point(12, 26);
            this.vlbROM.Name = "vlbROM";
            this.vlbROM.Size = new System.Drawing.Size(173, 452);
            this.vlbROM.TabIndex = 0;
            this.vlbROM.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.vlbROM_DrawItem);
            // 
            // lbBreakConditions
            // 
            this.lbBreakConditions.FormattingEnabled = true;
            this.lbBreakConditions.Location = new System.Drawing.Point(191, 344);
            this.lbBreakConditions.Name = "lbBreakConditions";
            this.lbBreakConditions.Size = new System.Drawing.Size(402, 134);
            this.lbBreakConditions.TabIndex = 15;
            // 
            // tbBreakCondition
            // 
            this.tbBreakCondition.Location = new System.Drawing.Point(191, 318);
            this.tbBreakCondition.Name = "tbBreakCondition";
            this.tbBreakCondition.Size = new System.Drawing.Size(170, 20);
            this.tbBreakCondition.TabIndex = 16;
            // 
            // bAddBreakCondition
            // 
            this.bAddBreakCondition.Location = new System.Drawing.Point(367, 315);
            this.bAddBreakCondition.Name = "bAddBreakCondition";
            this.bAddBreakCondition.Size = new System.Drawing.Size(75, 23);
            this.bAddBreakCondition.TabIndex = 17;
            this.bAddBreakCondition.Text = "Add";
            this.bAddBreakCondition.UseVisualStyleBackColor = true;
            // 
            // bRemoveBreakCondition
            // 
            this.bRemoveBreakCondition.Location = new System.Drawing.Point(448, 315);
            this.bRemoveBreakCondition.Name = "bRemoveBreakCondition";
            this.bRemoveBreakCondition.Size = new System.Drawing.Size(75, 23);
            this.bRemoveBreakCondition.TabIndex = 18;
            this.bRemoveBreakCondition.Text = "Remove";
            this.bRemoveBreakCondition.UseVisualStyleBackColor = true;
            // 
            // bRun
            // 
            this.bRun.Location = new System.Drawing.Point(353, 255);
            this.bRun.Name = "bRun";
            this.bRun.Size = new System.Drawing.Size(75, 23);
            this.bRun.TabIndex = 19;
            this.bRun.Text = "Run";
            this.bRun.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(434, 255);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 20;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // frmDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 486);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bRun);
            this.Controls.Add(this.bRemoveBreakCondition);
            this.Controls.Add(this.bAddBreakCondition);
            this.Controls.Add(this.tbBreakCondition);
            this.Controls.Add(this.lbBreakConditions);
            this.Controls.Add(this.bReset);
            this.Controls.Add(this.bStep);
            this.Controls.Add(this.lblCyclesRun);
            this.Controls.Add(this.lblUICyclesRun);
            this.Controls.Add(this.lblDIVUpdate);
            this.Controls.Add(this.lblTIMAUpdate);
            this.Controls.Add(this.lblUIDivUpdate);
            this.Controls.Add(this.lblUITIMAUpdate);
            this.Controls.Add(this.bEditRAM);
            this.Controls.Add(this.grpRegisters);
            this.Controls.Add(this.bJumpToPC);
            this.Controls.Add(this.lblUIMemory);
            this.Controls.Add(this.vlbRAM);
            this.Controls.Add(this.lblUIROM);
            this.Controls.Add(this.vlbROM);
            this.Name = "frmDebugger";
            this.Text = "Debugger";
            this.grpRegisters.ResumeLayout(false);
            this.grpRegisters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VirtualListBox.VListBox vlbROM;
        private System.Windows.Forms.Label lblUIROM;
        private VirtualListBox.VListBox vlbRAM;
        private System.Windows.Forms.Label lblUIMemory;
        private System.Windows.Forms.Button bJumpToPC;
        private System.Windows.Forms.GroupBox grpRegisters;
        private System.Windows.Forms.TextBox tbE;
        private System.Windows.Forms.Label lblUIE;
        private System.Windows.Forms.TextBox tbC;
        private System.Windows.Forms.Label lblUIC;
        private System.Windows.Forms.TextBox tbF;
        private System.Windows.Forms.Label lblUIF;
        private System.Windows.Forms.TextBox tbHL;
        private System.Windows.Forms.Label lblUIHL;
        private System.Windows.Forms.TextBox tbD;
        private System.Windows.Forms.Label lblUID;
        private System.Windows.Forms.TextBox tbB;
        private System.Windows.Forms.Label lblUIB;
        private System.Windows.Forms.TextBox tbA;
        private System.Windows.Forms.Label lblUIA;
        private System.Windows.Forms.CheckBox cbC;
        private System.Windows.Forms.CheckBox cbH;
        private System.Windows.Forms.CheckBox cbN;
        private System.Windows.Forms.CheckBox cbZ;
        private System.Windows.Forms.TextBox tbSP;
        private System.Windows.Forms.Label lblUISP;
        private System.Windows.Forms.TextBox tbPC;
        private System.Windows.Forms.Label lblUIPC;
        private System.Windows.Forms.Button bEditRAM;
        private System.Windows.Forms.Label lblUITIMAUpdate;
        private System.Windows.Forms.Label lblUIDivUpdate;
        private System.Windows.Forms.Label lblTIMAUpdate;
        private System.Windows.Forms.Label lblDIVUpdate;
        private System.Windows.Forms.Label lblCyclesRun;
        private System.Windows.Forms.Label lblUICyclesRun;
        private System.Windows.Forms.Button bStep;
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.ListBox lbBreakConditions;
        private System.Windows.Forms.TextBox tbBreakCondition;
        private System.Windows.Forms.Button bAddBreakCondition;
        private System.Windows.Forms.Button bRemoveBreakCondition;
        private System.Windows.Forms.Button bRun;
        private System.Windows.Forms.Button button2;
    }
}