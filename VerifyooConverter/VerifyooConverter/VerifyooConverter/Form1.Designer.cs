﻿namespace VerifyooConverter
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
            this.btnStart.Location = new System.Drawing.Point(12, 375);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 566);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(282, 22);
            this.txtPath.TabIndex = 2;
            this.txtPath.Text = "C:\\Temp\\Result.csv";
            this.txtPath.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(93, 375);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 600);
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
    }
}

