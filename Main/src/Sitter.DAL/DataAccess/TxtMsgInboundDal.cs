using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MySitterHub.DAL.General;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.DAL.DataAccess
{
    public class TxtMsgInboundDal
    {
        #region Insert

        public void Insert(TxtMsgInbound msg)
        {
            msg.Id = ObjectId.GenerateNewId().ToString();
            MongoCnn.GetTxtMsgInboundCollection().Insert(msg);
        }

        #endregion

        #region Get

        public List<TxtMsgInbound> GetAll()
        {
            return MongoCnn.GetTxtMsgInboundCollection().FindAll().ToList();
        }

        public TxtMsgInbound GetById(string id)
        {
            return MongoCnn.GetTxtMsgInboundCollection().AsQueryable().FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Get One New Inbound Message. Process oldest first (FIFO).
        /// </summary>
        /// <returns></returns>
        public TxtMsgInbound GetOneNewInbound()
        {
            return MongoCnn.GetTxtMsgInboundCollection().AsQueryable().OrderBy(m=> m.Id).FirstOrDefault(m => m.State == TxtMsgProcessState.New); 
        }

#if OFF
        /// <summary>
        ///  Get job reply for job invitation.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static TxtMsgInbound GetJobReplies(int jobId, int userId)
        {
            return MongoCnn.GetTxtMsgInboundCollection().AsQueryable().FirstOrDefault(m => m.State == TxtMsgProcessState.Processed
                && m.InboundMessageType == InboundMessageType.PostJobResponse
                && m.UserId == userId
                && m.JobId == jobId);
        }
#endif
        #endregion

        #region Update

        public void UpdateState(string id, TxtMsgProcessState state, string error = null)
        {
            TxtMsgInbound msg = GetById(id);
            msg.State = state;
            msg.ProessedDate = TimeUtil.GetCurrentUtcTime();
            msg.ProcessError = error;
            MongoCnn.GetTxtMsgInboundCollection().Save(msg);
        }

        #endregion
    }
}