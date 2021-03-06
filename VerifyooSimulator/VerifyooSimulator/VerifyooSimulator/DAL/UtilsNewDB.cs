﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VerifyooSimulator.Models;

namespace VerifyooSimulator.DAL
{
    class UtilsNewDB
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
        public static MongoDatabase GetDbInstanceCloud()
        {
            const string connectionString = "mongodb://52.29.210.15/?safe=true";            
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();

            const string databaseName = "extserver-dev";
            MongoDatabase db = mongoServer.GetDatabase(databaseName);

            return db;
        }

        public static MongoCollection<ModelTemplate> GetTemplates()
        {
            MongoDatabase dataBase = GetDbInstance();
            MongoCollection<ModelTemplate> templates = dataBase.GetCollection<ModelTemplate>("templatedemos");

            return templates;
        }

        public static MongoCollection<ModelTemplate> GetTemplatesCloud()
        {
            MongoDatabase dataBase = GetDbInstanceCloud();
            MongoCollection<ModelTemplate> templates = dataBase.GetCollection<ModelTemplate>("templatedemos");

            return templates;
        }        
    }
}
