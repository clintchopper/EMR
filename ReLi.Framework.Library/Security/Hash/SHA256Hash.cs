namespace ReLi.Framework.Library.Security.Hash
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    public class SHA256Hash : HashBase
    {
        private SHA256Managed _objHashObject;

        public SHA256Hash()
        {
            _objHashObject = new SHA256Managed();
        }

        public override void ComputeHash(byte[] bytInputData, out byte[] bytOutputData)
        {
            bytOutputData = _objHashObject.ComputeHash(bytInputData);
        }

        public override void ComputeHash(Stream objInputStream, out byte[] bytOutputData)
        {
            if (objInputStream == null)
            {
                throw new ArgumentNullException("objInputStream", "The parameter does not represent a valid stream.");
            }

            using (BufferedStream objBufferedStream = new BufferedStream(objInputStream, 65536))
            {
                bytOutputData = _objHashObject.ComputeHash(objBufferedStream);
            }
        }

        public override int Size
        {
            get
            {
                int intSize = 256;
                return intSize;
            }
        }
    }
}
