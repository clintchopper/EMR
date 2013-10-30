namespace ReLi.Framework.Library.Collections
{
    #region Using Declarations

    using System;

    #endregion

	public class ItemRemovedEventArgs<TObjectType> : EventArgs
    {
        private TObjectType _objItem;

        public ItemRemovedEventArgs(TObjectType objItem)
        {
            Item = objItem;
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
    }
}
