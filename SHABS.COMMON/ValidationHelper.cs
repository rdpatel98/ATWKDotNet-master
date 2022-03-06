using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SHABS.COMMON
{
    public class ValidationHelper
    {
        public static string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        public static string alphabetMatchingPattern = @"^[a-zA-Z & .]+$";
        public static string numberMatchingPattern = @"^[0-9]+$";

        public static bool IsNull<TType>(TType value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsValidAlphablets(string alphabets)
        {
            if (IsNull(alphabets))
            {
                return false;
            }
            else if (Regex.IsMatch(alphabets, alphabetMatchingPattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsValidNumber(string numbers)
        {
            if (IsNull(numbers))
            {
                return false;
            }
            else if (Regex.IsMatch(numbers, numberMatchingPattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsEmail(string email)
        {

            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            else if (Regex.IsMatch(email, MatchEmailPattern))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
