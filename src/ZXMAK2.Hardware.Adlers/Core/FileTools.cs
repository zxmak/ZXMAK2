using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZXMAK2.Hardware.Adlers.Core
{
    class FileTools
    {
        public static string GetFileDirectory(string i_fileName)
        {
            int indexOfLastPathSep = i_fileName.LastIndexOf(Path.DirectorySeparatorChar);

            return i_fileName.Substring(0, indexOfLastPathSep) + Path.DirectorySeparatorChar;
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        public static bool ReadFile(string i_fileName, out string o_strFileText)
        {
            o_strFileText = null;
            if (!File.Exists(i_fileName))
                return false;
            FileInfo fileInfo = new FileInfo(i_fileName);
            if (FileTools.IsFileLocked(fileInfo))
                return false;

            int s_len = (int)fileInfo.Length;
            byte[] data = new byte[s_len];
            using (FileStream fs = new FileStream(i_fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                fs.Read(data, 0, data.Length);
            o_strFileText = Encoding.UTF8.GetString(data, 0, data.Length);

            return true;
        }

        public static bool IsPathCorrect(string i_PathToCheck)
        {
            try
            {
                Path.Combine(i_PathToCheck);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
