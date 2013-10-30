namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;

    #endregion
        
	public class ConversionHelper
    {
        private static readonly int _intChunkSize = 1024;

        private ConversionHelper()
        {}

        public static int ChunkSize
        {
            get
            {
                return _intChunkSize;
            }
        }

        public static string StreamToString(Stream objStream)
        {
            string strValue = string.Empty;

            using (StreamReader objReader = new StreamReader(objStream))
            {
                strValue = objReader.ReadToEnd();
            }

            return strValue;
        }

        public static string ByteArrayToBase16String(byte[] bytInputData)
        {
            StringBuilder objStringConverter = new StringBuilder(bytInputData.Length);
            foreach (byte bytData in bytInputData)
            {
                objStringConverter.Append(bytData.ToString("X2"));
            }

            return objStringConverter.ToString();
        }

        public static byte[] Base16StringToByteArray(string strBase16String)
        {
            int intTotalBytes = (strBase16String.Length) / 2;
            byte[] arrBytes = new byte[intTotalBytes];

            for (int intIndex = 0; intIndex < intTotalBytes; intIndex++)
            {
                arrBytes[intIndex] = Convert.ToByte(strBase16String.Substring(intIndex * 2, 2), 16);
            }

            return arrBytes;
        }

        public static byte[] StreamToByteArray(Stream objInputStream)
        {
            byte[] bytResults = null;

            using (MemoryStream objMemoryStream = new MemoryStream())
            {         
                int intBytesRead;
                byte[] bytBuffer = new byte[32768];

                while ((intBytesRead = objInputStream.Read(bytBuffer, 0, bytBuffer.Length)) > 0)
                {
                    objMemoryStream.Write(bytBuffer, 0, intBytesRead);
                }

                bytResults = objMemoryStream.ToArray();
            }

            return bytResults;
        }

        public static string[] ConvertIntArrayToStringArray(int[] intValues)
        {
            string[] strValues = Array.ConvertAll<int, string>(intValues, new Converter<int, string>(Convert.ToString));
            return strValues;
        }

        public static int[] ConvertEnumArrayToStringArray<TEnumType>(TEnumType[] enuValues)
        {
            int[] intValues = Array.ConvertAll<TEnumType, int>(enuValues, delegate(TEnumType enuValue) { return Convert.ToInt32(enuValue); });
            return intValues;
        }

        public static int[] ConvertStringArrayToIntArray(string[] strValues)
        {
            int[] intValues = Array.ConvertAll<string, int>(strValues, new Converter<string, int>(Convert.ToInt32));
            return intValues;
        }

        public static string ToBase64String(string strValue)
        {
            byte[] bytData = Encoding.UTF8.GetBytes(strValue);
            return Convert.ToBase64String(bytData);
        }

        public static string FromBase64String(string strValue)
        {
            byte[] bytData = Convert.FromBase64String(strValue);
            return Encoding.UTF8.GetString(bytData);
        }

        public static byte[] ToByteArray(Stream objInputStream)
        {
            byte[] bytData = new byte[16 * 1024];
            using (MemoryStream objMemoryStream = new MemoryStream())
            {
                int intBytesRead;
                while ((intBytesRead = objInputStream.Read(bytData, 0, bytData.Length)) > 0)
                {
                    objMemoryStream.Write(bytData, 0, intBytesRead);
                }
                return objMemoryStream.ToArray();
            }
        }

        public static byte[] ToByteArray(string strInputString)
        {
            return System.Text.Encoding.UTF8.GetBytes(strInputString);
        }
    }
}
