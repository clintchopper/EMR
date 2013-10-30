namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.ComponentModel;

    #endregion

    public interface IFormatter
    {
        void SerializeToFile(SerializedObject objSerializedObject, string strFilePath);
        
        void SerializeToStream(SerializedObject objSerializedObject, Stream objStream);

        byte[] SerializeToByteArray(SerializedObject objSerializedObject);

        SerializedObject DeserializeFromFile(string strFilePath);
        
        SerializedObject DeserializeFromStream(Stream objStream);

        SerializedObject DeserializeFromByteArray(byte[] bytData);
    }
}
