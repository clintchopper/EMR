namespace ReLi.Framework.Library.Remoting.Channels
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Http;
    using System.Runtime.Serialization.Formatters;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.IO;

    #endregion
        
	public class HttpRemoteChannel : RemoteChannel
    {
        private const string ChannelPrefix = "http";

        public HttpRemoteChannel(int intPort, string strChannelName)
            : base(intPort, strChannelName)
        {}

        public HttpRemoteChannel(string strMachineName, int intPort, string strChannelName)
            : base(strMachineName, intPort, strChannelName)
        {}

        public HttpRemoteChannel(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public HttpRemoteChannel(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        protected override string GetUri(string strMachineName, int intPort, string strChannelName)
        {
            string GetUri = "";

            GetUri += ChannelPrefix + "://" + strMachineName;
            GetUri += ((intPort > 0) ? ":" + intPort.ToString() : "");
            GetUri += "/" + strChannelName;

            return GetUri;
        }

        protected override IChannel CreateChannel(IDictionary objChannelProperties, IClientChannelSinkProvider objClientFormatter, IServerChannelSinkProvider objServerFormatter)
        {
            return new HttpChannel(objChannelProperties, objClientFormatter, objServerFormatter);
        }

        protected override Type GetChannelType()
        {
            return typeof(HttpChannel);
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);
        }

        #endregion
    }
}
