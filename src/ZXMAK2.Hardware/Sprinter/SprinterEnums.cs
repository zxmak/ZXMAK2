

namespace ZXMAK2.Hardware.Sprinter
{
    public enum ROMPages
    {
        rpROMExpansion = 0,
        rpROMTRDOS = 1,
        rpROMBasic128 = 2,
        rpROMBasic48 = 3,
        rpROMExpansionAlt = 4,
        rpROMTRDOSAlt = 5,
        rpROMBasic128Alt = 6,
        rpROMBasic48Alt = 7,
        rpRAM0 = 8,
        rpRAM1 = 9,
        rpRAM2 = 10,
        rpROMSystem = 11,
        rpRAMCache = 12,
        rpROMSystemAlt = 15
    }

    public enum DCPports
    {
        dcpNull = 0,
        dcpWG1F = 0x10,
        dcpWG3F = 0x11,
        dcpWG5F = 0x12,
        dcpWG7F = 0x13,
        dcpWGFF = 0x14,    //Порт на запись - состояние контроллера дисковода (FF)
        dcpJoystik = 0x15, //Порт на чтение - джойстик и IRQ/INTRQ контроллера
        dcpHDDData = 0x20,  //HDD - регистр данных
        dcpHDDStat = 0x21, //HDD - регистр состояния/ошибок
        dcpHDDSectCnt = 0x22,  //HDD - регистр количества секторов для операций R/W
        dcpHDDSector = 0x23,   //HDD - регистр сектора
        dcpHDDCylLow = 0x24,   //HDD - регистр дорожки low
        dcpHDDCylHigh = 0x25,   //HDD - регистр дорожки high
        dcpHDDHead = 0x26,   //HDD - регистр головок/выборка мастер-слэйв
        dcpHDDCommand = 0x27,   //HDD - регистр команд
        dcpHDD3F6 = 0x28,   //HDD - дополнительный регистр  управления 3F6
        dcpHDD3F7 = 0x29,   //HDD - дополнительный регистр  состояния 3F7
        dcpISA1MemoryRW = 0x30,     //ISA-SLOT #1 memory R/W
        dcpISA2MemoryRW = 0x31,     //ISA-SLOT #2 memory R/W
        dcpISA1PortsRW = 0x32,     //ISA-SLOT #1 ports R/W
        dcpISA2PortsRW = 0x33,     //ISA-SLOT #2 ports R/W
        dcpZXKeyboard = 0x40,
        dcpCovoxBlaster = 0x88,    //Covox/Covox-Blaster
        dcpAYBFFD = 0x90,
        dcpAYFFFD = 0x91,
        dcpScorp1FFD = 0xC0,          //Scorpion 1FFD port
        dcpPent7FFD = 0xC1,            //Pentagon 7FFD port
        dcpBorder = 0xc2,       //Border Write Only
        dcpRGADR = 0xC4,
        dcpRGMOD = 0xC5,
        dcpROMExpansion = 0xE0,
        dcpROMTRDOS = 0xE1,
        dcpROMB128 = 0xE2,
        dcpROMB48 = 0xE3,
        dcpROMExpansionAlt = 0xE4,
        dcpROMTRDOSAlt = 0xE5,
        dcpROMB128Alt = 0xE6,
        dcpROMB48Alt = 0xE7,
        dcpRAM0 = 0xE8,         //RAM Page (окно 0000-3fff)
        dcpRAM1 = 0xE9,         //RAM Page (окно 4000-7fff)
        dcpRAM2 = 0xEA,         //RAM Page (окно 8000-bfff)
        dcpROMSys = 0xEB,       //ROM page SYSTEM
        dcpRAMCache = 0xEC,     //RAM page CACHE
        dcpROMSysAlt = 0xEF, //ROM Page SYSTEM'
        dcpRAMPage0 = 0xF0,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage1 = 0xF1,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage2 = 0xF2,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage3 = 0xF3,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage4 = 0xF4,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage5 = 0xF5,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage6 = 0xF6,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage7 = 0xF7,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage8 = 0xF8,     //RAM Pages (окно C000-FFFF)
        dcpRAMPage9 = 0xF9,     //RAM Pages (окно C000-FFFF)
        dcpRAMPageA = 0xFA,     //RAM Pages (окно C000-FFFF)
        dcpRAMPageB = 0xFB,     //RAM Pages (окно C000-FFFF)
        dcpRAMPageC = 0xFC,     //RAM Pages (окно C000-FFFF)
        dcpRAMPageD = 0xFD,     //RAM Pages (окно C000-FFFF)
        dcpRAMPageE = 0xFE,     //RAM Pages (окно C000-FFFF)
        dcpRAMPageF = 0xFF      //RAM Pages (окно C000-FFFF)

    }

    public enum AccelCMD
    {
        Invalid = -1,
        Off = 1,     //ld b,b    выкл акселератор
        On = 2,      //ld d,d    вкл акселератор, указать размер блока
        Fill = 3,    //ld c,c    
        GrFill = 4,  //ld e,e
        Reserved = 5,  //ld h,h
        CopyBlok = 6, //ld l,l
        GrCopyBlok = 7 //ld a,a  копирование блока гр.экрана, вертикальные линии
    }

    public enum AccelSubCMD
    {
        None = 0,
        XORBlok = 1,
        ORBlok = 2,
        ANDBlok = 3
    }
}
