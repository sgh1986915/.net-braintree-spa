using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Sitter.Toolbox.Model
{
    /// <summary> 
    /// This is a lightweight ViewModel base class which is used by hybrid model classes which are not wrapped in a ViewModel. t only impliments INotifyPropertyChanged.
    /// Another reason for this class is that ViewModelBase is located in the UI layer (CommonApp.CommonQuan) and is not accessible by CdbDatabaseManager in the data layer.
    /// See also: ViewModelBase
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isDirty;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyFunction)
        {
            var body = propertyFunction.Body as MemberExpression;
            if (body != null)
                OnPropertyChanged(body.Member.Name);
        }

        public bool PropertyChangedWillBeObserved
        {
            get { return PropertyChanged != null; }
        }

        public virtual void RefreshBindings()
        {
            foreach (PropertyInfo propInfo in GetType().GetProperties())
            {
                OnPropertyChanged(propInfo.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value != _isDirty)
                {
                    _isDirty = value;
                    OnPropertyChanged(() => IsDirty);
                    DirtyStateChanged(value);
                }
            }
        }

        protected virtual void DirtyStateChanged(bool newDirtyState)
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
