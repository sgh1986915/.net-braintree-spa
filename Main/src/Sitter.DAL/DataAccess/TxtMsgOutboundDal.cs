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
    public class TxtMsgOutboundDal
    {
        #region Insert

        public void Insert(TxtMsgOutbound msg)
        {
            msg.Id = ObjectId.GenerateNewId().ToString();
            MongoCnn.GetTxtMsgOutboundCollection().Insert(msg);
        }

        #endregion

        #region Get

        /// <summary>
        /// Get One New Outbound Message. Process oldest first (FIFO).
        /// </summary>
        public TxtMsgOutbound GetOneNewOutbound()
        {
            return MongoCnn.GetTxtMsgOutboundCollection().AsQueryable().OrderBy(m => m.Id).FirstOrDefault(m => m.State == TxtMsgProcessState.New);
        }


        public List<TxtMsgOutbound> GetAll()
        {
            return MongoCnn.GetTxtMsgOutboundCollection().FindAll().ToList();
        }

        public TxtMsgOutbound GetById(string id)
        {
            return MongoCnn.GetTxtMsgOutboundCollection().AsQueryable().FirstOrDefault(m => m.Id == id);
        }

        #endregion

        #region Update

        public void UpdateState(TxtMsgOutbound txtMsgOutbound, TxtMsgProcessState state)
        {
            txtMsgOutbound.State = state;
            txtMsgOutbound.ProessedDate = TimeUtil.GetCurrentUtcTime();
            MongoCnn.GetTxtMsgOutboundCollection().Save(txtMsgOutbound);
        }

        #endregion

    }
}