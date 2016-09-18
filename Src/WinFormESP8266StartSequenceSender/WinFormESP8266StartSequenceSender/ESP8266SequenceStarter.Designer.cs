namespace WinFormESP8266StartSequenceSender
{
    partial class ESP8266SequenceStarter
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
            this.txtNumMCUs = new System.Windows.Forms.TextBox();
            this.btnAIPA = new System.Windows.Forms.Button();
            this.btnSHTTPR = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.gBoxChooseAction = new System.Windows.Forms.GroupBox();
            this.radRestartLEDSeq = new System.Windows.Forms.RadioButton();
            this.radStopLEDSeq = new System.Windows.Forms.RadioButton();
            this.radStartLEDSeq = new System.Windows.Forms.RadioButton();
            this.btnFindMCUs = new System.Windows.Forms.Button();
            this.gBoxChooseAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter # of Microcontrollers to Sync";
            // 
            // txtNumMCUs
            // 
            this.txtNumMCUs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumMCUs.Location = new System.Drawing.Point(73, 33);
            this.txtNumMCUs.Name = "txtNumMCUs";
            this.txtNumMCUs.Size = new System.Drawing.Size(100, 23);
            this.txtNumMCUs.TabIndex = 1;
            // 
            // btnAIPA
            // 
            this.btnAIPA.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAIPA.Location = new System.Drawing.Point(264, 14);
            this.btnAIPA.Name = "btnAIPA";
            this.btnAIPA.Size = new System.Drawing.Size(119, 42);
            this.btnAIPA.TabIndex = 2;
            this.btnAIPA.Text = "&Add IP Addresses";
            this.btnAIPA.UseVisualStyleBackColor = true;
            this.btnAIPA.Click += new System.EventHandler(this.btnAIPA_Click);
            // 
            // btnSHTTPR
            // 
            this.btnSHTTPR.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSHTTPR.Location = new System.Drawing.Point(32, 154);
            this.btnSHTTPR.Name = "btnSHTTPR";
            this.btnSHTTPR.Size = new System.Drawing.Size(155, 30);
            this.btnSHTTPR.TabIndex = 3;
            this.btnSHTTPR.Text = "&Send HTTP Request";
            this.btnSHTTPR.UseVisualStyleBackColor = true;
            this.btnSHTTPR.Click += new System.EventHandler(this.btnSHTTPR_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(228, 154);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(155, 30);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "E&xit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // gBoxChooseAction
            // 
            this.gBoxChooseAction.Controls.Add(this.radRestartLEDSeq);
            this.gBoxChooseAction.Controls.Add(this.radStopLEDSeq);
            this.gBoxChooseAction.Controls.Add(this.radStartLEDSeq);
            this.gBoxChooseAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBoxChooseAction.Location = new System.Drawing.Point(12, 78);
            this.gBoxChooseAction.Name = "gBoxChooseAction";
            this.gBoxChooseAction.Size = new System.Drawing.Size(377, 56);
            this.gBoxChooseAction.TabIndex = 5;
            this.gBoxChooseAction.TabStop = false;
            this.gBoxChooseAction.Text = "Choose Action...";
            // 
            // radRestartLEDSeq
            // 
            this.radRestartLEDSeq.AutoSize = true;
            this.radRestartLEDSeq.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radRestartLEDSeq.Location = new System.Drawing.Point(239, 22);
            this.radRestartLEDSeq.Name = "radRestartLEDSeq";
            this.radRestartLEDSeq.Size = new System.Drawing.Size(132, 21);
            this.radRestartLEDSeq.TabIndex = 8;
            this.radRestartLEDSeq.TabStop = true;
            this.radRestartLEDSeq.Text = "Restart LED Seq";
            this.radRestartLEDSeq.UseVisualStyleBackColor = true;
            // 
            // radStopLEDSeq
            // 
            this.radStopLEDSeq.AutoSize = true;
            this.radStopLEDSeq.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radStopLEDSeq.Location = new System.Drawing.Point(118, 22);
            this.radStopLEDSeq.Name = "radStopLEDSeq";
            this.radStopLEDSeq.Size = new System.Drawing.Size(115, 21);
            this.radStopLEDSeq.TabIndex = 7;
            this.radStopLEDSeq.TabStop = true;
            this.radStopLEDSeq.Text = "Stop LED Seq";
            this.radStopLEDSeq.UseVisualStyleBackColor = true;
            // 
            // radStartLEDSeq
            // 
            this.radStartLEDSeq.AutoSize = true;
            this.radStartLEDSeq.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radStartLEDSeq.Location = new System.Drawing.Point(6, 22);
            this.radStartLEDSeq.Name = "radStartLEDSeq";
            this.radStartLEDSeq.Size = new System.Drawing.Size(116, 21);
            this.radStartLEDSeq.TabIndex = 6;
            this.radStartLEDSeq.TabStop = true;
            this.radStartLEDSeq.Text = "Start LED Seq";
            this.radStartLEDSeq.UseVisualStyleBackColor = true;
            // 
            // btnFindMCUs
            // 
            this.btnFindMCUs.Location = new System.Drawing.Point(170, 62);
            this.btnFindMCUs.Name = "btnFindMCUs";
            this.btnFindMCUs.Size = new System.Drawing.Size(111, 23);
            this.btnFindMCUs.TabIndex = 6;
            this.btnFindMCUs.Text = "Find MCUs";
            this.btnFindMCUs.UseVisualStyleBackColor = true;
            this.btnFindMCUs.Click += new System.EventHandler(this.btnFindMCUs_Click);
            // 
            // ESP8266SequenceStarter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 210);
            this.Controls.Add(this.btnFindMCUs);
            this.Controls.Add(this.gBoxChooseAction);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSHTTPR);
            this.Controls.Add(this.btnAIPA);
            this.Controls.Add(this.txtNumMCUs);
            this.Controls.Add(this.label1);
            this.Name = "ESP8266SequenceStarter";
            this.Text = "ESP8266 -> MCU LED Sequence Starter Sender";
            this.gBoxChooseAction.ResumeLayout(false);
            this.gBoxChooseAction.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNumMCUs;
        private System.Windows.Forms.Button btnAIPA;
        private System.Windows.Forms.Button btnSHTTPR;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox gBoxChooseAction;
        private System.Windows.Forms.RadioButton radRestartLEDSeq;
        private System.Windows.Forms.RadioButton radStopLEDSeq;
        private System.Windows.Forms.RadioButton radStartLEDSeq;
        private System.Windows.Forms.Button btnFindMCUs;
    }
}

