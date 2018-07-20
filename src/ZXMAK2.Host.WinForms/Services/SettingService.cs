using System;
using Microsoft.Win32;
using ZXMAK2.Host.WinForms.Controls;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.WinForms.Tools;
using ZXMAK2.Host.Presentation.Interfaces;


namespace ZXMAK2.Host.WinForms.Services
{
    public class SettingService : ISettingService
    {
        private const string RegistryPath = "SOFTWARE\\ZXMAK2";

        
        public int WindowWidth
        {
            get { return GetValue("WindowWidth", 640); }
            set { SetValue("WindowWidth", value); }
        }

        public int WindowHeight
        {
            get { return GetValue("WindowHeight", 512); }
            set { SetValue("WindowHeight", value); }
        }

        public bool IsToolBarVisible
        {
            get { return GetValue("ViewShowToolBar", true); }
            set { SetValue("ViewShowToolBar", value); }
        }

        public bool IsStatusBarVisible
        {
            get { return GetValue("ViewShowStatusBar", true); }
            set { SetValue("ViewShowStatusBar", value); }
        }


        public SyncSource SyncSource 
        {
            get { return GetValue("SyncSource", default(SyncSource)); }
            set { SetValue("SyncSource", value); }
        }

        public ScaleMode RenderScaleMode
        {
            get { return GetValue("RenderScaleMode", default(ScaleMode)); }
            set { SetValue("RenderScaleMode", value); }
        }

        public VideoFilter RenderVideoFilter
        {
            get { return GetValue("RenderVideoFilter", default(VideoFilter)); }
            set { SetValue("RenderVideoFilter", value); }
        }

        public bool RenderSmooth
        {
            get { return GetValue("RenderSmoothing", false); }
            set { SetValue("RenderSmoothing", value); }
        }

        public bool RenderMimicTv
        {
            get { return GetValue("RenderMimicTv", true); }
            set { SetValue("RenderMimicTv", value); }
        }

        public bool RenderDisplayIcon
        {
            get { return GetValue("RenderDisplayIcon", true); }
            set { SetValue("RenderDisplayIcon", value); }
        }

        public bool RenderDebugInfo
        {
            get { return GetValue("RenderDebugInfo", false); }
            set { SetValue("RenderDebugInfo", value); }
        }



        #region Private

        private bool GetValue(string name, bool defValue)
        {
            try
            {
                return GetValue(name, defValue ? 1 : 0) != 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return defValue;
            }
        }

        private void SetValue(string name, bool value)
        {
            try
            {
                SetValue(name, value ? 1 : 0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private SyncSource GetValue(string name, SyncSource defValue) 
        {
            try
            {
                var iValue = GetValue(name, (int)defValue);
                if (Enum.IsDefined(typeof(SyncSource), iValue))
                {
                    return (SyncSource)iValue;
                }
                return defValue;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return defValue;
            }
        }

        private void SetValue(string name, SyncSource value)
        {
            try
            {
                SetValue(name, (int)value);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private ScaleMode GetValue(string name, ScaleMode defValue)
        {
            try
            {
                var iValue = GetValue(name, (int)defValue);
                if (Enum.IsDefined(typeof(ScaleMode), iValue))
                {
                    return (ScaleMode)iValue;
                }
                return defValue;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return defValue;
            }
        }

        private void SetValue(string name, ScaleMode value)
        {
            try
            {
                SetValue(name, (int)value);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private VideoFilter GetValue(string name, VideoFilter defValue)
        {
            try
            {
                var iValue = GetValue(name, (int)defValue);
                if (Enum.IsDefined(typeof(VideoFilter), iValue))
                {
                    return (VideoFilter)iValue;
                }
                return defValue;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return defValue;
            }
        }

        private void SetValue(string name, VideoFilter value)
        {
            try
            {
                SetValue(name, (int)value);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private int GetValue(string name, int defValue)
        {
            try
            {
                var rkey = Registry.CurrentUser.CreateSubKey(RegistryPath);
                if (rkey == null)
                {
                    return defValue;
                }
                var objValue = rkey.GetValue(name);
                if (objValue == null)
                {
                    return defValue;
                }
                return Convert.ToInt32(objValue);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return defValue;
            }
        }

        private void SetValue(string name, int value)
        {
            try
            {
                var rkey = Registry.CurrentUser.CreateSubKey(RegistryPath);
                if (rkey == null)
                {
                    return;
                }
                rkey.SetValue(name, value, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion Private
    }
}
