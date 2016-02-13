using System;
using System.Collections.Generic;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Repository;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.Logic.Messaging
{
    public class NewCommandManager
    {
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly OutboundMessageManager _omm = new OutboundMessageManager();


        public void ProcessNewConverastion(TxtMsgInbound msgInbound)
        {
            // STEP - Check if user is member
            AppUser user = _appUserDal.GetByMobile(msgInbound.MobilePhone);
            if (user == null)
            {
                if (msgInbound.Message == NewCommandType.signup.ToString())
                {
                    ProcessSignupCommand(msgInbound, null);
                    return;
                }

                string msg = "To signup for mysitterhub.com, say 'signup' or go to the website";
                _omm.SendFeedbackToInboundMessage(msgInbound, msg, AppUser.NotSignedUpUserId);
                return;
            }

            NewCommandType newCommand;
            Enum.TryParse(msgInbound.Message, out newCommand);

            switch (newCommand)
            {
                case NewCommandType.unrecognized:
                    _omm.SendFeedbackToInboundMessage(msgInbound, "mysitterhub.com " + MessageTemplates.FormatValidCommands(user.UserRole), user.Id);
                    break;
                case NewCommandType.job:
                    ProcessPostJobCommand(msgInbound, user);
                    break;
                case NewCommandType.help:
                    ProcessHelpCommand(msgInbound, user);
                    break;
                case NewCommandType.invite:
                    ProcessInviteCommand(msgInbound, user);
                    break;
                case NewCommandType.status:
                    ProcessStatusCommand(msgInbound, user);
                    break;
                case NewCommandType.signup:
                    ProcessSignupCommand(msgInbound, user);
                    break;
                default:
                    throw new Exception("command not implimented: " + newCommand);
            }
        }

        private void ProcessHelpCommand(TxtMsgInbound msgInbound, AppUser user)
        {
            _omm.SendFeedbackToInboundMessage(msgInbound, "mysitterhub.com help. " + MessageTemplates.FormatValidCommands(user.UserRole), user.Id);
        }

        private void ProcessStatusCommand(TxtMsgInbound msgInbound, AppUser user)
        {
            _omm.SendFeedbackToInboundMessage(msgInbound, "Ok, I'll list your jobs. Your open jobs are..", user.Id);
            //FutureDev: implement
        }

        private void ProcessInviteCommand(TxtMsgInbound msgInbound, AppUser user)
        {
            if (user.UserRole != UserRole.Parent)
            {
                //STEP - make sure user is a Parent, if not say only parents can invite sitters.
                _omm.SendFeedbackToInboundMessage(msgInbound, MessageTemplates.OnlyParentsCanSendSignupInvites(), user.Id);
            }
            else
            {
                _omm.SendFeedbackToInboundMessageAndAwait(msgInbound, "Ok, I'll invite someone to join your MySitters. What is their mobile number?", user.Id, InboundMessageType.SignupInvite);
            }
        }

        public string DaysOfWeekStartingToday(AppUser user)
        {
            DateTime now = user.ToLocalTime(TimeUtil.GetCurrentUtcTime());

            var days = new List<string>();
            var day = (DayOfWeekMessage) (int) now.DayOfWeek;

            while (days.Count < 7)
            {
                days.Add(day.ToString());
                day = (DayOfWeekMessage) (((int) day + 1)%7);
            }

            return string.Join(" ", days); // "Mon Tue Wed Thu Fri Sat or Sun?";
        }

        private void ProcessSignupCommand(TxtMsgInbound msgInbound, AppUser user)
        {
            if (user != null)
            {
                _omm.SendFeedbackToInboundMessage(msgInbound, "You are already signed up for mysitterhub.com", user.Id);
                return;
            }

            string msg = MessageTemplates.MySitterHubPefix() + " OK, I'll sign you up for mysitterhub.com, the easiest way to hire a sitter. Would you like to signup as a parent or a sitter?";
            _omm.SendFeedbackToInboundMessageAndAwait(msgInbound, msg, AppUser.NotSignedUpUserId, InboundMessageType.SelfSignup_Step1_ParentOrSitter);
        }

        public void ProcessPostJobCommand(TxtMsgInbound msgInbound, AppUser parentUser)
        {
            if (parentUser.UserRole != UserRole.Parent)
            {
                string msg3 = string.Format(MessageTemplates.MySitterHubPefix() + " Sorry, only parents can post jobs.");
                _omm.SendFeedbackToInboundMessage(msgInbound, msg3, parentUser.Id);
                return;
            }
            int count = new ParentRepository().GetSitterCountForParent(parentUser.Id);
            if (count == 0)
            {
                string msg2 = string.Format(MessageTemplates.MySitterHubPefix() + " Sorry, you can't post a job until you have added sitters. Say 'invite' to invite sitters.");
                _omm.SendFeedbackToInboundMessage(msgInbound, msg2, parentUser.Id);
                return;
            }

            string msg = string.Format("I'll post a job for you. Which day: {0}?", DaysOfWeekStartingToday(parentUser));
            _omm.SendFeedbackToInboundMessageAndAwait(msgInbound, msg, parentUser.Id, InboundMessageType.PostJob_Step2_DayOfWeek);
        }

        private enum NewCommandType
        {
            unrecognized, // Command not recognized
            job, // Parent post new job
            status, // Future: parent request status of sitter responses to posted job
            help, // Get command help
            invite, // Parent invite sitter to join my sitters, 
            // Future: sitter invite sitter, parent invite parent, sitter invite parent
            signup // Self-signup
        }
    }
}