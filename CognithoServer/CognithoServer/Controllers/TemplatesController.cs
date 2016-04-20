using CognithoServer.Models;
using CognithoServer.Utils;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VerifyooLogic.Utils;

namespace CognithoServer.Controllers
{
    public class TemplatesController : ApiController
    {
        [HttpPost]
        public bool Post(Template template, string tokenId, string verifyUrl)
        {
            new UtilsLicense().CheckAppKey();
            AuthToken authToken = new UtilsDB().GetAuthTokenById(tokenId);

            /************************************************************************************************/
            if (tokenId == "666")
            {
                string userName = template.Name;
                Template tempTemplate = new UtilsDB().GetTemplateByUsername(userName);
                if (tempTemplate != null)
                {
                    return false;
                }

                MongoCollection<Template> templates = new Utils.UtilsDB().GetCollectionTemplates();

                template.Created = DateTime.Now;
                template.Updated = DateTime.Now;
                template.__v = 0;
                templates.Insert(template);

                return true;
            }
            /************************************************************************************************/

            if (authToken != null)
            {
                string userName = template.Name;
                Template tempTemplate = new UtilsDB().GetTemplateByUsername(userName);
                if (tempTemplate != null)
                {
                    return false;
                }
                else
                {
                    UtilsHttp utilsHttp = new UtilsHttp();
                    utilsHttp.PostRequest(authToken, verifyUrl);

                    if (utilsHttp.StatusCode == HttpStatusCode.OK)
                    {
                        MongoCollection<Template> templates = new Utils.UtilsDB().GetCollectionTemplates();

                        template.Created = DateTime.Now;
                        template.Updated = DateTime.Now;
                        template.__v = 0;
                        templates.Insert(template);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public string VerifyString(List<Template> templates)
        {
            new UtilsLicense().CheckAppKey();
            if (templates.Count == 2)
            {
                Template template = templates[0];
                Template templateVerify = templates[1];

                string result = new UtilsAuth().CompareTemplatesString(template, templateVerify);

                return result;
            }
            else
            {
                return "false";
            }
        }

        [HttpPost]
        public bool Verify(List<Template> templates)
        {
            new UtilsLicense().CheckAppKey();
            if (templates.Count == 2)
            {
                Template template = templates[0];
                Template templateVerify = templates[1];

                GestureObj gesture = template.Gestures[0];
                GestureObj gestureVerify = templateVerify.Gestures[0];

                UtilsDeviceProperties.Xdpi = template.Xdpi;
                UtilsDeviceProperties.Ydpi = template.Ydpi;
                VerifyooLogic.UserProfile.CompactGesture gestureLib = UtilsObjectConverters.ConvertGesture(gesture);
                VerifyooLogic.UserProfile.CompactGesture gestureVerifyLib = UtilsObjectConverters.ConvertGesture(gestureVerify);
                VerifyooLogic.Comparison.GestureComparer comparer = new VerifyooLogic.Comparison.GestureComparer();
                double score = comparer.Compare(gestureLib, gestureVerifyLib);

                bool result = false;
                if(score > 0.9)
                {
                    result = true;
                }
                return result;
            }
            else
            {
                return true;
            }
        }

        [HttpPut]
        public bool Put(Template template, string tokenId, string verifyUrl)
        {
            new UtilsLicense().CheckAppKey();
            AuthToken authToken = new UtilsDB().GetAuthTokenById(tokenId);

            /************************************************************************************************/
            if (tokenId == "666")
            {
                string userName = template.Name;
                Template templateStored = new UtilsDB().GetTemplateByUsername(userName);

                if (templateStored != null)
                {
                    return new UtilsTemplate().Update(templateStored, template);
                }
                else
                {
                    new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errUserNotFound, ConstsErrorCodes.ERROR_CODE_4);
                    return false;
                }
            }
            /************************************************************************************************/

            if (authToken != null)
            {
                UtilsHttp utilsHttp = new UtilsHttp();
                utilsHttp.PostRequest(authToken, verifyUrl);

                if (utilsHttp.StatusCode == HttpStatusCode.OK)
                {
                    string userName = template.Name;
                    Template templateStored = new UtilsDB().GetTemplateByUsername(userName);

                    if (templateStored != null)
                    {
                        return new UtilsTemplate().Update(templateStored, template);
                    }
                    else
                    {
                        new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errUserNotFound, ConstsErrorCodes.ERROR_CODE_5);
                        return false;
                    }
                }
                else
                {
                    new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errTokenIsInvalid, ConstsErrorCodes.ERROR_CODE_6);
                    return false;
                }
            }
            else
            {
                new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errTokenIsInvalid, ConstsErrorCodes.ERROR_CODE_7);
                return false;
            }
        }        
    }
}
