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

    public class WcfClientChannel<TServiceType> : IDisposable
        where TServiceType : class
    {
        private bool _blnDisposed = false;
        private object _objSyncObject = new object();
        private TServiceType _objProxy;
        private WcfChannelFactory<TServiceType> _objChannelFactory;

        public WcfClientChannel(WcfChannelFactory<TServiceType> objChannelFactory)
        {
            ChannelFactory = objChannelFactory;
            Proxy = null;
        }

        public WcfChannelFactory<TServiceType> ChannelFactory
        {
            get
            {
                return _objChannelFactory;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ChannelFactory", string.Format("A valid non-null WcfChannelFactory<{0}> is required.", typeof(TServiceType).Name));
                }

                _objChannelFactory = value;
            }
        }

        public TServiceType Proxy
        {
            get
            {
                if (_objProxy == null)
                {
                    lock (_objSyncObject)
                    {
                        if (_objProxy == null)
                        {
                            _objProxy = ChannelFactory.CreateChannel();
                            ((IClientChannel)_objProxy).Faulted += new EventHandler(Channel_Faulted);
                            ((IClientChannel)_objProxy).Open();
                        }
                    }
                }

                return _objProxy;
            }
            private set
            {
                _objProxy = value;
            }
        }

        public bool IsChannelValid
        {
            get
            {
                bool blnIsValidChannel = false;
                if (_objProxy != null)
                {
                    IChannel objChannel = _objProxy as IChannel;
                    blnIsValidChannel = ((objChannel != null) && (objChannel.State == CommunicationState.Opened));
                }

                return blnIsValidChannel;
            }
        }

        public void Close()
        {
            lock (_objSyncObject)
            {
                if (_objProxy != null)
                {
                    IChannel objChannel = _objProxy as IChannel;
                    if ((objChannel != null) && (objChannel.State == CommunicationState.Opened))
                    {
                        objChannel.Close();
                    }
                }
            }
        }

        public void Abort()
        {
            lock (_objSyncObject)
            {
                if (_objProxy != null)
                {
                    IChannel objChannel = _objProxy as IChannel;
                    if (objChannel != null)
                    {
                        objChannel.Abort();
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_blnDisposed == false)
            {
                lock (_objSyncObject)
                {
                    if (_objProxy != null)
                    {
                        IChannel objChannel = _objProxy as IChannel;
                        if (objChannel != null)
                        {
                            if (objChannel.State == CommunicationState.Opened)
                            {
                                objChannel.Close();
                            }
                            else if (objChannel.State == CommunicationState.Faulted)
                            {
                                objChannel.Abort();
                                objChannel.Close();
                            }

                            objChannel.Faulted -= new EventHandler(Channel_Faulted);
                            objChannel = null;
                        }
                    }

                    if ((_objProxy != null) && (_objProxy is IDisposable))
                    {
                        ((IDisposable)_objProxy).Dispose();
                        _objProxy = null;
                    }
                }
            }
        }

        private void Channel_Faulted(object objSender, EventArgs objArguments)
        {
            Dispose();
            _blnDisposed = false;
        }
    }
}
