namespace ReLi.Framework.Library.Wcf
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Diagnostics;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Net.WebServices;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
    public class WcfClientChannelConnectionSettings : ObjectBase  
    {
        private int _intPriority;
        private IWcfBinding _objBinding;
        private WcfEndpointAddress _objEndpointAddress;

        public WcfClientChannelConnectionSettings(IWcfBinding objBinding, WcfEndpointAddress objEndpointAddress)
            : this(objBinding, objEndpointAddress, DefaultPriority)
        {}

        public WcfClientChannelConnectionSettings(IWcfBinding objBinding, WcfEndpointAddress objEndpointAddress, int intPriority)
            : base()
        {
            Binding = objBinding;
            EndpointAddress = objEndpointAddress;
            Priority = intPriority;
        }

        public WcfClientChannelConnectionSettings(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfClientChannelConnectionSettings(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public IWcfBinding Binding
        {
            get
            {
                return _objBinding;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Binding", "A valid non-null IWcfBinding is required.");
                }

                _objBinding = value;
            }
        }

        public WcfEndpointAddress EndpointAddress
        {
            get
            {
                return _objEndpointAddress;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("EndpointAddress", "A valid non-null WcfEndpointAddress is required.");
                }

                _objEndpointAddress = value;
            }
        }

        public int Priority
        {
            get
            {
                return _intPriority;
            }
            set
            {
                _intPriority = value;
            }
        }

        public bool IsChannelAvaliable()
        {
            return IsChannelAvaliable(DefaultTimeoutInSeconds);
        }

        public bool IsChannelAvaliable(int intTimeoutInSeconds)
        {
            Socket objSocket = null;

            try
            {
                int intTimeout = intTimeoutInSeconds * 1000;
                Uri objUri = new Uri(EndpointAddress.Address);

                Stopwatch objStopwatch = new Stopwatch();
                objStopwatch.Start();                

                IAsyncResult objAsyncResult = Dns.BeginGetHostEntry(objUri.Host, null, null);
                if (objAsyncResult.AsyncWaitHandle.WaitOne(intTimeout, true) == false)
                {
                    return false;
                }

                objSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                objAsyncResult = objSocket.BeginConnect(objUri.Host, objUri.Port, null, null);

                intTimeout = Math.Max(intTimeout - (int)objStopwatch.ElapsedMilliseconds, 0);
                if (objAsyncResult.AsyncWaitHandle.WaitOne(intTimeout, true) == false)
                {
                    return false;
                }

                objSocket.EndConnect(objAsyncResult);
                return objSocket.Connected;
            }

            catch
            {
                return false;
            }

            finally
            {
                if (objSocket != null)
                {
                    objSocket.Close();
                }
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Objects.Add("Binding", Binding);
            objSerializedObject.Objects.Add("EndpointAddress", EndpointAddress);
            objSerializedObject.Values.Add("Priority", Priority);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Binding = objSerializedObject.Objects.GetObject<IWcfBinding>("Binding", null);
            EndpointAddress = objSerializedObject.Objects.GetObject<WcfEndpointAddress>("EndpointAddress", null);
            Priority = objSerializedObject.Values.GetValue<int>("Priority", DefaultPriority);            
        }

        #endregion       

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(Binding);
            objBinaryWriter.WriteTransportableObject(EndpointAddress);
            objBinaryWriter.Write(Priority);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Binding = objBinaryReader.ReadTransportableObject<IWcfBinding>();
            EndpointAddress = objBinaryReader.ReadTransportableObject<WcfEndpointAddress>();
            Priority = objBinaryReader.ReadInt32();
        }

        #endregion

        #region Static Members

        public static readonly int DefaultTimeoutInSeconds = 10;
        public static readonly int DefaultPriority = 100;

        #endregion
    }
}
