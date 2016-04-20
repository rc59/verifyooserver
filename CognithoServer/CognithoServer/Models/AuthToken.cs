using CognithoServer.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class AuthToken
    {
        public ObjectId _id { get; set; }
        public string TokenType { get; set; }
        public string UserName { get; set; }
        public BsonDateTime Created { get; set; }
        public string AuthTokenId { get; set; }
        public string AuthTokenKey { get; set; }

        public AuthToken()
        {
        }

        public AuthToken(TokenType tokenType, string userName)
        {
            UserName = userName;
            TokenType = tokenType.ToString();
            Created = DateTime.Now;
            AuthTokenId = Guid.NewGuid().ToString();
            AuthTokenKey = Guid.NewGuid().ToString();
        }
    }
}