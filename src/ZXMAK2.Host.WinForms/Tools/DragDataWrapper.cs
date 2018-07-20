using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;


namespace ZXMAK2.Host.WinForms.Tools
{
    public class DragDataWrapper
    {
        private IDataObject m_dataObject;

        #region Properties

        public bool IsFileDrop
        {
            get { return m_dataObject.GetDataPresent(DataFormatEx.FileDrop); }
        }

        public bool IsLinkDrop
        {
            get { return m_dataObject.GetDataPresent(DataFormatEx.Uri); }
        }

        #endregion

        #region Public

        public DragDataWrapper(IDataObject dataObject)
        {
            m_dataObject = dataObject;
        }

        public string GetFilePath()
        {
            object objData = m_dataObject.GetData(DataFormatEx.FileDrop);
            string[] fileArray = GetStringArray(objData as Array);
            return fileArray.Length == 1 ? fileArray[0] : string.Empty;
        }

        public string GetLinkUri()
        {
            var objData = m_dataObject.GetData(DataFormatEx.Uri);
            var fileUri = string.Empty;
            using (var ms = objData as MemoryStream)
            {
                var data = new byte[ms.Length];
                ms.Read(data, 0, data.Length);
                var length = 0;
                for (; length < data.Length; length++)
                {
                    if (data[length] == 0)
                    {
                        break;
                    }
                }
                fileUri = Encoding.ASCII.GetString(data, 0, length);
            }
            return fileUri.Trim();
        }

        #endregion Public

        #region Private

        private static string[] GetStringArray(Array dataArray)
        {
            var list = new List<string>();
            if (dataArray != null)
            {
                foreach (string value in dataArray)
                {
                    list.Add(value);
                }
            }
            return list.ToArray();
        }

        private static class DataFormatEx
        {
            public static readonly string FileDrop = DataFormats.FileDrop;
            public static readonly string Uri = "UniformResourceLocator";
        }

        #endregion Private
    }
}
