using System.Collections.Specialized;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;
using RestSharp.Contrib;

namespace MySitterHub.Logic.Repository
{
    public class TwilioRepository
    {
        private readonly AppLogger _log = new AppLogger();
        private readonly TxtMsgInboundDal _txtMsgInboundDal = new TxtMsgInboundDal();

        public bool PersistIncommingTwillioMessage(string twilioMessageUrl)
        {
            NameValueCollection vals = HttpUtility.ParseQueryString(twilioMessageUrl);

            var twilioMessage = new TwilioMessageSM
            {
                From = vals["From"],
                Body = vals["body"],
            };

            if (twilioMessage.Body == null)
            {
                _log.Warning("twilioMessage body is null, unable to process.");
                return false;
            }
            if (twilioMessage.From == null)
            {
                _log.Warning("twilioMessage From is null, unable to process.");
                return false;
            }

            var txt = new TxtMsgInbound
            {
                MobilePhone = twilioMessage.From,
                Message = twilioMessage.Body,
                State = TxtMsgProcessState.New,
            };
            _txtMsgInboundDal.Insert(txt);

            return true;
        }

        public class TwilioMessageSM
        {
            public string From { get; set; }
            public string Body { get; set; }
        }
    }
}