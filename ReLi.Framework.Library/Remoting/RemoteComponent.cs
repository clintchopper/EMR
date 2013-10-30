namespace ReLi.Framework.Library.Remoting
{
    #region Using Declarations

    using System;
    using System.Runtime.Remoting;

    #endregion

    public class RemoteComponent : MarshalByRefObject
    {
        public bool IsAlive
        {
            get
            {
                return true;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }        
    }
}
