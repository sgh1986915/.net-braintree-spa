using System;
using System.Collections.Generic;
using System.Linq;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Messaging;
using MySitterHub.Logic.ServiceModels;
using MySitterHub.Logic.Util;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;
using MySitterHub.Logic.Payment;

namespace MySitterHub.Logic.Repository
{
    public class JobRepository
    {
        private readonly JobDal _jobDal = new JobDal();
        private readonly ParentRepository _parentRepo = new ParentRepository();
        private readonly SitterDal _sitterDal = new SitterDal();
        private  readonly  OutboundMessageManager _omm = new OutboundMessageManager();
        private  readonly  AppLogger _log = new AppLogger();
        private PaymentManager _paymentManager = new PaymentManager();
        
        public List<Job> GetAll()
        {
            return _jobDal.GetAll();
        }

        public Job GetById(int id)
        {
            return _jobDal.GetById(id);
        }

        public List<ParentJobSM> GetParentMyJobs(int parentId)
        {
            List<Job> jobs = _jobDal.GetByParentId(parentId).OrderByDescending(m => m.Start).ToList();
            var jobVMs = new List<ParentJobSM>();
            foreach (Job job in jobs)
            {
                var sm = JobSM.FromJob<ParentJobSM>(job, true);
                jobVMs.Add(sm);
                sm.PopulateFinanaicalForParent();

                // STEP - vm.InvitedSitters
                //sm.InvitedSitters = new List<Sitter>();
                sm.JobInvites  = new List<JobInviteSM>();
                foreach (JobInvite item in job.JobInvites)
                {
                    Sitter s = _sitterDal.GetById(item.SitterId);
                    JobInviteSM jobInviteSM;
                    if (s != null)
                    {
                        jobInviteSM = JobInviteSM.FromJobInvite(item);
                        jobInviteSM.SitterName = s.User.FullName();
                        sm.JobInvites.Add(jobInviteSM);                                            
                    }
                    else
                    {
                        _log.Warning(string.Format("Sitter not found with id {0} on jobInvite for jobId {1}", item.SitterId, job.Id));
                    }

                    //sm.InvitedSitters.Add(s);
                    // Set Sitter response on message.
                    //TxtMsgInbound reply = TxtMsgInboundDal.GetJobReplies(job.Id, item.SitterId);

                    //s.responseOnJobInvitation = reply != null ? reply.Message : "";
                    //jobId
                    //sm.InvitedSitters.Add(s);
                }

                if (job.AcceptedSitter() != null)
                {
                    sm.AcceptedSitterInvite = sm.JobInvites.FirstOrDefault(m => m.SitterId == job.AcceptedSitter().SitterId);
                    sm.OtherPartyName = sm.AcceptedSitterInvite == null ?  job.AcceptedSitter().SitterId.ToString() : sm.AcceptedSitterInvite.SitterName;
                }
                else
                {
                    sm.OtherPartyName = string.Format("({0} invited)", job.JobInvites.Count);
                }
            }

            return jobVMs;
        }

        public List<SitterJobSM> GetSitterMyJobs(int sitterId)
        {
            var jobVMs = new List<SitterJobSM>();
            List<Job> allJobs = _jobDal.GetAll();

            List<Job> sitterJobs =
                allJobs.Where(m => (m.AcceptedSitter() == null && m.JobInvites.Any(i => i.SitterId == sitterId)) ||
                                   (m.AcceptedSitter() != null && m.AcceptedSitter().SitterId == sitterId))
                                   .OrderByDescending(j=>j.Start).ToList(); //FutureDev: convert to Mongo query

            foreach (Job job in sitterJobs)
            {
                Parent parent = _parentRepo.GetById(job.ParentId);

                var sm = JobSM.FromJob<SitterJobSM>(job, false);
                jobVMs.Add(sm);
                sm.MyInvite = job.JobInvites.FirstOrDefault(m => m.SitterId == sitterId);
                if (sm.MyInvite == null) throw new AppException("sitterJobSm.MyInvite is null");
                if (job.State == JobState.Posted)
                {
                    switch (sm.MyInvite.State)
                    {
                        case InvitationState.InvitePending:
                            sm.StatusLabel = "Invite Pending";
                            break;
                        case InvitationState.Invited:
                            sm.StatusLabel = "Invited";
                            break;
                        case InvitationState.InvalidResponse:
                            sm.StatusLabel = "Invited, but invalid response from sitter";
                            break;
                        case InvitationState.Accepted:
                            sm.StatusLabel = "Accepted";
                            break;
                        case InvitationState.Declined:
                            sm.StatusLabel = "Declined";
                            sm.ShowAcceptButton = false;
                            break;
                    }
                }

                sm.Job.JobInvites = null; //makes sure we don't show other sitters to this sitter.

                sm.PopulateFinanaicalForSitter();
                sm.OtherPartyName = parent.User.FullName();
            }

            return jobVMs;
        }

