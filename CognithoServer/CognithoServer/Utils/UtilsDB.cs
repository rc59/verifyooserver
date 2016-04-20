using CognithoServer.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsDB
    {
        public MongoDatabase GetDbInstance()
        {
            const string connectionString = "mongodb://localhost/?safe=true";
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();

            const string databaseName = "cognithoserver-dev";
            MongoDatabase db = mongoServer.GetDatabase(databaseName);

            return db;
        }

        public Template GetTemplateByUsername(string userName)
        {
            MongoCollection<Template> templates = GetCollectionTemplates();

            IMongoQuery query = Query<Template>.EQ(c => c.Name, userName);
            var filter = Builders<Template>.Filter.Eq(Consts.FIELD_NAME, userName);

            Template template = templates.FindOne(query);

            return template;
        }

        public List<Template> GetStoredTemplatesByUsername(string userName)
        {
            MongoCollection<Template> templates = GetCollectionTemplates();

            IMongoQuery query = Query<Template>.EQ(c => c.Name, userName);
            var filter = Builders<Template>.Filter.Eq(Consts.FIELD_NAME, userName);

            MongoCursor<Template> mongoCursorTemplates = templates.Find(query);

            List<Template> listTemplates = new List<Template>();

            foreach(Template tempTemplate in mongoCursorTemplates)
            {
                listTemplates.Add(tempTemplate);
            }

            return listTemplates;
        }


        public Application GetAppById(string appId)
        {
            MongoCollection<Application> applications = GetCollectionApplications();

            IMongoQuery query = Query<Application>.EQ(c => c.AppId, appId);
            var filter = Builders<Application>.Filter.Eq(Consts.FIELD_AUTH_APP_ID, appId);

            Application app = applications.FindOne(query);

            return app;
        }
        
        public AuthToken GetAuthTokenById(string authTokenId)
        {
            MongoCollection<AuthToken> authTokens = GetCollectionAuthTokens();

            IMongoQuery query = Query<AuthToken>.EQ(c => c.AuthTokenId, authTokenId);
            var filter = Builders<AuthToken>.Filter.Eq(Consts.FIELD_AUTH_TOKEN_ID, authTokenId);

            AuthToken authToken = authTokens.FindOne(query);

            return authToken;
        }

        public MongoCollection<Template> GetCollectionTemplates()
        {
            MongoDatabase dataBase = GetDbInstance();
            MongoCollection<Template> templates = dataBase.GetCollection<Template>("templates");

            return templates;
        }
        
        public MongoCollection<InstructionDistributions> GetCollectionDistributions()
        {
            MongoDatabase dataBase = GetDbInstance();
            MongoCollection<InstructionDistributions> dists = dataBase.GetCollection<InstructionDistributions>("distributions");

            return dists;
        }

        public InstructionDistributions GetDistributionsByInstructionIdx(int instructionIdx)
        {
            MongoCollection<InstructionDistributions> dists = GetCollectionDistributions();

            IMongoQuery query = Query<InstructionDistributions>.EQ(c => c.InstructionIdx, instructionIdx);
            var filter = Builders<InstructionDistributions>.Filter.Eq(Consts.FIELD_INSTRUCTION_IDX, instructionIdx);

            InstructionDistributions instructionDists = dists.FindOne(query);

            return instructionDists;
        }

        public MongoCollection<Instruction> GetCollectionInstructions()
        {
            MongoDatabase dataBase = GetDbInstance();
            MongoCollection<Instruction> instructions = dataBase.GetCollection<Instruction>("instructions");

            return instructions;
        }
    
        public MongoCollection<Application> GetCollectionApplications()
        {
            MongoDatabase dataBase = GetDbInstance();
            MongoCollection<Application> applications = dataBase.GetCollection<Application>("applications");

            return applications;
        }

        public MongoCollection<AuthToken> GetCollectionAuthTokens()
        {
            MongoDatabase dataBase = GetDbInstance();
            MongoCollection<AuthToken> authTokens = dataBase.GetCollection<AuthToken>("authtokens");

            return authTokens;
        }


        public List<T> ConvertCursorToList<T>(MongoCursor<T> collection)
        {
            List<T> tempList = new List<T>();
            foreach (T singleObj in collection)
            {
                tempList.Add(singleObj);
            }

            return tempList;
        }

        public List<T> ConvertCollectionToList<T>(MongoCollection<T> collection)
        {
            List<T> tempList = new List<T>();
            foreach (T singleObj in collection.FindAll())
            {
                tempList.Add(singleObj);
            }

            return tempList;
        }
    }
}