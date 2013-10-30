namespace ReLi.Framework.Library.Collections
{
    #region Using Declarations

    using System;

    #endregion

    public delegate void ItemAddedEventHandler<TObjectType>(object objSender, ItemAddedEventArgs<TObjectType> objArguments);
}
