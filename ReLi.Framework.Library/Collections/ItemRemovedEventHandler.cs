namespace ReLi.Framework.Library.Collections
{
    #region Using Declarations

    using System;

    #endregion

    public delegate void ItemRemovedEventHandler<TObjectType>(object objSender, ItemRemovedEventArgs<TObjectType> objArguments);
}
