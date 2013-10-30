namespace ReLi.Framework.Library.Collections
{
    #region Using Declarations

    using System;

    #endregion

    public delegate void BeforeItemRemovedEventHandler<TObjectType>(object objSender, BeforeItemRemovedEventArgs<TObjectType> objArguments);
}
