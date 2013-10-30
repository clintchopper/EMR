namespace ReLi.Framework.Library.Wcf
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    #endregion

    public class WcfChannelFactoryManager
    {
        private object _objSyncObject;
        private Dictionary<string, object> _objChannelFactoryCache;

        private WcfChannelFactoryManager()
        {
            _objSyncObject = new object();
            _objChannelFactoryCache = new Dictionary<string, object>();
        }

        public ChannelFactory<TServiceType> GetChannelFactory<TServiceType>(WcfClientChannelConnectionSettings objChannelConnection)
            where TServiceType : class
        {
            return GetChannelFactory<TServiceType>(new WcfClientChannelConnectionSettingsList(new WcfClientChannelConnectionSettings[] { objChannelConnection }));
        }

        public ChannelFactory<TServiceType> GetChannelFactory<TServiceType>(WcfClientChannelConnectionSettingsList objChannelConnections)
            where TServiceType : class
        {
            ChannelFactory<TServiceType> objChannelFactory = null;

            objChannelConnections.Sort();
            foreach (WcfClientChannelConnectionSettings objChannelConnection in objChannelConnections)
            {
                string strKey = objChannelConnection.EndpointAddress.Address;
                if (_objChannelFactoryCache.ContainsKey(strKey) == true)
                {
                    objChannelFactory = (ChannelFactory<TServiceType>)_objChannelFactoryCache[strKey];
                    if ((objChannelFactory == null) || (objChannelFactory.State == CommunicationState.Faulted) || (objChannelFactory.State == CommunicationState.Closed) || (objChannelFactory.State == CommunicationState.Closing))
                    {
                        if (objChannelFactory != null)
                        {
                            objChannelFactory.Abort();
                            objChannelFactory.Close();
                            objChannelFactory.Faulted -= new EventHandler(ChannelFactory_Faulted);
                        }

                        RemoveFromCache(strKey);
                        objChannelFactory = null;
                    }
                    else
                    {
                        break;
                    }
                }

                if (objChannelConnection.IsChannelAvaliable() == true)
                {
                    try
                    {
                        objChannelFactory = new ChannelFactory<TServiceType>(objChannelConnection.Binding.Binding, objChannelConnection.EndpointAddress.Address);
                        objChannelFactory.Faulted += new EventHandler(ChannelFactory_Faulted);
                        objChannelFactory.Open();

                        AddToCache(strKey, objChannelFactory);
                        break;
                    }
                    catch (CommunicationException)
                    {
                        /// Try to connect to the next channel
                        /// 
                    }
                }
            }
            if (objChannelFactory == null)
            {
                throw new Exception(string.Format("There is no channel available for {0}.", typeof(TServiceType)));
            }

            return objChannelFactory;
        }

        private void AddToCache(string strKey, object objValue)
        {
            lock (_objSyncObject)
            {
                if (_objChannelFactoryCache.ContainsKey(strKey) == true)
                {
                    _objChannelFactoryCache.Remove(strKey);
                }
                _objChannelFactoryCache.Add(strKey, objValue);
            }
        }

        private void RemoveFromCache(string strKey)
        {
            lock (_objSyncObject)
            {
                if (_objChannelFactoryCache.ContainsKey(strKey) == true)
                {
                    _objChannelFactoryCache.Remove(strKey);
                }
            }
        }

        private void ChannelFactory_Faulted(object objSender, EventArgs objArguments)
        {
            ICommunicationObject objCommunicationObject = objSender as ICommunicationObject;
            if (objCommunicationObject != null)
            {
                objCommunicationObject.Abort();
                objCommunicationObject.Close();
                objCommunicationObject.Faulted -= new EventHandler(ChannelFactory_Faulted);
            }
        }

        #region Static Members

        private static WcfChannelFactoryManager _objInstance;

        static WcfChannelFactoryManager()
        {
            _objInstance = new WcfChannelFactoryManager();
        }

        public static WcfChannelFactoryManager Instance
        {
            get
            {
                return _objInstance;
            }
        }

        #endregion        
    }
}