        public PostJobResultSM PostJob(PostJobSM jobSm)
        {
            foreach (var invite in jobSm.JobInvites)
            {
                invite.InvitedDate = TimeUtil.GetCurrentUtcTime();
            }

            var job = new Job();
            job.ParentId = jobSm.ParentId;
            job.Notes = jobSm.Notes;
            job.Start = jobSm.DateTimeStartDte != null ? jobSm.DateTimeStartDte.Value : DateTime.Parse(jobSm.DateTimeStart);
            job.Duration = jobSm.Duration;
            job.JobInvites = jobSm.JobInvites;
            job.State = JobState.Posted;
            
            PostJobResultSM result = ValidateJobForInsert(job);
            if (!result.HasError)
            {
                _jobDal.Insert(job);
            }
            return result;
        }

        private PostJobResultSM ValidateJobForInsert(Job job)
        {
            var result = new PostJobResultSM();
            var now = TimeUtil.GetCurrentUtcTime();
            if (job.Start < now)
            {
                result.Error += "Job Start is in the past";
                return result;
            }
            if (job.Duration <= 0)
            {
                result.Error += "Job Duration is not greater than zero";
                return result;
            }
            if (job.JobInvites == null || job.JobInvites.Count == 0)
            {
                result.Error += "Must invite at least one sitter";
                return result;
            }

            return result;
        }

        public bool CancelJob(CancelJobSM cancel)
        {
            Job job = _jobDal.GetById(cancel.JobId);
            if (job.ParentId != cancel.UserId)
                throw new Exception("Unable to cancel job, parentId is not equal to authenticated user.");

            job.SetCancelled(CloseReason.ParentCancelled);

            _jobDal.Update(job);
            return true;
        }

        public bool ProcessSitterResponse(SitterJobInviteResponseSM response, Job jobIn)
        {
            Job job = jobIn ?? _jobDal.GetById(response.JobId);
            JobInvite invite = job.JobInvites.FirstOrDefault(m => m.SitterId == response.SitterId);
            if (invite == null)
            {
                throw new AppException(string.Format("Unable to find invite on job {0} for sitterId {1}", job.Id, response.SitterId));
            }
            invite.LatestResponseDate = TimeUtil.GetCurrentUtcTime();
            invite.LatestResponseMessage = response.Message;

            if (response.Response == SitterResponse.Accept)
            {
                job.State = JobState.Accepted;
                job.AcceptedSitterId = response.SitterId;

                // STEP - Send Text to parent and other sitters of job closed.
                try
                {
                    // STEP - Send confirm to parent, and closed notice to other sitters
                    _omm.SendNoticeOfSitterAccept(job);
                }
                catch (Exception ex)
                {
                    _log.Error(job.ParentId.ToString(), "error while SendParentNoticeOfSitterAccept()." , ex);
                    throw;
                }

            }
            else if (response.Response == SitterResponse.Decline)
            {
                invite.State = InvitationState.Declined;
                bool allSittersDeclined = job.JobInvites.All(m => m.State == InvitationState.Declined);
                job.State = JobState.Closed;
                job.CloseReason = CloseReason.AllSittersDeclined;
                _omm.SendParentNoticeOfSitterDecline(job, response.SitterId, allSittersDeclined);
            }
            else
            {
                invite.State = InvitationState.InvalidResponse;
            }
            _jobDal.Update(job);

            return true;
        }

