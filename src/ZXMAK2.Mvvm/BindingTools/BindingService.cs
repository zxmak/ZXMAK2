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
using System.Globalization;
using System.Reflection;
using System.Diagnostics;


namespace ZXMAK2.Mvvm.BindingTools
{
    public sealed class BindingService : IDisposable
    {
        #region Fields

        private readonly BindingObserver _sourceObserver = new BindingObserver();
        private readonly Dictionary<Type, Func<object, IBindingAdapter>> _adapterFactories = new Dictionary<Type, Func<object, IBindingAdapter>>();
        private readonly Dictionary<object, IBindingAdapter> _targetAdapters = new Dictionary<object, IBindingAdapter>();
        private readonly Dictionary<object, List<BindingInfo>> _targetBindings = new Dictionary<object, List<BindingInfo>>();
        private readonly IValueConverter _defaultConverter = new DefaultConverter();
        private readonly HashSet<UpdateInfo> _activeUpdate = new HashSet<UpdateInfo>();

        #endregion Fields


        #region .ctor

        public BindingService()
        {
            _sourceObserver.PropertyChanged += Observer_OnPropertyChanged;
        }

        public void Dispose()
        {
            _targetAdapters.Keys
                .ToList()
                .ForEach(RemoveTarget);
            _sourceObserver.PropertyChanged -= Observer_OnPropertyChanged;
            DataContext = null;
        }

        #endregion .ctor


        #region Public

        public object DataContext
        {
            get { return _sourceObserver.DataContext; }
            set { _sourceObserver.DataContext = value; }
        }

        public void RegisterAdapterFactory<T>(Func<T, IBindingAdapter> adapterFactory)
        {
            _adapterFactories[typeof(T)] = arg => adapterFactory((T)arg);
        }

        [DebuggerStepThrough]
        public void Bind(object target, BindingInfo binding)
        {
            if (!_targetAdapters.ContainsKey(target))
            {
                var adapterFactory = FindTargetAdapterFactory(target.GetType());
                _targetAdapters[target] = adapterFactory(target);
                _targetAdapters[target].PropertyChanged += Target_OnPropertyChanged;
                _targetBindings[target] = new List<BindingInfo>();
            }
            if (_targetAdapters[target].GetTargetPropertyType(binding.TargetName) == null)
            {
                throw new ArgumentException(
                    string.Format("Property not found: {0}", binding.TargetName),
                    "binding");
            }
            _targetBindings[target].Add(binding);
            _sourceObserver.Register(binding.SourcePath);
        }

        public void RemoveTarget(object target)
        {
            if (!_targetAdapters.ContainsKey(target))
            {
                return;
            }
            _targetAdapters[target].Dispose();
            _targetAdapters.Remove(target);
            _targetBindings.Remove(target);
        }

        #endregion Public


        #region Private

        private void Observer_OnPropertyChanged(object sender, ObserverPropertyChangedEventArgs e)
        {
            _targetBindings
                .SelectMany(pair => pair.Value.Select(arg => new { Target = pair.Key, Binding = arg, }))
                .Where(arg => arg.Binding.SourcePath == e.Path)
                .ToList()
                .ForEach(arg => UpdateTarget(arg.Target, arg.Binding));
        }

        private void Target_OnPropertyChanged(object sender, TargetPropertyChangedEventArgs e)
        {
            var target = _targetAdapters
                .First(pair => pair.Value == sender)
                .Key;
            _targetBindings[target]
                .Where(arg => arg.TargetName == e.PropertyName)
                .ToList()
                .ForEach(arg => UpdateSource(target, arg, e.Trigger));
        }

        private void UpdateTarget(
            object target, 
            BindingInfo binding)
        {
            var updateInfo = new UpdateInfo() { Target=target, Binding=binding, };
            if (_activeUpdate.Contains(updateInfo))
            {
                return;
            }
            _activeUpdate.Add(updateInfo);
            try
            {
                var sourceValue = _sourceObserver.GetPropertyValue(binding.SourcePath);
                if (sourceValue == BindingInfo.DoNothing)
                {
                    return;
                }
                var adapter = _targetAdapters[target];
                var targetType = adapter.GetTargetPropertyType(binding.TargetName);
                if (targetType == null)
                {
                    return;
                }

                var converter = binding.Converter ?? _defaultConverter;
                var value = converter.Convert(
                    sourceValue, 
                    targetType, 
                    binding.ConverterParameter, 
                    CultureInfo.CurrentCulture);
                if (value == BindingInfo.DoNothing)
                {
                    return;
                }
                adapter.SetTargetPropertyValue(binding.TargetName, value);
            }
            finally
            {
                _activeUpdate.Remove(updateInfo);
            }
        }

        private void UpdateSource(
            object target, 
            BindingInfo binding, 
            BindingTrigger trigger)
        {
            var updateInfo = new UpdateInfo() { Target=target, Binding=binding, };
            if (_activeUpdate.Contains(updateInfo))
            {
                return;
            }
            _activeUpdate.Add(updateInfo);
            try
            {
                var sourceType = _sourceObserver.GetPropertyType(binding.SourcePath);
                if (sourceType == null)
                {
                    return;
                }
                var adapter = _targetAdapters[target];
                var triggerExpected = binding.Trigger;
                if (triggerExpected == BindingTrigger.Default)
                {
                    triggerExpected = adapter.GetDefaultPropertyTrigger(binding.TargetName);
                }
                if (trigger != triggerExpected)
                {
                    return;
                }
                var targetType = adapter.GetTargetPropertyType(binding.TargetName);
                if (targetType == null)
                {
                    return;
                }

                var targetValue = adapter.GetTargetPropertyValue(binding.TargetName);
                if (targetValue == BindingInfo.DoNothing)
                {
                    return;
                }
                var converter = binding.Converter ?? _defaultConverter;
                var value = converter.ConvertBack(
                    targetValue, 
                    sourceType, 
                    binding.ConverterParameter, 
                    CultureInfo.CurrentCulture);
                if (value == BindingInfo.DoNothing)
                {
                    return;
                }
                _sourceObserver.SetPropertyValue(binding.SourcePath, value);
            }
            finally
            {
                _activeUpdate.Remove(updateInfo);
            }
        }


        private Func<object, IBindingAdapter> FindTargetAdapterFactory(Type type)
        {
            if (_adapterFactories.ContainsKey(type))
            {
                return _adapterFactories[type];
            }
            if (type.BaseType != null)
            {
                return FindTargetAdapterFactory(type.BaseType);
            }
            return arg => new DefaultBindingAdapter(arg);
        }

        #endregion Private


        private struct UpdateInfo
        {
            public object Target;
            public BindingInfo Binding;
        }
    }
}
