using System.Collections.Generic;
using System.Linq;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Model.Core;

namespace MySitterHub.Logic.Repository
{
    public class SmsSimulatorRepository
    {
        private readonly TxtMsgInboundDal _txtMsgInboundDal = new TxtMsgInboundDal();
        private readonly TxtMsgOutboundDal _txtMsgOutboundDal = new TxtMsgOutboundDal();

        public IEnumerable<TxtMsg> GetAll()
        {
            return _txtMsgInboundDal.GetAll();
        }

        /// <summary>
        ///     Most recent 100 messages
        /// </summary>
        public List<TxtMsg> GetAllInboundAndOutbound()
        {
            var _all = new List<TxtMsg>();
            _all.AddRange(_txtMsgInboundDal.GetAll());
            _all.AddRange(_txtMsgOutboundDal.GetAll());
            _all = _all.OrderByDescending(m => m.MessageDate).Take(100).ToList();
            return _all;
        }

        public void InsertInbound(TxtMsgInbound msg)
        {
            //TODO: validation

            _txtMsgInboundDal.Insert(msg);
        }
    }
}