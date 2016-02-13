using MySitterHub.DAL.DataAccess;
using MySitterHub.Model.Core;

namespace MySitterHub.Logic.Repository
{
    public class SmsOutboundRepository
    {
        public string QueueSmsForSend(TxtMsgOutbound txtMsgOutbound)
        {
            txtMsgOutbound.State = TxtMsgProcessState.New;
            new TxtMsgOutboundDal().Insert(txtMsgOutbound);
            return txtMsgOutbound.Id;
        }
    }
}