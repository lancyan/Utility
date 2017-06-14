using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;
using System.Web;

namespace Utility
{
    public class HtmlHelper
    {
        public HtmlHelper()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; };
            try
            {
                wf = new LogWriter("htmllog.txt");
            }
            catch (Exception ex)
            {
                wf = new LogWriter(System.Configuration.ConfigurationManager.AppSettings["htmllog"].ToString());
            }
        }


        public LogWriter wf = null;


        /// <summary>
        /// 流转字节码
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private byte[] getByteContent(Stream stream)
        {
            List<byte> list = new List<byte>();
            int offset = 1024;
            byte[] buffer = new byte[1024];
            int count = 0;
            do
            {
                count = stream.Read(buffer, 0, offset);
                for (int i = 0; i < count; i++)
                {
                    list.Add(buffer[i]);
                }
            }
            while (count > 0);
            return list.ToArray();
        }

        /// <summary>
        /// POST请求数据
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parasString">参数</param>
        /// <param name="inCookie">cookie</param>
        /// <returns>返回字符串</returns>
        public string Post(string url, string parasString, string inCookie = null)
        {
            string htmlString = string.Empty;
            string outCookie = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(inCookie))
                {
                    request.Headers.Add(HttpRequestHeader.Cookie, inCookie);
                }
                request.Accept = "*/*";
                request.Method = "POST";
                request.KeepAlive = false;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1)";
                request.Timeout = 10000;
                request.ReadWriteTimeout = 10000;
                request.ServicePoint.ConnectionLimit = int.MaxValue;
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                request.AllowWriteStreamBuffering = false;
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                byte[] bs = Encoding.UTF8.GetBytes(parasString);
                request.ContentLength = bs.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    HttpStatusCode status = response.StatusCode;
                    if (status == HttpStatusCode.OK || status == HttpStatusCode.Moved)
                    {
                        Stream stream = response.GetResponseStream();
                        string outCookieTemp = response.Headers["Set-Cookie"];
                        if (!string.IsNullOrEmpty(outCookieTemp))
                        {
                            outCookie = outCookieTemp;
                        }
                        string contentEncode = response.ContentEncoding.ToLower();
                        byte[] buffer = getBuffer(contentEncode, stream);
                        htmlString = EncodeHelper.GetEncoding(buffer).GetString(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                wf.WriteLine(DateTime.Now.ToString("yyyyMMdd HHmmss") + " URL: " + url + " error: " + ex.Message);
            }
            return htmlString;
        }

        /// <summary>
        /// 流解压成字节码
        /// </summary>
        /// <param name="contentEncode"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private byte[] getBuffer(string contentEncode, Stream stream)
        {
            byte[] buffer = null;
            if (contentEncode.Contains("gzip"))
            {
                using (GZipStream gs = new GZipStream(stream, CompressionMode.Decompress))
                {
                    buffer = getByteContent(gs);
                }
            }
            else if (contentEncode.Contains("deflate"))
            {
                using (DeflateStream ds = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    buffer = getByteContent(ds);
                }
            }
            else
            {
                buffer = getByteContent(stream);
            }
            return buffer;
        }


        /// <summary>
        /// GET 请求数据
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="encode">编码方式</param>
        /// <param name="inCookie">cookie</param>
        /// <returns>返回字符串</returns>
        public string Get(string url, string encode = null, string inCookie = null)
        {
            string html = string.Empty;
            Stream stream = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Accept = "*/*";
                request.Method = "GET";
                request.KeepAlive = false;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1)";
                //request.Timeout = 10000;
                //request.ReadWriteTimeout = 10000;
                request.ServicePoint.ConnectionLimit = int.MaxValue;
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                request.AllowWriteStreamBuffering = false;
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                if (!string.IsNullOrEmpty(inCookie))
                {
                    request.Headers.Add(HttpRequestHeader.Cookie, inCookie);
                }
                response = (HttpWebResponse)request.GetResponse();
                //if (!request.HaveResponse)
                //{
                //    response.Close();
                //    request.Abort();
                //    return html;
                //}
                HttpStatusCode status = response.StatusCode;
                if (status == HttpStatusCode.OK || status == HttpStatusCode.Moved)
                {
                    stream = response.GetResponseStream();
                    string contentEncode = response.ContentEncoding.ToLower();

                    byte[] buffer = getBuffer(contentEncode, stream);
                    if (string.IsNullOrEmpty(encode))
                    {
                        Encoding encoding = EncodeHelper.GetEncoding(buffer);
                        html = encoding.GetString(buffer);
                    }
                    else
                    {
                        html = System.Text.Encoding.GetEncoding(encode).GetString(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                wf.WriteLine(DateTime.Now.ToString("yyyyMMdd HHmmss") + " URL: " + url + " error: " + ex.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
                if (stream != null)
                    stream.Close();
            }
            return html;
        }
    }
}
