using System;
using System.Threading;
using MySitterHub.AppService.Sms;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Messaging;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.AppService.Startup
{
    public class MessageWorker
    {
        private const int ErrorThreshod = 1000;
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly InboundMessageManager _imm = new InboundMessageManager();

        private readonly AppLogger _log = new AppLogger();
        private readonly OutboundMessageManager _omm = new OutboundMessageManager();
        private readonly TwilioSmsSender _twilioSmsSender = new TwilioSmsSender();

        private readonly TxtMsgAwaitingResponseDal _txtMsgAwaitingResponseDal = new TxtMsgAwaitingResponseDal();
        private readonly TxtMsgInboundDal _txtMsgInboundDal = new TxtMsgInboundDal();
        private readonly TxtMsgOutboundDal _txtMsgOutboundDal = new TxtMsgOutboundDal();
        private int Delay = 100;
        private int ErrorCount;
        private DateTime LastTime;

        public void DoWork()
        {
            _log.Info("Watching MongoDB collections: 'txtmsgoutbound' for SMS messages to send and 'txtmsginbound' for SMS messages to process");
            while (true)
            {
                try
                {
                    LastTime = DateTime.UtcNow;

                    SendOutboundMessages();

                    ProcessNextOutboundTxtMessage();

                    ProcessNextInboundTxtMessage();

                    double elapsed = (DateTime.UtcNow - LastTime).TotalMilliseconds;
                    if (elapsed < Delay)
                    {
                        Thread.Sleep(Delay);
                    }
                    else
                    {
                       _log.Info("No delay, elapsed: " + elapsed);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(null, "MessageWorker.DoWork unhandled error ", ex);
                    ErrorCount++;
                    if (ErrorCount > ErrorThreshod)
                        return;
                }
            }
        }

        public void SendOutboundMessages()
        {
            _omm.SendMessagesToOneOpenJob();
            _omm.SendOneSignupInvite();
        }

        public void ProcessNextInboundTxtMessage()
        {
            TxtMsgInbound msgInbound = _txtMsgInboundDal.GetOneNewInbound();
            if (msgInbound == null)
                return;
            TxtMsgAwaitingResponse awaiting = null;
            try
            {
                // STEP - Update state to Processing
                _txtMsgInboundDal.UpdateState(msgInbound.Id, TxtMsgProcessState.Processing);

                // STEP - Check for TxtMsgAwaitingResponse
                awaiting = _txtMsgAwaitingResponseDal.GetByAwaitingUserMobile(msgInbound.MobilePhone);

                // STEP - ToLower,Trim, and Log
                msgInbound.Message = msgInbound.Message == null ? "" : msgInbound.Message.ToLower().Trim();
                _log.Info("procesing inbound message from " + msgInbound.MobilePhone + " msg:" + msgInbound.Message);

                // STEP - Send not awaiting
                if (awaiting == null)
                {
                    new NewCommandManager().ProcessNewConverastion(msgInbound);
                    return;
                }

                // STEP - Process message
                msgInbound.InboundMessageType = awaiting.AwaitingResponseType;
                AppUser user = _appUserDal.GetByMobile(msgInbound.MobilePhone);
                if (user == null)
                {
                    throw new Exception(string.Format("Awaiting message from mobile '{0}' but not user exist for this mobile.", msgInbound.MobilePhone));
                }
                switch (awaiting.AwaitingResponseType)
                {
                    case InboundMessageType.PostJobResponse:
                        _imm.ProcessSitterResonseToJobPosting(msgInbound, awaiting, user);
                        break;
                    case InboundMessageType.SignupInviteResponse:
                        _imm.ProcessResponseToSignupInvite(msgInbound, awaiting, user);
                        break;
                    case InboundMessageType.SignupInvite:
                        _imm.ProcessSignupInviteConversation(msgInbound, awaiting, user);
                        break;
                    case InboundMessageType.SelfSignup_Step1_ParentOrSitter:
                        _imm.ProcessSelfSignupConversation_Step1(msgInbound, awaiting, user);
                        break;
                    case InboundMessageType.SelfSignup_Step2_Name:
                        _imm.ProcessSelfSignupConversation_Step2(msgInbound, awaiting, user);
                        break;
                    case InboundMessageType.PostJob_Step2_DayOfWeek:
                    case InboundMessageType.PostJob_Step3_StartTime:
                    case InboundMessageType.PostJob_Step4_SelectSitters:
                    case InboundMessageType.PostJob_Step5_Confirm:
                        _imm.ProcessPostJobConversation(msgInbound, awaiting, user);
                        break;
                    default:
                        throw new Exception("inboundMessageType not recognized. " + awaiting.AwaitingResponseType);
                }
            }
            catch (Exception ex)
            {
                string errMsg = "Unhandled exception in ProcessNextInboundTxtMessage: ";

                _txtMsgInboundDal.UpdateState(msgInbound.Id, TxtMsgProcessState.Error, errMsg + ex);
                if (awaiting != null)
                {
                    string err = "Sorry, mysitterhub.com experienced an error while processing your message";
                    _omm.SendFeedbackToInboundMessage(msgInbound, err, AppUser.AppServiceUserId);
                    _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
                    _log.Error(msgInbound.MobilePhone, err, ex);
                }
            }
        }

        public void ProcessNextOutboundTxtMessage()
        {
            TxtMsgOutbound txtMsgOutbound = _txtMsgOutboundDal.GetOneNewOutbound();
            if (txtMsgOutbound == null)
                return;

            // STEP - Update state to Processing
            _txtMsgOutboundDal.UpdateState(txtMsgOutbound, TxtMsgProcessState.Processing);

            _twilioSmsSender.SendSMS(txtMsgOutbound);
        }
    }
}