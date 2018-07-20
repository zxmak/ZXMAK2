

namespace ZXMAK2.Hardware.GdbServer.Gdb
{
    /// <summary>
    /// Errno Values
    /// https://sourceware.org/gdb/onlinedocs/gdb/Errno-Values.html#Errno-Values
    /// </summary>
    public enum Errno
    {
        /// <summary>
        /// Operation not permitted
        /// </summary>
        EPERM        =   1,
        /// <summary>
        /// No such file or directory
        /// </summary>
        ENOENT       =   2,
        /// <summary>
        /// Interrupted system call
        /// </summary>
        EINTR        =   4,
        /// <summary>
        /// Bad file number
        /// </summary>
        EBADF        =   9,
        /// <summary>
        /// Permission denied
        /// </summary>
        EACCES       =  13,
        /// <summary>
        /// Bad address
        /// </summary>
        EFAULT       =  14,
        /// <summary>
        /// Device or resource busy
        /// </summary>
        EBUSY        =  16,
        /// <summary>
        /// File exists
        /// </summary>
        EEXIST       =  17,
        /// <summary>
        /// No such device
        /// </summary>
        ENODEV       =  19,
        /// <summary>
        /// Not a directory
        /// </summary>
        ENOTDIR      =  20,
        /// <summary>
        /// Is a directory
        /// </summary>
        EISDIR       =  21,
        /// <summary>
        /// Invalid argument
        /// </summary>
        EINVAL       =  22,
        /// <summary>
        /// File table overflow
        /// </summary>
        ENFILE       =  23,
        /// <summary>
        /// Too many open files
        /// </summary>
        EMFILE       =  24,
        /// <summary>
        /// File too large
        /// </summary>
        EFBIG        =  27,
        /// <summary>
        /// No space left on device
        /// </summary>
        ENOSPC       =  28,
        /// <summary>
        /// Illegal seek
        /// </summary>
        ESPIPE       =  29,
        /// <summary>
        /// Read-only file system
        /// </summary>
        EROFS        =  30,
        /// <summary>
        /// File name too long
        /// </summary>
        ENAMETOOLONG =  91,
        /// <summary>
        /// EUNKNOWN is used as a fallback error value if a host system returns any error value not in the list of supported error numbers
        /// </summary>
        EUNKNOWN     =  9999,
    }
}
