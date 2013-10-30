namespace ReLi.Framework.Library.Remoting.Channels
{
    #region Using Declarations

    using System;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Runtime.Serialization.Formatters;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.IO;

    #endregion
        
	public class TcpRemoteChannel : RemoteChannel
    {
        private const string ChannelPrefix = "tcp";

        public TcpRemoteChannel(int intPort, string strChannelName)
            : base(intPort, strChannelName)
        {}
        
        public TcpRemoteChannel(string strMachineName, int intPort, string strChannelName)
            : base(strMachineName, intPort, strChannelName)
        {}

        public TcpRemoteChannel(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public TcpRemoteChannel(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        protected override string GetUri(string strMachineName, int intPort, string strChannelName)
        {
            string strUri = "";

            strUri += ChannelPrefix + "://" + strMachineName;
            strUri += ((intPort > 0) ? ":" + intPort.ToString() : "");
            strUri += "/" + strChannelName;

            return strUri;
        }
        
        protected override IChannel CreateChannel(IDictionary objChannelProperties, IClientChannelSinkProvider objClientFormatter, IServerChannelSinkProvider objServerFormatter)
        {
            return new TcpChannel(objChannelProperties, objClientFormatter, objServerFormatter);
        }
        
        protected override Type GetChannelType()
        {
            return typeof(TcpChannel);
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
