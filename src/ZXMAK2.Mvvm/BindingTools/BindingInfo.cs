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


namespace ZXMAK2.Mvvm.BindingTools
{
    public sealed class BindingInfo
    {
        public BindingInfo(string targetName, string sourcePath)
            : this (targetName, sourcePath, null, null, BindingTrigger.Default)
        {
        }

        public BindingInfo(string targetName, string sourcePath, IValueConverter converter)
            : this(targetName, sourcePath, converter, null, BindingTrigger.Default)
        {
        }

        public BindingInfo(string targetName, string sourcePath, IValueConverter converter, object converterParameter)
            : this(targetName, sourcePath, converter, converterParameter, BindingTrigger.Default)
        {
        }

        public BindingInfo(
            string targetName,
            string sourcePath,
            IValueConverter converter,
            object converterParameter,
            BindingTrigger trigger)
        {
            SourcePath = sourcePath;
            TargetName = targetName;
            Converter = converter;
            ConverterParameter = converterParameter;
            Trigger = trigger;
        }

        public string TargetName { get; private set; }
        public string SourcePath { get; private set; }
        public IValueConverter Converter { get; private set; }
        public object ConverterParameter { get; private set; }
        public BindingTrigger Trigger { get; private set; }
    
        /// <summary>
        /// Used as a returned value to instruct the binding engine not to perform any action.
        /// </summary>
        public static readonly object DoNothing = new object();
    }
}
