namespace ReLi.Framework.Library.Remoting
{
    #region Using Declarations

    using System;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using ReLi.Framework.Library.Remoting.Channels;

    #endregion

	public class RemoteServer : IDisposable
    {
        private bool _blnIsDisposed = false;
        private ObjRef _objReferencePointer;
        private IChannel _objChannel;
        private RemoteComponent _objRemoteComponent;
        private RemoteChannel _objRemoteChannel;
        private RemoteServerStatusType _enuRemoteServerStatus;

        public RemoteServer(RemoteComponent objRemoteComponent, RemoteChannel objRemoteChannel)
        {
            Status = RemoteServerStatusType.Stopped;
            RemoteComponent = objRemoteComponent;
            RemoteChannel = objRemoteChannel;
            Channel = null;
        }

        public RemoteServerStatusType Status
        {
            get
            {
                return _enuRemoteServerStatus;
            }
            private set
            {
                _enuRemoteServerStatus = value;
            }
        }

        protected RemoteChannel RemoteChannel
        {
            get
            {
                return _objRemoteChannel;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("RemoteChannel", "A valid non-null RemoteChannel is required.");
                }
                _objRemoteChannel = value;
            }
        }

        protected RemoteComponent RemoteComponent
        {
            get
            {
                return _objRemoteComponent;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("RemoteComponent", "A valid non-null RemoteComponent is required.");
                }
                _objRemoteComponent = value;
            }
        }

        protected IChannel Channel
        {
            get
            {
                return _objChannel;
            }
            set
            {
                _objChannel = value;
            }
        }

        private ObjRef ReferencePointer
        {
            get
            {
                return _objReferencePointer;
            }
            set
            {
                _objReferencePointer = value;
            }
        }

        public void Start()
        {
            if (Channel == null)
            {
                Channel = RemoteChannel.RegisterChannel(RemoteChannel);
            }

            if (ReferencePointer == null)
            {
                ReferencePointer = RemotingServices.Marshal(RemoteComponent, RemoteChannel.ChannelName);
            }

            Status = RemoteServerStatusType.Running;
        }

        public void Stop()
        {
            ReferencePointer = null;
            RemotingServices.Disconnect(RemoteComponent);
            Status = RemoteServerStatusType.Stopped;
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
                if (_objReferencePointer != null)
                {
                    RemotingServices.Unmarshal(_objReferencePointer);
                    _objReferencePointer = null;
                }
                if (_objRemoteComponent != null)
                {
                    RemotingServices.Disconnect(_objRemoteComponent);
                    _objRemoteComponent = null;
                }
                if (_objChannel != null)
                {
                    RemoteChannel.UnRegisterChannel(_objChannel);
                    _objChannel = null;
                }
            }

            _blnIsDisposed = true;
        }
        
        #endregion

        #region Static Members

        public static TRemoteComponent Connect<TRemoteComponent>(RemoteChannel objRemoteChannel)
            where TRemoteComponent : RemoteComponent 
        {
            RemoteChannel.RegisterCallBackChannel(objRemoteChannel);
            return (TRemoteComponent)Activator.GetObject(typeof(TRemoteComponent), objRemoteChannel.Uri);
        }

        #endregion
    }

}
