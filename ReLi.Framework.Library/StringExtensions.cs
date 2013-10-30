namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Reflection;

    #endregion

    public static class StringExtensions
    {
        #region Static Members

        public static bool InSet(this string strValue, params string[] strTestValues)
        {
            bool blnInSet = false;

            foreach (string strTestValue in strTestValues)
            {
                if (strValue.CompareTo(strTestValue) == 0)
                {
                    blnInSet = true;
                    break;
                }
            }

            return blnInSet;
        }

        public static bool NotInSet(this string strValue, params string[] strTestValues)
        {
            bool blnNotInSet = true;

            foreach (string strTestValue in strTestValues)
            {
                if (strValue.CompareTo(strTestValue) == 0)
                {
                    blnNotInSet = false;
                    break;
                }
            }

            return blnNotInSet;
        }

        public static byte[] ToByteArray(this string strInputString)
        {
            return System.Text.Encoding.UTF8.GetBytes(strInputString);
        }

        #endregion
    }
}
