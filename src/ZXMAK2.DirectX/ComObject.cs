/* 
 *  Copyright 2008-2018 Alex Makeev
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
 *  Description: DirectX native wrapper
 *  Date: 10.07.2018
 */
using System;
using System.Reflection;
using System.Diagnostics;
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX
{
    public class ComObject : IUnknown, IDisposable
    {
        private static readonly string PointerErrorMessage = "COM Object pointer is null";

        protected unsafe void* _nativePointer;

        public unsafe IntPtr NativePointer
        {
            get { return (IntPtr)_nativePointer; }
            protected set { _nativePointer = (void*)value; }
        }

        public ComObject(IntPtr nativePointer)
        {
            NativePointer = nativePointer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (NativePointer == IntPtr.Zero)
                return;
            var iUnknown = (IUnknown)this;
            iUnknown.Release();
            NativePointer = IntPtr.Zero;
        }

        [DebuggerStepThrough]
        public T QueryInterface<T>() where T : ComObject
        {
            var guid = typeof(T).GUID;
            var iUnknown = (IUnknown)this;
            IntPtr comObjectPtr;
            var hr = iUnknown.QueryInterface(ref guid, out comObjectPtr);
            hr.CheckError();
            if (comObjectPtr == IntPtr.Zero)
                return default(T);
            return (T)((object)Activator.CreateInstance(typeof(T), new object[] { comObjectPtr }));
        }

        [DebuggerStepThrough]
        protected void QuerySelfInterfaceFrom<T>(T comObj) 
            where T : ComObject
        {
            var guid = this.GetType().GUID;
            var iUnknown = (IUnknown)comObj;
            IntPtr comObjectPtr;
            var hr = iUnknown.QueryInterface(ref guid, out comObjectPtr);
            NativePointer = comObjectPtr;
        }


        #region IUnknown

        [DebuggerStepThrough]
        unsafe HRESULT IUnknown.QueryInterface(ref Guid guid, out IntPtr comObject)
        {
            if (NativePointer == IntPtr.Zero)
                throw new InvalidOperationException(PointerErrorMessage);
            //return Marshal.QueryInterface(NativePointer, ref guid, out comObject);
            fixed (void* pGuid = &guid)
            fixed (void* pComObject = &comObject)
            {
                return (HRESULT)NativeHelper.CalliInt32(0, _nativePointer, (void*)pGuid, (void*)pComObject);
            }
        }

        [DebuggerStepThrough]
        unsafe int IUnknown.AddRef()
        {
            if (NativePointer == IntPtr.Zero)
                throw new InvalidOperationException(PointerErrorMessage);
            //return Marshal.AddRef(NativePointer);
            return NativeHelper.CalliInt32(1, _nativePointer);
        }

        [DebuggerStepThrough]
        unsafe int IUnknown.Release()
        {
            if (NativePointer == IntPtr.Zero)
                throw new InvalidOperationException(PointerErrorMessage);
            //return Marshal.Release(NativePointer);        
            return NativeHelper.CalliInt32(2, _nativePointer);
        }

        #endregion IUnknown
    }
}
