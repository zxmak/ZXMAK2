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


namespace ZXMAK2.DirectX
{
    public enum ErrorCode : uint
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/hh404141(v=vs.85).aspx
        // https://referencesource.microsoft.com/#WindowsBase/Base/MS/Internal/Interop/ErrorCodes.cs,78e97c636449b047,references

        #region Common

        S_OK = 0x00000000,
        S_FALSE = 0x00000001,
        E_NOTIMPL = 0x80004001,
        E_NOINTERFACE = 0x80004002,
        E_POINTER = 0x80004003,
        E_ABORT = 0x80004004,
        E_FAIL = 0x80004005,
        E_UNEXPECTED = 0x8000FFFF,
        E_ACCESSDENIED = 0x80070005,
        E_HANDLE = 0x80070006,
        E_OUTOFMEMORY = 0x8007000E,
        E_INVALIDARG = 0x80070057,

        // ?
        WAIT_ABANDONED = 0x00000080,
        WAIT_TIMEOUT = 0x00000102,

        E_PENDING = 0x80070007, // dinput.h

        #endregion Common


        #region DirectSound
        
        //DS_OK,
        DS_NO_VIRTUALIZATION        = 0x0878000A,
        DS_INCOMPLETE               = 0x08780014,
        //DSERR_ACCESSDENIED          = 0x80070005,
        DSERR_ALLOCATED             = 0x8878000A,
        DSERR_ALREADYINITIALIZED    = 0x88780082,
        DSERR_BADFORMAT             = 0x88780064,
        DSERR_BADSENDBUFFERGUID     = 0x887810D2,
        DSERR_BUFFERLOST            = 0x88780096,
        DSERR_BUFFERTOOSMALL        = 0x887810B4,
        DSERR_CONTROLUNAVAIL        = 0x8878001E,
        DSERR_DS8_REQUIRED          = 0x887810BE,
        DSERR_FXUNAVAILABLE         = 0x887810DC,
        //DSERR_GENERIC               = 0x80004005,
        DSERR_INVALIDCALL           = 0x88780032,
        //DSERR_INVALIDPARAM          = 0x80070057,
        //DSERR_NOAGGREGATION,
        DSERR_NODRIVER              = 0x88780078,
        DSERR_NOINTERFACE           = 0x000001AE,
        DSERR_OBJECTNOTFOUND        = 0x88781161,
        DSERR_OTHERAPPHASPRIO       = 0x887800A0,
        DSERR_OUTOFMEMORY           = 0x00000007,
        DSERR_PRIOLEVELNEEDED       = 0x88780046,
        DSERR_SENDLOOP              = 0x887810C8,
        DSERR_UNINITIALIZED         = 0x887800AA,
        //DSERR_UNSUPPORTED           = 0x80004001,

        #endregion DirectSound


        #region DirectInput

        // https://github.com/id-Software/Quake/blob/master/QW/dxsdk/sdk/inc/dinput.h
        // https://docs.microsoft.com/en-us/windows/desktop/debug/system-error-codes--0-499-

        /// <summary>
        /// The device is a polled device.  As a result, device buffering will not collect any data and event notifications will not be signalled until GetDeviceState is called.
        /// </summary>
        DI_POLLEDDEVICE                 = 0x00000002,

        // SEVERITY_ERROR = 1
        // FACILITY_WIN32 = 7
        DIERR_OLDDIRECTINPUTVERSION     = 0x047E | (7u << 16) | (1u << 31),
        DIERR_BETADIRECTINPUTVERSION    = 0x0481 | (7u << 16) | (1u << 31),
        DIERR_BADDRIVERVER              = 0x0077 | (7u << 16) | (1u << 31),
        DIERR_DEVICENOTREG              = 0x80040154,
        DIERR_OBJECTNOTFOUND            = 0x0002 | (7u << 16) | (1u << 31),
        DIERR_NOTINITIALIZED            = 0x0015 | (7u << 16) | (1u << 31),
        DIERR_ALREADYINITIALIZED        = 0x04DF | (7u << 16) | (1u << 31),
        DIERR_NOAGGREGATION             = 0x80040110,
        DIERR_OTHERAPPHASPRIO           = E_ACCESSDENIED,
        DIERR_INPUTLOST                 = 0x001E | (7u << 16) | (1u << 31),
        DIERR_ACQUIRED                  = 0x008E | (7u << 16) | (1u << 31),
        DIERR_NOTACQUIRED               = 0x000C | (7u << 16) | (1u << 31),
        //DIERR_READONLY                  = E_ACCESSDENIED,
        //DIERR_HANDLEEXISTS              = E_ACCESSDENIED,

        #endregion DirectInput


        #region Direct3D

        // d3d9.h
        //#define _FACD3D  0x876
        //#define MAKE_D3DHRESULT( code )  MAKE_HRESULT( 1, _FACD3D, code )
        //#define MAKE_D3DSTATUS( code )  MAKE_HRESULT( 0, _FACD3D, code )
        //#define MAKE_HRESULT(sev,fac,code) ((HRESULT) (((unsigned long)(sev)<<31) | ((unsigned long)(fac)<<16) | ((unsigned long)(code))) )

