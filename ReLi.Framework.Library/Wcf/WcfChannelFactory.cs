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

    public class WcfChannelFactory<TServiceType> : IDisposable  
        where TServiceType : class 
    {
        private bool _blnDisposed = false;
        private object _objSyncObject = new object();
        private ChannelFactory<TServiceType> _objChannelFactory;
        private WcfClientChannelConnectionSettingsList _objChannelConnectionSettings;

        public WcfChannelFactory(WcfClientChannelConnectionSettingsList objChannelConnectionSettings)
        {
            ChannelConnectionSettings = objChannelConnectionSettings;
            ChannelFactory = null;
        }

        private ChannelFactory<TServiceType> ChannelFactory
        {
            get
            {
                return _objChannelFactory;
            }
            set
            {
                _objChannelFactory = value;
            }
        }

        public WcfClientChannelConnectionSettingsList ChannelConnectionSettings
        {
            get
            {
                return _objChannelConnectionSettings;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ChannelConnectionSettings", "A valid non-null WcfClientChannelConnectionSettingsList is required.");
                }

                _objChannelConnectionSettings = value;
            }
        }

        public TServiceType CreateChannel()
        {
            int intRetryCount = 1;
            TServiceType objChannel = null;            

            while (true)
            {
                try
                {
                    if (_objChannelFactory == null)
                    {
                        lock (_objSyncObject)
                        {
                            if (_objChannelFactory == null)
                            {
                                _objChannelFactory = WcfChannelFactoryManager.Instance.GetChannelFactory<TServiceType>(ChannelConnectionSettings);
                            }
                        }
                    }

                    objChannel = _objChannelFactory.CreateChannel();
                    break;
                }
                catch
                {
                    if (intRetryCount == 0)
                    {
                        throw;
                    }
                    intRetryCount--;
                    _objChannelFactory = null;
                }
            }

            return objChannel;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_blnDisposed == false)
            {
                lock (_objSyncObject)
                {
                    _objChannelFactory = null;
                    _objChannelConnectionSettings = null;
                }

                _blnDisposed = true;
            }
        }

        #endregion
    }
}
