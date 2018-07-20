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
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;

namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>IDirectSoundBuffer</unmanaged>	
    [Guid("279afa85-4981-11ce-a521-0020af0be560")]
    public class SoundBuffer : ComObject
    {
        public SoundBuffer(IntPtr nativePtr)
            : base(nativePtr)
        {
        }

        public int Volume
        {
            get
            {
                int result;
                this.GetVolume(out result);
                return result;
            }
            set { this.SetVolume(value); }
        }

        public int Pan
        {
            get
            {
                int result;
                this.GetPan(out result);
                return result;
            }
            set { this.SetPan(value); }
        }

        public int Frequency
        {
            get
            {
                int result;
                this.GetFrequency(out result);
                return result;
            }
            set { this.SetFrequency(value); }
        }

        public DSBSTATUS Status
        {
            get
            {
                int result;
                this.GetStatus(out result);
                return (DSBSTATUS)result;
            }
        }

        public int CurrentPosition
        {
            set { this.SetCurrentPosition(value); }
        }

        public int PlayPosition
        {
            get
            {
                int dwCurrentPlayPosition;
                int dwCurrentWritePosition;
                this.GetCurrentPosition(out dwCurrentPlayPosition, out dwCurrentWritePosition);
                return dwCurrentPlayPosition;
            }
        }

        public WaveFormat Format
        {
            set
            {
                IntPtr intPtr = WaveFormat.MarshalToPtr(value);
                try
                {
                    this.SetFormat(intPtr);
                }
                finally
                {
                    Marshal.FreeHGlobal(intPtr);
                }
            }
        }


        
        
        // <unmanaged>HRESULT IDirectSoundBuffer::GetCurrentPosition([Out, Optional] unsigned int* pdwCurrentPlayCursor,[Out, Optional] unsigned int* pdwCurrentWriteCursor)</unmanaged>	
        public unsafe void GetCurrentPosition(out int currentPlayCursorRef, out int currentWriteCursorRef)
		{
            int dwCurrentPlayCursor;
            int dwCurrentWriteCursor;
            //result = calli(System.Int32(System.Void*,System.Void*,System.Void*), this._nativePointer, ptr, ptr2, *(*(IntPtr*)this._nativePointer + (IntPtr)4 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(4, _nativePointer, &dwCurrentPlayCursor, &dwCurrentWriteCursor);
            currentPlayCursorRef = dwCurrentPlayCursor;
            currentWriteCursorRef = dwCurrentWriteCursor;
            result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::GetFormat([Out, Buffer, Optional] WAVEFORMATEX* pwfxFormat,[In] unsigned int dwSizeAllocated,[Out, Optional] unsigned int* pdwSizeWritten)</unmanaged>	
        public unsafe void GetFormat(WaveFormat[] wfxFormatRef, int sizeAllocated, out int sizeWrittenRef)
		{
			WaveFormat.__Native[] array = (wfxFormatRef == null) ? null : new WaveFormat.__Native[wfxFormatRef.Length];
            HRESULT result;
			fixed (WaveFormat.__Native* ptr = array)
			{
				int dwSizeWritten;
                //result = calli(System.Int32(System.Void*,System.Void*,System.Int32,System.Void*), this._nativePointer, ptr, sizeAllocated, ptr2, *(*(IntPtr*)this._nativePointer + (IntPtr)5 * (IntPtr)sizeof(void*)));
                result = (HRESULT)NativeHelper.CalliInt32(5, _nativePointer, (void*)ptr, sizeAllocated, (void*)&dwSizeWritten);
                sizeWrittenRef = dwSizeWritten;
			}
			for (int i = 0; i < wfxFormatRef.Length; i++)
			{
				wfxFormatRef[i].__MarshalFrom(ref array[i]);
			}
			result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::GetVolume([Out] int* plVolume)</unmanaged>	
        internal unsafe void GetVolume(out int lVolumeRef)
		{
            int lVolume;
            //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)6 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(6, _nativePointer, &lVolume);
            lVolumeRef = lVolume;
			result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::GetPan([Out] int* plPan)</unmanaged>	
        internal unsafe void GetPan(out int lPanRef)
		{
			int lPan;
            //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)7 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(7, _nativePointer, &lPan);
            lPanRef = lPan;
            result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::GetFrequency([Out] unsigned int* pdwFrequency)</unmanaged>	
        internal unsafe void GetFrequency(out int frequencyRef)
		{
			int dwFrequency;
            //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(8, _nativePointer, &dwFrequency);
            frequencyRef = dwFrequency;
            result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::GetStatus([Out] unsigned int* pdwStatus)</unmanaged>	
        internal unsafe void GetStatus(out int statusRef)
		{
			int dwStatus;
            //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)9 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(9, _nativePointer, &dwStatus);
            statusRef = dwStatus;
            result.CheckError();
		}

        
        
        // <unmanaged>HRESULT IDirectSoundBuffer::Initialize([In] IDirectSound* pDirectSound,[In] const DSBUFFERDESC* pcDSBufferDesc)</unmanaged>	
        public unsafe void Initialize(DirectSoundBase directSoundRef, DSBUFFERDESC cDSBufferDescRef)
		{
			DSBUFFERDESC.__Native _Native = DSBUFFERDESC.__NewNative();
			cDSBufferDescRef.__MarshalTo(ref _Native);
			//Result result = calli(System.Int32(System.Void*,System.Void*,System.Void*), this._nativePointer, (void*)((directSoundRef == null) ? IntPtr.Zero : directSoundRef.NativePointer), &_Native, *(*(IntPtr*)this._nativePointer + (IntPtr)10 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(10, _nativePointer, (void*)((directSoundRef == null) ? IntPtr.Zero : directSoundRef.NativePointer), (void*)&_Native);
			cDSBufferDescRef.__MarshalFree(ref _Native);
			result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::Lock([In] unsigned int dwOffset,[In] unsigned int dwBytes,[Out] void** ppvAudioPtr1,[Out] unsigned int* pdwAudioBytes1,[Out] void** ppvAudioPtr2,[Out, Optional] unsigned int* pdwAudioBytes2,[In] DSBLOCK_ENUM dwFlags)</unmanaged>	
        internal unsafe void Lock(int offset, int bytes, out IntPtr audioPtr1Out, out int audioBytes1Ref, out IntPtr audioPtr2Out, out int audioBytes2Ref, DSBLOCK flags)
		{
            IntPtr pvAudioPtr1;
            int dwAudioBytes1;
            IntPtr pvAudioPtr2;
            int dwAudioBytes2;
            //result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Void*,System.Void*,System.Void*,System.Void*,System.Int32), this._nativePointer, offset, bytes, ptr, ptr2, ptr3, ptr4, flags, *(*(IntPtr*)this._nativePointer + (IntPtr)11 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(11, _nativePointer, offset, bytes, (void*)&pvAudioPtr1, (void*)&dwAudioBytes1, (void*)&pvAudioPtr2, (void*)&dwAudioBytes2, (int)flags);
            audioPtr1Out = pvAudioPtr1;
            audioBytes1Ref = dwAudioBytes1;
            audioPtr2Out = pvAudioPtr2;
            audioBytes2Ref = dwAudioBytes2;
            result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::Play([In] unsigned int dwReserved1,[In] unsigned int dwPriority,[In] DSBPLAY_FLAGS dwFlags)</unmanaged>	
        internal unsafe void Play(int reserved1, int priority, DSBPLAY_FLAGS flags)
		{
			//calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Int32), this._nativePointer, reserved1, priority, flags, *(*(IntPtr*)this._nativePointer + (IntPtr)12 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(12, _nativePointer, reserved1, priority, (int)flags);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::SetCurrentPosition([In] unsigned int dwNewPosition)</unmanaged>	
        internal unsafe void SetCurrentPosition(int newPosition)
		{
			//calli(System.Int32(System.Void*,System.Int32), this._nativePointer, newPosition, *(*(IntPtr*)this._nativePointer + (IntPtr)13 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(13, _nativePointer, newPosition);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::SetFormat([In] const void* pcfxFormat)</unmanaged>	
        internal unsafe void SetFormat(IntPtr cfxFormatRef)
		{
			//calli(System.Int32(System.Void*,System.Void*), this._nativePointer, (void*)cfxFormatRef, *(*(IntPtr*)this._nativePointer + (IntPtr)14 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(14, _nativePointer, (void*)cfxFormatRef);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::SetVolume([In] int lVolume)</unmanaged>	
        internal unsafe void SetVolume(int volume)
		{
			//calli(System.Int32(System.Void*,System.Int32), this._nativePointer, volume, *(*(IntPtr*)this._nativePointer + (IntPtr)15 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(15, _nativePointer, volume);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::SetPan([In] int lPan)</unmanaged>	
        internal unsafe void SetPan(int pan)
		{
			//calli(System.Int32(System.Void*,System.Int32), this._nativePointer, pan, *(*(IntPtr*)this._nativePointer + (IntPtr)16 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(16, _nativePointer, pan);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::SetFrequency([In] unsigned int dwFrequency)</unmanaged>	
        internal unsafe void SetFrequency(int frequency)
		{
			//calli(System.Int32(System.Void*,System.Int32), this._nativePointer, frequency, *(*(IntPtr*)this._nativePointer + (IntPtr)17 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(17, _nativePointer, frequency);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::Stop()</unmanaged>	
        public unsafe void Stop()
		{
			//calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)18 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(18, _nativePointer);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::Unlock([In, Buffer] void* pvAudioPtr1,[In] unsigned int dwAudioBytes1,[In, Buffer, Optional] void* pvAudioPtr2,[In] unsigned int dwAudioBytes2)</unmanaged>	
        internal unsafe void Unlock(IntPtr vAudioPtr1Ref, int audioBytes1, IntPtr vAudioPtr2Ref, int audioBytes2)
		{
			//calli(System.Int32(System.Void*,System.Void*,System.Int32,System.Void*,System.Int32), this._nativePointer, (void*)vAudioPtr1Ref, audioBytes1, (void*)vAudioPtr2Ref, audioBytes2, *(*(IntPtr*)this._nativePointer + (IntPtr)19 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(19, _nativePointer, (void*)vAudioPtr1Ref, audioBytes1, (void*)vAudioPtr2Ref, audioBytes2);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer::Restore()</unmanaged>	
        public unsafe void Restore()
		{
			//calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)20 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(20, _nativePointer);
            hr.CheckError();
		}

        public void Play(int priority, DSBPLAY_FLAGS flags)
        {
            this.Play(0, priority, flags);
        }

        public void SetNotificationPositions(DSBPOSITIONNOTIFY[] positions)
        {
            using (SoundBufferNotifier soundBufferNotifier = this.QueryInterface<SoundBufferNotifier>())
            {
                soundBufferNotifier.SetNotificationPositions(positions.Length, positions);
            }
        }


        #region Helpers

        public unsafe void Write(int position, void* pBuffer, int length, DSBLOCK flags)
        {
            IntPtr pvAudioPtr1;
            int dwAudioBytes1;
            IntPtr pvAudioPtr2;
            int dwAudioBytes2;
            this.Lock(
                position,
                length,
                out pvAudioPtr1,
                out dwAudioBytes1,
                out pvAudioPtr2,
                out dwAudioBytes2,
                flags);
            try
            {
                int count1 = Math.Min(dwAudioBytes1, length);
                int count2 = length - count1;
                NativeHelper.CPBLK((void*)pvAudioPtr1, (void*)pBuffer, count1);
                if (count2 > 0)
                {
                    NativeHelper.CPBLK((void*)(pvAudioPtr1 + count1), (void*)((byte*)pBuffer + count1), count2);
                }
            }
            finally
            {
                this.Unlock(pvAudioPtr1, dwAudioBytes1, pvAudioPtr2, dwAudioBytes2);
            }
        }

        #endregion Helpers
    }
}
