namespace ESP8266StructSenderProg
{
    partial class StructSenderFrm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNumPixels = new System.Windows.Forms.TextBox();
            this.txtDataPin = new System.Windows.Forms.TextBox();
            this.txtClockPin = new System.Windows.Forms.TextBox();
            this.txtDotstarType = new System.Windows.Forms.TextBox();
            this.txtPixelIndex = new System.Windows.Forms.TextBox();
            this.cboxColor = new System.Windows.Forms.ComboBox();
            this.btnPinStruct = new System.Windows.Forms.Button();
            this.btnChangeColorStruct = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtNumStructs = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(36, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pin Setup Struct";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(363, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Change Pixel Color Struct";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Number of Pixels: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Data Pin #: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Clock Pin #: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(37, 232);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "DOTSTAR_BRG: ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(364, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Pixel Index #: ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(364, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Color: ";
            // 
            // txtNumPixels
            // 
            this.txtNumPixels.Location = new System.Drawing.Point(135, 72);
            this.txtNumPixels.Name = "txtNumPixels";
            this.txtNumPixels.Size = new System.Drawing.Size(100, 20);
            this.txtNumPixels.TabIndex = 8;
            this.txtNumPixels.Text = "10";
            // 
            // txtDataPin
            // 
            this.txtDataPin.Location = new System.Drawing.Point(135, 122);
            this.txtDataPin.Name = "txtDataPin";
            this.txtDataPin.Size = new System.Drawing.Size(100, 20);
            this.txtDataPin.TabIndex = 9;
            this.txtDataPin.Text = "4";
            // 
            // txtClockPin
            // 
            this.txtClockPin.Location = new System.Drawing.Point(135, 174);
            this.txtClockPin.Name = "txtClockPin";
            this.txtClockPin.Size = new System.Drawing.Size(100, 20);
            this.txtClockPin.TabIndex = 10;
            this.txtClockPin.Text = "5";
            // 
            // txtDotstarType
            // 
            this.txtDotstarType.Enabled = false;
            this.txtDotstarType.Location = new System.Drawing.Point(135, 225);
            this.txtDotstarType.Name = "txtDotstarType";
            this.txtDotstarType.Size = new System.Drawing.Size(100, 20);
            this.txtDotstarType.TabIndex = 11;
            this.txtDotstarType.Text = "DOTSTAR_BRG";
            // 
            // txtPixelIndex
            // 
            this.txtPixelIndex.Location = new System.Drawing.Point(444, 72);
            this.txtPixelIndex.Name = "txtPixelIndex";
            this.txtPixelIndex.Size = new System.Drawing.Size(100, 20);
            this.txtPixelIndex.TabIndex = 12;
            this.txtPixelIndex.Text = "0";
            // 
            // cboxColor
            // 
            this.cboxColor.FormattingEnabled = true;
            this.cboxColor.Items.AddRange(new object[] {
            "Red - 0xFF0000",
            "Green - 0x00FF00",
            "Blue - 0x0000FF"});
            this.cboxColor.Location = new System.Drawing.Point(444, 121);
            this.cboxColor.Name = "cboxColor";
            this.cboxColor.Size = new System.Drawing.Size(121, 21);
            this.cboxColor.TabIndex = 13;
            // 
            // btnPinStruct
            // 
            this.btnPinStruct.Location = new System.Drawing.Point(40, 303);
            this.btnPinStruct.Name = "btnPinStruct";
            this.btnPinStruct.Size = new System.Drawing.Size(180, 25);
            this.btnPinStruct.TabIndex = 14;
            this.btnPinStruct.Text = "Send Pin Struct";
            this.btnPinStruct.UseVisualStyleBackColor = true;
            this.btnPinStruct.Click += new System.EventHandler(this.btnPinStruct_Click);
            // 
            // btnChangeColorStruct
            // 
            this.btnChangeColorStruct.Location = new System.Drawing.Point(367, 303);
            this.btnChangeColorStruct.Name = "btnChangeColorStruct";
            this.btnChangeColorStruct.Size = new System.Drawing.Size(180, 25);
            this.btnChangeColorStruct.TabIndex = 15;
            this.btnChangeColorStruct.Text = "Send Change Color Struct";
            this.btnChangeColorStruct.UseVisualStyleBackColor = true;
            this.btnChangeColorStruct.Click += new System.EventHandler(this.btnChangeColorStruct_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(210, 365);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(180, 25);
            this.btnExit.TabIndex = 16;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(244, 268);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "# of Structs to Send";
            // 
            // txtNumStructs
            // 
            this.txtNumStructs.Location = new System.Drawing.Point(246, 284);
            this.txtNumStructs.Name = "txtNumStructs";
            this.txtNumStructs.Size = new System.Drawing.Size(100, 20);
            this.txtNumStructs.TabIndex = 18;
            this.txtNumStructs.Text = "1";
            this.txtNumStructs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(550, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "0 = Head";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(367, 225);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(109, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "ESP8266 IP Address:";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(482, 218);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(134, 20);
            this.txtIPAddress.TabIndex = 21;
            this.txtIPAddress.Text = "192.168.1.81";
            // 
            // StructSenderFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 405);
            this.Controls.Add(this.txtIPAddress);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtNumStructs);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnChangeColorStruct);
            this.Controls.Add(this.btnPinStruct);
            this.Controls.Add(this.cboxColor);
            this.Controls.Add(this.txtPixelIndex);
            this.Controls.Add(this.txtDotstarType);
            this.Controls.Add(this.txtClockPin);
            this.Controls.Add(this.txtDataPin);
            this.Controls.Add(this.txtNumPixels);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "StructSenderFrm";
            this.Text = "ESP8266 Struct(s) Sender - Send Struct to ESP8266";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtNumPixels;
        private System.Windows.Forms.TextBox txtDataPin;
        private System.Windows.Forms.TextBox txtClockPin;
        private System.Windows.Forms.TextBox txtDotstarType;
        private System.Windows.Forms.TextBox txtPixelIndex;
        private System.Windows.Forms.ComboBox cboxColor;
        private System.Windows.Forms.Button btnPinStruct;
        private System.Windows.Forms.Button btnChangeColorStruct;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtNumStructs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtIPAddress;
    }
}

