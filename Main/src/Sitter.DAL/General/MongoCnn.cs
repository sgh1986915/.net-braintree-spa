using MySitterHub.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.DAL.General
{
    public struct DbCollection
    {
        public const string user = "appuser";
        public const string userpass = "appuserpass";
        public const string parent = "parent";
        public const string parentsitter = "parentsitter";
        public const string sitter = "sitter";
        public const string job = "job";
        public const string counter = "counter";
        public const string txtmsgoutbound = "txtmsgoutbound";
        public const string txtmsginbound = "txtmsginbound";

        public const string txtmsgawaitingresponse = "txtmsgawaitingresponse";
        public const string changePasswordRequest = "changePasswordRequest";

    }

    public class MongoCnn
    {
        private const string cnn = "mongodb://localhost";
        private static string _mongoDbName = "mysitterhub";

        /// <summary>
        ///     Registery Types for Mongo Serialization
        /// </summary>
        static MongoCnn()
        {
            MongoBsonClassMapper.Map();
        }

        public static MongoDatabase GetDbConnection()
        {
            const string connectionString = cnn;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            return server.GetDatabase(_mongoDbName);
        }

        public static void SetDbName(string dbName)
        {
            _mongoDbName = dbName;
        }

        #region Connection<T>

        public static MongoCollection<AppUser> GetAppUserCollection()
        {
            MongoDatabase database = GetDbConnection();
            return database.GetCollection<AppUser>(DbCollection.user);
        }

        public static MongoCollection<AppUserPass> GetUserPassCollection()
        {
            MongoDatabase database = GetDbConnection();
            return database.GetCollection<AppUserPass>(DbCollection.userpass);
        }

        public static MongoCollection<Sitter> GetSitterCollection()
        {
            MongoDatabase database = GetDbConnection();
            return database.GetCollection<Sitter>(DbCollection.sitter);
        }

        public static MongoCollection<Parent> GetParentCollection()
        {
            MongoDatabase database = GetDbConnection();
            return database.GetCollection<Parent>(DbCollection.parent);
        }

        public static MongoCollection<Job> GetJobCollection()
        {
            MongoDatabase database = GetDbConnection();
            return database.GetCollection<Job>(DbCollection.job);
        }

        public static MongoCollection<TxtMsgOutbound> GetTxtMsgOutboundCollection()
        {
            return GetDbConnection().GetCollection<TxtMsgOutbound>(DbCollection.txtmsgoutbound);
        }

        public static MongoCollection<TxtMsgInbound> GetTxtMsgInboundCollection()
        {
            return GetDbConnection().GetCollection<TxtMsgInbound>(DbCollection.txtmsginbound);
        }


        public static MongoCollection<TxtMsgAwaitingResponse> GetTxtMsgAwaitingResponseCollection()
        {
            return GetDbConnection().GetCollection<TxtMsgAwaitingResponse>(DbCollection.txtmsgawaitingresponse);
        }

        public static MongoCollection<ChangePasswordRequest> GetChangePasswordRequestCollection()
        {
            return GetDbConnection().GetCollection<ChangePasswordRequest>(DbCollection.changePasswordRequest);
        }

        #endregion

        #region Utility

        public static long CheckCommandSuccess(CommandResult result)
        {
            if (!result.Ok)
                throw new Exception("Database write result not OK");
            return result.Response["n"].AsInt64;
        }

        public static int CountRecordsAffected(CommandResult result)
        {
            const string nIndexesWas = "nIndexesWas";
            if (result.Response.ToHashtable().Contains(nIndexesWas))
                return result.Response[nIndexesWas].AsInt32;
            return 0;
        }

        public static long CheckBatchCommandSuccess(IEnumerable<WriteConcernResult> results)
        {
            long c = 0;
            foreach (WriteConcernResult r in results)
            {
                if (!r.Ok)
                    throw new Exception("Database write result not OK");
                c += r.Response["n"].AsInt64;
            }
            return c;
        }

        #endregion
    }
}