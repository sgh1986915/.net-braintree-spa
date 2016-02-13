using System.Collections.Generic;

namespace MySitterHub.AppService.Sms
{
    public class MessagingConstants
    {
        internal static readonly HashSet<string> Whitelist = new HashSet<string>
        {
            // "+79137110364", // Artem
            "+15129219530", // Joseph
            "+15127514094", // Sarah
            "+15125219854" // testingiPhone 5 black 
        };
    }
}