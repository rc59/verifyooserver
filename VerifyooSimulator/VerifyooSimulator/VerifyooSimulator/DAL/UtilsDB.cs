﻿using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyooSimulator.Models;

namespace VerifyooSimulator.DAL
{
    public class UtilsDB
    {
        public static MongoDatabase GetDbInstance()
        {
            //const string connectionString = "mongodb://52.29.210.15/?safe=true";
            const string connectionString = "mongodb://localhost/?safe=true";
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();

            const string databaseName = "extserver-dev";
            MongoDatabase db = mongoServer.GetDatabase(databaseName);

            return db;
        }

        public static MongoCollection<ModelTemplate> GetTemplates()
        {
            MongoDatabase dataBase = GetDbInstance();            
            MongoCollection<ModelTemplate> templates = dataBase.GetCollection<ModelTemplate>("templates");

            return templates;
        }
    }
}
