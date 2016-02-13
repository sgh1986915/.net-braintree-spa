using System;
using System.Collections.Generic;
using System.Linq;

namespace MySitterHub.Model.Core
{
    public class Job
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public DateTime Start { get; set; }
        public decimal Duration { get; set; }
        public string Notes { get; set; }
        public List<JobInvite> JobInvites { get; set; }
        public int? AcceptedSitterId { get; set; }

        public JobState State { get; set; }
        
        public CloseReason? CloseReason { get; set; }
        public PaymentState? PaymentState { get; set; }
        public FinalPayment FinalPayment { get; set; }
        public bool SitterRequestedPayment { get; set; }

        public JobInvite AcceptedSitter()
        {
            if (AcceptedSitterId == null || JobInvites == null)
                return null;
            return JobInvites.FirstOrDefault(m => m.SitterId == AcceptedSitterId);
        }

    }

    public class JobInvite
    {
        public int SitterId { get; set; }
        public decimal RatePerHour { get; set; }

        public InvitationState State { get; set; }
        public DateTime? InvitedDate { get; set; }
        public DateTime? LatestResponseDate { get; set; }
        public string LatestResponseMessage { get; set; }
    }

    public enum FuturePastType
    {
        Future,
        InProgress,
        Past
    }

    public enum InvitationState
    {
        InvitePending,
        Invited,
        InvalidResponse,
        InvalidSitterIdNotFound,        
        Declined,
        Accepted
    }

    public class FinalPayment
    {
        public decimal Duration { get; set; }
        public decimal Bonus { get; set; }
        public decimal RatePerHour { get; set; }
        public decimal PaidToSitter { get; set; }
        public decimal PaidByParent { get; set; }

        public decimal MySitterHubFee()
        {
            return PaidByParent - PaidToSitter;
        }

    }

    /// <summary>
    ///     Keep track of state changes to Jobs
    /// </summary>
    public class JobAuditHistoryItem
    {
        public string Id { get; set; }
        public int JobId { get; set; }
        public string Message { get; set; }
    }

    public enum JobState
    {
        Posted,
        Accepted,
        Closed
    }

    public enum SitterCancelReason
    {
        Undefined
    }

    public enum CloseReason
    {
        Paid,
        AllSittersDeclined,
        ParentCancelled,
        SitterCancelled,
        PaidOffline,
        Other
    }

    public enum PaymentState
    {
        NotStarted,
        ParentInitiated,
        ConfirmedSend,
        ConfirmedReceive
    }
}