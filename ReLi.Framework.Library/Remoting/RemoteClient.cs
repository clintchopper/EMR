namespace ReLi.Framework.Library.Remoting
{
    #region Using Declarations

    using System;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using ReLi.Framework.Library.Remoting.Channels;

    #endregion

	public class RemoteClient<TComponentType> : IDisposable
        where TComponentType : RemoteComponent 
    {
        private bool _blnIsDisposed = false;
        protected RemoteChannel _objRemoteChannel;
        protected TComponentType _objRemoteComponent;

        public RemoteClient(RemoteChannel objRemoteChannel)
        {
            RemoteChannel = objRemoteChannel;
            RemoteComponent = null;
        }

        public bool IsAlive
        {
            get
            {
                bool blnIsAlive = false;

                if (_objRemoteComponent != null)
                {
                    try
                    {
                        return _objRemoteComponent.IsAlive;
                    }
                    catch
                    {
                        blnIsAlive = false;
                    }
                }

                return blnIsAlive;
            }
        }

        public TComponentType RemoteComponent
        {
            get
            {
                if (IsAlive == false)
                {
                    Refresh();                   
                }

                return _objRemoteComponent;
            }
            private set
            {
                _objRemoteComponent = value;
            }
        }

        public RemoteChannel RemoteChannel
        {
            get
            {
                return _objRemoteChannel;
            }
            private set
            {
                _objRemoteChannel = value;
            }
        }

        public void Refresh()
        {
            if (IsAlive == false)
            {
                try
                {
                    _objRemoteComponent = RemoteServer.Connect<TComponentType>(_objRemoteChannel);
                }
                catch
                {
                    _objRemoteComponent = null;
                }
            }
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool blnDisposing)
        {
            if (_blnIsDisposed == false)
            {
                if (blnDisposing == true)
                {
                }

                _objRemoteChannel = null;
                _objRemoteChannel = null;
            }

            _blnIsDisposed = true;
        }

        #endregion       
    }
}
