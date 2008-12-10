using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ISXBotHelper
{
    internal static class clsExtensions
    {
        #region Strings

        /// <summary>
        /// Convert to a boolean
        /// </summary>
        public static bool ConvertToBool(this string strVal, bool DefaultVal)
        {
            if (string.IsNullOrEmpty(strVal))
                return DefaultVal;

            // try to convert
            bool bRes;
            if (bool.TryParse(strVal, out bRes))
                return bRes;
            else
                return DefaultVal;
        }

        /// <summary>
        /// Convert to an int
        /// </summary>
        public static int ConvertToInt(this string strVal, int DefaultVal)
        {
            return IsNumeric(strVal) ? Convert.ToInt32(strVal) : DefaultVal;
        }

        /// <summary>
        /// returns true ONLY if the entire string is numeric
        /// </summary>
        public static bool IsNumeric(this string input)
        {
            // return false if no string
            if (string.IsNullOrEmpty(input))
                return false;

            //Valid user input
            return new Regex(@"^[0-9]*\.?[0-9]+$").IsMatch(input.Trim());
        }

        // Strings
        #endregion
    }
}
