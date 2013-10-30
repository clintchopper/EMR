namespace ReLi.Framework.Library.XmlLite
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion
     
    
	public class XmlLiteAttributeCollection
    {
        private Dictionary<string, XmlLiteAttribute> _objAttributes;

        public XmlLiteAttributeCollection()
        {
            _objAttributes = new Dictionary<string, XmlLiteAttribute>();
        }

        public int Count
        {
            get
            {
                return _objAttributes.Count;
            }
        }

        public XmlLiteAttribute this[string strName]
        {
            get
            {
                if (strName == null)
                {
                    throw new ArgumentNullException("strName", "The name cannot be null.");
                }

                XmlLiteAttribute objAttribute = null;
                _objAttributes.TryGetValue(strName, out objAttribute);

                return objAttribute;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "The attribute cannot be null.");
                }

                _objAttributes[value.Name] = value;
            }
        }

        public void Add(string strName)
        {
            Add(strName, string.Empty);
        }

        public void Add(string strName, string strValue)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "The name cannot be null.");
            }

            XmlLiteAttribute objAttribute = new XmlLiteAttribute(strName, strValue);
            this[strName] = objAttribute;
        }

        public void Add(XmlLiteAttribute objAttribute)
        {
            if (objAttribute == null)
            {
                throw new ArgumentNullException("objAttribute", "The attribute cannot be null.");
            }

            this[objAttribute.Name] = objAttribute;
        }

        public XmlLiteAttribute Attribute(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "The name cannot be null.");
            }

            XmlLiteAttribute objAttribute = this[strName];
            return objAttribute;
        }

        public bool Exists(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "The name cannot be null.");
            }

            bool blnExists = _objAttributes.ContainsKey(strName);
            return blnExists;
        }

        public string GetValue(string strName, string strDefaultValue)
        {
            string strValue = strDefaultValue;

            XmlLiteAttribute objAttribute = this[strName];
            if (objAttribute != null)
            {
                strValue = objAttribute.Value;
            }

            return strValue;
        }

        public void Clear()
        {
            _objAttributes.Clear();
        }

        public void Remove(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "The name cannot be null.");
            }

            if (_objAttributes.ContainsKey(strName) == true)
            {
                _objAttributes.Remove(strName);
            }
        }

        public XmlLiteAttribute[] ToArray()
        {
            XmlLiteAttribute[] arrAttributes = new XmlLiteAttribute[_objAttributes.Count];
            _objAttributes.Values.CopyTo(arrAttributes, 0);

            return arrAttributes;
        }
    }
}
