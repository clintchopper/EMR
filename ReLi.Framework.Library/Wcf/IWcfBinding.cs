namespace ReLi.Framework.Library.Wcf
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;
    using System.ServiceModel.Channels;
    using ReLi.Framework.Library;

    #endregion

    public interface IWcfBinding : IObjectBase
    {
        Binding Binding
        {
            get;
        }
    }
}

