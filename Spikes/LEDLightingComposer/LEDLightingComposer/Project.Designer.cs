namespace LEDLightingComposer
{
    partial class Project
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
            this.label1 = new System.Windows.Forms.Label();
            this.cBoxProjectName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cBoxMCUName = new System.Windows.Forms.ComboBox();
            this.cBoxPinSetup = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNumLEDs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cBoxLEffect = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtEffectStartTime = new System.Windows.Forms.TextBox();
            this.txtEffectDuration = new System.Windows.Forms.TextBox();
            this.btnSave2Project = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLEDPArray = new System.Windows.Forms.Button();
            this.btnLEDCArray = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSongPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project Name";
            // 
            // cBoxProjectName
            // 
            this.cBoxProjectName.FormattingEnabled = true;
            this.cBoxProjectName.Location = new System.Drawing.Point(16, 39);
            this.cBoxProjectName.Name = "cBoxProjectName";
            this.cBoxProjectName.Size = new System.Drawing.Size(102, 21);
            this.cBoxProjectName.TabIndex = 1;
            this.cBoxProjectName.DropDownClosed += new System.EventHandler(this.cBoxProjectName_DropDownClosed);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(121, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MCU Name";
            // 
            // cBoxMCUName
            // 
            this.cBoxMCUName.FormattingEnabled = true;
            this.cBoxMCUName.Location = new System.Drawing.Point(124, 39);
            this.cBoxMCUName.Name = "cBoxMCUName";
            this.cBoxMCUName.Size = new System.Drawing.Size(102, 21);
            this.cBoxMCUName.TabIndex = 3;
            this.cBoxMCUName.DropDownClosed += new System.EventHandler(this.cBoxMCUName_DropDownClosed);
            // 
            // cBoxPinSetup
            // 
            this.cBoxPinSetup.FormattingEnabled = true;
            this.cBoxPinSetup.Location = new System.Drawing.Point(232, 39);
            this.cBoxPinSetup.Name = "cBoxPinSetup";
            this.cBoxPinSetup.Size = new System.Drawing.Size(102, 21);
            this.cBoxPinSetup.TabIndex = 4;
            this.cBoxPinSetup.DropDownClosed += new System.EventHandler(this.cBoxPinSetup_DropDownClosed);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(229, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pin Setup";
            // 
            // txtNumLEDs
            // 
            this.txtNumLEDs.Location = new System.Drawing.Point(341, 39);
            this.txtNumLEDs.Name = "txtNumLEDs";
            this.txtNumLEDs.Size = new System.Drawing.Size(52, 20);
            this.txtNumLEDs.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(338, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "# of LEDs";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(399, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "LED Position Array";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(495, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "LED Color Array";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Lighting Effect";
            // 
            // cBoxLEffect
            // 
            this.cBoxLEffect.FormattingEnabled = true;
            this.cBoxLEffect.Location = new System.Drawing.Point(16, 95);
            this.cBoxLEffect.Name = "cBoxLEffect";
            this.cBoxLEffect.Size = new System.Drawing.Size(102, 21);
            this.cBoxLEffect.TabIndex = 11;
            this.cBoxLEffect.DropDownClosed += new System.EventHandler(this.cBoxLEffect_DropDownClosed);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(121, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Effect_Start (Seconds)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(241, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(129, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Effect Duration (Seconds)";
            // 
            // txtEffectStartTime
            // 
            this.txtEffectStartTime.Location = new System.Drawing.Point(151, 96);
            this.txtEffectStartTime.Name = "txtEffectStartTime";
            this.txtEffectStartTime.Size = new System.Drawing.Size(52, 20);
            this.txtEffectStartTime.TabIndex = 14;
            // 
            // txtEffectDuration
            // 
            this.txtEffectDuration.Location = new System.Drawing.Point(275, 96);
            this.txtEffectDuration.Name = "txtEffectDuration";
            this.txtEffectDuration.Size = new System.Drawing.Size(52, 20);
            this.txtEffectDuration.TabIndex = 15;
            // 
            // btnSave2Project
            // 
            this.btnSave2Project.Location = new System.Drawing.Point(78, 168);
            this.btnSave2Project.Name = "btnSave2Project";
            this.btnSave2Project.Size = new System.Drawing.Size(125, 25);
            this.btnSave2Project.TabIndex = 16;
            this.btnSave2Project.Text = "Save 2 Project";
            this.btnSave2Project.UseVisualStyleBackColor = true;
            this.btnSave2Project.Click += new System.EventHandler(this.btnSave2Project_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(369, 168);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(125, 25);
            this.btnExit.TabIndex = 17;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLEDPArray
            // 
            this.btnLEDPArray.Location = new System.Drawing.Point(399, 36);
            this.btnLEDPArray.Name = "btnLEDPArray";
            this.btnLEDPArray.Size = new System.Drawing.Size(86, 36);
            this.btnLEDPArray.TabIndex = 18;
            this.btnLEDPArray.Text = "Create/Edit Array";
            this.btnLEDPArray.UseVisualStyleBackColor = true;
            this.btnLEDPArray.Click += new System.EventHandler(this.btnLEDPArray_Click);
            // 
            // btnLEDCArray
            // 
            this.btnLEDCArray.Location = new System.Drawing.Point(498, 36);
            this.btnLEDCArray.Name = "btnLEDCArray";
            this.btnLEDCArray.Size = new System.Drawing.Size(86, 36);
            this.btnLEDCArray.TabIndex = 19;
            this.btnLEDCArray.Text = "Create/Edit Array";
            this.btnLEDCArray.UseVisualStyleBackColor = true;
            this.btnLEDCArray.Click += new System.EventHandler(this.btnLEDCArray_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(376, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Song Path";
            // 
            // txtSongPath
            // 
            this.txtSongPath.Enabled = false;
            this.txtSongPath.Location = new System.Drawing.Point(379, 96);
            this.txtSongPath.Name = "txtSongPath";
            this.txtSongPath.Size = new System.Drawing.Size(205, 20);
            this.txtSongPath.TabIndex = 21;
            // 
            // Project
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 205);
            this.Controls.Add(this.txtSongPath);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnLEDCArray);
            this.Controls.Add(this.btnLEDPArray);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave2Project);
            this.Controls.Add(this.txtEffectDuration);
            this.Controls.Add(this.txtEffectStartTime);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cBoxLEffect);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNumLEDs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cBoxPinSetup);
            this.Controls.Add(this.cBoxMCUName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cBoxProjectName);
            this.Controls.Add(this.label1);
            this.Name = "Project";
            this.Text = "Project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cBoxProjectName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cBoxMCUName;
        private System.Windows.Forms.ComboBox cBoxPinSetup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNumLEDs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cBoxLEffect;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtEffectStartTime;
        private System.Windows.Forms.TextBox txtEffectDuration;
        private System.Windows.Forms.Button btnSave2Project;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnLEDPArray;
        private System.Windows.Forms.Button btnLEDCArray;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSongPath;
    }
}