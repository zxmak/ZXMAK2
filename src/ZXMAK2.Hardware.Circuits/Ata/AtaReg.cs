

namespace ZXMAK2.Hardware.Circuits.Ata
{
    public enum AtaReg
    {
        /// <summary>
        /// Error information register when read. 
        /// The write precompensation register when written.
        /// [/CS0,/CS1]=01, [A2-A0]=001
        /// </summary>
        FeatureError = 1,
        /// <summary>
        /// Sector counter register.
        /// This register could be used to make multi-sector transfers. You'd have to write the number of sectors to transfer in this register.
        /// [/CS0,/CS1]=01, [A2-A0]=010
        /// </summary>
        SectorCount = 2,
        /// <summary>
        /// Start sector register. 
        /// This register holds the start sector of the current track to start reading/ writing to.
        /// [/CS0,/CS1]=01, [A2-A0]=011
        /// </summary>
        SectorNumber = 3,
        /// <summary>
        /// Low byte of the cylinder number. 
        /// This register holds low byte of the cylinder number for a disk transfer.
        /// [/CS0,/CS1]=01, [A2-A0]=100
        /// </summary>
        CylinderLow = 4,
        /// <summary>
        /// High two bits of the cylinder number. 
        /// The traditional IDE interface allows only cylinder numbers in the range 0..1023. 
        /// This register gets the two upper bits of this number.
        /// [/CS0,/CS1]=01, [A2-A0]=101
        /// </summary>
        CylinderHigh = 5,
        /// <summary>
        /// Head and device select register. 
        /// The bits 3..0 of this register hold the head number (0..15) for a transfer. 
        /// The bit 4 is to be written 0 for access to the IDE master device, 1 for access to the IDE slave device. 
        /// The bit 6 selects between CHS (0) and LBA (1) addressing mode.
        /// The bits 7 and 5 are fixed at 1x1B in the traditional interface.
        /// [/CS0,/CS1]=01, [A2-A0]=110
        /// </summary>
        HeadAndDrive = 6,
        /// <summary>
        /// Command/status register. 
        /// When written the IDE device regards the data you write to this register as a command. 
        /// When read you get the status of the IDE device. 
        /// Reading this register also clears any interrupts from the IDE device to the controller.
        /// [/CS0,/CS1]=01, [A2-A0]=111
        /// </summary>
        CommandStatus = 7,
        /// <summary>
        /// Second status register/interrupt and reset register. 
        /// When read this register gives you the same status byte as the primary ([/CS0,/CS1]=01, [A2..A0]=111) status register. 
        /// The only difference is that reading this register does not clear the interrupt from the IDE device when read. 
        /// When written you can enable/disable the interrupts the IDE device generates. 
        /// Also you can give a software reset to the IDE device.
        /// [/CS0,/CS1]=10, [A2-A0]=110
        /// </summary>
        ControlAltStatus = 8,
    }
}
