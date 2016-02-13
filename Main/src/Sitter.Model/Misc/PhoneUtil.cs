using System.Text.RegularExpressions;

namespace MySitterHub.Model.Misc
{
    public class PhoneUtil
    {
        public const string PhoneRegex = @"^\+\d{1,3}\d{10}$";

        private static Regex _phoneFullRegex = new Regex(PhoneRegex);
        private static Regex _phoneDigitsOnlyRegex = new Regex(@"^\d{10}$");

        public static bool IsValidPhoneNumber(string phone)
        {
            bool valid = _phoneFullRegex.IsMatch(phone);
            return valid;
        }

        /*
         * Seems like this function needed just to try make phone valid even if something wrong e.g. with country code. 
         */
        public static string CleanAndEnsureCountryCode(string phone)
        {
            // If we have valid phone - great.
            if (_phoneFullRegex.IsMatch(phone))
            {
                return phone;

            }
            // OK let's check: maybe phone is without code but it is still valid?
            else if (_phoneDigitsOnlyRegex.IsMatch(phone))
            {
                return "+1" + phone;
            }
            // ...Otherwise we have invalid phone.
            else
            {
                return phone;
            }
        }
    }
}