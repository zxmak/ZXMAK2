using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Hardware.Adlers.Core
{
   class MouseSelectionArea
   {
        private Boolean   m_bHaveMouse;
        //private Boolean   m_bIsSelected;
        private Point     ptOriginal = new Point();
        private Point     ptLast = new Point();
        private Rectangle m_rectCropArea;

        public MouseSelectionArea()
        {
            m_bHaveMouse = false;
        }

        public Boolean isSelected()
        {
            return (m_rectCropArea.Width > 0 && m_rectCropArea.Height > 0);
        }

        public Rectangle getCroppedArea()
        {
           if (isSelected())
               return m_rectCropArea;
           else
               return new Rectangle(new Point(0, 0), new Size());
        }

        public Point GetStartSelection()
        {
            return ptOriginal;
        }

        public void ResetSelectionArea()
        {
            m_rectCropArea = new Rectangle(new Point(0, 0), new Size());
        }

        // return false in case the rectangle is not drawed => not visible
        public Boolean Paint(PaintEventArgs e)
        {
           //if (!this.isSelected())
           //{
              Pen drawLine = new Pen(Color.Red);
              drawLine.DashStyle = DashStyle.Dash;

              if (isSelected())
                e.Graphics.DrawRectangle(drawLine, getCroppedArea());

              //m_bIsSelected = true;
              return true;
           /*}
           else
           {
              m_bIsSelected = false;
              return false;
           }*/
        }

        public Image Crop(Image i_sourcePictureBox) //BtnCrop_Click(object sender, EventArgs e)
        {
            Bitmap bmpRealBitmap = new Bitmap(i_sourcePictureBox, 512, 192*2); // because speccy size is always 256x192

            // Create the new bitmap and associated graphics object
            Bitmap bmpSelectedArea = new Bitmap(m_rectCropArea.Width, m_rectCropArea.Height);
            Graphics g = Graphics.FromImage(bmpSelectedArea);

            // Draw the specified section of the source bitmap to the new one
            g.DrawImage(bmpRealBitmap, 0, 0, m_rectCropArea, GraphicsUnit.Pixel);

            // Clean up
            g.Dispose();

            // Return the bitmap
            return bmpSelectedArea;
        }
      
        public void MouseDown( PictureBox i_pictureBox, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ResetSelectionArea();

                m_bHaveMouse = false;
            }
            else
            {
                m_bHaveMouse = true;

                // Store the "starting point" for this rubber-band rectangle.
                ptOriginal.X = e.X; // Math.Max(e.X - 2, 0);
                ptOriginal.Y = e.Y; // Math.Max(e.Y - 2, 0);

                ptLast.X = -1;
                ptLast.Y = -1;

                m_rectCropArea = new Rectangle(new Point(ptOriginal.X, ptOriginal.Y), new Size());
            }
        }

        public void MouseUp(ref PictureBox i_PictureBox, MouseEventArgs e)
        {
            // Set internal flag to know we no longer "have the mouse".
            m_bHaveMouse = false;

            // Set flags to know that there is no "previous" line to reverse.
            ptLast.X = -1;
            ptLast.Y = -1;
            ptOriginal.X = -1;
            ptOriginal.Y = -1;
        }

        public void MouseMove(ref PictureBox i_PictureBox, MouseEventArgs e)
        {
            // If we "have the mouse", then we draw our lines.
            if (m_bHaveMouse)
            {
                int localX = e.X;
                int localY = e.Y;

                if (localX < 0)
                   localX = 0;
                if (localX > i_PictureBox.Width)
                {
                   localX = i_PictureBox.Width;
                   //Debug.WriteLine(localX.ToString() + " max width=" + i_PictureBox.Width + " a E.X=" + e.X.ToString());
                }

                if (localY < 0)
                   localY = 0;
                if (localY > i_PictureBox.Height)
                {
                   localY = i_PictureBox.Height;
                   //Debug.WriteLine(localY.ToString() + " max width=" + i_PictureBox.Height + " a E.Y=" + e.Y.ToString());
                }

                Point ptCurrent = new Point(localX, localY);

                // Update last point.
                ptLast = ptCurrent;

                // Draw new lines.
                // e.X - rectCropArea.X;
                // normal
                if (localX > ptOriginal.X && localY > ptOriginal.Y)
                {
                    m_rectCropArea.Width = localX - ptOriginal.X;

                    // localY - rectCropArea.Height;
                    m_rectCropArea.Height = localY - ptOriginal.Y;
                }
                else if (localX < ptOriginal.X && localY > ptOriginal.Y)
                {
                    m_rectCropArea.Width = ptOriginal.X - localX;
                    m_rectCropArea.Height = localY - ptOriginal.Y;
                    m_rectCropArea.X = localX;
                    m_rectCropArea.Y = ptOriginal.Y;
                }
                else if (localX > ptOriginal.X && localY < ptOriginal.Y)
                {
                    m_rectCropArea.Width = localX - ptOriginal.X;
                    m_rectCropArea.Height = ptOriginal.Y - localY;

                    m_rectCropArea.X = ptOriginal.X;
                    m_rectCropArea.Y = localY;
                }
                else
                {
                    m_rectCropArea.Width = ptOriginal.X - localX;

                    // localY - rectCropArea.Height;
                    m_rectCropArea.Height = ptOriginal.Y - localY;
                    m_rectCropArea.X = localX;
                    m_rectCropArea.Y = localY;
                }

                i_PictureBox.Invalidate();

                i_PictureBox.Refresh();
            }
        }

        public Boolean manualCrop(ref PictureBox i_pcbxZXScreen, string i_strCoordinates, bool i_hexValues)
        {
            // Parameter checks
            if (i_strCoordinates == String.Empty || i_strCoordinates == null || i_strCoordinates == "")
                return false;

            i_pcbxZXScreen.Invalidate();
            //i_pcbxZXScreen.Refresh();

            //Prepare a new Bitmap on which the cropped image will be drawn
            Bitmap sourceBitmap = new Bitmap(i_pcbxZXScreen.Image, i_pcbxZXScreen.Width, i_pcbxZXScreen.Height); 
            Graphics g = i_pcbxZXScreen.CreateGraphics();

            //Checks if the co-rdinates check-box is checked. If yes, then Selection is based on co-rdinates mentioned in the textbox
            //logic to retrieve co-rdinates from comma-separated string values

                string[] cordinates = i_strCoordinates.Split(',');
                int cordX, cordY, cordWidth, cordHeight;
                
                try
                {
                    cordX = i_hexValues ? ConvertRadix.ParseUInt16(cordinates[0],16) : Convert.ToInt32(cordinates[0]);
                    if (cordX > GraphicsTools.MAX_X_PIXEL)
                        cordX = GraphicsTools.MAX_X_PIXEL * 2;
                    else
                        cordX *= 2;

                    cordY = i_hexValues ? ConvertRadix.ParseUInt16(cordinates[1], 16) : Convert.ToInt32(cordinates[1]);
                    if (cordY > GraphicsTools.MAX_Y_PIXEL)
                        cordY = GraphicsTools.MAX_Y_PIXEL * 2;
                    else
                        cordY *= 2;

                    cordWidth = i_hexValues ? ConvertRadix.ParseUInt16(cordinates[2], 16) : Convert.ToInt32(cordinates[2]);
                    if (cordWidth > GraphicsTools.MAX_X_PIXEL)
                        cordWidth = GraphicsTools.MAX_X_PIXEL * 2;
                    else
                        cordWidth *= 2;

                    cordHeight = i_hexValues ? ConvertRadix.ParseUInt16(cordinates[3], 16) : Convert.ToInt32(cordinates[3]);
                    if (cordHeight > GraphicsTools.MAX_Y_PIXEL)
                        cordHeight = GraphicsTools.MAX_Y_PIXEL * 2;
                    else
                        cordHeight *= 2;
                }
                catch (Exception)
                {
                    Locator.Resolve<IUserMessage>().Error("Error parsing coordinates for selection area...\n\nCorrection needed !");
                    return false;
                }

                m_rectCropArea = new Rectangle(cordX, cordY, cordWidth, cordHeight);

            //Draw the image on the Graphics object with the new dimensions
            g.DrawImage(sourceBitmap, new Rectangle(0, 0, i_pcbxZXScreen.Width, i_pcbxZXScreen.Height), 
                m_rectCropArea, GraphicsUnit.Pixel);

            sourceBitmap.Dispose();

            return true;
        }
   }
}
