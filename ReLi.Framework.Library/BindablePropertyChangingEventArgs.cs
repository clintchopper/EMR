namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;

    #endregion

	public class BindablePropertyChangingEventArgs : EventArgs
    {
        private string _strPropertyName;
        private object _objOldValue;
        private object _objNewValue;
        private bool _blnCancel;

        public BindablePropertyChangingEventArgs(string strPropertyName, object objOldValue, object objNewValue)
        {
            PropertyName = strPropertyName;
            OldValue = objOldValue;
            NewValue = objNewValue;
            Cancel = false;
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

        public bool Cancel
        {
            get
            {
                return _blnCancel;
            }
            set
            {
                _blnCancel = value;
            }
        }
    }
}
    