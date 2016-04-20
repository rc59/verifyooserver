using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class Consts
    {
        public static double SCORE_FOR_IDENTICAL_SHAPES = 100;
        public static double SHAPE_REC_THREASHOLD = 3;
        public static double Z_SCORE_THRESHOLD = 2.7;
        public static double ANGLE_CHANGE_THRESHOLD_LOW = 10;
        public static double ANGLE_CHANGE_THRESHOLD_MED = 30;
        public static double ANGLE_CHANGE_THRESHOLD_HIGH = 75;

        public static double MIN_STROKE_LENGTH = 100;
        
        public static int ALLOWED_MISMATCH = 1;

        public static string APPLICATION_OBJ_NORM_MGR = "APPLICATION_OBJ_NORM_MGR";

        public static string FIELD_INSTRUCTION_IDX = "InstructionIdx";
        public static string FIELD_NAME = "Name";
        public static string FIELD_AUTH_TOKEN_ID = "AuthTokenId";
        public static string FIELD_AUTH_APP_ID = "AppId";

        public static string BASE_URL_AUTH = "http://www.cognitho.com/launch?action={0}&tokenId={1}&user={2}&callbackUrl={3}";
        public static string BASE_URL_REGISTER_UPDATE = "http://www.cognitho.com/launch?action={0}&tokenId={1}&user={2}&urlVerify={3}";
    }

    public enum TokenType
    {
        CREATE,
        UPDATE,
        SIGNIN
    }
}