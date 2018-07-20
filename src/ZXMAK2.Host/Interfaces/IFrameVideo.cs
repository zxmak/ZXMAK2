using System;
using System.Drawing;


namespace ZXMAK2.Host.Interfaces
{
    public interface IFrameVideo
    {
        /// <summary>
        /// Raw Video Buffer content 32 bits per pixel
        /// </summary>
        int[] Buffer { get; }
        
        /// <summary>
        /// Raw Video Buffer size
        /// </summary>
        Size Size { get; }
        
        /// <summary>
        /// Height to width ratio
        /// </summary>
        float Ratio { get; }
    }
}
