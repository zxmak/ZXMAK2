using System;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using ZXMAK2.Mvvm.Attributes;


namespace ZXMAK2.Mvvm
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly IDictionary<string, IEnumerable<string>> _dependencyTree;

        #endregion Fields


        #region .ctor

        public BaseViewModel()
        {
            _dependencyTree = DependsOnPropertyAttribute.GetDependencyTree(GetType());
            ValidateDependencyTree();
        }

        #endregion .ctor


        #region Public

        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion Public


        #region Protected

        protected bool PropertyChangeRef<T>(string propertyName, ref T fieldRef, T value)
            where T : class
        {
            if (object.ReferenceEquals(fieldRef, value))
            {
                return false;
            }
            fieldRef = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool PropertyChangeVal<T>(string propertyName, ref T fieldRef, T value)
            where T : struct
        {
            if (object.Equals(fieldRef, value))
            {
                return false;
            }
            fieldRef = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool PropertyChangeNul<T>(string propertyName, ref Nullable<T> fieldRef, Nullable<T> value)
            where T : struct
        {
            if (object.Equals(fieldRef, value))
            {
                return false;
            }
            fieldRef = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, null);
        }

        protected virtual void OnPropertyChanged(string propertyName, ISet<string> notified)
        {
            if (notified == null)
            {
                notified = new HashSet<string>();
            }
            if (notified.Contains(propertyName))
            {
                return;
            }
            ValidatePropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
            notified.Add(propertyName);

            IEnumerable<string> dependencies;
            if (!_dependencyTree.TryGetValue(propertyName, out dependencies))
            {
                return;
            }
            foreach (var name in dependencies)
            {
                OnPropertyChanged(name, notified);
            }
        }

        #endregion Protected


        #region Private

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void ValidateDependencyTree()
        {
            foreach (var name in _dependencyTree.Keys)
            {
                ValidatePropertyName(name);
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void ValidatePropertyName(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) ||
                GetType().GetProperty(propertyName) == null)
            {
                throw new ArgumentException(
                    "Invalid property name: {0}",
                    propertyName);
            }
        }

        #endregion Private
    }
}
