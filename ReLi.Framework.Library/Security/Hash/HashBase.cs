namespace ReLi.Framework.Library.Security.Hash
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;

    #endregion

	public abstract class HashBase
    {
        public abstract int Size
        {
            get;
        }

        public string ComputeHash(string strInput)
        {
            byte[] objBytes = Encoding.UTF8.GetBytes(strInput);
            byte[] objOutput;
            ComputeHash(objBytes, out objOutput);

            return ConversionHelper.ByteArrayToBase16String(objOutput);
        }

        public void ComputeHash(Stream objInputStream, out string strOutputData)
        {
            if (objInputStream == null)
            {
                throw new ArgumentNullException("objInputStream", "The parameter does not represent a valid stream.");
            }

            byte[] bytOutputData;
            ComputeHash(objInputStream, out bytOutputData);

            strOutputData = ConversionHelper.ByteArrayToBase16String(bytOutputData);
        }

        public void ComputeHash(byte[] bytInputData, out string strOutputData)
        {
            byte[] bytOutputData;
            ComputeHash(bytInputData, out bytOutputData);

            strOutputData = ConversionHelper.ByteArrayToBase16String(bytOutputData);

        }

        public abstract void ComputeHash(byte[] bytInputData, out byte[] bytOutputData);

        public abstract void ComputeHash(Stream objInputData, out byte[] bytOutputData);

        #region Static Members 

        public static HashBase Instance
        {
            get
            {
                return Default;
            }
        }

        public static HashBase Default
        {
            get
            {
                HashBase objHash = new SHA1Hash();
                return objHash;
            }
        }

        public static HashBase Sha1
        {
            get
            {
                HashBase objHash = new SHA1Hash();
                return objHash;
            }
        }

        public static HashBase Sha256
        {
            get
            {
                HashBase objHash = new SHA256Hash();
                return objHash;
            }
        }

        public static HashBase Sha384
        {
            get
            {
                HashBase objHash = new SHA384Hash();
                return objHash;
            }
        }

        public static HashBase Sha512
        {
            get
            {
                HashBase objHash = new SHA512Hash();
                return objHash;
            }
        }

        #endregion

    }
}
