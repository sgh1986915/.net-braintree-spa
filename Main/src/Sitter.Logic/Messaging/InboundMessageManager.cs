using System;
using System.Collections.Generic;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Repository;
using MySitterHub.Logic.ServiceModels;
using MySitterHub.Logic.Util;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.Logic.Messaging
{
    public class InboundMessageManager
    {
        private readonly JobRepository _jobRepo = new JobRepository();
        private readonly AppLogger _log = new AppLogger();
        private readonly OutboundMessageManager _omm = new OutboundMessageManager();
        private readonly ParentRepository _parentRepository = new ParentRepository();
        private readonly TxtMsgAwaitingResponseDal _txtMsgAwaitingResponseDal = new TxtMsgAwaitingResponseDal();
        private readonly TxtMsgInboundDal _txtMsgInboundDal = new TxtMsgInboundDal();

        public void ProcessSitterResonseToJobPosting(TxtMsgInbound msgInbound, TxtMsgAwaitingResponse awaiting, AppUser user)
        {
            string feedback = null;
            bool stillAwaiting = true;
            Job job = _jobRepo.GetById(awaiting.JobId);

            if (job == null)
            {
                string msgError = "Unable to find job with id " + awaiting.JobId;
                _log.Error(msgInbound.MobilePhone, msgError, null);
                _txtMsgInboundDal.UpdateState(msgInbound.Id, TxtMsgProcessState.Error, msgError);
                _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
                return;
            }
            var response = new SitterJobInviteResponseSM
            {
                JobId = job.Id,
                SitterId = awaiting.WaitingForUserId,
                Message = msgInbound.Message
            };

            MessageAffirmativeType aff = MessageUtility.ParseAffirmativeType(msgInbound);

            if (aff == MessageAffirmativeType.yes)
            {
                response.Response = SitterResponse.Accept;
                feedback = MessageTemplates.FormatSitterAcceptJob();
                stillAwaiting = false;
            }
            else if (aff == MessageAffirmativeType.no)
            {
                response.Response = SitterResponse.Decline;
                feedback = MessageTemplates.FormatSitterDeclineJob("parent"); //TODO
                stillAwaiting = false;
            }
            else
            {
                response.Response = SitterResponse.Unrecognized;
                feedback = MessageTemplates.FormatSitterInvalidResponseToJobOrSignup(true);
                stillAwaiting = true;
            }
            _jobRepo.ProcessSitterResponse(response, job);


            _txtMsgInboundDal.UpdateState(msgInbound.Id, TxtMsgProcessState.Processed);

            if (feedback != null)
            {
                _omm.SendFeedbackToInboundMessage(msgInbound, feedback, user.Id);
            }

            if (!stillAwaiting)
            {
                _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
            }

            if (aff == MessageAffirmativeType.yes && job != null)
            {
                try
                {
                    // STEP - Send confirm to parent, and closed notice to other sitters
                    _omm.SendNoticeOfSitterAccept(job);
                }
                catch (Exception ex)
                {
                    _log.Error(job.ParentId.ToString(), "error while SendParentNoticeOfSitterAccept(). msgInbound.Id:" + msgInbound.Id, ex);
                    throw;
                }
            }
        }

        public void ProcessResponseToSignupInvite(TxtMsgInbound msgInbound, TxtMsgAwaitingResponse awaiting, AppUser user)
        {
            MessageAffirmativeType affirmativeType = MessageUtility.ParseAffirmativeType(msgInbound);
            string feedback = null;
            Parent parent = _parentRepository.GetById(awaiting.ParentId, true);
            if (parent == null)
                throw new Exception("Error while processing inbound message. Parent not found with ID" + awaiting.ParentId);
            // If user answers 'yes'.
            if (affirmativeType == MessageAffirmativeType.yes)
            {
                // If user already exists we simply add hib to parent's sitterhub.
                if (user != null)
                {
                    if (user.UserRole == UserRole.Sitter)
                    {
                        // ..sure if this user is a sitter.
                        AddSitterToParentMySitters(user.Id, parent, msgInbound.MobilePhone);
                        feedback = MessageTemplates.FormatUserAddedToSitters(parent);
                    }
                    else
                    {
                        // Otherwise we cannot add user because he is not a sitter.
                        feedback = MessageTemplates.FormatInvitedUserIsNotASitter();
                    }
                }
                    // If user doesn't exist we need to create new one.
                else
                {
                    // Create new (default) sitter profile.
                    var newSitter = new SignupInfo
                    {
                        User = new AppUser
                        {
                            FirstName = "New Sitter",
                            LastName = "",
                            Email = "",
                            MobilePhone = msgInbound.MobilePhone,
                            UserRole = UserRole.Sitter,
                        },
                        Pass = GeneratePassForNewUser() //FutureDev: text this password to sitter as their new temp password.
                    };
                    newSitter.SitterSignupInfo = new SitterSignupInfo {ParentEmail = "", ParentMobile = ""};

                    SignupResult result = new SignupRepository().SaveNewSignup(newSitter, false, false);
                    if (!result.IsSuccess)
                        throw new Exception("Failed to create new profile for sitter during parent invite." + result.Error);
                    // If there were no errors we can continue.
                    // We need to add newly created user into parent's sitterhub.
                    AddSitterToParentMySitters(result.NewId, parent, msgInbound.MobilePhone);
                    // Send message about new account created and user successfully added to parent's hub.
                    feedback = MessageTemplates.FormatUserAddedToSittersAndProfileCreated(parent, newSitter.Pass);
                }
            }
                // If user answered 'no'.
            else if (affirmativeType == MessageAffirmativeType.no)
            {
                // TODO: separate failure message.
                // We send message to sitter that we didn't added him to parent's hub.
                feedback = MessageTemplates.FormatDeclineInvitationToSitterhub(parent);
                _parentRepository.CancelInviteSitter(parent.Id, msgInbound.MobilePhone, true);
            }
                // If user's answer can't be parsed.
            else
            {
                feedback = MessageTemplates.FormatSitterInvalidResponseToJobOrSignup(false);
            }
            // Update state of message.
            _txtMsgInboundDal.UpdateState(msgInbound.Id, TxtMsgProcessState.Processed);
            // If sitter's answer was successfully parsed we can mark this message as processed.
            if (affirmativeType != MessageAffirmativeType.Unknown)
            {
                _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
            }
            // TODO: where we send messages to parent? Seems like we can do it here.
            if (feedback != null)
            {
                _omm.SendFeedbackToInboundMessage(msgInbound, feedback, user.Id);
            }
        }

        private string GeneratePassForNewUser()
        {
            int pass = new Random().Next(1001, 9994);
            return pass.ToString();
        }

        private void AddSitterToParentMySitters(int sitterId, Parent parent, string sitterMobilePhone)
        {
            var newS = new ParentMySitter {SitterId = sitterId, Rate = LogicConstants.DefaultSitterRate, SortOrder = 99};
            _parentRepository.AddSitter(parent.Id, newS);
            _parentRepository.CancelInviteSitter(parent.Id, sitterMobilePhone);
        }

        public void ProcessSignupInviteConversation(TxtMsgInbound msgInbound, TxtMsgAwaitingResponse awaiting, AppUser user)
        {
            string mobileToInvite = PhoneUtil.CleanAndEnsureCountryCode(msgInbound.Message);
            if (MessageUtility.IsCancelRequested(msgInbound))
            {
                _omm.SendFeedbackToInboundMessageAndDeleteAwaiting(msgInbound, string.Format("Ok, invitation cancelled."), user.Id, awaiting);
                return;
            }
            if (!PhoneUtil.IsValidPhoneNumber(mobileToInvite))
            {
                _omm.SendFeedbackToInboundMessage(msgInbound, string.Format("'{0}' is not a valid mobile number to invite to signup for mysitterhub.com. Please enter 10 digit number. Or say 'cancel'.", msgInbound.Message), user.Id);
                return;
            }

            int parentId = awaiting.WaitingForUserId;
            AppUser parentUser = new AppUserDal().GetById(parentId);

            if (parentUser.UserRole != UserRole.Parent)
            {
                //STEP - make sure user is a Parent, if not say only parents can invite sitters.
                _omm.SendFeedbackToInboundMessage(msgInbound, MessageTemplates.OnlyParentsCanSendSignupInvites(), user.Id);
            }
            else
            {
                //STEP - confirm sending invite
                _omm.SendFeedbackToInboundMessage(msgInbound, "Sending invite to " + mobileToInvite, user.Id);
                var sitterInvite = new InviteToSignup {MobilePhone = mobileToInvite};
                ServiceResult result = new ParentRepository().AddSitterInviteByMobile(parentId, sitterInvite);
                if (!result.IsSuccess)
                {
                    _omm.SendFeedbackToInboundMessage(msgInbound, "Error while sending invite to " + mobileToInvite + " please try again.", user.Id);
                }
            }
            _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
        }


        public void ProcessSelfSignupConversation_Step1(TxtMsgInbound msgInbound, TxtMsgAwaitingResponse awaiting, AppUser user)
        {
            bool deleteAwaiting = true;
            if (MessageUtility.IsCancelRequested(msgInbound))
            {
                _omm.SendFeedbackToInboundMessage(msgInbound, "Ok, signup cancelled", user.Id);
            }
            else if (msgInbound.Message == "sitter" || msgInbound.Message == "parent" || msgInbound.Message == "babysitter")
            {
                bool isSitter = (msgInbound.Message == "sitter" || msgInbound.Message == "babysitter");
                // STEP - Create new AppUser
                var newUser = new SignupInfo
                {
                    User = new AppUser
                    {
                        FirstName = "New Self Signup",
                        LastName = "",
                        Email = "",
                        MobilePhone = msgInbound.MobilePhone,
                        UserRole = isSitter ? UserRole.Sitter : UserRole.Parent,
                    },
                    Pass = GeneratePassForNewUser()
                };
                if (isSitter)
                {
                    newUser.SitterSignupInfo = new SitterSignupInfo {ParentEmail = "", ParentMobile = ""};
                }

                SignupResult result = new SignupRepository().SaveNewSignup(newUser, false);
                if (result.IsSuccess)
                {
                    _omm.SendFeedbackToInboundMessage(msgInbound, string.Format("Ok, you have signed up for mysitterhub.com, you can login at mysitterhub.com with your mobile number and password: {0}.", newUser.Pass), user.Id);
                    //_omm.SendFeedbackToInboundMessage(msgInbound, string.Format("You can finish filling out your profile. What is your name? Or say 'cancel'."));
                    //_txtMsgAwaitingResponseDal.Insert(new TxtMsgAwaitingResponse(outboundTxtMsgId, 0, msgInbound.MobilePhone, InboundMessageType.SelfSignup_Step2_Name, 0)); //FutureDev
                }
                else
                {
                    _omm.SendFeedbackToInboundMessage(msgInbound, string.Format("Error while signing up:" + result.Error), user.Id);
                    new LogUtil().LogMessage("signup unsuccessful " + result.Error);
                    deleteAwaiting = false;
                }
            }
            else
            {
                _omm.SendFeedbackToInboundMessage(msgInbound, string.Format("Invalid response, are you a 'parent' or a 'sitter'? Or say 'cancel' to quit the signup process."), user.Id);
                deleteAwaiting = false;
            }

            if (deleteAwaiting)
                _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
        }

        public void ProcessSelfSignupConversation_Step2(TxtMsgInbound msgInbound, TxtMsgAwaitingResponse awaiting, AppUser user)
        {
        }

        public void ProcessPostJobConversation(TxtMsgInbound msgInbound, TxtMsgAwaitingResponse awaiting, AppUser parentUser)
        {
            string conversationContext;
            string msg;

            if (MessageUtility.IsCancelRequested(msgInbound))
            {
                msg = "Ok, job posting cancelled";
                _omm.SendFeedbackToInboundMessageAndDeleteAwaiting(msgInbound, msg, parentUser.Id, awaiting);
            }

            switch (msgInbound.InboundMessageType)
            {
                case InboundMessageType.PostJob_Step2_DayOfWeek:
                    DayOfWeekMessage? dayOfWeek = MessageUtility.ParseDayOfWeek(msgInbound.Message);
                    if (dayOfWeek == null)
                    {
                        msg = string.Format("Invalid day of week, post a job for which day: {0}? Or say 'cancel'", new NewCommandManager().DaysOfWeekStartingToday(parentUser));
                        _omm.SendFeedbackToInboundMessage(msgInbound, msg, parentUser.Id);
                    }
                    else
                    {
                        conversationContext = JsonSerializerHelper.Serialize(new PostJobViaTxtAnswers {DayOfWeek = dayOfWeek.Value});
                        msg = "What time?";
                        _omm.SendFeedbackToInboundMessageAndAwait(msgInbound, msg, parentUser.Id, InboundMessageType.PostJob_Step3_StartTime, awaiting, conversationContext);
                    }
                    break;
                case InboundMessageType.PostJob_Step3_StartTime:
                    JobTimeSmall startHour = MessageUtility.ParseStartTime(msgInbound.Message, _log);
                    if (startHour == null)
                    {
                        msg = string.Format("Please enter a valid job start time. Valid examples: 9am, 9pm, 12:30am, 10:00pm. What time?");
                        _omm.SendFeedbackToInboundMessage(msgInbound, msg, parentUser.Id);
                    }
                    else
                    {
                        var postJobAnswers = JsonSerializerHelper.DeSerialize<PostJobViaTxtAnswers>(awaiting.ConversationMemory);
                        postJobAnswers.StartHour = startHour;
                        conversationContext = JsonSerializerHelper.Serialize(postJobAnswers);
#if OFF
                        var sits = ListSittersForParent(msgInbound.UserId);
                        if (sits == null)
                        {
                            msg = "Unable to post job because you do not have any sitters. Please invite babysitters to your MySitters."; //FutureDev: do this earlier
                            _omm.SendFeedbackToInboundMessage(msgInbound, msg);
                            _txtMsgAwaitingResponseDal.DeleteAwaiting(awaiting.Id);
                        }
                        else
                        {
                            newAwaitingType = InboundMessageType.PostJob_Step5_Confirm;
                            msg = string.Format("Which sitters? {0}", sits);
                        }
#endif
                        msg = MessageUtility.FormatJobConfirm(postJobAnswers, parentUser, _parentRepository);
                        _omm.SendFeedbackToInboundMessageAndAwait(msgInbound, msg, parentUser.Id, InboundMessageType.PostJob_Step5_Confirm, awaiting, conversationContext);
                    }
                    break;
#if OFF
                case InboundMessageType.PostJob_Step4_SelectSitters:
                    int? sitterId = ParseSitterNum(msgInbound.Message, msgInbound.UserId);

#endif

                case InboundMessageType.PostJob_Step5_Confirm:
                    MessageAffirmativeType affirmative = MessageUtility.ParseAffirmativeType(msgInbound, true);

                    if (affirmative == MessageAffirmativeType.yes)
                    {
                        var pa = JsonSerializerHelper.DeSerialize<PostJobViaTxtAnswers>(awaiting.ConversationMemory);

                        var jobSM = new PostJobSM();


                        jobSM.DateTimeStartDte = MessageUtility.CalcJobStartTime(pa).AddHours(- parentUser.TimezoneOffset); // Convert to UTC
                        jobSM.Duration = 3; // note, default to 3
                        jobSM.JobInvites = new List<JobInvite>();
                        List<ParentMySitterSM> psitters = _parentRepository.GetSittersForParent(parentUser.Id).Sitters;
                        foreach (ParentMySitterSM s in psitters)
                        {
                            jobSM.JobInvites.Add(new JobInvite { SitterId = s.Id, InvitedDate = TimeUtil.GetCurrentUtcTime(), RatePerHour = s.Rate, State = InvitationState.InvitePending });
                        }

                        jobSM.ParentId = parentUser.Id;
                        _jobRepo.PostJob(jobSM);

                        _omm.SendFeedbackToInboundMessageAndDeleteAwaiting(msgInbound, "Congrats! Job posted. You can view details at mysitterhub.com", parentUser.Id, awaiting);
                    }
                    else if (affirmative == MessageAffirmativeType.no)
                    {
                        _omm.SendFeedbackToInboundMessageAndDeleteAwaiting(msgInbound, "Ok, Job posting cancelled", parentUser.Id, awaiting); //note, this will only get hit when the user says "no".
                    }
                    else
                    {
                        _omm.SendFeedbackToInboundMessage(msgInbound, "Please say 'yes' to post job or 'cancel'", parentUser.Id);
                    }

                    break;
                default:
                    throw new Exception("InboundMessageType not recognized in ProcessPostJobConversation. " + msgInbound.InboundMessageType);
            }
        }

#if OFF
        private int? ParseSitterNum(string message, int parentId)
        {
            ParentMySittersDataSM parentMySitters = _parentRepository.GetSittersForParent(parentId);
            foreach (var s in parentMySitters.Sitters)
            {
                    
            }

            int sitterNum;

            if (int.TryParse(message, out sitterNum))
            {
                if (sitterNum >= 0 && sitterNum < parentMySitters.Sitters.Count + 1)
                {
                    return sitterNum;
                }
            }
        }

         private string ListSittersForParent(int parentId)
        {
            ParentMySittersDataSM parentMySitters = _parentRepository.GetSittersForParent(parentId);
            if (parentMySitters == null || parentMySitters.Sitters.Count == 0)
            {
                return null;
            }
            string ret = "1)all ";
            int counter = 2;
            foreach (var s in parentMySitters.Sitters)
            {
                ret += string.Format("{0}){1} ", counter, s.FirstName);
                counter++;
            }

            return ret;
        }
#endif
    }
}