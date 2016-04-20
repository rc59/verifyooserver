using CognithoServer.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsToken
    {
        public string CreateTokenAndUrl(AuthToken token, string urlCallback, TokenType tokenType)
        {
            if (token != null)
            {
                MongoCollection<AuthToken> authTokens = new UtilsDB().GetCollectionAuthTokens();
                token.TokenType = tokenType.ToString();
                token.Created = DateTime.Now;
                authTokens.Insert(token);

                string url = String.Format(Consts.BASE_URL_AUTH, token.TokenType, token.AuthTokenId, token.UserName, urlCallback);

                return url;
            }
            else
            {
                new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errTokenIsInvalid, ConstsErrorCodes.ERROR_CODE_8);
                return "";
            }
        }
    }
}