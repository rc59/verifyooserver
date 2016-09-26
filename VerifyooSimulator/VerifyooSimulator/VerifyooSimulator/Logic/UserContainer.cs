using Data.UserProfile.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyooSimulator.Models;

namespace VerifyooSimulator.Logic
{
    public class UserContainer
    {
        public string Name { get; set; }

        public ModelTemplate TemplateRegistration;
        public List<ModelTemplate> ListTemplatesAuthentication;
        public List<ModelTemplate> ListTemplatesHack;

        public UserContainer(string name) { 
            Name = name.ToLower();

            ListTemplatesAuthentication = new List<ModelTemplate>();
            ListTemplatesHack = new List<ModelTemplate>();
        }
    }
}
