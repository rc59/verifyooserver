using JsonConverter.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooConverter.Logic
{
    class UtilsDB
    {
        public static MongoDatabase GetDbInstance()
        {
            //const string connectionString = "mongodb://52.26.178.48/?safe=true";
            const string connectionString = "mongodb://localhost/?safe=true";
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();

            const string databaseName = "extserver-dev";
            MongoDatabase db = mongoServer.GetDatabase(databaseName);

            return db;
        }

        public static MongoCollection<ModelTemplate> GetCollShapes()
        {
            MongoDatabase dataBase = GetDbInstance();
            MongoCollection<ModelTemplate> templates = dataBase.GetCollection<ModelTemplate>("templates");

            return templates;
        }
    }
}
