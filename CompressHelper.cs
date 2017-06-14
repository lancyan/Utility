using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Utility
{
    public class CompressHelper
    {
        /// <summary>  
        /// GZip压缩  
        /// </summary>  
        /// <param name="rawData"></param>  
        /// <returns></returns>  
        public static byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }
        /// <summary>  
        /// ZIP解压  
        /// </summary>  
        /// <param name="zippedData"></param>  
        /// <returns></returns>  
        public static byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }

        /// <summary>
        /// 压缩某文件夹下的文件
        /// </summary>
        /// <param name="dirPath">文件夹地址</param>
        /// <param name="zipFilePath">压缩包文件路径</param>
        /// <returns></returns>
        public static bool DirCompress(string dirPath, string zipFilePath)
        {
            try
            {
                string[] filePaths = Directory.GetFiles(dirPath);
                using (ZipFile zip = ZipFile.Create(zipFilePath))
                {
                    zip.NameTransform = new ZipNameTransform(dirPath);
                    zip.BeginUpdate();
                    foreach (string filePath in filePaths)
                    {
                        zip.Add(filePath);
                    }
                    zip.CommitUpdate();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        public static byte[] File2Bytes(string fileName)
        {
            byte[] arrFile = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                arrFile = new byte[fs.Length];
                fs.Read(arrFile, 0, arrFile.Length);
            }
            return arrFile;
        }

    }
}
