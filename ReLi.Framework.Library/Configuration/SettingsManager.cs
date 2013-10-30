namespace ReLi.Framework.Library.Configuration
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Reflection;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Security.Encryption;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class SettingsManager : ObjectBase 
    {
        private Dictionary<string, object> _objSettings;
        
        public SettingsManager()
            : base()
        {
            
        }

        public SettingsManager(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public SettingsManager(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        protected override void Initialize()
        {
            base.Initialize();

            Settings = new Dictionary<string, object>();
        }

        private Dictionary<string, object> Settings
        {
            get
            {
                return _objSettings;
            }
            set
            {
                if (value == null) 
                {
                    throw new ArgumentException("Settings", "A valid non-null Dictionary<string, object> is requird.");
                }

                _objSettings = value;
            }
        }

        public bool Contains(string strName)
        {
            bool blnContainsValue = Settings.ContainsKey(strName);
            return blnContainsValue;
        }

        public void Clear()
        {
            Settings.Clear();
        }

        public void Remove(string strName)
        {
            if (Contains(strName) == true)
            {
                Settings.Remove(strName);
            }
        }

        public TValueType GetValue<TValueType>(string strName, TValueType objDefault)
        {
            TValueType objValue = objDefault;

            if (Contains(strName) == true)
            {
                objValue = (TValueType)Settings[strName];
            }

            return objValue;
        }

        public void SetValue(string strName, ICustomSerializer objObject)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            if (objObject != null)
            {
                Settings.Add(strName, objObject);
            }
        }
         
        public void SetValue(string strName, bool objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, byte objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, byte[] objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, char objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, DateTime objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Decimal objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Double objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Guid objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Enum objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Int16 objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Int32 objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Int64 objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, SByte objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, Single objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, String objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, UInt16 objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, UInt32 objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, UInt64 objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        public void SetValue(string strName, object objValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            Settings.Remove(strName);
            Settings.Add(strName, objValue);
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            SerializedObject objSettings = objSerializedObject.Objects.Add("Settings");
            foreach (KeyValuePair<string, object> objKeyValuePair in Settings)
            {
                SerializedValueType enuValueType = SerializedValueTypeHelper.GetTypeFromObject(objKeyValuePair.Value);
                if (enuValueType != SerializedValueType.Unknown)
                {
                    objSettings.Values.Add(objKeyValuePair.Key, objKeyValuePair.Value);
                }
                else if (objKeyValuePair.Value is ICustomSerializer)
                {
                    objSettings.Objects.Add(objKeyValuePair.Key, (ICustomSerializer)objKeyValuePair.Value);
                }
                else
                {
                    objSettings.Objects.Add(objKeyValuePair.Key, new SerializedWrapperObject(objKeyValuePair.Value));
                }
            }
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            SerializedObject objSettings = objSerializedObject.Objects["Settings"];

            foreach (SerializedValue objItem in objSettings.Values.ToArray())
            {
                Settings.Add(objItem.Name, objItem.Value);
            }

            foreach (SerializedObject objItem in objSettings.Objects.ToArray())
            {
                Settings.Add(objItem.Name, objItem.Deserialize());
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Settings.Count);
            foreach (KeyValuePair<string, object> objKeyValuePair in Settings)
            {
                objBinaryWriter.Write(objKeyValuePair.Key);
                objBinaryWriter.WriteObject(objKeyValuePair.Value);
            }
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            int intCount = objBinaryReader.ReadInt32();
            for (int intIndex = 0; intIndex < intCount; intIndex++)
            {
                string strKey = objBinaryReader.ReadString();
                object objValue = objBinaryReader.ReadObject();

                Settings.Add(strKey, objValue);
            }
        }

        #endregion

        #region Static Members

        public static string GetSettingsDirectory()
        {
            
            string strRootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string strSettingsDirectory = Path.Combine(strRootDirectory, "settings");

            while (DirectoryManager.Exists(strSettingsDirectory) == false)
            {
                strRootDirectory = Path.GetDirectoryName(strRootDirectory);
                if (strRootDirectory == null)
                {
                    strSettingsDirectory = null;
                    break;
                }
                else
                {
                    strSettingsDirectory = Path.Combine(strRootDirectory, "settings");
                }
            }

            return strSettingsDirectory;
        }

        #endregion

    }
}
