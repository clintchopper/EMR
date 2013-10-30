namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Remoting;
    using ReLi.Framework.Library.XmlLite;

    #endregion

    public interface IObjectBase : ICustomSerializer, ITransportableObject
    {
        bool Initializing
        {
            get;
        }

        void BeginInit();

        void EndInit();

        void CopyTo(IObjectBase objDestinationObject);

        IObjectBase Clone();
    }
}
