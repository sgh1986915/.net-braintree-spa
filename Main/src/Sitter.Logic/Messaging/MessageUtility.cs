using System;
using System.Linq;
using System.Text.RegularExpressions;
using MySitterHub.Logic.Repository;
using MySitterHub.Logic.ServiceModels;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.Logic.Messaging
{
    public class MessageUtility
    {
        public static MessageAffirmativeType ParseAffirmativeType(TxtMsgInbound msgInbound, bool cancelAsNo = false)
        {
            if (msgInbound.Message == "yes" || msgInbound.Message == "y")
            {
                return MessageAffirmativeType.yes;
            }
            if (msgInbound.Message == "no" || msgInbound.Message == "n" || (cancelAsNo && msgInbound.Message == "cancel"))
            {
                return MessageAffirmativeType.no;
            }
            return MessageAffirmativeType.Unknown;
        }

        public static bool IsCancelRequested(TxtMsgInbound msgInbound)
        {
            if (msgInbound.Message == "cancel")
            {
                return true;
            }
            return false;
        }

        public static string FormatJobConfirm(PostJobViaTxtAnswers postJobAnswers, AppUser parentUser, ParentRepository parentRepository)
        {
            DateTime jobStartDateTime = CalcJobStartTime(postJobAnswers);
            string sitters = ListSittersForParent(parentUser.Id, parentRepository);
            if (sitters == null)
                throw new Exception("Parent does not have any sitters, unable to post job");

            return string.Format(@"Post Job for {0} at {1} to {2}? Say 'yes' to post or 'cancel'", jobStartDateTime.ToString("dddd MMM dd"), jobStartDateTime.ToString("h:mm tt"), sitters);
        }

        public static DateTime CalcJobStartTime(PostJobViaTxtAnswers pa)
        {
            var now = TimeUtil.GetCurrentUtcTime();

            int addDays = (int)pa.DayOfWeek - (int)now.DayOfWeek;
            if ((int)pa.DayOfWeek < (int)now.DayOfWeek)
            {
                addDays += 7;
            }

            DateTime jobStartDateTime = now.Date.AddHours(pa.StartHour.Hour).AddDays(addDays);
            return jobStartDateTime;
        }

        public static string ListSittersForParent(int parentId, ParentRepository parentRepository)
        {
            ParentMySittersDataSM parentMySitters = parentRepository.GetSittersForParent(parentId);
            if (parentMySitters == null || parentMySitters.Sitters == null || parentMySitters.Sitters.Count == 0)
            {
                return null;
            }
            string ret = string.Join(", ", parentMySitters.Sitters.Select(m => m.FirstName));
            return ret;
        }

        public static JobTimeSmall ParseStartTime(string message, AppLogger log)
        {
            try
            {
                string pattern = "^([0-9]|1[0-9]|2[0-3])(:00|:30)?(pm|am)$";
                Match match = Regex.Match(message, pattern);
                if (match.Success && (match.Groups.Count == 3 || match.Groups.Count == 4))
                {
                    var time = new JobTimeSmall();
                    time.Hour = int.Parse(match.Groups[1].Value);
                    if (match.Groups[2].Value == ":30")
                    {
                        time.Hour += .5f;
                    }
                    string amPm = match.Groups[match.Groups.Count - 1].Value;
                    if (amPm == "pm")
                    {
                        time.Hour += 12;
                    }

                    return time;
                }
                return null;
            }
            catch (Exception ex)
            {
                log.Error("", "Error while parsing StartTime from message " + message, ex);
                return null;
            }
        }

        public static DayOfWeekMessage? ParseDayOfWeek(string message)
        {
            // STEP - Clean
            message = message.Replace(".", "");

            // STEP - Parse
            DayOfWeekMessage dayOfWeek;
            if (Enum.TryParse(message, out dayOfWeek))
            {
                return dayOfWeek;
            }
            else
            {
                return null;
            }
        }
    }
}