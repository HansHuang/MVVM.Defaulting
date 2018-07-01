using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MVVM.Defaulting.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Split string by pascal case patten
        /// (e.g. "GetABCTicket" will retuen "Get ABC Ticket")
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static string SplitPascalCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return Regex.Replace(input, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1").Trim();
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool HasText(this string value) 
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Retrieves the value of the current string, or the default value when string is empty/null.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetValueOrDefault(this string source, string defaultValue = "")
        {
            return string.IsNullOrWhiteSpace(source) ? defaultValue : source;
        }

        public static string XmlEncode(this string str)
        {
            return System.Security.SecurityElement.Escape(str);
        }

        public static string XmlDecode(this string str)
        {
            //https://msdn.microsoft.com/en-us/library/system.security.securityelement.escape(v=vs.110).aspx
            return str.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&amp;", "&");
        }

    }
}
