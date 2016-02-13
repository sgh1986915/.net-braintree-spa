using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MySitterHub.DAL.General;
using MySitterHub.Model.Core;

namespace MySitterHub.DAL.DataAccess
{
    public class TxtMsgAwaitingResponseDal
    {
        #region Insert

        public void Insert(TxtMsgAwaitingResponse msg)
        {
            // Note that TxtMsgAwaitingResponse.Id is the same as TxtMsg
            if (string.IsNullOrEmpty(msg.Id))
                throw new ArgumentException("Id is null");
            MongoCnn.GetTxtMsgAwaitingResponseCollection().Insert(msg);
        }

        #endregion

        #region Get

        public List<TxtMsgAwaitingResponse> GetAll()
        {
            return MongoCnn.GetTxtMsgAwaitingResponseCollection().FindAll().OrderByDescending(m=> m.Id).ToList();
        }

        //public TxtMsgAwaitingResponse GetById(string id)
        //{
        //    return MongoCnn.GetTxtMsgAwaitingResponseCollection().AsQueryable().FirstOrDefault(m => m.Id == id);
        //}

        public TxtMsgAwaitingResponse GetByAwaitingUserMobile(string mobile)
        {
            return MongoCnn.GetTxtMsgAwaitingResponseCollection().AsQueryable().Where(m => m.WaitingForUserMobile == mobile).OrderByDescending(m => m.Id).FirstOrDefault();
        }

        #endregion

        #region Delete

        public void DeleteAwaiting(string id)
        {
            IMongoQuery query = Query<TxtMsgAwaitingResponse>.EQ(m => m.Id, id);
            MongoCnn.GetTxtMsgAwaitingResponseCollection().Remove(query);
        }

        #endregion
    }
}