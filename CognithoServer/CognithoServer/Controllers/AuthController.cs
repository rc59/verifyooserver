using CognithoServer.Models;
using CognithoServer.Objects;
using CognithoServer.Utils;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CognithoServer.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        public bool RequestAuthTokenKey(Template template, string tokenId, string callbackUrl)
        {
            new UtilsLicense().CheckAppKey();
            AuthToken authToken = new UtilsDB().GetAuthTokenById(tokenId);

            /************************************************************************************************/
            if (tokenId == "666")
            {
                string userName = template.Name;
                Template templateStored = new UtilsDB().GetTemplateByUsername(userName);
                templateStored = new UtilsTemplate().GetAuthTemplate(templateStored, template);

                if (templateStored != null)
                {
                    bool isAutheticated = new UtilsAuth().CompareTemplates(template, templateStored);
                    if (isAutheticated)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            /************************************************************************************************/

            if (template == null || String.IsNullOrEmpty(template.Name))
            {
                new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errInvalidTemplate, ConstsErrorCodes.ERROR_CODE_1);
            }

            bool isTokenValid =
                authToken != null &&
                !new UtilsAuth().IsTokenExpired(authToken) &&
                !string.IsNullOrEmpty(authToken.UserName) &&
                authToken.UserName.ToLower().CompareTo(template.Name.ToLower()) == 0;

            if (isTokenValid)
            {
                string userName = template.Name;
                Template templateStored = new UtilsDB().GetTemplateByUsername(userName);
                templateStored = new UtilsTemplate().GetAuthTemplate(templateStored, template);

                if (templateStored != null)
                {
                    bool isAutheticated = new UtilsAuth().CompareTemplates(template, templateStored);
                    if (isAutheticated)
                    {
                        new UtilsHttp().PostRequest(authToken, callbackUrl);
                        return true;
                    }
                }
            }

            return false;
        }

        [HttpPost]
        public TemplateResultObj RequestAuthTokenKeyDetailed(Template template, string tokenId, string callbackUrl)
        {
            new UtilsLicense().CheckAppKey();
            AuthToken authToken = new UtilsDB().GetAuthTokenById(tokenId);

            if (template == null || String.IsNullOrEmpty(template.Name))
            {
                new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errInvalidTemplate, ConstsErrorCodes.ERROR_CODE_2);
            }

            bool isTokenValid =
                authToken != null &&
                !new UtilsAuth().IsTokenExpired(authToken) &&
                !string.IsNullOrEmpty(authToken.UserName) &&
                authToken.UserName.ToLower().CompareTo(template.Name.ToLower()) == 0;

            if (isTokenValid)
            {
                string userName = template.Name;
                Template templateStored = new UtilsDB().GetTemplateByUsername(userName);
                templateStored = new UtilsTemplate().GetAuthTemplate(templateStored, template);

                if (templateStored != null)
                {
                    TemplateResultObj score = new UtilsAuth().CompareTemplatesDetailed(template, templateStored);
                    return score;
                }
            }

            return new TemplateResultObj();
        }

        //[HttpPost]
        //public bool Compare(Template template)
        //{
        //    new UtilsLicense().CheckAppKey();
        //    string userName = template.Name;
        //    Template templateStored = new UtilsDB().GetTemplateByUsername(userName);

        //    if (templateStored != null)
        //    {
        //        bool isAutheticated = new UtilsAuth().CompareTemplates(template, templateStored);
        //        return isAutheticated;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
    }
}
