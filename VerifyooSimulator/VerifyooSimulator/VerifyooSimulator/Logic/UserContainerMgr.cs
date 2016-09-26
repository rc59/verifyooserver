using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyooSimulator.Models;

namespace VerifyooSimulator.Logic
{
    public class UserContainerMgr
    {
        Dictionary<string, UserContainer> mDictUserContainers = new Dictionary<string, UserContainer>();

        public Dictionary<string, UserContainer> GetUserContainers()
        {
            return mDictUserContainers;
        }

        public void AddTemplateRegister(ModelTemplate template)
        {
            if (mDictUserContainers.ContainsKey(template.Name.ToLower()))
            {
                mDictUserContainers[template.Name.ToLower()].TemplateRegistration = template;
            }
            else
            {
                UserContainer tempUserContainer = new UserContainer(template.Name.ToLower());
                tempUserContainer.TemplateRegistration = template;
                mDictUserContainers.Add(template.Name.ToLower(), tempUserContainer);
            }
        }

        public void AddTemplateAuth(ModelTemplate template)
        {
            if(mDictUserContainers.ContainsKey(template.Name))
            {
                mDictUserContainers[template.Name].ListTemplatesAuthentication.Add(template);
            }
            else
            {
                UserContainer tempUserContainer = new UserContainer(template.Name);
                tempUserContainer.ListTemplatesAuthentication.Add(template);
                mDictUserContainers.Add(template.Name, tempUserContainer);
            }
        }

        public void AddTemplateHack(ModelTemplate template)
        {
            if (mDictUserContainers.ContainsKey(template.Name))
            {
                mDictUserContainers[template.Name].ListTemplatesHack.Add(template);
            }
            else
            {
                UserContainer tempUserContainer = new UserContainer(template.Name);
                tempUserContainer.ListTemplatesHack.Add(template);
                mDictUserContainers.Add(template.Name, tempUserContainer);
            }
        }
    }
}
