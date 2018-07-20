/* 
 *  Copyright 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *  
 * 
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;


namespace ZXMAK2.Mvvm.BindingTools
{
    public class BindingObserver
    {
        #region Fields

        private readonly Dictionary<string, BindingObserver> _subObservers = new Dictionary<string, BindingObserver>();
        private readonly Dictionary<string, int> _nameRefs = new Dictionary<string, int>();
        private readonly Dictionary<string, PropertyInfo> _propInfoCache = new Dictionary<string, PropertyInfo>();
        private object _dataContext;

        #endregion Fields

        
        #region Properties

        public event EventHandler<ObserverPropertyChangedEventArgs> PropertyChanged;
        
        public string Name { get; set; }
        
        public bool IsEmpty 
        { 
            get 
            { 
                return _nameRefs.Count == 0 && 
                    _subObservers.Count == 0; 
            } 
        }

        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                if (object.Equals(_dataContext, value))
                {
                    return;
                }
                var notify = _dataContext as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged -= OnDataContextPropertyChanged;
                }
                _dataContext = value;
                OnCacheRefresh();
                notify = _dataContext as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged += OnDataContextPropertyChanged;
                }
                OnDataContextChanged();
            }
        }

        #endregion Properties


        #region Public

        public void Register(string path)
        {
            var name = path;
            var subName = string.Empty;
            var dotIndex = path.IndexOf('.');
            if (dotIndex >= 0)
            {
                name = path.Substring(0, dotIndex);
                var subPos = dotIndex + 1;
                subName = path.Substring(subPos, path.Length - subPos);
            }
            if (subName.Length > 0)
            {
                if (!_subObservers.ContainsKey(name))
                {
                    OnCacheAdd(name);
                    var subObserver = new BindingObserver();
                    subObserver.Name = name;

                    var propInfo = _dataContext != null && _propInfoCache.ContainsKey(name) ? _propInfoCache[name] : null;
                    var dataContext = _dataContext != null && propInfo.CanRead ? propInfo.GetValue(_dataContext, null) : null;
                    subObserver.DataContext = _dataContext != null && propInfo.CanRead ? propInfo.GetValue(_dataContext, null) : null;

                    subObserver.PropertyChanged += SubObserver_OnPropertyChanged;
                    _subObservers[name] = subObserver;
                }
                _subObservers[name].Register(subName);
            }
            else
            {
                if (!_nameRefs.ContainsKey(name))
                {
                    _nameRefs[name] = 0;
                }
                _nameRefs[name]++;
                OnCacheAdd(name);
                
                // init
                var handler = PropertyChanged;
                if (handler != null)
                {
                    var args = new ObserverPropertyChangedEventArgs(name, DataContext, GetProperty(name));
                    handler(this, args);
                }
            }
        }

        public void Unregister(string path)
        {
            var name = path;
            var subName = string.Empty;
            var dotIndex = path.IndexOf('.');
            if (dotIndex >= 0)
            {
                name = path.Substring(0, dotIndex);
                var subPos = dotIndex + 1;
                subName = path.Substring(subPos, path.Length - subPos);
            }
            if (subName.Length > 0)
            {
                if (!_subObservers.ContainsKey(name))
                {
                    // path not found
                    return;
                }
                _subObservers[name].Unregister(subName);
                if (_subObservers[name].IsEmpty)
                {
                    _subObservers.Remove(name);
                }
            }
            else
            {
                if (!_nameRefs.ContainsKey(name))
                {
                    // path not found
                    return;
                }
                _nameRefs[name]--;
                if (_nameRefs[name] == 0)
                {
                    _nameRefs.Remove(name);
                }
            }
        }

        public PropertyInfo GetProperty(string path)
        {
            if (_dataContext == null)
            {
                return null;
            }
            var name = path;
            var subName = string.Empty;
            var dotIndex = path.IndexOf('.');
            if (dotIndex >= 0)
            {
                name = path.Substring(0, dotIndex);
                var subPos = dotIndex + 1;
                subName = path.Substring(subPos, path.Length - subPos);
            }
            if (subName.Length > 0)
            {
                return _subObservers[name].GetProperty(subName);
            }
            if (!_propInfoCache.ContainsKey(name))
            {
                return null;
            }
            return _propInfoCache[name];
        }

        public Type GetPropertyType(string path)
        {
            if (_dataContext == null)
            {
                return null;
            }
            var name = path;
            var subName = string.Empty;
            var dotIndex = path.IndexOf('.');
            if (dotIndex >= 0)
            {
                name = path.Substring(0, dotIndex);
                var subPos = dotIndex + 1;
                subName = path.Substring(subPos, path.Length - subPos);
            }
            if (subName.Length > 0)
            {
                return _subObservers[name].GetPropertyType(subName);
            }
            if (!_propInfoCache.ContainsKey(name))
            {
                return null;
            }
            var propInfo = _propInfoCache[name];
            if (propInfo == null)
            {
                return null;
            }
            return propInfo.PropertyType;
        }

        public object GetPropertyValue(string path)
        {
            if (_dataContext == null)
            {
                return BindingInfo.DoNothing;
            }
            var name = path;
            var subName = string.Empty;
            var dotIndex = path.IndexOf('.');
            if (dotIndex >= 0)
            {
                name = path.Substring(0, dotIndex);
                var subPos = dotIndex + 1;
                subName = path.Substring(subPos, path.Length - subPos);
            }
            if (subName.Length > 0)
            {
                return _subObservers[name].GetPropertyValue(subName);
            }
            if (!_propInfoCache.ContainsKey(name))
            {
                return BindingInfo.DoNothing;
            }
            var propInfo = _propInfoCache[name];
            if (propInfo == null || !propInfo.CanRead)
            {
                return BindingInfo.DoNothing;
            }
            return propInfo.GetValue(_dataContext, null);
        }

        public void SetPropertyValue(string path, object value)
        {
            if (_dataContext == null || value == BindingInfo.DoNothing)
            {
                return;
            }
            var name = path;
            var subName = string.Empty;
            var dotIndex = path.IndexOf('.');
            if (dotIndex >= 0)
            {
                name = path.Substring(0, dotIndex);
                var subPos = dotIndex + 1;
                subName = path.Substring(subPos, path.Length - subPos);
            }
            if (subName.Length > 0)
            {
                _subObservers[name].SetPropertyValue(subName, value);
                return;
            }
            if (!_propInfoCache.ContainsKey(name))
            {
                return;
            }
            var propInfo = _propInfoCache[name];
            if (propInfo == null || !propInfo.CanWrite)
            {
                return;
            }
            propInfo.SetValue(_dataContext, value, null);
        }

        #endregion Public


        #region Private

        private void OnCacheAdd(string name)
        {
            if (_dataContext == null)
            {
                return;
            }
            var type = _dataContext.GetType();
            _propInfoCache[name] = type.GetProperty(name);
        }

        private void OnCacheRefresh()
        {
            _propInfoCache.Clear();
            if (_dataContext == null)
            {
                return;
            }
            var type = _dataContext.GetType();
            var names = _subObservers.Keys
                .Concat(_nameRefs.Keys)
                .Distinct();
            foreach (var name in names)
            {
                _propInfoCache[name] = type.GetProperty(name);
            }
        }

        private void SubObserver_OnPropertyChanged(object sender, ObserverPropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var subObserver = sender as BindingObserver;
                var args = new ObserverPropertyChangedEventArgs(
                    subObserver.Name + "." + e.Path, 
                    e.DataContext,
                    e.PropertyInfo);
                handler(this, args);
            }
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_subObservers.ContainsKey(e.PropertyName))
            {
                var propInfo = _dataContext != null && _propInfoCache.ContainsKey(e.PropertyName) ? _propInfoCache[e.PropertyName] : null;
                var dataContext = _dataContext != null && propInfo.CanRead ? propInfo.GetValue(_dataContext, null) : null;
                _subObservers[e.PropertyName].DataContext = dataContext;
            }
            if (!_nameRefs.ContainsKey(e.PropertyName))
            {
                // do not raise events for non-registered path (sub observer will do it)
                return;
            }
            var handler = PropertyChanged;
            if (handler != null && _nameRefs.ContainsKey(e.PropertyName))
            {
                var propInfo = _dataContext != null && _propInfoCache.ContainsKey(e.PropertyName) ? _propInfoCache[e.PropertyName] : null;
                var args = new ObserverPropertyChangedEventArgs(
                    e.PropertyName,
                    DataContext,
                    propInfo);
                handler(this, args);
            }
        }

        private void OnDataContextChanged()
        {
            var names = _subObservers.Keys
                .Concat(_nameRefs.Keys)
                .Distinct();
            foreach (var name in names)
            {
                OnDataContextPropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion Private
    }
}
