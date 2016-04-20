using CognithoServer.Models;
using CognithoServer.Utils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CognithoServer.Controllers
{
    public class ServicesController : ApiController
    {       
        [HttpPost]
        public string AuthenticateUser(AuthToken token, string urlCallback)
        {
            new UtilsLicense().CheckAppKey();
            return new UtilsToken().CreateTokenAndUrl(token, urlCallback, TokenType.SIGNIN);
        }

        [HttpPost]
        public string RegisterUser(AuthToken token, string urlVerify)
        {
            new UtilsLicense().CheckAppKey();
            return new UtilsToken().CreateTokenAndUrl(token, urlVerify, TokenType.CREATE);
        }

        [HttpPost]
        public string UpdateUser(AuthToken token, string urlVerify)
        {
            new UtilsLicense().CheckAppKey();
            return new UtilsToken().CreateTokenAndUrl(token, urlVerify, TokenType.UPDATE);
        }
    }
}
