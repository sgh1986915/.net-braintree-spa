using System;
using System.Collections.Generic;
using MySitterHub.Logic.Messaging;
using MySitterHub.Logic.Repository;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.Logic.ServiceModels
{
    public class PostJobSM
    {
        public int ParentId { get; set; }
        /// <summary>
        /// Used if DateTimeStartDte is null
        /// </summary>
        public string DateTimeStart { get; set; }
        public DateTime? DateTimeStartDte { get; set; }
        public decimal Duration { get; set; }
        public string Notes { get; set; }
        public List<JobInvite> JobInvites { get; set; }
    }

    public class ParentJobSM : JobSM
    {
        public bool ShowInvitedSitters { get; set; }
        public List<JobInviteSM> JobInvites { get; set; } //FutureDev: rename to JobInvitesSM (this is a bit redundant since we have job.invites also, except for sitter name.)
        public JobInviteSM AcceptedSitterInvite { get; set; }
        public bool ShowPayButton { get; set; }
        public bool ShowParentCancelButton { get; set; }
    }

    public class SitterJobSM : JobSM
    {
        public JobInvite MyInvite { get; set; }
        public bool ShowAcceptButton { get; set; }
        public bool ShowSitterCancelButton { get; set; }
        public bool ShowRequestPayment { get; set; }
    }

    public class JobSM
    {
        public Job Job { get; set; }
        public FuturePastType FuturePast { get; set; }
        public bool IsOpen { get; set; }
        public string OtherPartyName { get; set; }

        public string StatusLabel { get; set; }
        public string Error { get; set; }

        public decimal EstRatePerHour { get; set; }
        public decimal EstPaidToSitter { get; set; }
        public decimal EstPaidByParent { get; set; }

        public static T FromJob<T>(Job job, bool isParent) where T : JobSM, new()
        {
            JobSM sm = new T();
            var psm = sm as ParentJobSM;
            var ssm = sm as SitterJobSM;

            sm.Job = job;
            sm.IsOpen = job.State != JobState.Closed;

            DateTime now = TimeUtil.GetCurrentUtcTime();
            if (now < job.Start)
            {
                sm.FuturePast = FuturePastType.Future;
            }
            else if (now >= job.Start && now <= job.EndTime())
            {
                sm.FuturePast = FuturePastType.InProgress;
            }
            else
            {
                sm.FuturePast = FuturePastType.Past;
            }

            switch (job.State)
            {
                case JobState.Posted:
                    sm.StatusLabel = "Waiting For Sitters";
                    if (psm != null) psm.ShowParentCancelButton = true;
                    if (psm != null) psm.ShowInvitedSitters = true;
                    if (ssm != null) ssm.ShowAcceptButton = true;
                    break;
                case JobState.Accepted:
                    switch (sm.FuturePast)
                    {
                        case FuturePastType.Future:
                            sm.StatusLabel = "Accepted";
                            if (ssm != null) ssm.ShowSitterCancelButton = true;
                            break;
                        case FuturePastType.InProgress:
                            sm.StatusLabel = "In Progress";
                            if (psm != null) psm.ShowPayButton = true;
                            break;
                        case FuturePastType.Past:
                            sm.StatusLabel = "Outstanding Payment";
                            if (psm != null) psm.ShowPayButton = true;
                            if (ssm != null) ssm.ShowRequestPayment = true;
                            break;
                    }
                    if (psm != null) psm.ShowParentCancelButton = true;
                    break;
                case JobState.Closed:
                    switch (job.CloseReason)
                    {
                        case CloseReason.Paid:
                            sm.StatusLabel = "Paid";
                            break;
                        case CloseReason.AllSittersDeclined:
                            sm.StatusLabel = "All Sitters Declined";
                            break;
                        case CloseReason.ParentCancelled:
                            sm.StatusLabel = "Parent Cancelled";
                            break;
                        case CloseReason.SitterCancelled:
                            sm.StatusLabel = "Sitter Cancelled";
                            break;
                        case CloseReason.PaidOffline:
                            sm.StatusLabel = "Paid Offline";
                            break;
                        case CloseReason.Other:
                            sm.StatusLabel = "Other";
                            break;
                    }
                    break;
            }

            return (T) sm;
        }

        public void PopulateFinanaicalForParent()
        {
            EstRatePerHour = Job.AcceptedSitter() == null ? 0 : Job.AcceptedSitter().RatePerHour;

            if (Job.FinalPayment != null)
            {
                EstPaidByParent = Job.FinalPayment.PaidByParent;
                EstPaidToSitter = Job.FinalPayment.PaidToSitter;
            }
            else if (Job.AcceptedSitter() != null)
            {
                EstPaidByParent = Job.CalculateParentTotal(Job.AcceptedSitter().RatePerHour);
                EstPaidToSitter = Job.CalculatePaidSitter(Job.AcceptedSitter().RatePerHour);
            }
        }

        public void PopulateFinanaicalForSitter()
        {
            var ssm = (SitterJobSM) this;
            EstRatePerHour = ssm.MyInvite == null ? 0 : ssm.MyInvite.RatePerHour;

            if (Job.FinalPayment != null)
            {
                EstPaidByParent = Job.FinalPayment.PaidByParent;
                EstPaidToSitter = Job.FinalPayment.PaidToSitter;
            }
            else if (ssm.MyInvite != null)
            {
                EstPaidByParent = Job.CalculateParentTotal(ssm.MyInvite.RatePerHour);
                EstPaidToSitter = Job.CalculatePaidSitter(ssm.MyInvite.RatePerHour);
            }
        }
    }

    public class PostJobResultSM
    {
        private string _error;

        public bool HasError { get; set; }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                HasError = true;
            }
        }
    }

    public class AcceptedSitterSM
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
    }

    public class CancelJobSM
    {
        public int JobId { get; set; }
        public int UserId { get; set; }
        public UserRole Role { get; set; }
    }

    public class JobInviteSM :JobInvite
    {
        public string SitterName { get; set; }

        public static JobInviteSM FromJobInvite(JobInvite jobInvite)
        {
            JobInviteSM sm = new JobInviteSM();
            sm.InvitedDate = jobInvite.InvitedDate;
            sm.LatestResponseDate = jobInvite.LatestResponseDate;
            sm.LatestResponseMessage = jobInvite.LatestResponseMessage;
            sm.RatePerHour = jobInvite.RatePerHour;
            sm.SitterId = jobInvite.SitterId;
            sm.SitterName = "";
            sm.State = jobInvite.State;
            return sm;
        }
    }

    public class PostJobViaTxtAnswers
    {
        public DayOfWeekMessage DayOfWeek { get; set; }
        public JobTimeSmall StartHour { get; set; }
        public List<int> InvitedSitterIds { get; set; }
    }
}