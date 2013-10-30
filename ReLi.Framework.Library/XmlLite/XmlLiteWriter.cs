namespace ReLi.Framework.Library.XmlLite
{
    #region Using Declarations

    using System;
    using System.Text;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.XmlLite;

    #endregion

    public class XmlLiteWriter
    {
        #region Static Members

        public static XmlLiteDocument ToXml(IXmlConvertible objXmlConvertible)
        {
            return ToXml(objXmlConvertible, objXmlConvertible.ElementName);
        }

        public static XmlLiteDocument ToXml(IXmlConvertible objXmlConvertible, string strElementName)
        {
            if (objXmlConvertible == null)
            {
                throw new ArgumentNullException("objXmlConvertible", "A valid non-null IXmlConvertible is required.");
            }

            XmlLiteDocument objXmlLiteDocument = new XmlLiteDocument(strElementName);

            try
            {
                objXmlConvertible.WriteXml(objXmlLiteDocument.Root);
            }
            catch (Exception objException)
            {
                string strErrorMessage = "Unable to generate XML for the message type '" + objXmlConvertible.GetType().FullName + ".";
                throw new Exception(strErrorMessage, objException);
            }

            return objXmlLiteDocument;
        }

        public static string ToXml(XmlLiteElement objXmlLiteElement)
        {
            if (objXmlLiteElement == null)
            {
                throw new ArgumentNullException("objXmlLiteElement", "A valid non-null XmlLiteElement is required.");
            }

            XmlLiteDocument objXmlLiteDocument = new XmlLiteDocument(objXmlLiteElement);
            return objXmlLiteDocument.ExportToXml();
        }

        #endregion
    }
}
