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
using System.Diagnostics;


namespace ZXMAK2.Mvvm.BindingTools
{
    public static class BindingServiceExtensions
    {
        [DebuggerStepThrough]
        public static void Bind(this BindingService service, object target, string propName, string path)
        {
            var binding = new BindingInfo(propName, path);
            service.Bind(target, binding);
        }

        [DebuggerStepThrough]
        public static void Bind(this BindingService service, object target, string propName, string path, IValueConverter converter)
        {
            var binding = new BindingInfo(propName, path, converter);
            service.Bind(target, binding);
        }

        [DebuggerStepThrough]
        public static void Bind(this BindingService service, object target, string propName, string path, Func<object, object> converter)
        {
            var funcConverter = new FuncConverter() { Function=converter, };
            var binding = new BindingInfo(propName, path, funcConverter);
            service.Bind(target, binding);
        }
    }
}
