using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSU_Helpers
{
    /// <summary>
    /// String helper classes
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Convert text to Title Case
        /// </summary>
        /// <param name="s">Input string to convert</param>
        /// <returns>String converted to Title Case</returns>
        public static string ToTitleCase(this string s)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
        }

        /// <summary>
        /// Gets the number characters from the left side of the string
        /// </summary>
        /// <param name="input_string">Input string value</param>
        /// <param name="length">Number of characters to pull from input string</param>
        /// <returns>The defined number of characters from the input string as a string type</returns>
        public static string Left(this string input_string, int length)
        {
            if (string.IsNullOrEmpty(input_string))
            {
                return input_string;
            }

            length = Math.Abs(length);

            return (input_string.Length <= length
                   ? input_string
                   : input_string.Substring(0, length)
                   );
        }
    }
}
