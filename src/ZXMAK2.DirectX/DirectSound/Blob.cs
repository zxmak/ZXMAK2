using System;
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX.DirectSound
{
    /// <unmanaged>ID3D10Blob</unmanaged>	
    [Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102")]
    public class Blob : ComObject
    {
        /// <unmanaged>void* ID3D10Blob::GetBufferPointer()</unmanaged>
        public IntPtr BufferPointer
        {
            get { return this.GetBufferPointer(); }
        }

        /// <unmanaged>SIZE_T ID3D10Blob::GetBufferSize()</unmanaged>
        public PointerSize BufferSize
        {
            get
            {
                return this.GetBufferSize();
            }
        }

        public static implicit operator DataPointer(Blob blob)
        {
            return new DataPointer(blob.BufferPointer, blob.BufferSize);
        }

        public Blob(IntPtr nativePtr)
            : base(nativePtr)
        {
        }

        //public new static explicit operator Blob(IntPtr nativePointer)
        //{
        //    if (!(nativePointer == IntPtr.Zero))
        //    {
        //        return new Blob(nativePointer);
        //    }
        //    return null;
        //}

        /// <unmanaged>void* ID3D10Blob::GetBufferPointer()</unmanaged>	
        internal unsafe IntPtr GetBufferPointer()
		{
			//return calli(System.IntPtr(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)3 * (IntPtr)sizeof(void*)));
            return NativeHelper.CalliIntPtr(3, _nativePointer);
		}

        /// <unmanaged>SIZE_T ID3D10Blob::GetBufferSize()</unmanaged>	
        internal unsafe PointerSize GetBufferSize()
		{
			//return calli(System.Void*(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)4 * (IntPtr)sizeof(void*)));
            return (void*)NativeHelper.CalliIntPtr(4, _nativePointer);
		}
    }
}
