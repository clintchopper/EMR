namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using ReLi.Framework.Library.Remoting;
    using ReLi.Framework.Library.Serialization;
    using System.ComponentModel;

    #endregion

    public interface ICustomSerializer 
    { 
        SerializedObject Serialize(string strName);

        void SerializeToFile(IFormatter objFormatter, string strFilePath);

        void SerializeToStream(IFormatter objFormatter, Stream objStream);

        byte[] SerializeToByteArray(IFormatter objFormatter);

        void WriteData(SerializedObject objSerializedObject);

        void ReadData(SerializedObject objSerializedObject);
    }
}
