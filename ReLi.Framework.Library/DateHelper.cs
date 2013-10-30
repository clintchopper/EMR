namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;

    #endregion

    public class DateHelper
    {
        #region Static Members 

        public static string ToUtcString(DateTime objDateTime)
        {
            DateTime objUtcTime = objDateTime.ToUniversalTime();

            int intHours = TimeZone.CurrentTimeZone.GetUtcOffset(objDateTime).Hours;
            intHours *= ((intHours < 0) ? -1 : 1);

            string strOffset = string.Format("{0}{1}", ((intHours > 0) ? "+" : ""), intHours.ToString("0"));

            string strUtcString = objUtcTime.ToString("s") + "." + strOffset + "Z";
            return strUtcString;
        }

        public static bool IsValidDate(string strDateTime)
        {
            bool blnIsValidDate = false;

            if ((strDateTime != null) && (strDateTime.Length > 0))
            {
                DateTime objDateTime;
                blnIsValidDate = DateTime.TryParse(strDateTime, out objDateTime);
            }

            return blnIsValidDate;
        }

        public static string FormatDate(string strDateTime, string strInputFormat, string strOutputFormat)
        {
            string strFormattedDate = string.Empty;
            
            if ((strDateTime != null) && (strDateTime.Length > 0))
            {
                DateTime objDateTime;
                bool blnIsValidDate = DateTime.TryParseExact(strDateTime, strInputFormat, null, System.Globalization.DateTimeStyles.None, out objDateTime);
                if (blnIsValidDate == true)
                {
                    strFormattedDate = objDateTime.ToString(strOutputFormat);
                }
            }

            return strFormattedDate;
        }

        #endregion
    }
}
