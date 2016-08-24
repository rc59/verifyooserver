using JsonConverter.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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
            MongoCollection<ModelTemplate> templates = dataBase.GetCollection<ModelTemplate>("templates_nexus");

            //IMongoQuery query = Query<ModelTemplate>.EQ(c => c.Name, "  brachak@menora.co.il");

            //List<string> listRemove = new List<string>();
            //listRemove.Add("  brachak@menora.co.il");
            //listRemove.Add("  brachak@menora.co.il");
            //listRemove.Add("  elnurit@gmail.com");
            //listRemove.Add("  hanygr1@gmail.com");
            //listRemove.Add("  reuvenoffice@gmail.com");
            //listRemove.Add(" rommymarzuk@gmail.com");
            //listRemove.Add(" sharon@shdema.co.il");
            //listRemove.Add(" tami_kaplan@walla.com");
            //listRemove.Add(" wag.michael@gmail.com");
            //listRemove.Add("abuchnick@yahoo.com");
            //listRemove.Add("aesil@netvision.net.il");
            //listRemove.Add("airubin22@gmail.com");
            //listRemove.Add("almoggorno@gmail.com");
            //listRemove.Add("Amit@dreamcatcher.co.il");
            //listRemove.Add("amitayir@zahav.net");
            //listRemove.Add("asaf.kleinbort@gmail.com");
            //listRemove.Add("aviayako@gmail.com");
            //listRemove.Add("avifayans@gmail.com");
            //listRemove.Add("avishygilad@walla.co.il");
            //listRemove.Add("avitaloren@gmail.com");
            //listRemove.Add("axol69@gmail.com");
            //listRemove.Add("ayalagi@netvision.net.il");
            //listRemove.Add("berger.tali@gmail.com");
            //listRemove.Add("Blah@blah.com");
            //listRemove.Add("Bohadana@viboapp.com");
            //listRemove.Add("chavi@ariel.ac.il");
            //listRemove.Add("chenc@billbeez.com");
            //listRemove.Add("danon.rami@gmail.com");
            //listRemove.Add("david.avikasis@gmail.com");
            //listRemove.Add("davidsk@walla.com");
            //listRemove.Add("dianoren@gmail.com");
            //listRemove.Add("dkvdganit@gmail.com");
            //listRemove.Add("dordali@gmail.com");
            //listRemove.Add("dorishasha36@gmail.com");
            //listRemove.Add("douglasa@trdf.technion.ac.il");
            //listRemove.Add("drorhe@ariel.ac.il");
            //listRemove.Add("dshtul@gmail.com");
            //listRemove.Add("eli7k@walla");
            //listRemove.Add("eviatar@gmail.com");
            //listRemove.Add("eviataryosef@gmail.com");
            //listRemove.Add("Fff@hhg.com");
            //listRemove.Add("gabrielm@shapir.co.il");
            //listRemove.Add("gil199787@gmail.com");
            //listRemove.Add("gilisofer@gmail.com");
            //listRemove.Add("glag@walla.com");
            //listRemove.Add("glikin.anna@gmail.com");
            //listRemove.Add("goldb.adam@gmail.com");
            //listRemove.Add("gott2101@gmail.com");
            //listRemove.Add("H0lyd4wg@gmail.com");
            //listRemove.Add("hagitandziv@gmail.com");
            //listRemove.Add("hanani34@gmail.com");
            //listRemove.Add("hmshua@zahav.net.il");
            //listRemove.Add("igal@gmail.com");
            //listRemove.Add("inna.dimer@gmail.com");
            //listRemove.Add("Irisne@walla.co.il");
            //listRemove.Add("iriteng@gmail.com");
            //listRemove.Add("itzchakr@gmail.com");
            //listRemove.Add("jsbelo@gmail.com");
            //listRemove.Add("junk.mail.stam.mail@gmail.com");
            //listRemove.Add("Kamelot007@gmail.com ");
            //listRemove.Add("kawaii89@gmail.com");
            //listRemove.Add("Kfirtal31@gmail.com ");
            //listRemove.Add("kfridel@hotmail.com");
            //listRemove.Add("kkeevviinn.kp@gmail.com");
            //listRemove.Add("kobysheftel@gmail.com");
            //listRemove.Add("lauren7boublil@gmail.com");
            //listRemove.Add("levfadv@gmail.com");
            //listRemove.Add("lijhrd@gmail.com");
            //listRemove.Add("limorafek0@gmail.com");
            //listRemove.Add("lior121987@gmail.com");
            //listRemove.Add("liord@gmail.com");
            //listRemove.Add("lipkinmaya@gmail.com");
            //listRemove.Add("lll@gmail.com");
            //listRemove.Add("marceloschiffer58@gmail.com");
            //listRemove.Add("marissa.shua@gmail.com");
            //listRemove.Add("meravg@trdf.technion.ac.il");
            //listRemove.Add("michael.hassoun@gmail.com");
            //listRemove.Add("Michalein3@gmail.com");            
            //listRemove.Add("Nadav@inst-ore.com");
            //listRemove.Add("nadavoren1@gmail.com");
            //listRemove.Add("nadavz@billbeez.com");
            //listRemove.Add("name@name.com");
            //listRemove.Add("naukakyiv@gmail.com");
            //listRemove.Add("niceuser@walla.com");
            //listRemove.Add("nirmargalit1987@gmail.com");
            //listRemove.Add("nitzan@effectivy.net");
            //listRemove.Add("noachy@kanat.co.il");
            //listRemove.Add("nuritala10@gmail.com");
            //listRemove.Add("o_bar_on@netvision.net.il");
            //listRemove.Add("ofir.chen83@gmail.com");
            //listRemove.Add("Ohad@miniapp.me");
            //listRemove.Add("omer.wag@gmail.com");
            //listRemove.Add("peerafeldman@gmail.com");
            //listRemove.Add("qeliran@gmail.com");
            //listRemove.Add("rachel1n@yahoo.com.sg");
            //listRemove.Add("rafi_VS_roy1");
            //listRemove.Add("Rafich1959@gmail.com");
            //listRemove.Add("ratzabi_a@mail.tel-aviv.gov.il");
            //listRemove.Add("ravit1619@walla.co.il");
            //listRemove.Add("relbr87@gmail.com");
            //listRemove.Add("ronlockri.gmail.com");
            //listRemove.Add("roy-STAM123");
            //listRemove.Add("sarina1985@gmail.com");
            //listRemove.Add("sgdgadha@wdd.com");
            //listRemove.Add("shaul73@gmail.com");
            //listRemove.Add("shukap01@gmail.com");
            //listRemove.Add("Sivanwieser10@gmail.com");
            //listRemove.Add("STAM-atzmona.raiskin@gmail.com");
            //listRemove.Add("talgini7@gmail.com");
            //listRemove.Add("tamirmalihi156@gmail.com");
            //listRemove.Add("Timestamp-STAM@Gmail.Com");
            //listRemove.Add("TomerReal290216");
            //listRemove.Add("treinman@gmail.com");
            //listRemove.Add("yaarakissos@gmail. com");
            //listRemove.Add("yaelmor58@gmail.com");
            //listRemove.Add("yaronbnb@gmail.com");
            //listRemove.Add("yonitbiu@gmail.com");
            //listRemove.Add("zmirabt@walla.com");
            //listRemove.Add("zvi@tamary-lad.co.il");


            //for (int idx = 0; idx < listRemove.Count; idx++)
            //{
            //    query = Query<ModelTemplate>.EQ(c => c.Name, listRemove[idx]);
            //    templates.Remove(query);
            //}
            
            return templates;
        }
    }
}
