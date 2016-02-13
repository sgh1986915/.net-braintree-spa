using MySitterHub.AppService.Startup;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;
using Twilio;

namespace MySitterHub.AppService.Sms
{
    /// <summary>
    /// Send Text Messages via Twilio
    /// </summary>
    public class TwilioSmsSender
    {
        private static int ThrottleMax = 1000;
        private static int ThrottleCount = 1;
        private readonly AppLogger _log = new AppLogger();
        private readonly TxtMsgOutboundDal _txtMsgOutboundDal = new TxtMsgOutboundDal();

        public string SendSMS(TxtMsgOutbound txtMsgOutbound)
        {

            if (!AppConfigSvcValues.Instance.SmsSimulationMode && OnWhiteList(txtMsgOutbound.MobilePhone))
            {
                ThrottleCount++;
                if (ThrottleCount >= ThrottleMax)
                {
                    _log.Warning("MaxThrottle count exceeded: " + ThrottleMax);
                }
                else
                {
                    string msg = string.Format("Sending SMS to {0}. message: '{1}'. ThrottleCount:{2}", txtMsgOutbound.MobilePhone, txtMsgOutbound.Message, ThrottleCount);
                    _log.Info(msg);

                    var twilio = new TwilioRestClient(AppConfigSvcValues.Instance.TwilioAccountSid, AppConfigSvcValues.Instance.TwilioAuthToken);
                    Message ret = twilio.SendMessage(AppConfigSvcValues.Instance.SourcePhone, txtMsgOutbound.MobilePhone, txtMsgOutbound.Message); //FutureDev: Send async
                    _log.Info("Sent SMS, status: " + ret.Status);

                    if (ret.Status != "queued")
                        _log.Info("Error. Send to Twilio not successful. Status:" + ret.Status + " destPhone:" + txtMsgOutbound.MobilePhone);
                }
            }
            else
            {
                string reason = AppConfigSvcValues.Instance.SmsSimulationMode ? "Simulation" : "not on whitelist";
                txtMsgOutbound.NotSendReason = reason;
                _log.Info("NOT Sending SMS to " + txtMsgOutbound.MobilePhone + " at " + txtMsgOutbound.MobilePhone + ". message: '" + txtMsgOutbound.Message + "' because " + reason);
            }

            _txtMsgOutboundDal.UpdateState(txtMsgOutbound, TxtMsgProcessState.Processed);
            return txtMsgOutbound.Id;
        }

        private bool OnWhiteList(string destPhone)
        {
            if (AppConfigSvcValues.Instance.WhiteList != WhiteListActive.On)
                return true;

            return MessagingConstants.Whitelist.Contains(destPhone);
        }
    }
}