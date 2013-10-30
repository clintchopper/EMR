namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    
    #endregion
        
	public class SerializedWrapperObject : SerializedWrapperBase<object>
    {
        public SerializedWrapperObject(object objData)
            : base(objData)
        {
            if (objData != null)
            {
                Type obyType = objData.GetType();
                if (obyType.IsSerializable == false)
                {
                    throw new SerializationException(string.Format("The type {0} is not serializable", obyType.FullName));
                }
            }
        }

        protected SerializedWrapperObject(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        #region SerializedWrapperBase Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            string strData = string.Empty;

            if (Data != null)
            {
                using (MemoryStream objMemoryStream = new MemoryStream())
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter objBinaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    objBinaryFormatter.Serialize(objMemoryStream, Data);
                    byte[] arrBuffer = objMemoryStream.ToArray();
                    strData = ConversionHelper.ByteArrayToBase16String(arrBuffer);
                }
            }

            objSerializedObject.Values.Add("Data", strData);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            object objData = null;

            string strData = objSerializedObject.Values.GetValue<string>("Data", string.Empty);
            if (strData.Length != 0)
            {
                byte[] arrBytes = ConversionHelper.Base16StringToByteArray(strData);
                using (MemoryStream objMemoryStream = new MemoryStream(arrBytes))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter objBinaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    objData = objBinaryFormatter.Deserialize(objMemoryStream);
                }
            }

            Data = objData;
        }

        #endregion
    }
}
