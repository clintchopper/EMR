namespace ReLi.Framework.Library.XmlLite
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion

    
	public class XmlLiteAttribute
    {
        private string _strName;
        private string _strValue;

        public XmlLiteAttribute(string strName)
            : this(strName, string.Empty)
        {}

        public XmlLiteAttribute(string strName, string strValue)
        {
            Name = strName;
            Value = strValue;
        }

        public string Name
        {
            get
            {
                return _strName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "The name cannot be null.");
                }
                if (value.Trim().Length == 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The name cannot be an empty string.");
                }

                _strName = value;
            }
        }

        public string Value
        {
            get
            {
                return _strValue;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "The value cannot be null.");
                }

                _strValue = value;
            }
        }
    }

}
