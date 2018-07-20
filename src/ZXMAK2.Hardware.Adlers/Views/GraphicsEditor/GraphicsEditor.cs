using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ZXMAK2.Dependency;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.Adlers.Core;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Hardware.Adlers.Views.GraphicsEditorView
{
    public partial class GraphicsEditor : Form
    {
        private static ushort ZX_SCREEN_WIDTH  = 512; //in pixels
        private static ushort ZX_SCREEN_HEIGHT = 384; //in pixels 

        private static GraphicsEditor m_instance = null;
        private IDebuggable _spectrum = null;

        private Bitmap bmpZXMonochromatic = null;

        private bool _isInitialised = false;

        private MouseSelectionArea _mouseSelectionArea;

        public GraphicsEditor(ref IDebuggable spectrum)
        {
            _spectrum = spectrum;

            InitializeComponent();
            this.Icon = Icon.FromHandle(global::ZXMAK2.Resources.ResourceImages.ImageZxLogo.GetHicon());
            this.ShowIcon = true;

            comboDisplayType.SelectedIndex = 0;
            comboSpriteWidth.SelectedIndex = 0;
            comboSpriteHeight.SelectedIndex = 0;

            bitmapGridSpriteView.Init(_spectrum, Int32.Parse(comboSpriteWidth.Items[0].ToString()), 24);

            _isInitialised = true;

            _mouseSelectionArea = null;
        }

        public static GraphicsEditor getInstance()
        {
            return m_instance;
        }

        public static void Show(ref IDebuggable spectrum)
        {
            if (m_instance == null || m_instance.IsDisposed)
            {
                m_instance = new GraphicsEditor(ref spectrum);
                m_instance.ShowInTaskbar = true;
                m_instance.Show();
                //return;
            }
            else
                m_instance.Show();

            m_instance.setZXImage();
        }

        #region Display options
        /// <summary>
        /// Screen View type
        /// </summary>
        public void setZXScreenView()
        {
            if (_spectrum == null || m_instance == null)
                return;

            pictureZXDisplay.Width = ZX_SCREEN_WIDTH;
            pictureZXDisplay.Height = ZX_SCREEN_HEIGHT;

            bmpZXMonochromatic = new Bitmap(ZX_SCREEN_WIDTH/2, ZX_SCREEN_HEIGHT/2);
            ushort screenPointer = (ushort)numericUpDownActualAddress.Value;

            //Screen View
            for (int segments = 0; segments < 3; segments++)
            {
                for (int eightLines = 0; eightLines < 8; eightLines++)
                {
                    //Cycle: Fill 8 lines in one segment
                    for (int linesInSegment = 0; linesInSegment < 64; linesInSegment += 8)
                    {
                        // Cycle: all attributes in 1 line
                        for (int attributes = 0; attributes < 32; attributes++)
                        {
                            byte blockByte = _spectrum.ReadMemory(screenPointer++);

                            BitArray spriteBits = GraphicsTools.getAttributePixels(blockByte, m_instance.checkBoxMirror.Checked);
                            if (spriteBits == null)
                                return;

                            // Cycle: fill 8 pixels for 1 attribute
                            for (int pixels = 0; pixels < 8; pixels++)
                            {
                                if (spriteBits[pixels])
                                    bmpZXMonochromatic.SetPixel(pixels + (attributes * 8), linesInSegment + eightLines + (segments * 64), Color.Black);
                                else
                                    bmpZXMonochromatic.SetPixel(pixels + (attributes * 8), linesInSegment + eightLines + (segments * 64), Color.White);
                            }
                        }
                    }
                }
            } // 3 segments of the ZX Screen

            //Size newSize = new Size((int)(pictureZXDisplay.Width), (int)(pictureZXDisplay.Height));
            /*pictureZXDisplay.Image = bmpZXMonochromatic;
            pictureZXDisplay.Width = ZX_SCREEN_WIDTH;
            pictureZXDisplay.Height = ZX_SCREEN_HEIGHT;*/
            Image resizedImage = bmpZXMonochromatic.GetThumbnailImage(ZX_SCREEN_WIDTH, ZX_SCREEN_HEIGHT, null, IntPtr.Zero);
            pictureZXDisplay.Image = resizedImage;
            pictureZXDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private Image getSpriteViewImage()
        {
            byte spriteWidth = Convert.ToByte(comboSpriteWidth.SelectedItem);
            //byte spriteHeight;
            ushort screenPointer = (ushort)numericUpDownActualAddress.Value;

            if (_spectrum == null || m_instance == null)
                return null;

            Bitmap bmpSpriteView = new Bitmap(spriteWidth, ZX_SCREEN_HEIGHT);

            for (int line = 0; line < ZX_SCREEN_HEIGHT; line++)
            {
                for (int widthCounter = 0; widthCounter < (spriteWidth / 8); widthCounter++)
                {
                    BitArray spriteBits = GraphicsTools.getAttributePixels(_spectrum.ReadMemory(screenPointer++), m_instance.checkBoxMirror.Checked);
                    if (spriteBits == null)
                        return null;

                    // Cycle: fill 8 pixels for 1 attribute
                    for (int pixelBit = 7; pixelBit > -1; pixelBit--)
                    {
                        if (spriteBits[pixelBit])
                            bmpSpriteView.SetPixel(pixelBit + widthCounter * 8, line, Color.Black);
                        else
                            bmpSpriteView.SetPixel(pixelBit + widthCounter * 8, line, Color.White);
                    }
                }
            }

            byte spriteZoomFactor = Convert.ToByte(numericUpDownZoomFactor.Value);
            Image resizedImage = bmpSpriteView.GetThumbnailImage(spriteWidth * spriteZoomFactor, (spriteWidth * spriteZoomFactor * bmpSpriteView.Height) /
                bmpSpriteView.Width, null, IntPtr.Zero);
            return resizedImage;
        }

        private Image getTileViewImage()
        {
            byte spriteWidth = Convert.ToByte(comboSpriteWidth.SelectedItem);
            //byte spriteHeight;
            ushort screenPointer = (ushort)numericUpDownActualAddress.Value;

            if (_spectrum == null || m_instance == null)
                return null;

            Bitmap bmpSpriteView = new Bitmap(spriteWidth, ZX_SCREEN_HEIGHT);

            for (int YPointerShift = 0; YPointerShift < ZX_SCREEN_HEIGHT; YPointerShift += 8)
            {
                for (int XPointerShift = 0; XPointerShift < spriteWidth; XPointerShift += 8)
                {
                    //draw one tile
                    for (int line = 0; line < 8; line++)
                    {
                        BitArray spriteBits = GraphicsTools.getAttributePixels(_spectrum.ReadMemory(screenPointer++), m_instance.checkBoxMirror.Checked);
                        if (spriteBits == null)
                            return null;

                        // Cycle: fill 8 pixels for 1 attribute
                        for (int pixelBit = 7; pixelBit > -1; pixelBit--)
                        {
                            if (spriteBits[pixelBit])
                                bmpSpriteView.SetPixel(pixelBit + XPointerShift, line + YPointerShift, Color.Black);
                            else
                                bmpSpriteView.SetPixel(pixelBit + XPointerShift, line + YPointerShift, Color.White);
                        }
                    }
                }
            }

            byte spriteZoomFactor = Convert.ToByte(numericUpDownZoomFactor.Value);
            Image resizedImage = bmpSpriteView.GetThumbnailImage(spriteWidth * spriteZoomFactor, (spriteWidth * spriteZoomFactor * bmpSpriteView.Height) /
                bmpSpriteView.Width, null, IntPtr.Zero);
            return resizedImage;
        }

        /// <summary>
        /// Sprite View type
        /// </summary>
        public void setZXSpriteView()
        {
            pictureZXDisplay.Image = getSpriteViewImage();
            pictureZXDisplay.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        /// <summary>
        /// Tile view; 8 lines per sprite
        /// Used in ISChess(Cyrus Chess) for instance
        /// </summary>
        public void setTileView()
        {
            pictureZXDisplay.Image = getTileViewImage();
            pictureZXDisplay.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        /// <summary>
        /// JetPac style view
        /// the image is mirrored and turned 180degrees
        /// </summary>
        public void setZXJetpacView()
        {
            comboSpriteWidth.SelectedIndex = 1;

            Image img = getSpriteViewImage();

            img.RotateFlip(RotateFlipType.Rotate180FlipY);
            img.RotateFlip(RotateFlipType.Rotate180FlipNone);
            pictureZXDisplay.Image = img;
            pictureZXDisplay.SizeMode = PictureBoxSizeMode.AutoSize;
        }
        #endregion

        private bool isSpriteViewType()
        {
            if (comboDisplayType.SelectedIndex == 0) //screen view
                return false;

            return true;
        }

        private void setZXImage()
        {
            if (!_isInitialised)
                return;

            bool bIsSpriteViewType = isSpriteViewType();
            comboSpriteWidth.Enabled = bIsSpriteViewType;
            //comboSpriteHeight.Enabled = bEnableControls;
            numericUpDownZoomFactor.Enabled = bIsSpriteViewType;
            groupBoxSelectionArea.Visible = groupBoxScreenInfo.Visible = !bIsSpriteViewType;
            groupBoxSpriteDetails.Visible = bIsSpriteViewType;
            pictureZoomedArea.Visible = !bIsSpriteViewType;

            switch (comboDisplayType.SelectedIndex)
            {
                case 0: //Screen view
                    comboSpriteHeight.Enabled = false;
                    setZXScreenView();
                    break;
                case 1: //Sprite view
                    setZXSpriteView();
                    groupBoxSpriteDetails.Enabled = true;
                    comboSpriteHeight.Enabled = true;
                    if (comboSpriteHeight.SelectedIndex == 0) //if '-' selected
                        comboSpriteHeight.SelectedIndex = 1;
 
                    bitmapGridSpriteView.setGridHeight(Convert.ToByte(comboSpriteHeight.Text));
                    bitmapGridSpriteView.setBitmapBits(_spectrum, Convert.ToUInt16(numericUpDownActualAddress.Value));
                    bitmapGridSpriteView.Draw(null);
                    break;
                case 3: //Tile view
                    groupBoxSpriteDetails.Enabled = true;
                    comboSpriteHeight.Enabled = false;
                    setTileView();
                    break;
                case 4: //JetPac style
                    groupBoxSpriteDetails.Enabled = false;
                    comboSpriteHeight.Enabled = false;
                    setZXJetpacView();
                    break;
                default:
                    break;
            }

            if (bIsSpriteViewType)
            {
                Point locationSpriteDetails = new Point();
                locationSpriteDetails.X = pictureZXDisplay.Location.X + pictureZXDisplay.Size.Width + 10;
                locationSpriteDetails.Y = groupBoxSpriteDetails.Location.Y;
                groupBoxSpriteDetails.Location = locationSpriteDetails;
            }
        }

        #region GUI methods
        private void pictureZXDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (comboDisplayType.SelectedIndex == 0 && _mouseSelectionArea != null)  //for ScreenView only
            {
                //paint mouse selection area if exists
                if (!_mouseSelectionArea.Paint(e))
                {
                    pictureZXDisplay.Invalidate();
                }
            }
        }
        private void numericUpDownActualAddress_ValueChanged(object sender, System.EventArgs e)
        {
            setZXImage();
        }
        private void buttonClose_Click(object sender, System.EventArgs e)
        {
            this.Hide();
        }
        private void comboDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            setZXImage();
        }
        private void comboSpriteHeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            setZXImage();
        }
        private void numericIncDecDelta_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownActualAddress.Increment = numericIncDecDelta.Value;
        }
        //Refresh button
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            setZXImage();
        }
        private void numericUpDownZoomFactor_ValueChanged(object sender, EventArgs e)
        {
            setZXImage();
        }
        private void comboSpriteWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitialised)
            {
                bitmapGridSpriteView.setGridWidth(Convert.ToByte(comboSpriteWidth.SelectedItem));
                bitmapGridSpriteView.setBitmapBits(_spectrum, Convert.ToUInt16(numericUpDownActualAddress.Value));
            }
            setZXImage();
        }
        private void pictureZXDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0)
                return;

            string numberFormat = this.hexNumbersToolStripMenuItem.Checked ? "#{0:X2}" : "{0}";

            int Xcoor = (e.X + 1) / 2;
            int Ycoor = (e.Y + 1) / 2;

            if( !isSpriteViewType() )
            {
                ushort screenAdress = GraphicsTools.getScreenAdress(Xcoor, Ycoor);
                textBoxScreenAddress.Text = String.Format(numberFormat, screenAdress);

                ushort attributeAdress = GraphicsTools.getAttributeAdress(Xcoor, Ycoor);
                textBoxAttributeAddress.Text = String.Format(numberFormat, attributeAdress);

                if (this.hexNumbersToolStripMenuItem.Checked)
                {
                    textBoxXCoorYCoor.Text = String.Format("#{0:X2}; #{1:X2}", Xcoor, Ycoor);
                    textBoxBytesAtAdress.Text = String.Format("#{0:X2}", _spectrum.ReadMemory(screenAdress));
                }
                else
                {
                    textBoxXCoorYCoor.Text = String.Format("{0}; {1}", Xcoor, Ycoor);
                    textBoxBytesAtAdress.Text = String.Format("{0}", _spectrum.ReadMemory(screenAdress));
                }

                for (ushort memValue = (ushort)(screenAdress + 1); memValue < screenAdress + 5; memValue++)
                {
                    textBoxBytesAtAdress.Text += "; " + String.Format(numberFormat, _spectrum.ReadMemory(memValue));
                }
            }
            else if (comboDisplayType.SelectedIndex == 1) //only Sprite View for now...ToDo other view types, e.g. Jetpac type
            {
                int zoomFactor = (int)numericUpDownZoomFactor.Value;
                ushort addressPointer = Convert.ToUInt16( numericUpDownActualAddress.Value );
                int addressUnderCursorBase = addressPointer + (Convert.ToByte(comboSpriteWidth.SelectedItem) / 8)*(Ycoor) + (Xcoor/8); //ToDo: zooming will crash this !
                ushort addressUnderCursor = Convert.ToUInt16(addressUnderCursorBase > 0xFFFF ? addressUnderCursorBase-0xFFFF: addressUnderCursorBase);

                //Sprite address
                textBoxSpriteAddress.Text = String.Format("#{0:X2}({1})", addressUnderCursor, addressUnderCursor);

                //Bytes at address
                textBoxSpriteBytes.Text = String.Format("#{0:X2}", _spectrum.ReadMemory(addressUnderCursor));
                for (ushort memValue = (ushort)(addressUnderCursor + 1); memValue < addressUnderCursor + 5; memValue++)
                {
                    textBoxSpriteBytes.Text += "; " + String.Format("#{0:X2}", _spectrum.ReadMemory(memValue));
                }
            }

            if (comboDisplayType.SelectedIndex == 0 && _mouseSelectionArea != null)  //for ScreenView only - update selection area if is cropping
            {
                _mouseSelectionArea.MouseMove(ref pictureZXDisplay, e);
            }
        }
        private void checkBoxMirror_CheckedChanged(object sender, EventArgs e)
        {
            setZXImage();
        }
        private void hexNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numericIncDecDelta.Hexadecimal = numericUpDownActualAddress.Hexadecimal = hexNumbersToolStripMenuItem.Checked;
            txtbxX0.Hexadecimal = txtbxY0.Hexadecimal = txtbxY1.Hexadecimal = txtbxX1.Hexadecimal = hexNumbersToolStripMenuItem.Checked;
            labelMemoryAddress.Text = String.Format("Memory address({0}):", hexNumbersToolStripMenuItem.Checked ? "hex" : "dec");
        }
        private void bitmapGridSpriteView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int clickedPixel = bitmapGridSpriteView.getClickedPixel(e);
                int temp = (int)numericUpDownActualAddress.Value + clickedPixel / 8;
                if (temp > 0xFFFF)
                    temp -= 0xFFFF;
                UInt16 bitToToggleAddress = Convert.ToUInt16(temp);
                if (bitToToggleAddress < 0x4000)
                    return; //cannot change ROM
                byte memValue = _spectrum.ReadMemory(bitToToggleAddress);

                memValue = (byte)GraphicsTools.ToggleBit(memValue, clickedPixel % 8);
                _spectrum.WriteMemory(Convert.ToUInt16(bitToToggleAddress), memValue);

                setZXImage(); //refresh
            }
        }
        private void pictureZXDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (comboDisplayType.SelectedIndex == 0)  //for ScreenView only
            {
                if (_mouseSelectionArea == null)
                    _mouseSelectionArea = new MouseSelectionArea();

                _mouseSelectionArea.MouseDown(pictureZXDisplay, e);
            }
        }
        private void pictureZXDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (comboDisplayType.SelectedIndex == 0)  //for ScreenView only
                {
                    if (_mouseSelectionArea == null)
                        return;

                    Point startPoint = _mouseSelectionArea.GetStartSelection();
                    int width = _mouseSelectionArea.getCroppedArea().Width / 2;
                    int height = _mouseSelectionArea.getCroppedArea().Height / 2;

                    _mouseSelectionArea.MouseUp(ref pictureZXDisplay, e);

                    //show selected area preview
                    ShowZoomedSelectionArea();

                    //update coords in manual
                    txtbxX0.Value = startPoint.X / 2;
                    txtbxY0.Value = startPoint.Y / 2;
                    txtbxX1.Value = width;
                    txtbxY1.Value = height;
                }
            }
        }
        private void bitmapGridSpriteView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuExportBitmap.Show(this.bitmapGridSpriteView, e.Location);
            }
        }

        public void ShowZoomedSelectionArea()
        {
            if (_mouseSelectionArea == null)
                return;

            //show selected area preview
            int width = _mouseSelectionArea.getCroppedArea().Width / 2;
            int height = _mouseSelectionArea.getCroppedArea().Height / 2;
            Bitmap croppedArea = GraphicsTools.GetBitmapCroppedArea(bmpZXMonochromatic, new Point(_mouseSelectionArea.getCroppedArea().X / 2, _mouseSelectionArea.getCroppedArea().Y / 2),
                new Size(width, height));
            pictureZoomedArea.Image = croppedArea;
        }

        private bool SaveScreenBytes(byte[] i_arrToSave, int i_WidthPixels, int i_Height)
        {
            //save bitmap as bytes
            string fileOut = String.Format("; defb #{0:X2}, #{1:x2} ; width[pixels] x height", i_WidthPixels, i_Height);
            int lineBytesCountMax = i_WidthPixels / 8;
            if (i_WidthPixels % 8 != 0)
                lineBytesCountMax++;
            int lineByteCounter = 0;

            fileOut += Environment.NewLine + "defb ";
            for (int counter = 0; counter < i_arrToSave.Length; counter++)
            {
                if (lineByteCounter == lineBytesCountMax)
                {
                    fileOut += Environment.NewLine + "defb ";
                    lineByteCounter = 0;
                }
                else if( counter != 0 )
                    fileOut += ", ";
                lineByteCounter++;
                fileOut += String.Format("#{0:X2}", i_arrToSave[counter]);
            }
            if (fileOut != String.Empty)
            {
                File.WriteAllText(Path.Combine(Utils.GetAppFolder(), "screen_bytes.asm"), fileOut);
                Locator.Resolve<IUserMessage>().Info("Sprite saved in screen_bytes.asm file !");
            }
            return true;
        }

        //ContextMenuExportBitmap
        private void SaveBitmapAs(ImageFormat i_imgFormat)
        {
            string fileName = @"image_export.";

            int zoomFactor = Convert.ToByte(numericUpDownZoomFactor.Value);
            int bitmapWidth = this.bitmapGridSpriteView.getGridWidth() * zoomFactor;
            int bitmapHeight = this.bitmapGridSpriteView.getGridHeight() * zoomFactor;

            if (i_imgFormat == null)
            {
                byte[] arrSprite = GraphicsTools.GetBytesFromBitmap(this.bitmapGridSpriteView);
                SaveScreenBytes(arrSprite, this.bitmapGridSpriteView.getGridWidth() / 8, this.bitmapGridSpriteView.getGridHeight());

                return;
            }
            else
            {
                if (i_imgFormat == ImageFormat.Png)
                    fileName += "png";
                else if (i_imgFormat == ImageFormat.Jpeg)
                    fileName += "jpg";
                else if (i_imgFormat == ImageFormat.Bmp)
                    fileName += "bmp";
                else
                {
                    Locator.Resolve<IUserMessage>().Error("Invalid format to save!");
                    return;
                }
            }

            string filePath = Path.Combine(Utils.GetAppFolder(), fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            //save cropped bitmap
            Bitmap bmp = pictureZXDisplay.Image as Bitmap;
            Bitmap cloneBitmap = bmp.Clone(new Rectangle(0, 0, bitmapWidth, bitmapHeight), bmp.PixelFormat);
            cloneBitmap.Save(filePath, i_imgFormat);

            Locator.Resolve<IUserMessage>().Info(String.Format("File '{0}' saved to root app directory!", fileName));
        }
        private void menuItemSaveBitmapAsPNG_Click(object sender, EventArgs e)
        {
            SaveBitmapAs(ImageFormat.Png);
        }
        private void menuItemSaveBitmapAsBitmap_Click(object sender, EventArgs e)
        {
            SaveBitmapAs(ImageFormat.Bmp);
        }
        private void menuItemSaveBitmapAsJPG_Click(object sender, EventArgs e)
        {
            SaveBitmapAs(ImageFormat.Jpeg);
        }
        private void menuItemSaveBitmapAsBytes_Click(object sender, EventArgs e)
        {
            SaveBitmapAs(null);
        }
        //SET manual selection area
        private void buttonSetManualSelectionArea_Click(object sender, EventArgs e)
        {
            string coords = String.Format("{0},{1},{2},{3}", txtbxX0.Text, txtbxY0.Text, txtbxX1.Text, txtbxY1.Text);
            if (_mouseSelectionArea == null)
                _mouseSelectionArea = new MouseSelectionArea();
            _mouseSelectionArea.manualCrop(ref pictureZXDisplay, coords, this.hexNumbersToolStripMenuItem.Checked);

            ShowZoomedSelectionArea();
        }
        private void menuItemMovePixelsLeft_Click(object sender, EventArgs e)
        {
            MovePixels(0);
        }
        private void menuItemMovePixelsRight_Click(object sender, EventArgs e)
        {
            MovePixels(1);
        }
        private void MovePixels(byte i_mode) //0 => move left; 1 => move right
        {
            if (comboDisplayType.SelectedIndex == 1) //Sprite view only for now
            {
                //move pixels left
                int bytesToMoveLeft = (this.bitmapGridSpriteView.getGridWidth() / 8) * this.bitmapGridSpriteView.getGridHeight();
                ushort screenPointer = (ushort)numericUpDownActualAddress.Value;

                //moving right to left
                int maxScreenByteToMove = (this.bitmapGridSpriteView.getGridWidth() / 8) * this.bitmapGridSpriteView.getGridHeight();
                int spriteViewWidthInTokens = this.bitmapGridSpriteView.getGridWidth() / 8;
                bool isLastTokenInLine = false;
                for (int pixelPointer = 0; pixelPointer < maxScreenByteToMove; pixelPointer++)
                {
                    bool setZeroBit;
                    if (screenPointer <= 0xFFFF && this.bitmapGridSpriteView.getGridWidth() > 8)
                        setZeroBit = GraphicsTools.IsBitSet(_spectrum.ReadMemory((ushort)(screenPointer + 1)), 7);
                    else
                        setZeroBit = false;

                    isLastTokenInLine = ((pixelPointer+1) % spriteViewWidthInTokens == 0);
                    isLastTokenInLine = isLastTokenInLine && spriteViewWidthInTokens != 1 && pixelPointer != 0 && pixelPointer != maxScreenByteToMove;
                    if (i_mode == 0)
                    {
                        byte actByte = (byte)(_spectrum.ReadMemory(screenPointer) << 1);

                        //set bit 0(most right) if there is a sprite view bitmap size more than 8 pixels(continous scrolling)
                        if (isLastTokenInLine == false && screenPointer + 1 <= 0xFFFF)
                        {
                            if (setZeroBit)
                                actByte |= 0x01;
                        }
                        //move left
                        _spectrum.WriteMemory(screenPointer, actByte);
                    }
                    else
                    {
                        //move right
                        _spectrum.WriteMemory(screenPointer, (byte)(_spectrum.ReadMemory(screenPointer) >> 1));
                    }

                    screenPointer++; //move to next byte
                }
            }
            else
                Locator.Resolve<IUserMessage>().Info("This feature implemented only for Sprite view...Todo!");

            //Refresh
            setZXImage();
        }

        //Context menu: Clear bitmap
        private void menuItemClearBitmap_Click(object sender, EventArgs e)
        {
            ushort address = (ushort)this.numericUpDownActualAddress.Value;
            int length = (bitmapGridSpriteView.getGridWidth() / 8 ) * bitmapGridSpriteView.getGridHeight();
            byte[] arrZeros = new byte[length];

            _spectrum.WriteMemory(address, arrZeros, 0, length);

            setZXImage(); //Refresh
        }

        private void buttonExportSelectionArea_Click(object sender, EventArgs e)
        {
            //export selection area to picture or bytes
            if (_mouseSelectionArea == null || _mouseSelectionArea.isSelected() == false)
                return;

            int width = _mouseSelectionArea.getCroppedArea().Width/2;
            int height =  _mouseSelectionArea.getCroppedArea().Height/2;

            Bitmap croppedArea = GraphicsTools.GetBitmapCroppedArea(bmpZXMonochromatic, new Point(_mouseSelectionArea.getCroppedArea().X/2, _mouseSelectionArea.getCroppedArea().Y/2),
                new Size(width, height));
            if( croppedArea == null )
            {
                Locator.Resolve<IUserMessage>().Warning("Could not get cropped area...nothing done!");
                return;
            }

            byte[] arrScreenBytes = GraphicsTools.GetBytesFromBitmap(croppedArea);
            SaveScreenBytes(arrScreenBytes, width, height);
            //croppedArea.Save(@"image_export_cropped.bmp");
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion GUI methods
    }
}
