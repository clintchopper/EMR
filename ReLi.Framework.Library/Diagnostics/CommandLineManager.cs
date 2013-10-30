namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations
    
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    #endregion
    
    public class CommandLineManager
    {
        private string _strArguments;
        private Dictionary<string, string> _objArguments;

        public CommandLineManager()
        {
            List<string> objCommandLineArguments = new List<string>(Environment.GetCommandLineArgs());
            objCommandLineArguments.RemoveAt(0);

            Arguments = string.Join(" ", objCommandLineArguments.ToArray());
            _objArguments = new Dictionary<string,string>();

            Regex objSplitter = new Regex(@"^/|=|:",RegexOptions.IgnoreCase|RegexOptions.Compiled);
            Regex objRemover =  new Regex(@"^['""]?(.*?)['""]?$",RegexOptions.IgnoreCase|RegexOptions.Compiled);

            string strArgument = null;
            string[] strParts = null;

            foreach (string strValue in objCommandLineArguments)
            {
                strParts = objSplitter.Split(strValue, 3);
                switch(strParts.Length)
                {
                    case(1):
                        
                        if(strArgument != null)
                        {
                            if(_objArguments.ContainsKey(strArgument))
                            {
                                strParts[0] = objRemover.Replace(strParts[0],"$1");
                                _objArguments.Add(strArgument, strParts[0]);
                            }
                            
                            strArgument = null;
                        } 
                        break;

                    case (2):

                        if (strArgument != null)
                        {
                            if (_objArguments.ContainsKey(strArgument) == false)
                            {
                                _objArguments.Add(strArgument, "true");
                            }                            
                        }

                        strArgument = strParts[1];
                        break;
 
                    case (3):

                        if (strArgument != null)
                        {
                            if (_objArguments.ContainsKey(strArgument) == false)
                            {
                                _objArguments.Add(strArgument, "true");
                            }
                        }

                        strArgument = strParts[1];

                        if(_objArguments.ContainsKey(strArgument) == false)
                        {
                            strParts[2]= objRemover.Replace(strParts[2],"$1");
                            _objArguments.Add(strArgument, strParts[2]);
                        }
                        
                        strArgument=null;
                        break;
                }
            }

            if (strArgument != null)
            {
                if (_objArguments.ContainsKey(strArgument) == false)
                {
                    _objArguments.Add(strArgument, "true");
                }
            }
        }

        public string this[string strArgumentName]
        {
            get
            {
                string strValue = null;

                if (_objArguments.ContainsKey(strArgumentName) == true)
                {
                    strValue = _objArguments[strArgumentName];
                }

                return strValue;
            }
        }

        public string Arguments
        {
            get
            {
                return _strArguments;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Arguments", "A valid non-null string is required.");
                }

                _strArguments = value;
            }
        }
    }
}

