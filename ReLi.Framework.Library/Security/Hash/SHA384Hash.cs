namespace ReLi.Framework.Library.Security.Hash
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    public class SHA384Hash : HashBase
    {
        private SHA384Managed _objHashObject;

        public SHA384Hash()
        {
            _objHashObject = new SHA384Managed();
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
                int intSize = 384;
                return intSize;
            }
        }
    }
}
