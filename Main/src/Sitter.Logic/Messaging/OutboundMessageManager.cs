using System;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Repository;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.Logic.Messaging
{
    public class OutboundMessageManager
    {
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly AppUserRepository _appUserRepo = new AppUserRepository();
        private readonly JobDal _jobDal = new JobDal();
        private readonly AppLogger _log = new AppLogger();
        private readonly ParentDal _parentDal = new ParentDal();
        private readonly ParentRepository _parentRepo = new ParentRepository();
        private readonly SitterDal _sitterDal = new SitterDal();
        private readonly SmsOutboundRepository _smsOutboundRepo = new SmsOutboundRepository();
        private readonly TxtMsgAwaitingResponseDal _txtMsgAwaitingResponseDal = new TxtMsgAwaitingResponseDal();

        public bool SendMessagesToOneOpenJob()
        {
            // STEP - Get An Open Job
            Job job = _jobDal.GetOneJobWithAnyPendingInvites();
            if (job == null)
            {
                return false;
            }
            Parent parent = _parentRepo.GetById(job.ParentId);

            // STEP - Send text to Sitters
            _jobDal.Update(job);

            foreach (JobInvite invite in job.JobInvites)
            {
                Sitter sitter = _sitterDal.GetById(invite.SitterId);
                if (sitter == null)
                {
                    _log.Warning(string.Format("sitter with ID {0} not found for jobId {1}", invite.SitterId, job.Id));
                    invite.State = InvitationState.InvalidSitterIdNotFound;
                    _jobDal.Update(job);
                    continue;
                }
                string msg = MessageTemplates.JobAvailable(parent.User.FullName(), sitter.User.ToLocalTime(job.Start), sitter.User.ToLocalTime(job.EndTime()), invite.RatePerHour);

                var txtMsgOutbound = new TxtMsgOutbound
                {
                    MobilePhone = sitter.User.MobilePhone,
                    ReceipientId = sitter.User.Id,
                    Message = msg,
                    //Url = MessageTemplates.FormatJobUrl(job.Id),
                    OutboundMessageType = OutboundMessageType.PostJob,
                    SenderId = job.ParentId,
                    JobId = job.Id
                };

                string outMsgId = _smsOutboundRepo.QueueSmsForSend(txtMsgOutbound);
                if (outMsgId != null)
                {
                    _txtMsgAwaitingResponseDal.Insert(new TxtMsgAwaitingResponse(txtMsgOutbound.Id, sitter.User.Id, sitter.User.MobilePhone, InboundMessageType.PostJobResponse, job.Id, parent.Id));
                }

                invite.InvitedDate = TimeUtil.GetCurrentUtcTime();
                invite.State = InvitationState.Invited;

                // STEP - Save JobInvite State
                _jobDal.Update(job);
            }

            return true;
        }

        public bool SendOneSignupInvite()
        {
            Parent parentWithInvite = _parentRepo.GetOneParentInviteSitterToSignup();
            if (parentWithInvite == null)
                return false;

            foreach (InviteToSignup invite in parentWithInvite.InviteToSignup)
            {
                if (invite.InviteStatus != InvitationState.InvitePending)
                    continue;

                var txtMsg = new TxtMsgOutbound
                {
                    MobilePhone = invite.MobilePhone,
                    Message = MessageTemplates.FormatParentInviteSitter(parentWithInvite.User.FullName()),
                    OutboundMessageType = OutboundMessageType.SignupInviteRequest,
                    SenderId = parentWithInvite.Id
                };

                // STEP - update status to sent (important, many messages will be sent without this line.
                invite.InviteStatus = InvitationState.Invited;
                _parentDal.Update(parentWithInvite);

                string outMsgId = _smsOutboundRepo.QueueSmsForSend(txtMsg);
                if (outMsgId != null)
                {
                    _txtMsgAwaitingResponseDal.Insert(new TxtMsgAwaitingResponse(txtMsg.Id, 0, txtMsg.MobilePhone, InboundMessageType.SignupInviteResponse, 0, parentWithInvite.Id));
                }
            }

            return true;
        }

        public string SendFeedbackToInboundMessage(TxtMsgInbound msgInbound, string feedback, int senderId)
        {
            var txtFeedback = new TxtMsgOutbound
            {
                MobilePhone = msgInbound.MobilePhone,
                Message = feedback,
                OutboundMessageType = OutboundMessageType.Feeback,
                SenderId = senderId,
                MessageDate =  TimeUtil.GetCurrentUtcTime()
            };

            return _smsOutboundRepo.QueueSmsForSend(txtFeedback);
        }

        public void SendFeedbackToInboundMessageAndDeleteAwaiting(TxtMsgInbound msgInbound, string feedback, int senderId, TxtMsgAwaitingResponse previousAwaitingMsgToDelete)
        {
            SendFeedbackToInboundMessage(msgInbound, feedback, senderId);
            _txtMsgAwaitingResponseDal.DeleteAwaiting(previousAwaitingMsgToDelete.Id);
        }

        public string SendFeedbackToInboundMessageAndAwait(TxtMsgInbound inboundMsg, string feedback, int senderId, InboundMessageType inboundMessageType, TxtMsgAwaitingResponse previousAwaitingMsg = null, string conversationMemory = null)
        {
            string outboundTxtMsgId = SendFeedbackToInboundMessage(inboundMsg, feedback, senderId);
            var t = new TxtMsgAwaitingResponse(outboundTxtMsgId, senderId, inboundMsg.MobilePhone, inboundMessageType,
                previousAwaitingMsg == null ? 0 : previousAwaitingMsg.JobId, previousAwaitingMsg == null ? 0 : previousAwaitingMsg.ParentId) {ConversationMemory = conversationMemory};

            _txtMsgAwaitingResponseDal.Insert(t);

            if (previousAwaitingMsg != null)
            {
                _txtMsgAwaitingResponseDal.DeleteAwaiting(previousAwaitingMsg.Id);
            }

            return outboundTxtMsgId;
        }

        public void SendNoticeOfSitterAccept(Job jobA)
        {
            AppUser parent = _appUserRepo.GetById(jobA.ParentId);
            if (jobA.AcceptedSitterId == null)
            {
                throw new Exception("Accepted sitter is null for SendParentNoticeOfSitterAccept()");
            }

            AppUser sitter = _appUserRepo.GetById(jobA.AcceptedSitterId.Value);

            // STEP - Notify parent
            var txtMsg = new TxtMsgOutbound
            {
                MobilePhone = parent.MobilePhone,
                Message = MessageTemplates.FormatSendParentNoticeOfSitterAccept(sitter.FullName()),
                OutboundMessageType = OutboundMessageType.ParentNotifySitterAccept,
                ReceipientId = parent.Id,
                SenderId = jobA.AcceptedSitterId.Value,
                JobId = jobA.Id
            };

            _smsOutboundRepo.QueueSmsForSend(txtMsg);

            // STEP - Notify other sitters
            foreach (JobInvite s in jobA.JobInvites)
            {
                if (s.SitterId == jobA.AcceptedSitterId)
                {
                    continue;
                }

                AppUser sitterOther = _appUserRepo.GetById(s.SitterId);
                var txtMsgSO = new TxtMsgOutbound
                {
                    MobilePhone = sitterOther.MobilePhone,
                    Message = MessageTemplates.FormatSendOtherSittersNoticeOfJobClose(parent.FullName(), sitterOther.ToLocalTime(jobA.Start)),
                    OutboundMessageType = OutboundMessageType.SitterNotifyJobClose,
                    ReceipientId = sitterOther.Id,
                    SenderId = jobA.ParentId,
                    JobId = jobA.Id
                };
                _smsOutboundRepo.QueueSmsForSend(txtMsgSO);

                //STEP - Stop waiting for response from other sitters
                TxtMsgAwaitingResponse awaiting = _txtMsgAwaitingResponseDal.GetByAwaitingUserMobile(sitterOther.MobilePhone);
                if (awaiting != null)
                    _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
            }
        }

        public void SendParentNoticeOfSitterDecline(Job job, int decliningSitterId, bool allSittersDeclined)
        {
            AppUser parent = _appUserRepo.GetById(job.ParentId);

            AppUser sitter = _appUserRepo.GetById(decliningSitterId);

            // STEP - Notify parent
            var txtMsg = new TxtMsgOutbound
            {
                MobilePhone = parent.MobilePhone,
                Message = MessageTemplates.FormatSendParentNoticeOfSitterDecline(sitter.FullName(), allSittersDeclined),
                OutboundMessageType = OutboundMessageType.ParentNotifySitterDecline,
                ReceipientId = parent.Id,
                SenderId = decliningSitterId,
                JobId = job.Id
            };

            _smsOutboundRepo.QueueSmsForSend(txtMsg);
        }

        public void SendParentNoticeOfSitterRenege(Job job, int renegeingSitterId)
        {
            AppUser parent = _appUserRepo.GetById(job.ParentId);

            DateTime date = parent.ToLocalTime(job.Start);

            AppUser sitter = _appUserRepo.GetById(renegeingSitterId);

            // STEP - Notify parent
            var txtMsg = new TxtMsgOutbound
            {
                MobilePhone = parent.MobilePhone,
                Message = MessageTemplates.FormatSendParentNoticeOfSitterRenege(sitter.FullName(), date),
                OutboundMessageType = OutboundMessageType.ParentNotifySitterRenege,
                ReceipientId = parent.Id,
                SenderId = renegeingSitterId,
                JobId = job.Id
            };

            _smsOutboundRepo.QueueSmsForSend(txtMsg);
        }

        public void SendPasswordToUserWhoForgotPassword(AppUser parent, string code)
        {
            var txtMsg = new TxtMsgOutbound
            {
                MobilePhone = parent.MobilePhone,
                Message = MessageTemplates.FormatSendPasswordToUserWhoForgotPassword(code),
                OutboundMessageType = OutboundMessageType.ForgotPassword,
                ReceipientId = parent.Id,
                SenderId = parent.Id
            };

            _smsOutboundRepo.QueueSmsForSend(txtMsg);
        }

        public bool SitterRequestPayment(SitterRequestPaymentSM requestPayment)
        {
            AppUser sitter = _appUserDal.GetById(requestPayment.SitterId);
            Job job = _jobDal.GetById(requestPayment.JobId);
            AppUser parent = _appUserDal.GetById(job.ParentId);

            var txtMsg = new TxtMsgOutbound
            {
                MobilePhone = parent.MobilePhone,
                Message = MessageTemplates.FormatSitterRequestPayment(sitter.FirstNameLastInitial(), job.StartDateFormatted()),
                OutboundMessageType = OutboundMessageType.SitterRequestPayment,
                ReceipientId = parent.Id,
                SenderId = sitter.Id,
                JobId = requestPayment.JobId
            };

            _smsOutboundRepo.QueueSmsForSend(txtMsg);

            return true;
        }
    }
}