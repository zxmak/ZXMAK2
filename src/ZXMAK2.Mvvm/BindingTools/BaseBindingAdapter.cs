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
using System.ComponentModel;


namespace ZXMAK2.Mvvm.BindingTools
{
    public abstract class BaseBindingAdapter : IBindingAdapter
    {
        protected BaseBindingAdapter(object target)
        {
            Target = target;
            TargetType = target.GetType();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        #region IBindingAdapter

        public event EventHandler<TargetPropertyChangedEventArgs> PropertyChanged;

        public virtual BindingTrigger GetDefaultPropertyTrigger(string name)
        {
            return BindingTrigger.PropertyChanged;
        }

        public virtual Type GetTargetPropertyType(string name)
        {
            var propInfo = TargetType.GetProperty(name);
            return propInfo != null ? propInfo.PropertyType : null;
        }

        public virtual object GetTargetPropertyValue(string name)
        {
            var propInfo = TargetType.GetProperty(name);
            if (!propInfo.CanRead)
            {
                return BindingInfo.DoNothing;
            }
            return propInfo != null ? propInfo.GetValue(Target, null) : null;
        }

        public virtual void SetTargetPropertyValue(string name, object value)
        {
            var propInfo = TargetType.GetProperty(name);
            if (propInfo == null || !propInfo.CanWrite)
            {
                return;
            }
            propInfo.SetValue(Target, value, null);
        }

        #endregion IBindingAdapter


        #region Protected

        protected object Target { get; private set; }
        protected Type TargetType { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected void OnPropertyChanged(string name, BindingTrigger trigger)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var args = new TargetPropertyChangedEventArgs(trigger, name);
                handler(this, args);
            }
        }

        #endregion Protected
    }
}
