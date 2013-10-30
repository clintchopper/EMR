namespace ReLi.Framework.Library.Security.Encryption
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;

    #endregion

    public class Rijndael128Encryption : EncryptionBase
    {
        public override int KeySize
        {
            get
            {
                int intKeySize = 128;
                return intKeySize;
            }
        }

        protected override ICryptoTransform CreateEncryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode)
        {
            RijndaelManaged objProvider = new RijndaelManaged();
            objProvider.Mode = enuCipherMode;

            ICryptoTransform objTransformation = objProvider.CreateEncryptor(bytKeyBytes, bytVectorBytes);
            return objTransformation;
        }

        protected override ICryptoTransform CreateDecryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode)
        {
            RijndaelManaged objProvider = new RijndaelManaged();
            objProvider.Mode = enuCipherMode;

            ICryptoTransform objTransformation = objProvider.CreateDecryptor(bytKeyBytes, bytVectorBytes);
            return objTransformation;
        }
    }
}
