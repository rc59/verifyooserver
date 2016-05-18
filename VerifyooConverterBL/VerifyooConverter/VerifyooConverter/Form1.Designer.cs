namespace VerifyooConverter
{
    partial class Form1
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
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblFpLow = new System.Windows.Forms.Label();
            this.lblFpMed = new System.Windows.Forms.Label();
            this.lblFpHigh = new System.Windows.Forms.Label();
            this.txtThreasholdLow = new System.Windows.Forms.TextBox();
            this.txtThreasholdMedium = new System.Windows.Forms.TextBox();
            this.txtThreasholdHigh = new System.Windows.Forms.TextBox();
            this.lblFnLow = new System.Windows.Forms.Label();
            this.lblFnMed = new System.Windows.Forms.Label();
            this.lblFnHigh = new System.Windows.Forms.Label();
            this.lblFNTotalGestures = new System.Windows.Forms.Label();
            this.lblFPTotalGestures = new System.Windows.Forms.Label();
            this.txtObjectID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSubCounter = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLimit = new System.Windows.Forms.TextBox();
            this.txtDevice = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInstruction = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblValidGestures = new System.Windows.Forms.Label();
            this.lblInvalidGestures = new System.Windows.Forms.Label();
            this.lblInvalidTemplates = new System.Windows.Forms.Label();
            this.lblValidTemplates = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 20);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(105, 17);
            this.lblProgress.TabIndex = 0;
            this.lblProgress.Text = "[Ready to start]";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 587);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 648);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(282, 22);
            this.txtPath.TabIndex = 2;
            this.txtPath.Text = "C:\\Temp\\Result.csv";
            this.txtPath.Visible = false;
            this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(93, 587);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblFpLow
            // 
            this.lblFpLow.AutoSize = true;
            this.lblFpLow.Location = new System.Drawing.Point(128, 134);
            this.lblFpLow.Name = "lblFpLow";
            this.lblFpLow.Size = new System.Drawing.Size(132, 17);
            this.lblFpLow.TabIndex = 4;
            this.lblFpLow.Text = "[False Positive Low]";
            // 
            // lblFpMed
            // 
            this.lblFpMed.AutoSize = true;
            this.lblFpMed.Location = new System.Drawing.Point(128, 214);
            this.lblFpMed.Name = "lblFpMed";
            this.lblFpMed.Size = new System.Drawing.Size(156, 17);
            this.lblFpMed.TabIndex = 5;
            this.lblFpMed.Text = "[False Positive Medium]";
            // 
            // lblFpHigh
            // 
            this.lblFpHigh.AutoSize = true;
            this.lblFpHigh.Location = new System.Drawing.Point(128, 294);
            this.lblFpHigh.Name = "lblFpHigh";
            this.lblFpHigh.Size = new System.Drawing.Size(136, 17);
            this.lblFpHigh.TabIndex = 6;
            this.lblFpHigh.Text = "[False Positive High]";
            // 
            // txtThreasholdLow
            // 
            this.txtThreasholdLow.Location = new System.Drawing.Point(12, 131);
            this.txtThreasholdLow.Name = "txtThreasholdLow";
            this.txtThreasholdLow.Size = new System.Drawing.Size(100, 22);
            this.txtThreasholdLow.TabIndex = 7;
            // 
            // txtThreasholdMedium
            // 
            this.txtThreasholdMedium.Location = new System.Drawing.Point(12, 209);
            this.txtThreasholdMedium.Name = "txtThreasholdMedium";
            this.txtThreasholdMedium.Size = new System.Drawing.Size(100, 22);
            this.txtThreasholdMedium.TabIndex = 8;
            // 
            // txtThreasholdHigh
            // 
            this.txtThreasholdHigh.Location = new System.Drawing.Point(12, 294);
            this.txtThreasholdHigh.Name = "txtThreasholdHigh";
            this.txtThreasholdHigh.Size = new System.Drawing.Size(100, 22);
            this.txtThreasholdHigh.TabIndex = 9;
            // 
            // lblFnLow
            // 
            this.lblFnLow.AutoSize = true;
            this.lblFnLow.Location = new System.Drawing.Point(128, 165);
            this.lblFnLow.Name = "lblFnLow";
            this.lblFnLow.Size = new System.Drawing.Size(139, 17);
            this.lblFnLow.TabIndex = 11;
            this.lblFnLow.Text = "[False Negative Low]";
            // 
            // lblFnMed
            // 
            this.lblFnMed.AutoSize = true;
            this.lblFnMed.Location = new System.Drawing.Point(128, 242);
            this.lblFnMed.Name = "lblFnMed";
            this.lblFnMed.Size = new System.Drawing.Size(163, 17);
            this.lblFnMed.TabIndex = 12;
            this.lblFnMed.Text = "[False Negative Medium]";
            // 
            // lblFnHigh
            // 
            this.lblFnHigh.AutoSize = true;
            this.lblFnHigh.Location = new System.Drawing.Point(128, 327);
            this.lblFnHigh.Name = "lblFnHigh";
            this.lblFnHigh.Size = new System.Drawing.Size(143, 17);
            this.lblFnHigh.TabIndex = 13;
            this.lblFnHigh.Text = "[False Negative High]";
            // 
            // lblFNTotalGestures
            // 
            this.lblFNTotalGestures.AutoSize = true;
            this.lblFNTotalGestures.Location = new System.Drawing.Point(12, 92);
            this.lblFNTotalGestures.Name = "lblFNTotalGestures";
            this.lblFNTotalGestures.Size = new System.Drawing.Size(132, 17);
            this.lblFNTotalGestures.TabIndex = 14;
            this.lblFNTotalGestures.Text = "[Total FN Gestures]";
            this.lblFNTotalGestures.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblFPTotalGestures
            // 
            this.lblFPTotalGestures.AutoSize = true;
            this.lblFPTotalGestures.Location = new System.Drawing.Point(12, 59);
            this.lblFPTotalGestures.Name = "lblFPTotalGestures";
            this.lblFPTotalGestures.Size = new System.Drawing.Size(131, 17);
            this.lblFPTotalGestures.TabIndex = 10;
            this.lblFPTotalGestures.Text = "[Total FP Gestures]";
            this.lblFPTotalGestures.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtObjectID
            // 
            this.txtObjectID.Location = new System.Drawing.Point(95, 464);
            this.txtObjectID.Name = "txtObjectID";
            this.txtObjectID.Size = new System.Drawing.Size(165, 22);
            this.txtObjectID.TabIndex = 15;
            this.txtObjectID.Text = "roy-STAM123";
            this.txtObjectID.TextChanged += new System.EventHandler(this.txtObjectID_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 464);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblSubCounter
            // 
            this.lblSubCounter.AutoSize = true;
            this.lblSubCounter.Location = new System.Drawing.Point(141, 20);
            this.lblSubCounter.Name = "lblSubCounter";
            this.lblSubCounter.Size = new System.Drawing.Size(0, 17);
            this.lblSubCounter.TabIndex = 18;
            this.lblSubCounter.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 556);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 17);
            this.label2.TabIndex = 19;
            this.label2.Text = "Limit:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // txtLimit
            // 
            this.txtLimit.Location = new System.Drawing.Point(93, 556);
            this.txtLimit.Name = "txtLimit";
            this.txtLimit.Size = new System.Drawing.Size(165, 22);
            this.txtLimit.TabIndex = 20;
            this.txtLimit.TextChanged += new System.EventHandler(this.txtLimit_TextChanged);
            // 
            // txtDevice
            // 
            this.txtDevice.Location = new System.Drawing.Point(93, 494);
            this.txtDevice.Name = "txtDevice";
            this.txtDevice.Size = new System.Drawing.Size(165, 22);
            this.txtDevice.TabIndex = 22;
            this.txtDevice.Text = "LGE Nexus 5";
            this.txtDevice.TextChanged += new System.EventHandler(this.txtDevice_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 494);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 17);
            this.label3.TabIndex = 21;
            this.label3.Text = "Device:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // txtInstruction
            // 
            this.txtInstruction.Location = new System.Drawing.Point(93, 525);
            this.txtInstruction.Name = "txtInstruction";
            this.txtInstruction.Size = new System.Drawing.Size(165, 22);
            this.txtInstruction.TabIndex = 23;
            this.txtInstruction.TextChanged += new System.EventHandler(this.txtInstruction_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 525);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 17);
            this.label4.TabIndex = 24;
            this.label4.Text = "Instruction:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 408);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 17);
            this.label5.TabIndex = 25;
            this.label5.Text = "Valid Gestures:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 434);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 17);
            this.label6.TabIndex = 26;
            this.label6.Text = "Invalid Gestures:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // lblValidGestures
            // 
            this.lblValidGestures.AutoSize = true;
            this.lblValidGestures.Location = new System.Drawing.Point(128, 408);
            this.lblValidGestures.Name = "lblValidGestures";
            this.lblValidGestures.Size = new System.Drawing.Size(109, 17);
            this.lblValidGestures.TabIndex = 27;
            this.lblValidGestures.Text = "[Valid Gestures]";
            this.lblValidGestures.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblValidGestures.Click += new System.EventHandler(this.lblValidGestures_Click);
            // 
            // lblInvalidGestures
            // 
            this.lblInvalidGestures.AutoSize = true;
            this.lblInvalidGestures.Location = new System.Drawing.Point(128, 434);
            this.lblInvalidGestures.Name = "lblInvalidGestures";
            this.lblInvalidGestures.Size = new System.Drawing.Size(118, 17);
            this.lblInvalidGestures.TabIndex = 28;
            this.lblInvalidGestures.Text = "[Invalid Gestures]";
            this.lblInvalidGestures.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblInvalidGestures.Click += new System.EventHandler(this.lblInvalidGestures_Click);
            // 
            // lblInvalidTemplates
            // 
            this.lblInvalidTemplates.AutoSize = true;
            this.lblInvalidTemplates.Location = new System.Drawing.Point(128, 382);
            this.lblInvalidTemplates.Name = "lblInvalidTemplates";
            this.lblInvalidTemplates.Size = new System.Drawing.Size(126, 17);
            this.lblInvalidTemplates.TabIndex = 32;
            this.lblInvalidTemplates.Text = "[Invalid Templates]";
            this.lblInvalidTemplates.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblInvalidTemplates.Click += new System.EventHandler(this.label7_Click);
            // 
            // lblValidTemplates
            // 
            this.lblValidTemplates.AutoSize = true;
            this.lblValidTemplates.Location = new System.Drawing.Point(128, 356);
            this.lblValidTemplates.Name = "lblValidTemplates";
            this.lblValidTemplates.Size = new System.Drawing.Size(117, 17);
            this.lblValidTemplates.TabIndex = 31;
            this.lblValidTemplates.Text = "[Valid Templates]";
            this.lblValidTemplates.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 382);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(122, 17);
            this.label9.TabIndex = 30;
            this.label9.Text = "Invalid Templates:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 356);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 17);
            this.label10.TabIndex = 29;
            this.label10.Text = "Valid Templates:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 691);
            this.Controls.Add(this.lblInvalidTemplates);
            this.Controls.Add(this.lblValidTemplates);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblInvalidGestures);
            this.Controls.Add(this.lblValidGestures);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtInstruction);
            this.Controls.Add(this.txtDevice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLimit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSubCounter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtObjectID);
            this.Controls.Add(this.lblFNTotalGestures);
            this.Controls.Add(this.lblFnHigh);
            this.Controls.Add(this.lblFnMed);
            this.Controls.Add(this.lblFnLow);
            this.Controls.Add(this.lblFPTotalGestures);
            this.Controls.Add(this.txtThreasholdHigh);
            this.Controls.Add(this.txtThreasholdMedium);
            this.Controls.Add(this.txtThreasholdLow);
            this.Controls.Add(this.lblFpHigh);
            this.Controls.Add(this.lblFpMed);
            this.Controls.Add(this.lblFpLow);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblProgress);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblFpLow;
        private System.Windows.Forms.Label lblFpMed;
        private System.Windows.Forms.Label lblFpHigh;
        private System.Windows.Forms.TextBox txtThreasholdLow;
        private System.Windows.Forms.TextBox txtThreasholdMedium;
        private System.Windows.Forms.TextBox txtThreasholdHigh;
        private System.Windows.Forms.Label lblFnLow;
        private System.Windows.Forms.Label lblFnMed;
        private System.Windows.Forms.Label lblFnHigh;
        private System.Windows.Forms.Label lblFNTotalGestures;
        private System.Windows.Forms.Label lblFPTotalGestures;
        private System.Windows.Forms.TextBox txtObjectID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSubCounter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLimit;
        private System.Windows.Forms.TextBox txtDevice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInstruction;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblValidGestures;
        private System.Windows.Forms.Label lblInvalidGestures;
        private System.Windows.Forms.Label lblInvalidTemplates;
        private System.Windows.Forms.Label lblValidTemplates;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}

