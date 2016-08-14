using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooConverter.Logic
{
    class UtilsIntructions
    {
        public static string IndexToInstruction(int instructionIdx)
        {
            string instruction = "";
            switch(instructionIdx)
            {
                case 0:
                    instruction = "FIVE";
                    break;
                case 1:
                    instruction = "EIGHT";
                    break;
                case 2:
                    instruction = "ALETTER";
                    break;
                case 3:
                    instruction = "RLETTER";
                    break;
                case 4:
                    instruction = "TRIANGULAR";
                    break;
                case 5:
                    instruction = "HEART";
                    break;
                case 6:
                    instruction = "LINE";
                    break;
            }

            return instruction;
        }
    }
}