        D3D_OK                                  = S_OK,

        D3DERR_WRONGTEXTUREFORMAT               = 2072 | 0x88760000,//MAKE_D3DHRESULT(2072)
        D3DERR_UNSUPPORTEDCOLOROPERATION        = 2073 | 0x88760000,//MAKE_D3DHRESULT(2073)
        D3DERR_UNSUPPORTEDCOLORARG              = 2074 | 0x88760000,//MAKE_D3DHRESULT(2074)
        D3DERR_UNSUPPORTEDALPHAOPERATION        = 2075 | 0x88760000,//MAKE_D3DHRESULT(2075)
        D3DERR_UNSUPPORTEDALPHAARG              = 2076 | 0x88760000,//MAKE_D3DHRESULT(2076)
        D3DERR_TOOMANYOPERATIONS                = 2077 | 0x88760000,//MAKE_D3DHRESULT(2077)
        D3DERR_CONFLICTINGTEXTUREFILTER         = 2078 | 0x88760000,//MAKE_D3DHRESULT(2078)
        D3DERR_UNSUPPORTEDFACTORVALUE           = 2079 | 0x88760000,//MAKE_D3DHRESULT(2079)
        D3DERR_CONFLICTINGRENDERSTATE           = 2081 | 0x88760000,//MAKE_D3DHRESULT(2081)
        D3DERR_UNSUPPORTEDTEXTUREFILTER         = 2082 | 0x88760000,//MAKE_D3DHRESULT(2082)
        D3DERR_CONFLICTINGTEXTUREPALETTE        = 2086 | 0x88760000,//MAKE_D3DHRESULT(2086)
        D3DERR_DRIVERINTERNALERROR              = 2087 | 0x88760000,//MAKE_D3DHRESULT(2087)
        
        D3DERR_NOTFOUND                         = 2150 | 0x88760000,//MAKE_D3DHRESULT(2150)
        D3DERR_MOREDATA                         = 2151 | 0x88760000,//MAKE_D3DHRESULT(2151)
        D3DERR_DEVICELOST                       = 2152 | 0x88760000,//MAKE_D3DHRESULT(2152)
        D3DERR_DEVICENOTRESET                   = 2153 | 0x88760000,//MAKE_D3DHRESULT(2153)
        D3DERR_NOTAVAILABLE                     = 2154 | 0x88760000,//MAKE_D3DHRESULT(2154)
        D3DERR_OUTOFVIDEOMEMORY                 =  380 | 0x88760000,//MAKE_D3DHRESULT(380)
        D3DERR_INVALIDDEVICE                    = 2155 | 0x88760000,//MAKE_D3DHRESULT(2155)
        D3DERR_INVALIDCALL                      = 2156 | 0x88760000,//MAKE_D3DHRESULT(2156)
        D3DERR_DRIVERINVALIDCALL                = 2157 | 0x88760000,//MAKE_D3DHRESULT(2157)
        D3DERR_WASSTILLDRAWING                  =  540 | 0x88760000,//MAKE_D3DHRESULT(540)
        D3DOK_NOAUTOGEN                         = 2159 | 0x08760000,//MAKE_D3DSTATUS(2159)

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        D3DERR_DEVICEREMOVED                    = 2160 | 0x88760000,//MAKE_D3DHRESULT(2160)
        S_NOT_RESIDENT                          = 2165 | 0x08760000,//MAKE_D3DSTATUS(2165)
        S_RESIDENT_IN_SHARED_MEMORY             = 2166 | 0x08760000,//MAKE_D3DSTATUS(2166)
        S_PRESENT_MODE_CHANGED                  = 2167 | 0x08760000,//MAKE_D3DSTATUS(2167)
        S_PRESENT_OCCLUDED                      = 2168 | 0x08760000,//MAKE_D3DSTATUS(2168)
        D3DERR_DEVICEHUNG                       = 2164 | 0x88760000,//MAKE_D3DHRESULT(2164)
        D3DERR_UNSUPPORTEDOVERLAY               = 2171 | 0x88760000,//MAKE_D3DHRESULT(2171)
        D3DERR_UNSUPPORTEDOVERLAYFORMAT         = 2172 | 0x88760000,//MAKE_D3DHRESULT(2172)
        D3DERR_CANNOTPROTECTCONTENT             = 2173 | 0x88760000,//MAKE_D3DHRESULT(2173)
        D3DERR_UNSUPPORTEDCRYPTO                = 2174 | 0x88760000,//MAKE_D3DHRESULT(2174)
        D3DERR_PRESENT_STATISTICS_DISJOINT      = 2180 | 0x88760000,//MAKE_D3DHRESULT(2180)

/* -- D3D9Ex only */
#endif // !D3D_DISABLE_9EX

        #endregion Direct3D
    }
}