        public bool SitterRenegeAcceptedJob(SitterCancelAcceptedJobSM renege)
        {
            var job = _jobDal.GetById(renege.JobId);
            job.State = JobState.Closed;
            job.CloseReason = CloseReason.SitterCancelled;
            _jobDal.Update(job);

            // STEP - Notify Parent
            _omm.SendParentNoticeOfSitterRenege(job, renege.SitterId);

            return true;
        }

        public MakePaymentResultSM FinalizePayment(FinalizeJobPaymentSM finalize)
        {
            // STEP - Save Finalize Payment
            Job job = _jobDal.GetById(finalize.JobId);
            job.Duration = finalize.Duration;
            
            //job.Start = finalize.Start; //TODO: set start time.
            ValidateFinalize(job, finalize);
            job.SetFinalPayment(finalize);
            _jobDal.Update(job);

            // STEP - Call payment gateway to make payment
            var sm = new MarketPayment();
            sm.Amount = job.FinalPayment.PaidByParent;
            sm.MySitterHubFee = job.FinalPayment.MySitterHubFee();
            sm.ParentId = job.ParentId;
            sm.SitterId = job.AcceptedSitter().SitterId;
            MakePaymentResultSM ret = _paymentManager.MakeMarketplaceTransaction(sm);
            return ret;
        }

        private void ValidateFinalize(Job job, FinalizeJobPaymentSM finalize)
        {
            if (job == null)
                throw new Exception(string.Format("Job with id '{0}' not found.", finalize.JobId));

            if (job.ParentId != finalize.UserId)
                throw new Exception("Unable to finalize job, parentId is not equal to authenticated user.");
            if (finalize.Bonus < 0)
                throw new Exception("bonus is less than 0");
            if (finalize.Bonus > 200)
                throw new Exception("bonus is greater than 200");
            if (finalize.Duration > 24)
                throw new Exception("duration is greater than 24 hours");
        }

        public bool SitterRequestPayment(SitterRequestPaymentSM requestPayment)
        {
            Job job = _jobDal.GetById(requestPayment.JobId);
            if (job.SitterRequestedPayment)
                return false;

            job.SitterRequestedPayment = true;
            _jobDal.Update(job);
            return _omm.SitterRequestPayment(requestPayment);
        }
    }

    public static class JobCalculations
    {
        public static decimal CalculatePaidSitter(this Job job, decimal rate, decimal bonus = 0)
        {
            decimal paid = job.Duration*rate + bonus;
            return paid;
        }

        public static decimal CalculateParentTotal(this Job job, decimal rate)
        {
            return job.CalculatePaidSitter(rate)*LogicConstants.MySitterHubCut;
        }

        /// <summary>
        ///     Duration in hours
        /// </summary>
        public static DateTime EndTime(this Job job)
        {
            return job.Start.AddHours(Convert.ToDouble(job.Duration));
        }

        public static void SetFinalPayment(this Job job, FinalizeJobPaymentSM finalize)
        {
            //FutureDev: validation
            job.FinalPayment = new FinalPayment();
            job.FinalPayment.Duration = finalize.Duration;
            job.FinalPayment.Bonus = finalize.Bonus;
            job.FinalPayment.RatePerHour = job.AcceptedSitter().RatePerHour;
            job.FinalPayment.PaidToSitter = job.CalculatePaidSitter(job.AcceptedSitter().RatePerHour, finalize.Bonus);
            job.FinalPayment.PaidByParent = job.FinalPayment.PaidToSitter*LogicConstants.MySitterHubCut;
            job.State = JobState.Closed;
            job.PaymentState = PaymentState.ParentInitiated;
        }

        public static void SetCancelled(this Job job,CloseReason closeReason)
        {
            job.State = JobState.Closed;
            job.CloseReason = closeReason;

        }

        public static string StartDateFormatted(this Job job)
        {
            return job.Start.ToShortDateString();
        }
    }

    public class FinalizeJobPaymentSM
    {
        public int JobId { get; set; }
        public int UserId { get; set; }

        public decimal Bonus { get; set; }
        public decimal Duration { get; set; }
        public DateTime Start { get; set; }
    }
}