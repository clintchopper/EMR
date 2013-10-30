namespace ReLi.Framework.Library.Remoting.Channels
{
    #region Using Declarations

    using System;
    using System.Net;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Runtime.Remoting.Channels.Http;
    using System.Runtime.Serialization.Formatters;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Net;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public abstract class RemoteChannel : ObjectBase 
    {
        private int _intPort;
        private string _strChannelName;
        private string _strMachineName;
        private string _strUri;

        protected RemoteChannel(int intPort, string strChannelName)
            : this("127.0.0.1", intPort, strChannelName)
        {}

        protected RemoteChannel(string strMachineName, int intPort, string strChannelName)
            : base()
        {
            MachineName = strMachineName;
            Port = intPort;
            ChannelName = strChannelName;            
        }

        public RemoteChannel(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public RemoteChannel(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public int Port
        {
            get
            {
                return _intPort;
            }
            private set
            {
                _intPort = value;
            }
        }

        public string MachineName
        {
            get
            {
                return _strMachineName;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MachineName", "A valid non-null string is required.");
                }

                _strMachineName = value;
            }
        }

        public string ChannelName
        {
            get
            {
                return _strChannelName;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ChannelName", "A valid non-null string is required.");
                }

                _strChannelName = value;
            }
        }

        public string Uri
        {
            get
            {
                if (_strUri == null)
                {
                    _strUri = GetUri(MachineName, Port, ChannelName);
                }

                return _strUri;
            }
            private set
            {
                _strUri = value;
            }
        }

        protected abstract string GetUri(string strMachineName, int intPort, string strChannelName);

        protected abstract Type GetChannelType();

        protected abstract IChannel CreateChannel(IDictionary objChannelProperties, IClientChannelSinkProvider objClientFormatter, IServerChannelSinkProvider objServerFormatter);
                
        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Port", Port);
            objSerializedObject.Values.Add("ChannelName", ChannelName);
            objSerializedObject.Values.Add("MachineName", MachineName);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Port = objSerializedObject.Values.GetValue<int>("Port", 0);
            ChannelName = objSerializedObject.Values.GetValue<string>("ChannelName", string.Empty);
            MachineName = objSerializedObject.Values.GetValue<string>("MachineName", string.Empty);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Port);
            objBinaryWriter.Write(ChannelName);
            objBinaryWriter.Write(MachineName);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Port = objBinaryReader.ReadInt32();
            ChannelName = objBinaryReader.ReadString();
            MachineName = objBinaryReader.ReadString();
        }

        #endregion

        #region Static Members

        private static object _objSyncObject = new object();

        public static IChannel RegisterCallBackChannel(RemoteChannel objRemotechannel)
        {
            IChannel objChannel = null;
            IDictionary objChannelProperties = null;
            BinaryServerFormatterSinkProvider objServerFormatter = null;
            BinaryClientFormatterSinkProvider objClientFormatter = null;

            lock (_objSyncObject)
            {
                foreach (IChannel objRegisteredChannel in ChannelServices.RegisteredChannels)
                {
                    if (objRegisteredChannel.GetType() == objRemotechannel.GetChannelType())
                    {
                        objChannel = objRegisteredChannel;
                        break;
                    }
                }
                if (objChannel == null)
                {
                    objClientFormatter = new BinaryClientFormatterSinkProvider();
                    objServerFormatter = new BinaryServerFormatterSinkProvider();
                    objServerFormatter.TypeFilterLevel = TypeFilterLevel.Full;

                    objChannelProperties = new Hashtable();
                    objChannelProperties["port"] = 0;
                    objChannelProperties["name"] = objRemotechannel.ChannelName;

                    ProxySettings objProxySettings = ProxySettings.Load();
                    if (objProxySettings != null)
                    {
                        objChannelProperties["proxyName"] = objProxySettings.Address;
                        objChannelProperties["proxyPort"] = objProxySettings.Port;
                        objChannelProperties["credentials"] = null;
                        if (objProxySettings.Credentials != null)
                        {
                            objChannelProperties["credentials"] = objProxySettings.Credentials.CreateNetworkCredentials();
                        }
                    }

                    objChannel = objRemotechannel.CreateChannel(objChannelProperties, objClientFormatter, objServerFormatter);
                    ChannelServices.RegisterChannel(objChannel, false);
                }
            }

            return objChannel;
        }

        public static IChannel RegisterChannel(RemoteChannel objRemotechannel)
        {
            IChannel objChannel = null;
            IDictionary objChannelProperties = null;
            BinaryServerFormatterSinkProvider objServerFormatter = null;
            BinaryClientFormatterSinkProvider objClientFormatter = null;

            lock (_objSyncObject)
            {
                foreach (IChannel objRegisteredChannel in ChannelServices.RegisteredChannels)
                {
                    if (objRegisteredChannel.ChannelName == objRemotechannel.ChannelName)
                    {
                        ChannelServices.UnregisterChannel(objRegisteredChannel);
                    }
                }

                objClientFormatter = new BinaryClientFormatterSinkProvider();
                objServerFormatter = new BinaryServerFormatterSinkProvider();
                objServerFormatter.TypeFilterLevel = TypeFilterLevel.Full;

                objChannelProperties = new Hashtable();
                objChannelProperties["port"] = objRemotechannel.Port;
                objChannelProperties["name"] = objRemotechannel.ChannelName;
                objChannelProperties["machineName"] = objRemotechannel.MachineName;

                objChannel = objRemotechannel.CreateChannel(objChannelProperties, objClientFormatter, objServerFormatter);
                ChannelServices.RegisterChannel(objChannel, false);
            }

            return objChannel;
        }

        public static void UnRegisterChannel(IChannel objChannel)
        {
            lock (_objSyncObject)
            {
                foreach (IChannel objRegisteredChannel in ChannelServices.RegisteredChannels)
                {
                    if (object.ReferenceEquals(objRegisteredChannel, objChannel) == true)
                    {
                        ChannelServices.UnregisterChannel(objRegisteredChannel);
                    }
                }
            }
        }

        #endregion
    }

}
