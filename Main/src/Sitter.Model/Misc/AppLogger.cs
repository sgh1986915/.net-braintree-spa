using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using log4net;
using log4net.Config;

namespace MySitterHub.Model.Misc
{
    public class AppLogger
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static AppLogger()
        {
            XmlConfigurator.Configure();
        }

        public void Log(LogLevel level, string userId, string msg )
        {
            switch (level)
            {
                case LogLevel.Info:
                    Info(userId, msg);
                    break;
                case LogLevel.Warning:
                    Warning(userId, msg);
                    break;
                case LogLevel.Error:
                    Error(userId, msg, null);
                    break;
            }
        }

        public void Info(string msg)
        {
            logger.Info(msg);
        }

        public void Info(string userId, string msg, params object[] args)
        {
            AppendUser(ref msg, ref userId);
            logger.InfoFormat(msg, args);
        }

        public void Warning(string msg)
        {
            logger.WarnFormat(msg);
        }

        public void Warning(string userId, string msg, params object[] args)
        {
            AppendUser(ref msg, ref userId);
            logger.WarnFormat(msg, args);
        }

        /// <summary>
        /// Log Error
        /// </summary>
        public string Error(string userId, string description, Exception ex)
        {
            AppendUser(ref description, ref userId);
            string tc = AppendTroubleshootingCode(ref description);
            logger.Error(description, ex);
            return tc;
        }

        /// <summary>
        ///     Format friendly error and return it. Log Error with stack info to logger
        /// </summary>
        public string LogFriendlyError(string userId, string description, Exception ex, [CallerMemberName] string memberName = "")
        {
            string errMessage = null;
            if (ex != null)
                errMessage = ex.GetBaseException().Message;

            string tc = Error(userId, description, ex);

            string friendlyError = string.Format("{0} {1},{2}{3}", description, errMessage, AppLoggerConstants.Tcode, tc);
            return friendlyError;
        }

        private string AppendTroubleshootingCode(ref string msg)
        {
            string troubleshootingCode = Guid.NewGuid().ToString().Substring(0, 6);
            msg += AppLoggerConstants.Tcode + troubleshootingCode;
            return troubleshootingCode;
        }

        private void AppendUser(ref string msg, ref string userId)
        {
            if (userId != null) msg += AppLoggerConstants.User + userId;
        }
    }

    internal class AppLoggerConstants
    {
        public const string Tcode = " TCode:";
        public const string User = " User:";
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }


}