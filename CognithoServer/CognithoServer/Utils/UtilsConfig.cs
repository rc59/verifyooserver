using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsConfig
    {
        public static double SCORE_THRESHOLD = 0.88;

        public static int INSTRUCTIONS_IN_TEMPLATE = 5;
        public static int INSTRUCTIONS_FOR_AUTH = 3;
        public static int NUM_FUTILITY_INSTRUCTIONS = 1;
    }
}