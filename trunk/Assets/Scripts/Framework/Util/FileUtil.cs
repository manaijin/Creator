using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Util
{
    public static class FileUtil
    {
        public static string ReadFileToString(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string content = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            return content;
        }

        public static void WriteFile(string fullPath, string content) 
        {
            var dirPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            FileStream fs = new FileStream(fullPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(content);
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 获取文件路径父节点名称
        /// </summary>
        /// <param name="fullPath">文件完整路径</param>
        /// <returns></returns>
        public static string GetParentDirName(string fullPath) 
        {
            var dirPath = Path.GetDirectoryName(fullPath);
            var superPath = Path.GetDirectoryName(dirPath);
            return dirPath.Replace(superPath, "").Substring(1);
        }

        /// <summary>
        /// 获取根目录下所有资源的路径
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public static string[] GetDirAllAssetFullPath(string dir, bool containChild = true) 
        {
            List<string> assetes = new List<string>();
            if (Directory.Exists(dir))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    if (Directory.Exists(fileInfos[i].FullName) && containChild)
                        assetes.AddRange(GetDirAllAssetFullPath(fileInfos[i].Name));

                    if (fileInfos[i].Name.EndsWith(".meta")) continue;
                    var fullName = fileInfos[i].FullName.Replace('\\','/');
                    assetes.Add(fullName);
                }
            }
            return assetes.ToArray();
        }
    }
}
