namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;    
    using System.IO;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public interface ITransportableObject
    {
        /// <summary>
        /// public [ClassName](BinaryReaderExtension objBinaryReader)
        /// {}
        /// </summary>

        byte[] Compress();

        void ReadData(BinaryReaderExtension objBinaryReader);

        void WriteData(BinaryWriterExtension objBinaryWriter);
    }
}
