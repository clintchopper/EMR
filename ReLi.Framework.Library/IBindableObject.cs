namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.ComponentModel;

    #endregion

    public interface IBindableObject : INotifyPropertyChanged, INotifyPropertyChanging 
    {
        event BindablePropertyChangingEventHandler BindablePropertyChanging;

        event BindablePropertyChangedEventHandler BindablePropertyChanged;

        event BindablePropertyChangedEventHandler BindablePropertyChangedAsync;

        event PropertyChangedEventHandler PropertyChangedAsync;
    }
}
