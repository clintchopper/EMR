namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Compression;

    #endregion

    public abstract class TransportableObject : ITransportableObject
    {
        public TransportableObject()
        { }

        public byte[] Compress()
        {
            byte[] bytData = null;

            using (MemoryStream objMemoryStream = new MemoryStream())
            {
                Compress(objMemoryStream);
                bytData = objMemoryStream.ToArray();
            }

            byte[] bytCompressedData = bytData;
            if (bytCompressedData.Length >= CompressionThreshold)
            {
                bytCompressedData = QuickLZ.compress(bytData, 1);
            }

            return bytCompressedData;
        }

        private void Compress(Stream objStream)
        {
            using (BinaryWriterExtension objBinaryWriter = new BinaryWriterExtension(objStream))
            {
                Type objType = this.GetType();

                objBinaryWriter.Write(RemotableSignature);
                objBinaryWriter.Write(objType.Assembly.FullName);
                objBinaryWriter.Write(objType.FullName);

                this.WriteData(objBinaryWriter);
                objBinaryWriter.Flush();
            }
        }

        public virtual void ReadData(BinaryReaderExtension objBinaryReader)
        { }

        public virtual void WriteData(BinaryWriterExtension objBinaryWriter)
        { }

        #region Static Members

        public static int CompressionThreshold = 1000;
        public static byte[] RemotableSignature = new byte[] { 12, 34, 56, 73, 185, 34, 65, 211, 15, 19 };

        public static TTransportableObjectType Expand<TTransportableObjectType>(byte[] bytData)
           where TTransportableObjectType : TransportableObject
        {
            if (bytData == null)
            {
                throw new ArgumentNullException("bytData", "A valid non-null byte[] is required.");
            }

            byte[] bytDecompressedData = bytData;
            if (QuickLZ.headerLen(bytDecompressedData) == QuickLZ.DEFAULT_HEADERLEN)
            {
                bytDecompressedData = QuickLZ.decompress(bytDecompressedData);
            }

            TTransportableObjectType objTransportableObject = default(TTransportableObjectType);

            using (MemoryStream objMemoryStream = new MemoryStream(bytDecompressedData))
            {
                objTransportableObject = Expand<TTransportableObjectType>(objMemoryStream);
            }

            return objTransportableObject;
        }

        private static TTransportableObjectType Expand<TTransportableObjectType>(Stream objStream)
            where TTransportableObjectType : TransportableObject
        {
            if (objStream == null)
            {
                throw new ArgumentNullException("objStream", "A valid non-null Stream is required.");
            }

            TTransportableObjectType objTransportableObject = default(TTransportableObjectType);

            using (BinaryReaderExtension objBinaryReader = new BinaryReaderExtension(objStream))
            {
                byte[] bytSignature = objBinaryReader.ReadBytes(RemotableSignature.Length);
                if (AreByteArraysEqual(bytSignature, RemotableSignature) == false)
                {
                    throw new Exception("The binary data does not represent a valid, serialized ITransportableObject instance.");
                }

                string strAssemblyName = objBinaryReader.ReadString();
                string strTypeName = objBinaryReader.ReadString();

                objTransportableObject = FastReflectionManager.CreateInstance<TTransportableObjectType>(typeof(TTransportableObjectType), new Type[] { typeof(BinaryReaderExtension) }, objBinaryReader);
            }

            return objTransportableObject;
        }

        public static ITransportableObject Expand(byte[] bytData)
        {
            if (bytData == null)
            {
                throw new ArgumentNullException("bytData", "A valid non-null byte[] is required.");
            }

            byte[] bytDecompressedData = bytData;
            if (QuickLZ.headerLen(bytDecompressedData) == QuickLZ.DEFAULT_HEADERLEN)
            {
                bytDecompressedData = QuickLZ.decompress(bytDecompressedData);
            }

            ITransportableObject objTransportableObject = null;

            using (MemoryStream objMemoryStream = new MemoryStream(bytDecompressedData))
            {
                objTransportableObject = Expand(objMemoryStream);
            }

            return objTransportableObject;
        }

        private static ITransportableObject Expand(Stream objStream)
        {
            if (objStream == null)
            {
                throw new ArgumentNullException("objStream", "A valid non-null Stream is required.");
            }

            ITransportableObject objTransportableObject = null;
            using (BinaryReaderExtension objBinaryReader = new BinaryReaderExtension(objStream))
            {
                byte[] bytSignature = objBinaryReader.ReadBytes(RemotableSignature.Length);
                if (AreByteArraysEqual(bytSignature, RemotableSignature) == false)
                {
                    throw new Exception("The binary data does not represent a valid, serialized ITransportableObject instance.");
                }

                string strAssemblyName = objBinaryReader.ReadString();
                string strTypeName = objBinaryReader.ReadString();

                Assembly objAssembly = null;
                try
                {
                    objAssembly = Assembly.Load(strAssemblyName);
                }
                catch (Exception objException)
                {
                    string strErrorMessage = "An error was encountered while loading the assembly - Assembly.Load('" + strAssemblyName + "'):\n";
                    throw new Exception(strErrorMessage, objException);
                }

                Type objType = null;
                try
                {
                    objType = objAssembly.GetType(strTypeName, true, true);
                }
                catch (Exception objException)
                {
                    string strErrorMessage = "An error was encountered while loading the type - objAssemblyName.GetType('" + strTypeName + "', True, True):\n";
                    throw new Exception(strErrorMessage, objException);
                }

                try
                {
                    objTransportableObject = FastReflectionManager.CreateInstance<ITransportableObject>(objType, new Type[] { typeof(BinaryReaderExtension) }, objBinaryReader);
                }
                catch (Exception objException)
                {
                    string strErrorMessage = "An error was encountered while creating the object - Activator.CreateInstance('" + objType.FullName + "'):\n";
                    throw new Exception(strErrorMessage, objException);
                }
            }

            return objTransportableObject;
        }

        private static bool AreByteArraysEqual(byte[] bytArray1, byte[] bytArray2)
        {
            if (bytArray1 == bytArray2)
            {
                return true;
            }
            if ((bytArray1 == null) || (bytArray2 == null))
            {
                return false;
            }
            if (bytArray1.Length != bytArray2.Length)
            {
                return false;
            }

            for (int intIndex = 0; intIndex < bytArray1.Length; intIndex++)
            {
                if (bytArray1[intIndex] != bytArray2[intIndex])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AreByteArraysEqual(byte[] bytArray1, byte[] bytArray2, int intLength)
        {
            if (bytArray1 == bytArray2)
            {
                return true;
            }
            if ((bytArray1 == null) || (bytArray2 == null))
            {
                return false;
            }
            if ((bytArray1.Length < intLength) || (bytArray2.Length < intLength))
            {
                return false;
            }

            for (int intIndex = 0; intIndex < intLength; intIndex++)
            {
                if (bytArray1[intIndex] != bytArray2[intIndex])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
