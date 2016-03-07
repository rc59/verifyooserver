namespace JsonConverter
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnConvertFromDB = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fromTextBox = new System.Windows.Forms.TextBox();
            this.ToTextBox = new System.Windows.Forms.TextBox();
            this.rangeTextLabel = new System.Windows.Forms.Label();
            this.toTextLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(293, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(439, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "[No File Selected]";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(233, 149);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "[Status]";
            // 
            // btnConvertFromDB
            // 
            this.btnConvertFromDB.Location = new System.Drawing.Point(293, 108);
            this.btnConvertFromDB.Name = "btnConvertFromDB";
            this.btnConvertFromDB.Size = new System.Drawing.Size(121, 23);
            this.btnConvertFromDB.TabIndex = 4;
            this.btnConvertFromDB.Text = "Convert from DB";
            this.btnConvertFromDB.UseVisualStyleBackColor = true;
            this.btnConvertFromDB.Click += new System.EventHandler(this.btnConvertFromDB_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(233, 176);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(54, 13);
            this.lblProgress.TabIndex = 5;
            this.lblProgress.Text = "[Progress]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(168, 176);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 6;
            // 
            // fromTextBox
            // 
            this.fromTextBox.Location = new System.Drawing.Point(236, 67);
            this.fromTextBox.Name = "fromTextBox";
            this.fromTextBox.Size = new System.Drawing.Size(100, 20);
            this.fromTextBox.TabIndex = 7;
            // 
            // ToTextBox
            // 
            this.ToTextBox.Location = new System.Drawing.Point(368, 67);
            this.ToTextBox.Name = "ToTextBox";
            this.ToTextBox.Size = new System.Drawing.Size(100, 20);
            this.ToTextBox.TabIndex = 8;
            // 
            // rangeTextLabel
            // 
            this.rangeTextLabel.AutoSize = true;
            this.rangeTextLabel.Location = new System.Drawing.Point(122, 70);
            this.rangeTextLabel.Name = "rangeTextLabel";
            this.rangeTextLabel.Size = new System.Drawing.Size(98, 13);
            this.rangeTextLabel.TabIndex = 9;
            this.rangeTextLabel.Text = "Select Range From";
            // 
            // toTextLabel
            // 
            this.toTextLabel.AutoSize = true;
            this.toTextLabel.Location = new System.Drawing.Point(342, 70);
            this.toTextLabel.Name = "toTextLabel";
            this.toTextLabel.Size = new System.Drawing.Size(20, 13);
            this.toTextLabel.TabIndex = 10;
            this.toTextLabel.Text = "To";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 212);
            this.Controls.Add(this.toTextLabel);
            this.Controls.Add(this.rangeTextLabel);
            this.Controls.Add(this.fromTextBox);
            this.Controls.Add(this.ToTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnConvertFromDB);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "JSON Converter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnConvertFromDB;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fromTextBox;
        private System.Windows.Forms.TextBox ToTextBox;
        private System.Windows.Forms.Label rangeTextLabel;
        private System.Windows.Forms.Label toTextLabel;
    }
}

