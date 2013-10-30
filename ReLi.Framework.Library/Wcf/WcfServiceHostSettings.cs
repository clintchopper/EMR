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
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Net.WebServices;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    #endregion

    public class WcfServiceHostSettings : ObjectBase
    {
        private Type _objType;
        private WcfServiceBehavior _objBehavior;
        private WcfServiceThrottling _objThrottling;
        private ListBase<string> _objBaseAddresses;
        private WcfServiceEndpointList _objServiceEndpoints;
        private WcfMetadataEndpoint _objMetadataEndpoint;

        public WcfServiceHostSettings(WcfServiceBehavior objBehavior, WcfServiceThrottling objThrottling, Type objType, ListBase<string> objBaseAddresses)
            : this(objBehavior, objThrottling, objType, objBaseAddresses, new WcfServiceEndpointList(), new WcfMetadataEndpoint())
        { }

        public WcfServiceHostSettings(WcfServiceBehavior objBehavior, WcfServiceThrottling objThrottling, Type objType, ListBase<string> objBaseAddresses, WcfServiceEndpointList objServiceEndpoints)
            : this(objBehavior, objThrottling, objType, objBaseAddresses, objServiceEndpoints, new WcfMetadataEndpoint())
        { }

        public WcfServiceHostSettings(WcfServiceBehavior objBehavior, WcfServiceThrottling objThrottling, Type objType, ListBase<string> objBaseAddresses, WcfServiceEndpointList objServiceEndpoints, WcfMetadataEndpoint objMetadataEndpoint)
            : base()
        {
            Behavior = objBehavior;
            Throttling = objThrottling;
            Type = objType;
            BaseAddresses = objBaseAddresses;
            ServiceEndpoints = objServiceEndpoints;
            MetadataEndpoint = objMetadataEndpoint;
        }

        public WcfServiceHostSettings(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public WcfServiceHostSettings(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public WcfServiceBehavior Behavior
        {
            get
            {
                if (_objBehavior == null)
                {
                    _objBehavior = new WcfServiceBehavior();
                }

                return _objBehavior;
            }
            set
            {
                _objBehavior = value;
            }
        }

        public WcfServiceThrottling Throttling
        {
            get
            {
                if (_objThrottling == null)
                {
                    _objThrottling = new WcfServiceThrottling();
                }

                return _objThrottling;
            }
            set
            {
                _objThrottling = value;
            }
        }

        public Type Type
        {
            get
            {
                return _objType;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Type", "A valid non-null Type is required.");
                }

                _objType = value;
            }
        }

        public ListBase<string> BaseAddresses
        {
            get
            {
                return _objBaseAddresses;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("BaseAddresses", "A valid non-null ListBase<string> is required.");
                }

                _objBaseAddresses = value;
            }
        }

        public WcfServiceEndpointList ServiceEndpoints
        {
            get
            {
                return _objServiceEndpoints;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ServiceEndpoints", "A valid non-null WcfServiceEndpointList is required.");
                }

                _objServiceEndpoints = value;
            }
        }

        public WcfMetadataEndpoint MetadataEndpoint
        {
            get
            {
                return _objMetadataEndpoint;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MetadataEndpoint", "A valid non-null WcfMetadataEndpoint is required.");
                }

                _objMetadataEndpoint = value;
            }
        }

        public ServiceHost CreateServiceHost()
        {
            List<Uri> objUriBaseAddresses = new List<Uri>();
            foreach (string strBaseAddress in BaseAddresses)
            {
                objUriBaseAddresses.Add(new Uri(strBaseAddress));
            }
            
            ServiceHost objServiceHost = new ServiceHost(Type, objUriBaseAddresses.ToArray());
            foreach (WcfServiceEndpoint objServiceEndpoint in ServiceEndpoints)
            {
                objServiceHost.AddServiceEndpoint(objServiceEndpoint.Type, objServiceEndpoint.Binding.Binding, objServiceEndpoint.Address);
            }

            ServiceBehaviorAttribute objServiceBehavior = objServiceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            if (objServiceBehavior == null)
            {
                objServiceBehavior = new ServiceBehaviorAttribute();
                objServiceHost.Description.Behaviors.Add(objServiceBehavior);
            }
            objServiceBehavior.ConcurrencyMode = Behavior.ConcurrencyMode;
            objServiceBehavior.InstanceContextMode = Behavior.InstanceContextMode;
            
            if (MetadataEndpoint.Enabled == true)
            {
                objServiceHost.Description.Behaviors.Add(MetadataEndpoint.CreateBehavior());
            }

            ServiceDebugBehavior objServiceDebugBehavior = objServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (objServiceDebugBehavior == null)
            {
                objServiceDebugBehavior = new ServiceDebugBehavior();
                objServiceHost.Description.Behaviors.Add(objServiceDebugBehavior);
            }

            objServiceDebugBehavior.IncludeExceptionDetailInFaults = true;

            ServiceThrottlingBehavior objThrottlingBehavior = objServiceHost.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (objThrottlingBehavior == null)
            {
                objThrottlingBehavior = new ServiceThrottlingBehavior();
                objServiceHost.Description.Behaviors.Add(objThrottlingBehavior);
            }

            objThrottlingBehavior.MaxConcurrentCalls = Throttling.MaxConcurrentCalls;
            objThrottlingBehavior.MaxConcurrentInstances = Throttling.MaxConcurrentCalls;
            objThrottlingBehavior.MaxConcurrentSessions = Throttling.MaxConcurrentSessions;

            return objServiceHost;
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Objects.Add("Behavior", Behavior);
            objSerializedObject.Objects.Add("Throttling", Throttling);            
            objSerializedObject.Values.Add("AssemblyQualifiedName", Type.AssemblyQualifiedName);
            objSerializedObject.Objects.Add("BaseAddresses", BaseAddresses);
            objSerializedObject.Objects.Add("ServiceEndpoints", ServiceEndpoints);
            objSerializedObject.Objects.Add("MetadataEndpoint", MetadataEndpoint);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Behavior = objSerializedObject.Objects.GetObject<WcfServiceBehavior>("Behavior", null);
            Throttling = objSerializedObject.Objects.GetObject<WcfServiceThrottling>("Throttling", null);

            string strType = objSerializedObject.Values.GetValue<string>("AssemblyQualifiedName", null);
            if (strType != null)
            {
                Type = System.Type.GetType(strType);
            }
            else
            {
                Type = null;
            }

            BaseAddresses = objSerializedObject.Objects.GetObject<ListBase<string>>("BaseAddresses", null);
            ServiceEndpoints = objSerializedObject.Objects.GetObject<WcfServiceEndpointList>("ServiceEndpoints", null);
            MetadataEndpoint = objSerializedObject.Objects.GetObject<WcfMetadataEndpoint>("MetadataEndpoint", null);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(Behavior);
            objBinaryWriter.WriteTransportableObject(Throttling);
            objBinaryWriter.Write(Type.FullName);
            objBinaryWriter.WriteTransportableObject(BaseAddresses);
            objBinaryWriter.WriteTransportableObject(ServiceEndpoints);
            objBinaryWriter.WriteTransportableObject(MetadataEndpoint);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Behavior = objBinaryReader.ReadTransportableObject<WcfServiceBehavior>();
            Throttling = objBinaryReader.ReadTransportableObject<WcfServiceThrottling>();

            string strType = objBinaryReader.ReadString();
            Type = System.Type.GetType(strType);

            BaseAddresses = objBinaryReader.ReadTransportableObject<ListBase<string>>();
            ServiceEndpoints = objBinaryReader.ReadTransportableObject<WcfServiceEndpointList>();
            MetadataEndpoint = objBinaryReader.ReadTransportableObject<WcfMetadataEndpoint>();
        }

        #endregion
    }
}
