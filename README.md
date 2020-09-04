# ZXMAK2

## ZX Spectrum Emulator - Virtual Machine

The project written in C# and needs only .NET framework 4 and DirectX 9.
Managed DirectX was removed and replaced with direct calls to native DirectX.
This is why it can run on any windows from Windows XP to Windows 10.
You can run it even on Linux under the Mono. But the graphics will be not so good as on DirectX.

This emulator is designed in the way to be Virtual Machine style. You can change emulated hardware just on the fly.

Full support for Windows XP/Vista/7/8/10 x86/x64.
Don't forgot to install DirectX 9!


![Screenshot](https://i.imgur.com/5KqlCWQ.png)


## Supported ZX Spectrum models

The following ZX Spectrum clones are supported:
* ZX Spectrum 48 (contended memory)
* ZX Spectrum 128 (contended memory)
* ZX Spectrum +3 (contended memory, but currently without FDD)
* Pentagon 128/512/1024
* SCORPION 256/1024, PROF-ROM 256/1024
* ATM 4.50
* ATM 7.10
* PentEvo 4096K
* PROFI 3.xx
* PROFI 5.xx
* SPRINTER (except spectrum config)
* QUORUM 64
* QUORUM 256
* Leningrad 1
* BYTE 48K
* LEC 48/528
* Other (custom configuration and plugins, for example LEC mod)

## History

The project was previously hosted on CodePlex. 
Here is the old link: https://archive.codeplex.com/?p=zxmak2

You can also visit ZXMAK2 discussions:
- English: https://www.worldofspectrum.org/forums/discussion/39647/
- Russian: http://zx-pk.ru/threads/16830-zxmak2-virtualnaya-mashina-zx-spectrum.html

You may also be interested about this emulator history:
- ZXMAK.NET - released in 2005-2008: https://sourceforge.net/projects/zxmak-dotnet/files/zxmak-dotnet/
- ZXMAK - first ZXMAK emulator written in C++, released in 2001-2003: http://zxmak.narod.ru/


## What's new:

2.9.2.39319:
- the last released version on CodePlex

2.9.3.4:
- first minor changes after CodePlex shut down; removed nuget and TFS settings linked to CodePlex

2.9.3.5
- Managed DirectX removed, the code rewritten to use direct calls to DirectX. Eliminated LoaderLock issue (Managed DirectX bug). Now emulator has full support of x64 mode. 

2.9.3.6
- a little change for port decode, covoxPentagon, CovoxScorpion were removed and replaced with CovoxMono è CovoxStereo. You can configure it with text editor in VMZ file.
- fixed ContextMenu bug - blinking menu (popup menu Wizard on Settings window)
- changed Kempston port decode setting for Spectrum 48 model (incorrect change, see next version)

2.9.3.7:
- fixed Kempston port decode setting for Spectrum 48 model. Now Timing_Tests-48k_v1.0.tap pass all tests again.
- moved a little change for joystick bug from MDX version (the last commit on CodePlex). I'm not sure if it can be reproduced with native DirectX. But moved this fix, because the fix is pretty easy.
- due to remove of MDX dependency, I set useLegacyV2RuntimeActivationPolicy="false" in ZXMAK2.exe.config, may be it will improve performance.
- the code moved to GITHUB  :)

2.9.3.8:
- added Intel HEX file loader (load data into zx spectrum address space)
- fixed multi-drive trdos support

