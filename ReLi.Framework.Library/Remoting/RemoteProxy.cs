namespace ReLi.Framework.Library.Remoting
{
    #region Using Declarations

    using System;
    using System.Runtime.Remoting;

    #endregion

	public class RemoteProxy : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
