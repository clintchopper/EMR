namespace ReLi.Framework.Library.XmlLite
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion

    public interface IXmlConvertible
    {
        string ElementName 
        {
            get;
        }

        void WriteXml(XmlLiteElement objXmlLiteElement);

        void ReadXml(XmlLiteElement objXmlLiteElement);
    }
}
