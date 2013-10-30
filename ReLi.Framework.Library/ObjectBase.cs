namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.ComponentModel;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Remoting;
    using ReLi.Framework.Library.XmlLite;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public class ObjectBase : TransportableObject, IObjectBase
    {
        private bool _blnInitializing;

        protected ObjectBase()
            : base()
        {
            Initialize();
        }

        public ObjectBase(SerializedObject objSerializedObject)
            : this()
        {
            BeginInit();
            ReadData(objSerializedObject);
            EndInit();
        }

        public ObjectBase(BinaryReaderExtension objBinaryReader)
            : this()
        {
            BeginInit();
            ReadData(objBinaryReader);
            EndInit();
        }

        protected virtual void Initialize()
        {
            Initializing = false;
        }

        #region IObjectBase Members

        public bool Initializing
        {
            get
            {
                return _blnInitializing;
            }
            private set
            {
                _blnInitializing = value;
            }
        }

        public virtual void BeginInit()
        {
            Initializing = true;
        }

        public virtual void EndInit()
        {
            Initializing = false;
        }

        public void CopyTo(IObjectBase objDestinationObject)
        {
            if (objDestinationObject == null)
            {
                throw new ArgumentNullException("objDestinationObject", "A valid non-null IObjectBase is required..");
            }

            string strSourceTypeName = this.GetType().Name;
            string strDestinationTypeName = objDestinationObject.GetType().Name;

            if (strSourceTypeName != strDestinationTypeName)
            {
                throw new ArgumentOutOfRangeException("objDestinationObject", "The destination type (" + strDestinationTypeName + ") is not same as the source type (" + strSourceTypeName + ").");
            }

            SerializedObject objSerializedObject = ((ICustomSerializer)this).Serialize("Copy");
            objDestinationObject.ReadData(objSerializedObject);
        }

        public IObjectBase Clone()
        {
            SerializedObject objSerializedObject = ((ICustomSerializer)this).Serialize("Clone");
            IObjectBase objClonedObject = (IObjectBase)objSerializedObject.Deserialize();

            return objClonedObject;
        }

        #endregion

        #region ICustomSerializer Members

        public SerializedObject Serialize(string strName)
        {
            return SerializedObject.Serialize(strName, this);
        }

        public virtual void WriteData(SerializedObject objSerializedObject)
        { }

        public virtual void ReadData(SerializedObject objSerializedObject)
        {
            Initialize();
        }

        public void SerializeToFile(IFormatter objFormatter, string strFilePath)
        {
            SerializedObject objSerializedObject = this.Serialize(this.GetType().Name);
            objFormatter.SerializeToFile(objSerializedObject, strFilePath);
        }

        public void SerializeToStream(IFormatter objFormatter, Stream objStream)
        {
            SerializedObject objSerializedObject = this.Serialize(this.GetType().Name);
            objFormatter.SerializeToStream(objSerializedObject, objStream);
        }

        public byte[] SerializeToByteArray(IFormatter objFormatter)
        {
            SerializedObject objSerializedObject = this.Serialize(this.GetType().Name);
            return objFormatter.SerializeToByteArray(objSerializedObject);
        }

        #endregion

        #region ITransportableObject Members

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            Initialize();

            base.ReadData(objBinaryReader);
        }

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);
        }

        #endregion

        #region Static Members

        public static TObjectType DeserializeFromFile<TObjectType>(IFormatter objFormatter, string strFilePath)
            where TObjectType : ObjectBase
        {
            TObjectType objValue = default(TObjectType);

            if ((strFilePath == null) || (strFilePath.Trim().Length == 0))
            {
                throw new ArgumentException("strFilePath", "A valid non-null, non-empty string is required.");
            }
            if (File.Exists(strFilePath) == false)
            {
                throw new FileNotFoundException("The requested '" + typeof(TObjectType).FullName + "' file could not be found.", strFilePath);
            }

            try
            {
                SerializedObject objSerializedObject = objFormatter.DeserializeFromFile(strFilePath);
                objValue = (TObjectType)objSerializedObject.Deserialize();
            }
            catch (Exception objException)
            {
                throw new Exception("An error was encountered while attempting to load '" + strFilePath + "'.", objException);
            }

            return objValue;
        }

        public static TObjectType DeserializeFromStream<TObjectType>(IFormatter objFormatter, Stream objStream)
            where TObjectType : ObjectBase
        {
            TObjectType objValue = default(TObjectType);

            if (objStream == null)
            {
                throw new ArgumentException("objStream", "A valid non-null Stream is required.");
            }

            try
            {
                SerializedObject objSerializedObject = objFormatter.DeserializeFromStream(objStream);
                objValue = (TObjectType)objSerializedObject.Deserialize();
            }
            catch (Exception objException)
            {
                throw new Exception("An error was encountered while attempting to load the stream.", objException);
            }

            return objValue;
        }

        public static TObjectType DeserializeFromByteArray<TObjectType>(IFormatter objFormatter, byte[] bytData)
            where TObjectType : ObjectBase
        {
            TObjectType objValue = default(TObjectType);

            if (bytData == null)
            {
                throw new ArgumentException("bytData", "A valid non-null byte[] is required.");
            }

            try
            {
                SerializedObject objSerializedObject = objFormatter.DeserializeFromByteArray(bytData);
                objValue = (TObjectType)objSerializedObject.Deserialize();
            }
            catch (Exception objException)
            {
                throw new Exception("An error was encountered while attempting to load the string.", objException);
            }

            return objValue;
        }

        #endregion
    }
}
