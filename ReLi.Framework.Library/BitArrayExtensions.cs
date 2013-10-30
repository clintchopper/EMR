namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    #endregion

    public static class BitArrayExtensions
    {
        #region Static Members

        public static Byte GetByte(this BitArray objBitArray)
        {
            Byte bytByte = 0;

            for (int intIndex = 7; intIndex >= 0; intIndex--)
            {
                bytByte = (byte)((bytByte << 1) | (objBitArray[intIndex] ? 1 : 0));
            }

            return bytByte;
        }

        public static Byte[] GetBytes(this BitArray objBitArray)
        {
            int intNumberOfBytes = objBitArray.Length / 8;

            if (objBitArray.Length % 8 != 0)
            {
                intNumberOfBytes += 1;
            }

            byte[] objBytes = new byte[intNumberOfBytes];
            objBitArray.CopyTo(objBytes, 0);
            
			return objBytes;
        }

        #endregion
    }
}
