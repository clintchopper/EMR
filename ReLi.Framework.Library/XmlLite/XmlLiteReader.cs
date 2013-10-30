namespace ReLi.Framework.Library.XmlLite
{
    #region Using Declarations

    using System;
    using System.Text;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.XmlLite;

    #endregion

    public class XmlLiteReader
    {
        #region Static Members

        public static TObjectType FromXml<TObjectType>(string strXml)
            where TObjectType : IXmlConvertible, new()
        {
            if (strXml == null)
            {
                throw new ArgumentNullException("strXml", "A valid non-null string is required.");
            }

            XmlLiteDocument objXmlLiteDocument = null;
            try
            {
                objXmlLiteDocument = XmlLiteDocument.LoadFromXml(strXml);
            }
            catch (Exception objException)
            {
                string strErrorMessage = "Unable to load the XML fragment:\n" + strXml;
                throw new Exception(strErrorMessage, objException);
            }

            return FromXml<TObjectType>(objXmlLiteDocument);
        }

        public static TObjectType FromXml<TObjectType>(XmlLiteDocument objXmlLiteDocument)
            where TObjectType : IXmlConvertible, new()
        {
            if (objXmlLiteDocument == null)
            {
                throw new ArgumentNullException("objXmlLiteDocument", "A valid non-null XmlLiteDocument is required.");
            }

            TObjectType objObject = default(TObjectType);
            try
            {
                objObject = new TObjectType();
                objObject.ReadXml(objXmlLiteDocument.Root);
            }
            catch (Exception objException)
            {
                string strErrorMessage = "Unable to create message from XML.";
                throw new Exception(strErrorMessage, objException);
            }

            return objObject;
        }

        #endregion
    }
}
