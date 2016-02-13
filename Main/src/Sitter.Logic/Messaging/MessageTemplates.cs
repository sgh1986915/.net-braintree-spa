using System;
using MySitterHub.Model.Core;

namespace MySitterHub.Logic.Messaging
{
    public class MessageTemplates
    {
        public static string JobAvailable(string parent, DateTime startDateTime, DateTime endDateTime, decimal rate)
        {
            return string.Format(@"{0} has invited you babysit on {1} from {2} - {3} for {4:C}/hr. Reply 'yes' to accept or 'no' to decline. Or see mysitterhub.com for more details.",
                parent, startDateTime.ToString("dddd, MMMM d"), startDateTime.ToString("h:mm tt"), endDateTime.ToString("h:mm tt"), rate);
        }

        public static string FormatJobUrl(int jobId)
        {
            return string.Format("http://mysitterhub.com/job/" + jobId);
        }

        public static string FormatNotAwaitingMessagesFromYou()
        {
            return string.Format("You have sent a message to the mysitterhub.com robot. However I am not expecting any messages from you at this time");
        }

        public static string FormatSitterAcceptJob()
        {
            return string.Format( "Congratulations! You are now scheduled for this job. Please login to your account at mysitterhub.com for more details.");
        }

        public static string FormatSitterDeclineJob(string parentName)
        {
            return string.Format(string.Format("You have declined the job invite from {0}", parentName));
        }

        public static string FormatSitterInvalidResponseToJobOrSignup(bool isJobInvite)
        {
            string inviteType = isJobInvite ? "job" : "parent";
            return string.Format("Invalid response to {0} invite. Please say 'yes' or 'no'.", inviteType);
        }

        /*
        * We send it to "sitter" when parent invites him to his private sitterhub.
        */
        public static string FormatParentInviteSitter(string parent)
        {
            return string.Format("{0} has invited you to join her group of babysitters on mysitterhub.com. Reply 'yes' to accept or 'no' to decline.", parent);
        }

        /*
         * We send it to "sitter" that replies 'yes' on parent's sitterhub invitation but he has parent role.
         */
        public static string FormatInvitedUserIsNotASitter()
        {
            return string.Format("Invited user is not a sitter role, unable to add to MySitters");            

        }

        public static string FormatOkAddedToParentMySitters(Parent parent, bool isAdded)
        {
            return string.Format("OK, you have {0} been added to {1}'s sitters.", isAdded ? "" : "not",
                (parent == null || parent.User == null ? "parent" : parent.User.FirstNameLastInitial()));
        }

        /*
         * We send it to sitter that replies 'yes' on parent's sitterhub invitation and new account was created for him.
         */
        public static string FormatUserAddedToSittersAndProfileCreated(Parent parent, string password)
        {
            string sweet = FormatUserAddedToSitters(parent);
            return sweet + string.Format(" An account has been set up for you. Your temporary password is {1}. Go to mysitterhub.com/profile to complete your profile.",
                (parent == null || parent.User == null ? "parent" : parent.User.FirstNameLastInitial()), password);
        }

        /*
         * We send it to sitter that replies 'yes' on parent's sitterhub invitation.
         */
        public static string FormatUserAddedToSitters(Parent parent)
        {
            return string.Format("Sweet! {0} can now invite you to babysit.",
                (parent == null || parent.User == null ? "parent" : parent.User.FirstNameLastInitial()));
        }

        /*
         * We send it to sitter that replies 'no' on parent's sitterhub invitation.
         */
        public static string FormatDeclineInvitationToSitterhub(Parent parent)
        {
            return string.Format("OK, we'll let {0} know you have declined the job.",
                (parent == null || parent.User == null ? "parent" : parent.User.FirstNameLastInitial()));
        }

        public static string FormatSendParentNoticeOfSitterAccept(string sitterName)
        {
            return string.Format("Congratulations! {0} is now scheduled for this job. For more details, login to your account at mysitterhub.com.", sitterName);
        }

        public static string FormatSendParentNoticeOfSitterDecline(string sitterName, bool allSittersDeclined)
        {
            string all = allSittersDeclined ? ". Sorry, all sitters have declined. You can repost your job to invite more sitters." : "";
            return string.Format("{0} declined job" + all, sitterName);
        }

        public static string FormatSendParentNoticeOfSitterRenege(string sitterName, DateTime date)
        {
            return string.Format("Sory for the inconvenience, {0} cancelled the job for {1:g}", sitterName, date); 
        }

        public static string FormatSendOtherSittersNoticeOfJobClose(string parentName, DateTime date)
        {
            return string.Format("Sorry, the job posting from {0} for {1} is already filled.", parentName, date.DayOfWeek);
        }

        public static string FormatSitterHasDeclinedThisJob(string sitterName)
        {
            return string.Format("{0} has declined this job.", sitterName); //TODO
        }

        public static string OnlyParentsCanSendSignupInvites()
        {
            return "Only parents can send signup invitations.";
        }

        public static string FormatSendPasswordToUserWhoForgotPassword(string code)
        {
            return string.Format("Your code is {0}. Don't tell anybody this code for security reasons.", code);
        }

        public static string FormatSitterRequestPayment(string sitterName, string jobDate)
        {
            return string.Format("{0} has requested payment for the job on {1}. Login to your account at mysitterhub.com to send payment.", sitterName, jobDate);
        }

        public static string FormatValidCommands(UserRole role)
        {
            string ret = "valid commands:\n";
            if (role == UserRole.Parent)
            {
                ret += "'invite' a babysitter to signup\n";
                ret += "'job' post a job\n";
            }
            ret += "'status' list jobs\n";
            ret += "'help'\n";

            return ret;
        }

        public static string MySitterHubPefix()
        {
            return "mysitterhub.com";
        }

    }
}