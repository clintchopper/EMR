namespace ReLi.Framework.Library.Collections
{
    #region Using Declarations

    using System;

    #endregion

	public class BeforeItemAddedEventArgs<TObjectType> : EventArgs
    {
        private bool _blnCancel;
        private TObjectType _objItem;

        public BeforeItemAddedEventArgs(TObjectType objItem)
        {
            Item = objItem;
            Cancel = false;
        }

        public TObjectType Item
        {
            get
            {
                return _objItem;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Item", "A valid non-null " + typeof(TObjectType).Name + " is required.");
                }

                _objItem = value;
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
