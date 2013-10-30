namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Reflection;

    #endregion

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumAttribute : Attribute
    {
        private string _strCode;
        private string _strName;
        private string _strDescription;

        public EnumAttribute(string strCode, string strName)
            : this(strCode, strName, string.Empty)
        {}

        public EnumAttribute(string strCode, string strName, string strDescription)
        {
            Code = strCode;
            Name = strName;
            Description = strDescription;
        }

        public string Code
        {
            get
            {
                return _strCode;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Code", "A valid non-null string is required.");
                }

                _strCode = value;
            }
        }

        public string Name
        {
            get
            {
                return _strName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Name", "A valid non-null string is required.");
                }

                _strName = value;
            }
        }

        public string Description
        {
            get
            {
                return _strDescription;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Description", "A valid non-null string is required.");
                }

                _strDescription = value;
            }
        }
    }
}
