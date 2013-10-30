namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;

    #endregion

	public class BindablePropertyChangedEventArgs : EventArgs
    {
        private string _strPropertyName;
        private object _objOldValue;
        private object _objNewValue;

        public BindablePropertyChangedEventArgs(string strPropertyName, object objOldValue, object objNewValue)
        {
            PropertyName = strPropertyName;
            OldValue = objOldValue;
            NewValue = objNewValue;
        }

        public string PropertyName
        {
            get
            {
                return _strPropertyName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("PropertyName", "A valid non-null string is required.");
                }

                _strPropertyName = value;
            }
        }

        public object OldValue
        {
            get
            {
                return _objOldValue;
            }
            protected set
            {
                _objOldValue = value;
            }
        }

        public object NewValue
        {
            get
            {
                return _objNewValue;
            }
            protected set
            {
                _objNewValue = value;
            }
        }
    }
}
