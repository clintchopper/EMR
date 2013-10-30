namespace ReLi.Framework.Library.Security.Encryption
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    #endregion

	public abstract class EncryptionBase
    {
        private readonly CipherMode _enuCipherMode = CipherMode.CBC;
        private readonly int _intChunkSize = 1024;
        private readonly int _intPasswordIterations = 2;
        private readonly string _strSalt = ")0(9_-+=";
        private readonly string _strVector = "alskdjfhgqpwoeir";
        private readonly string _strPassword = "!1@2#3$4%5^6&7*8(9";
        private readonly string _strHashAlgorithm = "SHA1";

        public abstract int KeySize
        {
            get;
        }

        protected CipherMode CipherMode
        {
            get
            {
                return _enuCipherMode;
            }
        }

        protected Int32 ChunkSize
        {
            get
            {
                return _intChunkSize;
            }
        }

        protected string HashAlgorithm
        {
            get
            {
                return _strHashAlgorithm;
            }
        }

        protected Int32 PasswordIterations
        {
            get
            {
                return _intPasswordIterations;
            }
        }

        protected virtual string Password
        {
            get
            {
                return _strPassword;
            }
        }
        
        protected string Salt
        {
            get
            {
                return _strSalt;
            }
        }

        protected byte[] SaltBytes
        {
            get
            {
                return Encoding.ASCII.GetBytes(this.Salt);
            }
        }

        protected string Vector
        {
            get
            {
                return _strVector;
            }
        }

        protected byte[] VectorBytes
        {
            get
            {
                return Encoding.ASCII.GetBytes(this.Vector);
            }
        }

        protected abstract ICryptoTransform CreateEncryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode);

        protected abstract ICryptoTransform CreateDecryptor(byte[] bytKeyBytes, byte[] bytVectorBytes, CipherMode enuCipherMode);

        public MemoryStream Encrypt(Stream objInput)
        {
            objInput.Position = 0;
            byte[] objBytes = new byte[objInput.Length];
            objInput.Read(objBytes, 0, objBytes.Length);

            byte[] objEncryptedBytes = Encrypt(objBytes);
            return new MemoryStream(objEncryptedBytes);
        }

        public string Encrypt(string strInput)
        {
            string strEncryptedString = string.Empty;

            if ((strInput != null) && (strInput.Length > 0))
            {
                byte[] objBytes = Encoding.UTF8.GetBytes(strInput);
                byte[] objEncryptedBytes = Encrypt(objBytes);

                strEncryptedString = Convert.ToBase64String(objEncryptedBytes);
            }

            return strEncryptedString;
        }

        public byte[] Encrypt(byte[] bytInput)
        {
            MemoryStream objMemoryStream = new MemoryStream();

            PasswordDeriveBytes objPasswordBytes = new PasswordDeriveBytes(this.Password, this.SaltBytes, this.HashAlgorithm, this.PasswordIterations);
            byte[] bytKeyBytes = objPasswordBytes.GetBytes(this.KeySize / 8);

            ICryptoTransform objTransform = this.CreateEncryptor(bytKeyBytes, this.VectorBytes, this.CipherMode);
            CryptoStream objEncryptionStream = new CryptoStream(objMemoryStream, objTransform, CryptoStreamMode.Write);

            objEncryptionStream.Write(bytInput, 0, bytInput.Length);
            objEncryptionStream.FlushFinalBlock();

            byte[] bytResults = objMemoryStream.ToArray();

            objMemoryStream.Close();
            objEncryptionStream.Close();

            return bytResults;
        }

        public MemoryStream Decrypt(Stream objInput)
        {
            UTF8Encoding objEncoding = new UTF8Encoding();

            objInput.Position = 0;
            byte[] objBytes = new byte[objInput.Length];
            objInput.Read(objBytes, 0, objBytes.Length);

            byte[] objEncryptedBytes = Decrypt(objBytes);
            return new MemoryStream(objEncryptedBytes);
        }

        public string Decrypt(string strInput)
        {
            string strDecryptedString = string.Empty;

            if ((strInput != null) && (strInput.Length > 0))
            {
                byte[] objBytes = Convert.FromBase64String(strInput);
                byte[] objDecryptedBytes = Decrypt(objBytes);

                strDecryptedString = Encoding.UTF8.GetString(objDecryptedBytes);
            }

            return strDecryptedString;
        }

        public byte[] Decrypt(byte[] bytInput)
        {
            MemoryStream objMemoryStream = new MemoryStream();

            PasswordDeriveBytes objPasswordBytes = new PasswordDeriveBytes(this.Password, this.SaltBytes, this.HashAlgorithm, this.PasswordIterations);
            byte[] bytKeyBytes = objPasswordBytes.GetBytes(this.KeySize / 8);

            ICryptoTransform objTransform = this.CreateDecryptor(bytKeyBytes, this.VectorBytes, this.CipherMode);
            CryptoStream objDecryptionStream = new CryptoStream(objMemoryStream, objTransform, CryptoStreamMode.Write);

            objDecryptionStream.Write(bytInput, 0, bytInput.Length);
            objDecryptionStream.FlushFinalBlock();

            byte[] bytResults = objMemoryStream.ToArray();

            objMemoryStream.Close();
            objDecryptionStream.Close();

            return bytResults;
        }

        #region Static Members

        public static EncryptionBase Instance
        {
            get
            {
                return Default;
            }
        }

        public static EncryptionBase Default
        {
            get
            {
                EncryptionBase objEncryption = new RijndaelEncryption();
                return objEncryption;
            }
        }

        public static EncryptionBase Rijndael
        {
            get
            {
                EncryptionBase objEncryption = new RijndaelEncryption();
                return objEncryption;
            }
        }

        public static EncryptionBase Rijndael128
        {
            get
            {
                EncryptionBase objEncryption = new Rijndael128Encryption();
                return objEncryption;
            }
        }

        public static EncryptionBase DES
        {
            get
            {
                EncryptionBase objEncryption = new DESEncryption();
                return objEncryption;
            }
        }

        public static EncryptionBase TripleDES
        {
            get
            {
                EncryptionBase objEncryption = new TripleDesEncryption();
                return objEncryption;
            }
        }

        #endregion

    }
}
