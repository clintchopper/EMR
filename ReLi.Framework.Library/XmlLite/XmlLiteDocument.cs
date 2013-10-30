namespace ReLi.Framework.Library.XmlLite
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Security;
    using System.Collections.Generic;

    #endregion

    
	public class XmlLiteDocument
    {
        private XmlLiteElement _objRootElement;

        public XmlLiteDocument(string strRootName)
            : this(new XmlLiteElement(strRootName))
        {}

        public XmlLiteDocument(XmlLiteElement objRootElement)
        {
            if (objRootElement == null)
            {
                throw new ArgumentNullException("objRootElement", "The root element cannot be null");
            }

            _objRootElement = objRootElement;
        }

        public XmlLiteElement Root
        {
            get
            {
                return _objRootElement;
            }
        }

        public string ExportToXml()
        {
            string strXml = string.Empty;

            using (StringWriter objStringWriter = new StringWriter())
            {
                GenerateXml(_objRootElement, objStringWriter, 0);
                strXml = objStringWriter.ToString();
            }

            return strXml;
        }

        public void ExportToFile(string strFilePath)
        {
            using (TextWriter objTextWriter = new StreamWriter(strFilePath))
            {
                ((StreamWriter)objTextWriter).AutoFlush = false;
                GenerateXml(_objRootElement, objTextWriter, 0);
            }
        }

        public void ExportToStream(Stream objStream)
        {
            TextWriter objTextWriter = new StreamWriter(objStream);
             ((StreamWriter)objTextWriter).AutoFlush = false;
             GenerateXml(_objRootElement, objTextWriter, 0);
             objTextWriter.Flush();
        }

        public string GenerateReadableString()
        {
            List<string> objStringParts = new List<string>();
            GenerateReadableString(_objRootElement, objStringParts, string.Empty);

            string strResult = string.Join(Environment.NewLine, objStringParts.ToArray());
            return strResult;
        }

        private void GenerateReadableString(XmlLiteElement objElement, List<string> objStringParts, string strRootValue)
        {
            string strEntryName = string.Empty;
            if (strRootValue.Length > 0)
            {
                strEntryName = string.Format("{0}.{1}", strRootValue, objElement.Name);
            }
            else
            {
                strEntryName = objElement.Name;
            }

            if (objElement.Value.Length > 0)
            {
                strEntryName = string.Format("{0} = {1}", strEntryName, objElement.Value);
                objStringParts.Add(strEntryName);
            }
            else
            {
                if ((objStringParts.Count > 0) && (objStringParts[objStringParts.Count - 1] != string.Empty))
                {
                    objStringParts.Add(string.Empty);
                }
                foreach (XmlLiteElement objChildElement in objElement.Elements.ToArray())
                {
                    GenerateReadableString(objChildElement, objStringParts, strEntryName);
                }
            }
        }

        public XmlReader ExportToReader()
        {
            XmlReader objXmlReader = null;

            try
            {
                XmlReaderSettings objReaderSettings = new XmlReaderSettings();
                objReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

                using (MemoryStream objStream = new MemoryStream())
                {
                    ExportToStream(objStream);
                    objXmlReader = XmlReader.Create(objStream, objReaderSettings);
                }
            }
            catch (Exception objException)
            {
                ExceptionHelper objExceptionHelper = new ExceptionHelper(objException);
                string strErrorMessage = objExceptionHelper.GetDetailedErrorMessage("The following error was encountered while building the xml reader:\n");
                throw new Exception(strErrorMessage);
            }

            return objXmlReader;
        }

        private void GenerateXml(XmlLiteElement objElement, TextWriter objTextWriter, int intIndentLevel)
        {
            string strElementName = objElement.Name;
            string strIndentLevel = new string('\t', intIndentLevel);

            objTextWriter.Write("{0}<{1}", strIndentLevel, strElementName);
            foreach (XmlLiteAttribute objAttribute in objElement.Attributes.ToArray())
            {
                objTextWriter.Write(" {0}=\"{1}\"", objAttribute.Name, objAttribute.Value);
            }

            bool blnIsEmpty = ((objElement.Value.Length == 0) && (objElement.Elements.Count == 0));
            if (blnIsEmpty == false)
            {
                objTextWriter.Write(">");
                if (objElement.Value.Length > 0)
                {
                    objTextWriter.Write(SecurityElement.Escape(objElement.Value));
                }

                if (objElement.Elements.Count > 0)
                {
                    objTextWriter.WriteLine();
                    intIndentLevel += 1;
                    foreach (XmlLiteElement objChildElement in objElement.Elements.ToArray())
                    {
                        GenerateXml(objChildElement, objTextWriter, intIndentLevel);
                    }
                    intIndentLevel -= 1;
                    objTextWriter.Write(new string('\t', intIndentLevel));
                }

                objTextWriter.WriteLine("</{0}>", strElementName);
            }
            else
            {
                objTextWriter.WriteLine("/>");
            }
        }

        #region Static Members

        public static XmlLiteDocument LoadFromXml(string strXml)
        {
            if ((strXml == null) || (strXml.Length == 0))
            {
                throw new ArgumentException("The supplied xml value cannot contain null or an empty string.", "strFilePath");
            }

            XmlLiteDocument objXmlDocument = null;

            try
            {
                using (StringReader objStringReader = new StringReader(strXml))
                {
                    XmlReaderSettings objReaderSettings = new XmlReaderSettings();
                    objReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

                    using (XmlReader objXmlReader = XmlReader.Create(objStringReader, objReaderSettings))
                    {
                        objXmlDocument = LoadFromReader(objXmlReader);
                    }
                }
            }
            catch (Exception objException)
            {
                ExceptionHelper objExceptionHelper = new ExceptionHelper(objException);
                string strErrorMessage = objExceptionHelper.GetDetailedErrorMessage("The following error was encountered while parsing the following xml:\n" + strXml + "\n\n");
                throw new Exception(strErrorMessage);
            }
            
            return objXmlDocument;
        }

        public static XmlLiteDocument LoadFromFile(string strFilePath)
        {
            if ((strFilePath == null) || (strFilePath.Length == 0))
            {
                throw new ArgumentException("A valid path to the file is required..", "strFilePath");
            }
            bool blnFileExists = File.Exists(strFilePath);
            if (blnFileExists == false)
            {
                throw new FileNotFoundException("The specified file does not exist", strFilePath);
            }

            XmlLiteDocument objXmlDocument = null;

            try
            {
                using (FileStream objFileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    objXmlDocument = LoadFromStream(objFileStream);
                }
            }
            catch (Exception objException)
            {
                ExceptionHelper objExceptionHelper = new ExceptionHelper(objException);
                string strErrorMessage = objExceptionHelper.GetDetailedErrorMessage("The following error was encountered while building the xml document for '" + strFilePath + "':\n");
                throw new Exception(strErrorMessage);
            }
            
            return objXmlDocument;
        }

        public static XmlLiteDocument LoadFromStream(Stream objStream)
        {
            if (objStream == null) 
            {
                throw new ArgumentNullException("objStream", "A valid non-null Stream is required.");
            }

            XmlLiteDocument objXmlDocument = null;

            try
            {
                XmlReaderSettings objReaderSettings = new XmlReaderSettings();
                objReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

                using (XmlReader objXmlReader = XmlReader.Create(objStream, objReaderSettings))
                {
                    objXmlDocument = LoadFromReader(objXmlReader);
                }
            }
            catch (Exception objException)
            {
                ExceptionHelper objExceptionHelper = new ExceptionHelper(objException);
                string strErrorMessage = objExceptionHelper.GetDetailedErrorMessage("The following error was encountered while building the xml document for the supplied stream:\n");
                throw new Exception(strErrorMessage);
            }

            return objXmlDocument;
        }

        public static XmlLiteDocument LoadFromReader(XmlReader objXmlReader)
        {
            if (objXmlReader == null)
            {
                throw new ArgumentNullException("objXmlReader", "The xml reader must represent a valid instance.");
            }
            
            int intDepth = 0;
            XmlLiteElement objRootElement = null;
            XmlLiteElement objCurrentElement = null;
            Stack<XmlLiteElement> objElements = new Stack<XmlLiteElement>();

            while (objXmlReader.EOF == false)
            {
                objXmlReader.Read();
                switch (objXmlReader.NodeType)
                {
                    case XmlNodeType.Element:

                        intDepth = objXmlReader.Depth;
                        objCurrentElement = new XmlLiteElement(objXmlReader.LocalName);
                        if (objRootElement == null)
                        {
                            objRootElement = objCurrentElement;
                            objElements.Push(objCurrentElement);
                        }
                        else
                        {
                            XmlLiteElement objParentElement = objElements.Peek();
                            objParentElement.Elements.Add(objCurrentElement);

                            if (objXmlReader.IsEmptyElement == false)
                            {
                                objElements.Push(objCurrentElement);
                            }
                        }
                        if (objXmlReader.HasAttributes == true)
                        {
                            while (objXmlReader.MoveToNextAttribute() == true)
                            {
                                objCurrentElement.Attributes.Add(objXmlReader.Name, objXmlReader.Value);
                            }
                        }

                        break;

                    case XmlNodeType.Attribute:
                        objCurrentElement.Attributes.Add(objXmlReader.Name, objXmlReader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        objElements.Pop();
                        break;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:

                        if (intDepth == objXmlReader.Depth)
                        {
                            XmlLiteElement objParentElement = objElements.Peek();
                            objParentElement.Value = objXmlReader.Value;
                        }
                        else
                        {
                            objCurrentElement.Value = objXmlReader.Value;
                        }
                        break;

                    default:
                        break;
                }
            }

            XmlLiteDocument objDocument = new XmlLiteDocument(objRootElement);
            return objDocument;
        }

        #endregion
    }
}
