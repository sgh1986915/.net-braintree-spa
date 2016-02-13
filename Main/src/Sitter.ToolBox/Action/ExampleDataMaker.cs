using System;
using System.Collections.Generic;
using System.Linq;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Repository;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace Sitter.ToolBox.Action
{
    public class ExampleDataMaker
    {
        private JobDal _jobDal = new JobDal();
        private ParentRepository _parentRepo = new ParentRepository();
        private SignupRepository _signupRepo = new SignupRepository();

        public void InsertExampleData()
        {
            var infoParent1 = new SignupInfo
            {
                User = new AppUser
                {
                    FirstName = "Sarah",
                    LastName = "Fluckiger",
                    Email = "sarah@fluckiger.org",
                    MobilePhone = "+15127514094",
                    UserRole = UserRole.Parent,
                    TimezoneOffset = -5,
                    HasProfilePic = true
                },
                Pass = "p"
            };
            SaveSignup(infoParent1);

            var infoParent2 = new SignupInfo
            {
                User = new AppUser
                {
                    FirstName = "Parent2",
                    LastName = "Jones",
                    Email = "parent2@example.com",
                    MobilePhone = "+15125552102",
                    UserRole = UserRole.Parent,
                    TimezoneOffset = -5,
                    HasProfilePic = true
                },
                Pass = "p"
            };
            SaveSignup(infoParent2);


            var infoSitter1 = new SignupInfo
            {
                User = new AppUser
                {
                    FirstName = "Joseph",
                    LastName = "Fluckiger",
                    Email = "joseph@fluckiger.org",
                    MobilePhone = "+15129219530",
                    UserRole = UserRole.Sitter,
                    TimezoneOffset = -5
                },
                SitterSignupInfo = new SitterSignupInfo
                {
                    Age = 12,
                    ParentEmail = "sitter1parent@example.com",
                    ParentMobile = "+1512555301"
                },
                Pass = "p"
            };
            SaveSignup(infoSitter1);

            var infoSMike = new SignupInfo
            {
                User = new AppUser
                {
                    UserRole = UserRole.Sitter,
                    FirstName = "Michael-Sitter",
                    LastName = "Fluckiger",
                    Email = "michael@fluckiger.org",
                    MobilePhone = "+15125219854",
                    TimezoneOffset = -5
                },
                SitterSignupInfo = new SitterSignupInfo
                {
                    Age = 14,
                    ParentEmail = "sitter2parent@example.com",
                    ParentMobile = "+15125554444"
                },
                Pass = "p"
            };
            // Michael As Parent
            infoSMike.User.UserRole = UserRole.Parent;
            infoSMike.User.FirstName = "Michael-Parent";

            SaveSignup(infoSMike);

            var infoSitterArtem = new SignupInfo
            {
                User = new AppUser
                {
                    UserRole = UserRole.Sitter,
                    FirstName = "Artem",
                    LastName = "Shelkov",
                    Email = "shelkov1991@gmail.com",
                    MobilePhone = "+79137110364",
                    TimezoneOffset = +7
                },
                SitterSignupInfo = new SitterSignupInfo
                {
                    Age = 23
                },
                Pass = "p"
            };
            SaveSignup(infoSitterArtem);

            // STEP - Assign sitters to Parent.
            Parent parent1 = _parentRepo.GetById(infoParent1.User.Id);
            parent1.Sitters = new List<ParentMySitter>
            {
                new ParentMySitter {SitterId = infoSitter1.User.Id, Rate = (decimal) 6.5, SortOrder = 1},
                new ParentMySitter {SitterId = infoSitterArtem.User.Id, Rate = (decimal) 7.5, SortOrder = 2},
                //new ParentMySitter {SitterId = infoSitter3.User.Id, Rate = (decimal) 8.5, SortOrder = 3},
            };
            parent1.InviteToSignup = new List<InviteToSignup>
            {
                new InviteToSignup
                {
                    MobilePhone = "+15125554545",
                    InviteNickName = "Tom",
                    InviteStatus = InvitationState.InvitePending,
                },
                new InviteToSignup
                {
                    MobilePhone = "+15125554546",
                    InviteNickName = "Jerry",
                    InviteStatus = InvitationState.InvitePending,
                },
            };

            var admin1 = new SignupInfo
            {
                User = new AppUser
                {
                    FirstName = "Admin1",
                    LastName = "Business",
                    Email = "admin1@example.com",
                    MobilePhone = "+15125553345",
                    UserRole = UserRole.Admin,
                    TimezoneOffset = -5
                },
                Pass = "p"
            };
            SaveSignup(admin1);

            _parentRepo.UpdateParent(parent1);

            generateJobs(infoParent1.User.Id,
                new List<AppUser> {infoSitter1.User, infoSMike.User});
        }

        private void SaveSignup(SignupInfo signup)
        {
            SignupResult ret = _signupRepo.SaveNewSignup(signup);
            if (!ret.IsSuccess)
                throw new AppException("Invalid Signup:" + ret.Error);
        }

        private void generateJobs(int parentId, List<AppUser> sitters)
        {
            DateTime currentDate = TimeUtil.GetCurrentUtcTime();
            DateTime currentDate5pm = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 17, 0, 0).ToUniversalTime();

            int countJobs = 5;
            var jobs = new List<Job>();
            for (int i = 0; i < countJobs; i++)
            {
                // Create Job.
                var job = new Job
                {
                    ParentId = parentId,
                    Notes = "note " + i,
                };
                jobs.Add(job);
            }
            List<int> sitterIds = sitters.Select(m => m.Id).ToList();

            Job j1 = jobs[0];
            j1.State = JobState.Closed;
            j1.Start = currentDate5pm.AddDays(-3);
            j1.Duration = 5.5m;
            j1.Notes = "Closed, 3 days in past, paid successfully with finalized payment";
            AddInvites(j1, sitterIds, sitterIds[0]);
            j1.CloseReason = CloseReason.Paid;
            var finalize = new FinalizeJobPaymentSM();
            //finalize.JobId = j1.Id;
            finalize.UserId = j1.ParentId;
            finalize.Duration = j1.Duration;
            finalize.Bonus = 5;
            j1.SetFinalPayment(finalize);

            Job j2 = jobs[1];
            j2.State = JobState.Accepted;
            j2.Start = currentDate5pm.AddDays(-2);
            j2.Notes = "accepted by SitterId " + sitterIds[1];
            AddInvites(j2, sitterIds, sitterIds[1]);
            j2.Duration = 3m;
            j2.CloseReason = CloseReason.PaidOffline;

            Job j3 = jobs[2];
            j3.State = JobState.Closed;
            j3.Start = currentDate5pm.AddDays(-1);
            j3.Duration = 5;
            j2.Notes = "Closed, parent cancelled. Accepted by SitterId " + sitterIds[0];
            AddInvites(j3, sitterIds, sitterIds[0]);
            j3.CloseReason = CloseReason.ParentCancelled;
            j3.AcceptedSitterId = sitterIds[0];

            Job j4 = jobs[3];
            j4.State = JobState.Accepted;
            j4.Start = currentDate5pm.AddDays(0);
            j4.Duration = 2.5m;
            j4.Notes = "Accepted by sitterId " + sitterIds[0];
            AddInvites(j4, sitterIds, sitterIds[0]);

            Job j5 = jobs[4];
            j5.State = JobState.Posted;
            j5.Start = currentDate5pm.AddDays(1);
            j5.Duration = 2;
            j5.Notes = "Posted, pending invite Sitter1 (results in real text message), 2nd sitter pending invite, 3rd sitter declines";
            j5.JobInvites = new List<JobInvite>();
            j5.JobInvites.Add(new JobInvite {SitterId = sitterIds[0], RatePerHour = 8, InvitedDate = j5.Start.AddDays(-1)});
            j5.JobInvites.Add(new JobInvite {SitterId = sitterIds[1], RatePerHour = 9, InvitedDate = j5.Start.AddDays(-1)});
            j5.JobInvites[1].State = InvitationState.Declined;

            // STEP Invites, and Insert into DAL
            for (int i = 0; i < countJobs; i++)
            {
                _jobDal.Insert(jobs[i]);
            }
        }

        private void AddInvites(Job job, IEnumerable<int> sitterIds, int? acceptedSitterId = null)
        {
            job.JobInvites = new List<JobInvite>();
            foreach (int id in sitterIds)
            {
                var invite = new JobInvite {SitterId = id, RatePerHour = 5 + id, InvitedDate = job.Start.AddDays(-3), LatestResponseDate = job.Start.AddDays(-2)};
                job.JobInvites.Add(invite);
                if (id == acceptedSitterId)
                {
                    invite.State = InvitationState.Accepted;
                    invite.LatestResponseMessage = "yes";
                }
                else
                {
                    invite.State = InvitationState.Declined; //Futuredev: make some responses blank
                    invite.LatestResponseMessage = "no";
                }
            }
            if (acceptedSitterId != null && job.JobInvites.All(m => m.SitterId != acceptedSitterId.Value))
                throw new Exception();

            job.AcceptedSitterId = acceptedSitterId;
        }

        public void InsertJobReadyForPayment()
        {
            Parent parent = _parentRepo.GetAll().FirstOrDefault(m => m.Id == 1);
            List<MySitterHub.Model.Core.Sitter> sitters = new SitterRepository().GetAll();

            MySitterHub.Model.Core.Sitter sitter = sitters[0];

            // Create Job.
            var job = new Job
            {
                ParentId = parent.Id,
                Duration = 3,
                Start = new DateTime(2015, 1, 1),
                State = JobState.Accepted
            };

            var invite = new JobInvite {SitterId = sitter.Id, RatePerHour = 10, InvitedDate = job.Start.AddDays(-3), LatestResponseDate = job.Start.AddDays(-2)};
            job.JobInvites = new List<JobInvite>();
            job.JobInvites.Add(invite);
            job.AcceptedSitterId = sitter.Id;

            invite.State = InvitationState.Accepted;
            invite.LatestResponseMessage = "yes";

           // var finalize = new FinalizeJobPaymentSM();
           //// finalize.JobId = job.Id;
           // finalize.UserId = job.ParentId;
           // finalize.Duration = job.Duration;
           // finalize.Bonus = 5;
           // job.SetFinalPayment(finalize);

            _jobDal.Insert(job);
        }
    }
}