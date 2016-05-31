using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string mPathInput;
        string mPathOutput;
        string mParamName;

        StreamWriter mSwOutput;
        StreamReader mSrInput;

        public Form1()
        {
            InitializeComponent();

            mPathInput = txtInput.Text;
            mPathOutput = txtOutput.Text;
            mParamName = txtParamName.Text;
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            mParamName = txtParamName.Text;
            if (string.Compare(mParamName, "[Param Name]") == 0)
            {
                MessageBox.Show("Please input param name");
            }
            else
            {
                try
                {
                    mSwOutput = File.CreateText(mPathOutput);
                    mSrInput = new StreamReader(mPathInput);
                    mSrInput.ReadLine();

                    WriteLine(mParamName, "INSTRUCTION_LETTER_A");
                    WriteLine(mParamName, "INSTRUCTION_EIGHT");
                    WriteLine(mParamName, "INSTRUCTION_FIVE");
                    WriteLine(mParamName, "INSTRUCTION_HEART");
                    WriteLine(mParamName, "INSTRUCTION_LINES");
                    WriteLine(mParamName, "INSTRUCTION_LETTER_R");
                    WriteLine(mParamName, "INSTRUCTION_TRIANGLE");

                    mSwOutput.Flush();
                    mSwOutput.Close();
                    mSrInput.Close();

                    MessageBox.Show("Finished!!");
                }
                catch (Exception exc)
                {
                    mSwOutput.Close();
                    mSrInput.Close();
                    MessageBox.Show("Error locating files!");
                }
            }

                    
        }

        private void WriteLine(string paramName, string instruction)
        {
            string inputStr = mSrInput.ReadLine();

            string[] listStr = inputStr.Split(',');

            string popMean = listStr[1];
            string popSD = listStr[2];
            string internalAvgSd = listStr[3];
            string internalSdSd = listStr[4];

            string output = string.Format("CreateDoubleNorm(Consts.ConstsParamNames.Gesture.{0}, ConstsInstructions.{1}, {2}, {3}, {4});", paramName, instruction, popMean, popSD, internalAvgSd);
            mSwOutput.WriteLine(output);
        }
    }
}
