namespace ReLi.Framework.Library.Security.Encryption
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Security.Cryptography;

    #endregion

	public class TripleDesEncryption : EncryptionBase
    {
        string _strPassword = null;

        public TripleDesEncryption()
        {
            _strPassword = base.Password;
        }

        public TripleDesEncryption(string strPassword)
        {
            if ((strPassword == null) || (strPassword.Length == 0))
            {
                throw new ArgumentOutOfRangeException("Password", "A valid non-null, non-empty string is required.");
            }

            _strPassword = strPassword;
        }

        public override int KeySize
        {
            get
            {
                int intKeySize = 192;
                return intKeySize;
            }
        }

        protected override string Password
        {
            get
            {
                return _strPassword;
            }
        }

        protected override ICryptoTransform CreateEncryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode)
        {
            TripleDESCryptoServiceProvider objProvider = new TripleDESCryptoServiceProvider();
            objProvider.Mode = enuCipherMode;

            ICryptoTransform objTransformation = objProvider.CreateEncryptor(bytKeyBytes, bytVectorBytes);
            return objTransformation;
        }

        protected override ICryptoTransform CreateDecryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode)
        {
            TripleDESCryptoServiceProvider objProvider = new TripleDESCryptoServiceProvider();
            objProvider.Mode = enuCipherMode;

            ICryptoTransform objTransformation = objProvider.CreateDecryptor(bytKeyBytes, bytVectorBytes);
            return objTransformation;
        }
    }
}
