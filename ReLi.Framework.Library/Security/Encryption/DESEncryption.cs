namespace ReLi.Framework.Library.Security.Encryption
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;

    #endregion

	public class DESEncryption : EncryptionBase
    {
        public override int KeySize
        {
            get
            {
                int intKeySize = 64;
                return intKeySize;
            }
        }

        protected override ICryptoTransform CreateEncryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode)
        {
            DESCryptoServiceProvider objProvider = new DESCryptoServiceProvider();
            objProvider.Mode = enuCipherMode;

            ICryptoTransform objTransformation = objProvider.CreateEncryptor(bytKeyBytes, bytVectorBytes);
            return objTransformation;
        }

        protected override ICryptoTransform CreateDecryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode)
        {
            DESCryptoServiceProvider objProvider = new DESCryptoServiceProvider();
            objProvider.Mode = enuCipherMode;

            ICryptoTransform objTransformation = objProvider.CreateDecryptor(bytKeyBytes, bytVectorBytes);
            return objTransformation;
        }
    }
}
