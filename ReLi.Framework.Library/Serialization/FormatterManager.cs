namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Security;
    using System.Reflection;
    using ReLi.Framework.Library.XmlLite;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Security.Hash;
    using ReLi.Framework.Library.Security.Encryption;

    #endregion

    public class FormatterManager
    {
        #region Static Members
        
        private static object _objSyncObject;
        private static XmlFormatter _objXmlFormatter;
        private static BinaryFormatter _objCompressedFormatter;

        static FormatterManager()
        {
            _objSyncObject = new object();
            _objXmlFormatter = null;
            _objCompressedFormatter = null;
        }

        public static XmlFormatter XmlFormatter
        {
            get
            {
                if (_objXmlFormatter == null)
                {
                    lock (_objSyncObject)
                    {
                        if (_objXmlFormatter == null)
                        {
                            _objXmlFormatter = new XmlFormatter();
                        }
                    }
                }

                return _objXmlFormatter;
            }
        }

        public static BinaryFormatter BinaryFormatter
        {
            get
            {
                if (_objCompressedFormatter == null)
                {
                    lock (_objSyncObject)
                    {
                        if (_objCompressedFormatter == null)
                        {
                            _objCompressedFormatter = new BinaryFormatter();
                        }
                    }
                }

                return _objCompressedFormatter;
            }
        }

        #endregion
    }
}
