using System;

namespace OSU_Helpers
{
    /// <summary>
    /// Date and time helpers for conversion from nvarchar data from SQL
    /// </summary>
    public static class DateTimeHelpers
    {
        /// <summary>
        /// Converts a string based date and time, with month full text month into a C# compliant DateTime
        /// </summary>
        /// <param name="DateTimeString">Input date time string such as: December 13 2018 09:51:54:199</param>
        /// <returns>Input as DateTime variable</returns>
        public static DateTime ConvertStringToDateTime(string DateTimeString)
        {
            DateTimeString = DateTimeString.Trim();
            int year, month, day, hour, minute, second, ms = 0;
            //[0]December [1]13 [2]2018 [3]09:51:54:199
            string[] s = DateTimeString.Split(' ');
            string[] t = s[3].Split(':');
            //[0]09 [1]51 [2]54 [3]199
            month = MonthFromString(s[0]);
            int.TryParse(s[2], out year);
            int.TryParse(s[1], out day);
            int.TryParse(t[0], out hour);
            int.TryParse(t[1], out minute);
            int.TryParse(t[2], out second);
            if (t.Length == 4)
            {
                int.TryParse(t[3], out ms);
            }
            return new DateTime(year, month, day, hour, minute, second, ms);
        }

        
        /// <summary>
        /// Convert from text month to integer month
        /// </summary>
        /// <param name="m">Input text month </param>
        /// <returns>Integer month 1-12</returns>
        private static int MonthFromString(string m)
        {
            switch (m)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
                default: return -1;
            }
        }

        /// <summary>
        /// Convert string based date with specified separator to C# compliant DateTime
        /// </summary>
        /// <param name="date">String date with hyphens as separator </param>
        /// <param name="separator">Separator character for date. Typically - or /</param>
        /// <returns>Date in DateTime format</returns>
        public static DateTime ConverToShortDateFromString(string date, char separator)
        {
            string[] array = date.Split(separator);
            return new DateTime(int.Parse(array[2]), int.Parse(array[0]), int.Parse(array[1]));
        }

        /// <summary>
        /// Returns C# compliant date time from the input date time string.
        /// </summary>
        /// <param name="DateTimeInput">Input date time in format 12/1/2019 1:00PM</param>
        /// <returns>Input as DateTime variable type or 1/1/1900 if input was in incorrect format.</returns>
        public static DateTime ConvertToDateTimeFromString(string DateTimeInput)
        {
            if (DateTimeInput != null && !(DateTimeInput == string.Empty))
            {
                string[] array = DateTimeInput.Split(' ');
                string[] array2 = array[0].Split('/');
                string[] array3 = array[1].Split(':');
                if (array[2] == "PM" && array3[0] != "12")
                {
                    array3[0] = (int.Parse(array3[0]) + 12).ToString();
                }
                return new DateTime(int.Parse(array2[2]), int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array3[0]), int.Parse(array3[1]), int.Parse(array3[2]));
            }
            return new DateTime(1900, 1, 1);
        }

        /// <summary>
        /// Returns the integer quarter of a specified date
        /// </summary>
        /// <param name="date">Input date as DateTime</param>
        /// <returns>Quarter as integer</returns>
        public static int Quarter(DateTime date)
        {
            if (date.Month >= 1 && date.Month < 4)
            {
                return 1;
            }
            else if (date.Month >= 4 && date.Month < 7)
            {
                return 2;

            }
            else if (date.Month >= 7 && date.Month < 10)
            {
                return 3;
            }
            else
            {
               return 4;
            }
        }

        /// <summary>
        /// Returns quarter integer as string type
        /// </summary>
        /// <param name="input">Input date for quarter as DateTime</param>
        /// <returns>Quarter integer as string type</returns>
        public static string QuarterAsString(DateTime input) => Quarter(input).ToString();

    }
}
