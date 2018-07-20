using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ZXMAK2.Host.Presentation.Tools
{
    public class WebDownloader
    {
        public WebFile Download(Uri uri)
        {
            var fileName = string.Empty;
            var webRequest = WebRequest.Create(uri);
            webRequest.Timeout = 15000;
            //webRequest.Credentials = new NetworkCredential("anonymous", "User@");
            var webResponse = webRequest.GetResponse();
            try
            {
                fileName = Path.GetFileName(webResponse.ResponseUri.LocalPath);
                if (webResponse.Headers["Content-Disposition"] != null)
                {
                    var dispName = GetContentFileName(
                        webResponse.Headers["Content-Disposition"]);
                    if (!string.IsNullOrEmpty(dispName))
                    {
                        fileName = dispName;
                    }
                    // fix name...
                    foreach (var c in Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(new string(c, 1), string.Empty);
                    }
                }
                using (var stream = webResponse.GetResponseStream())
                {
                    var data = webResponse.ContentLength >= 0 ?
                        DownloadStream(stream, webResponse.ContentLength, webRequest.Timeout) :
                        DownloadStreamNoLength(stream, webRequest.Timeout);
                    return new WebFile(fileName, data);
                }
            }
            finally
            {
                webResponse.Close();
            }
        }

        private static string GetContentFileName(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                return null;
            }
            try
            {
                var contDisp = new ContentDisposition(header);
                if (!string.IsNullOrEmpty(contDisp.FileName))
                {
                    return contDisp.FileName;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Warn("content-disposition bad format: {0}", header);
            try
            {
                var contDisp = new ContentDispositionEx(header);
                if (!string.IsNullOrEmpty(contDisp.FileName))
                    return contDisp.FileName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        private static byte[] DownloadStream(
            Stream stream,
            long length,
            int timeOut)
        {
            var data = new byte[length];
            var read = 0L;
            var tickCount = Environment.TickCount;
            while (read < length)
            {
                read += stream.Read(
                    data,
                    (int)read,
                    (int)(length - read));
                if ((Environment.TickCount - tickCount) > timeOut)
                {
                    throw new TimeoutException("Download timeout error!");
                }
            }
            return data;
        }

        private static byte[] DownloadStreamNoLength(
            Stream stream,
            int timeOut)
        {
            var list = new List<byte>();
            var readBuffer = new byte[0x10000];
            var tickCount = Environment.TickCount;
            while (true)
            {
                if ((Environment.TickCount - tickCount) > timeOut)
                {
                    throw new TimeoutException("Download timeout error!");
                }
                var len = stream.Read(readBuffer, 0, readBuffer.Length);
                if (len == 0)
                {
                    break;
                }
                for (var i = 0; i < len; i++)
                {
                    list.Add(readBuffer[i]);
                }
            }
            return list.ToArray();
        }
    }

    public class WebFile
    {
        public String FileName { get; private set; }
        public byte[] Content { get; private set; }

        public WebFile(String fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
        }
    }

    public class ContentDispositionEx
    {
        private readonly StringDictionary m_params = new StringDictionary();
        private string m_dispType;

        public ContentDispositionEx(string rawValue)
        {
            Parse(rawValue);
        }

        protected void Parse(string rawValue)
        {
            m_params.Clear();
            string[] keyPairs = rawValue.Split(';');
            m_dispType = keyPairs[0];

            for (int i = 1; i < keyPairs.Length; i++)
            {
                string keyPair = keyPairs[i];
                int index = keyPair.IndexOf('=');
                if (index < 0)
                {
                    Logger.Error(
                        "ContentDispositionEx.Parse: invalid key pair '{0}'",
                        keyPair);
                    continue;
                }
                string key = keyPair.Substring(0, index).Trim();
                string value = keyPair.Substring(index + 1).Trim();
                if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 1)
                {
                    value = value.Substring(1, value.Length - 1);
                    value = value.Replace("\\\"", "\"").Trim();
                }
                key = key.ToLower();
                m_params[key] = value;
            }
        }

        public string DispositionType
        {
            get { return m_dispType; }
            set { m_dispType = value; }
        }

        public string FileName
        {
            get { return m_params["filename"]; }
            set { m_params["filename"] = value; }
        }

        public long Size
        {
            get { return m_params.ContainsKey("size") ? Convert.ToInt64(m_params["size"]) : -1; }
            set { if (value < 0) m_params.Remove("size"); else m_params["size"] = Convert.ToString(value); }
        }

        //public DateTime CreationDate
        //{
        //    get { return ParseDateRFC822(m_params["creation-date"]); }
        //    set { m_params["creation-date"] = ToStringDateRFC822(value); }
        //}

        //public DateTime ModificationDate
        //{
        //    get { return ParseDateRFC822(m_params["modification-date"]); }
        //    set { m_params["modification-date"] = ToStringDateRFC822(value); }
        //}
    }
}
