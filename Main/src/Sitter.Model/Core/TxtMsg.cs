using System;
using Amazon.IdentityManagement.Model;
using MySitterHub.Model.Misc;

namespace MySitterHub.Model.Core
{
    public class TxtMsg
    {
        public TxtMsg()
        {
            MessageDate = TimeUtil.GetCurrentUtcTime(); //FutureDev: perhaps remove this from the constructor. create a factory instead.
        }

        public string Id { get; set; }
        public string MobilePhone { get; set; }
        public string Message { get; set; }
        //public string Url { get; set; } //not sure exactly how this is used yet.

        /// <summary>
        ///     UTC Date Message was recieved or sent respectively
        /// </summary>
        public DateTime MessageDate { get; set; }

        public int JobId { get; set; }

        public DateTime ProessedDate { get; set; }
        public string ProcessError { get; set; }
        public TxtMsgProcessState State { get; set; }

    }

    public class TxtMsgInbound : TxtMsg
    {
        public InboundMessageType InboundMessageType { get; set; }

        public static TxtMsgInbound Create() //FutureDev: perhaps remove this from the constructor. create a factory instead.
        {
            return new TxtMsgInbound();
        }
    }

    public class TxtMsgOutbound : TxtMsg
    {
        public OutboundMessageType OutboundMessageType { get; set; }

        public int ReceipientId { get; set; }
        public int SenderId { get; set; }
        public string SenderMobilePhone { get; set; }
        public string NotSendReason { get; set; }
    }

    public enum InboundMessageType
    {
        Unknown,
        SelfSignup_Step1_ParentOrSitter,
        SelfSignup_Step2_Name,

        SignupInvite, // Parent invites sitter to signup
        SignupInviteResponse, // Sitter responds to parent invite
        PostJobResponse,
        PostJob_Step1,
        PostJob_Step2_DayOfWeek,
        PostJob_Step3_StartTime,
        PostJob_Step4_SelectSitters,
        PostJob_Step5_Confirm,
    }

    public enum OutboundMessageType
    {
        Unknown,
        ForgotPassword,
        SignupInviteRequest,
        Feeback,
        PostJob,
        ParentNotifySitterAccept,
        ParentNotifySitterDecline,
        ParentNotifySitterRenege,
        SitterNotifyJobClose,
        SitterRequestPayment,
        NotAwaitingMessages
    }

    public enum TxtMsgProcessState
    {
        New,
        Processing,
        Error,
        Processed
    }

    public class TxtMsgAwaitingResponse
    {
        /// <summary>
        /// Used by MongoDb deserialization
        /// </summary>
        public TxtMsgAwaitingResponse()
        {            
        }

        public TxtMsgAwaitingResponse(string id, int waitingForUserId, string waitingForUserMobile,InboundMessageType inboundMessageType, int jobId = 0, int parentId =0)
        {
            Id = id;
            WaitingForUserId = waitingForUserId;
            WaitingForUserMobile = waitingForUserMobile;
            AwaitingResponseType = inboundMessageType;
            JobId = jobId;
            ParentId = parentId;
        }

        public string Id { get; set; }

        /// <summary>
        /// JobId if AwaitingResponseType = JobInvite, otherwise 0
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// ParentId if AwaitingResponseType = InboundMessageType.Invite
        /// </summary>
        public int ParentId { get; set; }
        public InboundMessageType AwaitingResponseType { get; set; }
        public int WaitingForUserId { get; set; }
        public string WaitingForUserMobile { get; set; }

        /// <summary>
        /// Store state about the conversation, previous answers to questions, for example.
        /// </summary>
        public string ConversationMemory { get; set; }
    }

    public class Conversation
    {
        /// <summary>
        /// "JobId" or "InviterAppUserId-InvitedAppUserId"
        /// </summary>
        public string CompositeKey { get; set; }
    }

    public class JobConversation : Conversation
    {
        public int JobId { get; set; }
    }

    public class SignupInviteConversation : Conversation
    {
        public int InviterAppUserId { get; set; }
        public int InvitedAppUserId { get; set; }
    }

    public enum MessageAffirmativeType
    {
        Unknown,
        yes,
        no,
    }

    public enum DayOfWeekMessage
    {
        sun,
        mon,
        tue,
        wed,
        thu,
        fri,
        sat
    }

    /// <summary>
    /// The hour of day of the start time of the job. (can only contain time increments of 30 minutes)
    /// Parsed from example values "9am" or "12:30am" or "10am"
    /// </summary>
    public class JobTimeSmall
    {
        public double Hour { get; set; }
    }
}