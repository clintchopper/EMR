namespace ReLi.Framework.Library.XmlLite
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion

    
	public class XmlLiteElementCollection
    {
        private Dictionary<string, List<XmlLiteElement>> _objIndexedElements;

        public XmlLiteElementCollection()
        {
            Elements = new Dictionary<string, List<XmlLiteElement>>();
        }

        private Dictionary<string, List<XmlLiteElement>> Elements
        {
            get
            {
                return _objIndexedElements;
            }
            set
            {
                _objIndexedElements = value;
            }
        }

        public XmlLiteElement this[string strName]
        {
            get
            {
                if (strName == null)
                {
                    throw new ArgumentNullException("strName", "The name cannot be null.");
                }

                XmlLiteElement objElement = null;
                if (Elements.ContainsKey(strName) == true)
                {
                    objElement = Elements[strName][0];
                }

                return objElement;
            }
        }

        public int Count
        {
            get
            {
                return Elements.Count;
            }
        }

        public void Add(string strName)
        {
            Add(strName, string.Empty);
        }

        public void Add(string strName, string strValue)
        {
            XmlLiteElement objElement = new XmlLiteElement(strName, strValue);
            Add(objElement);
        }

        public void Add(XmlLiteElement objElement)
        {
            if (objElement == null)
            {
                throw new ArgumentNullException("objElement", "The element cannot be null.");
            }

            string strKey = objElement.Name;
            List<XmlLiteElement> objIndexedElements = null;
            if (_objIndexedElements.TryGetValue(strKey, out objIndexedElements) == false)
            {
                objIndexedElements = new List<XmlLiteElement>();
                _objIndexedElements.Add(strKey, objIndexedElements);
            }

            objIndexedElements.Add(objElement);
        }

        public void Clear()
        {
            Elements.Clear();
        }

        public bool Exists(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "The name cannot be null.");
            }

            bool blnExists = Elements.ContainsKey(strName);
            return blnExists;            
        }

        public XmlLiteElement GetElement(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "The name cannot be null.");
            }

            XmlLiteElement objElement = null;
            if (Elements.ContainsKey(strName) == true)
            {
                // Return the first element.
                //
                List<XmlLiteElement> objIndexedElements = Elements[strName];
                if (objIndexedElements.Count > 0)
                {
                    objElement = objIndexedElements[0];
                }
            }

            return objElement;            
        }

        public XmlLiteElement[] GetElements(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "The name cannot be null.");
            }

            XmlLiteElement[] objElements = new XmlLiteElement[0];
            if (Elements.ContainsKey(strName) == true)
            {
                List<XmlLiteElement> objIndexedElements = Elements[strName];
                objElements = objIndexedElements.ToArray();
            }

            return objElements;
        }

        public void Remove(XmlLiteElement objElement)
        {
            if (objElement == null)
            {
                throw new ArgumentNullException("objElement", "The element cannot be null.");
            }

            string strKey = objElement.Name;
            if (Elements.ContainsKey(strKey) == true)
            {
                List<XmlLiteElement> objIndexedElements = Elements[strKey];
                objIndexedElements.Remove(objElement);
            }
        }

        public XmlLiteElement[] ToArray()
        {
            List<XmlLiteElement> objElements = new List<XmlLiteElement>();
            foreach (List<XmlLiteElement> objChildElement in Elements.Values)
            {
                objElements.AddRange(objChildElement);
            }

            return objElements.ToArray();
        }  
    }
}
