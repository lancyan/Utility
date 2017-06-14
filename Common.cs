
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections;



namespace Utility
{
    public static class Common
    {

        private static readonly Hashtable hashDict = Hashtable.Synchronized(new Hashtable(10240));

       
        /// <summary>
        /// 序列化对象成JSON字符串
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>JSON字符串</returns>
        public static string Serialize(object o)
        {
            if (o != null)
            {
                IsoDateTimeConverter dt = new IsoDateTimeConverter();
                dt.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                return JsonConvert.SerializeObject(o, dt);
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 反序列化JSON字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">字符串</param>
        /// <returns>对象</returns>
        public static T Deserialize<T>(string data)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(data))
                {
                    return JsonConvert.DeserializeObject<T>(data);
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                throw;
            }
        }

        public static string GetQuerySQL<T>(T model, bool ignoreDefault = true)
        {
            string query = "";
            if (model != null)
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                StringBuilder sb = new StringBuilder();
                Type tp = typeof(DateTime);
                for (int i = 0, len = pis.Length; i < len; i++)
                {
                    PropertyInfo pi = pis[i];
                    object v = pi.GetValue(model, null);
                    Type type = pi.PropertyType;
                    bool isDefault = JudgeDefault(type, v);

                    if (!isDefault || (isDefault && !ignoreDefault))
                    {
                        string prePN = GetAttributeName(pi);
                        if (type.Equals(typeof(DateTime)))
                        {
                            DateTime dt = Convert.ToDateTime(v);
                            sb.Append(string.Format("{0}{1} between '{2}'  and  '{3}' and ", prePN, pi.Name, dt.Date.ToString(), dt.AddDays(1).ToString()));
                        }
                        else if (type.IsValueType)
                        {
                            sb.Append(string.Format("{0}{1}={2} and ", prePN, pi.Name, v));
                        }
                        else
                        {
                            sb.Append(string.Format("{0}{1} like '%{2}%' and ", prePN, pi.Name, v));
                        }
                    }
                }
                string tmpQuery = sb.ToString();
                if (tmpQuery.EndsWith("and "))
                {
                    tmpQuery = tmpQuery.Remove(tmpQuery.Length - 4, 4);
                }
                query = tmpQuery;
            }
            return query;
        }

        static DateTime defaultDate = new DateTime(1900, 1, 1);

        private static bool JudgeDefault(Type type, object v)
        {
            bool isDefault = false;

            if (type.IsValueType)
            {
                if (type.Name == "Nullable`1")//type.GetGenericTypeDefinition() == typeof(Nullable<>)
                {
                    if (v == null)
                    {
                        isDefault = true;
                    }
                }
                else if (type.Name == "DateTime")
                {
                    if (DateTime.MinValue.Equals(v) || defaultDate.Equals(v))
                    {
                        isDefault = true;
                    }
                }
                else
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj.Equals(v))
                    {
                        isDefault = true;
                    }
                }
            }
            else
            {
                if (v == null || string.IsNullOrEmpty(v.ToString()))
                {
                    isDefault = true;
                }
            }
            return isDefault;
        }

        public static string Where2Query<T>(string where)
        {
            var str = hashDict[where];
            if (str != null)
            {
                return str.ToString();
            }
            else
            {
                string sql = "";
                T obj = default(T);
                try
                {
                    if (!string.IsNullOrWhiteSpace(where))
                    {
                        obj = Deserialize<T>(where);
                        sql = GetQuerySQL<T>(obj);
                        hashDict[where] = sql;
                    }
                }
                catch
                {
                    throw;
                }
                return sql;
            }
        }

        #region 将Base64编码的文本转换成普通文本
        /// <summary>
        /// 将Base64编码的文本转换成普通文本
        /// </summary>
        /// <param name="base64">Base64编码的文本</param>
        /// <returns></returns>
        public static string Base64ToString(string base64)
        {
            if (!string.IsNullOrWhiteSpace(base64))
            {
                char[] charBuffer = base64.ToCharArray();
                byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
                return Encoding.Default.GetString(bytes);
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region 字符串转为base64字符串
        public static string StringToBase64(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                byte[] b = Encoding.Default.GetBytes(str);
                return Convert.ToBase64String(b);
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion


        public static string GetAttributeName(MemberInfo mi)
        {
            DescriptionAttribute att = Attribute.GetCustomAttribute(mi, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return att != null ? att.Description : "";
        }

        public static string GetQueryString<T>(T obj, bool ignoreDefault = true)
        {
            StringBuilder sb = new StringBuilder();
            if (obj != null)
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                for (int i = 0, len = pis.Length; i < len; i++)
                {
                    PropertyInfo pi = pis[i];
                    string pn = pi.Name;
                    object v = pi.GetValue(obj);
                    Type type = pi.PropertyType;
                    bool isDefault = JudgeDefault(type, v);

                    if (!isDefault || (isDefault && !ignoreDefault))
                    {
                        if (v != null)
                        {
                            if (IsBaseType(type))
                            {
                                if (!string.IsNullOrEmpty(v.ToString()))
                                {
                                    sb.Append(pn + "=" + v.ToString() + "&");
                                }
                            }
                            else
                            {
                                sb.Append(pn + "=" + JsonConvert.SerializeObject(v) + "&");
                            }
                        }
                    }
                }
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            return sb.ToString();
        }

        public static string GetQueryString<T>(T obj, bool ignoreNull, bool ignoreDefault = true)
        {
            StringBuilder sb = new StringBuilder();
            if (obj != null)
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                for (int i = 0, len = pis.Length; i < len; i++)
                {
                    PropertyInfo pi = pis[i];
                    Type type = pi.PropertyType;
                    string pn = pi.Name;
                    object v = pi.GetValue(obj);
                    bool isDefault = JudgeDefault(type, v);

                    if (!isDefault || (isDefault && !ignoreDefault))
                    {
                        if (v != null)
                        {
                            if (IsBaseType(type))
                            {
                                if (!string.IsNullOrEmpty(v.ToString()) )
                                {
                                    sb.Append(pn + "=" + v.ToString() + "&");
                                }
                                else if(!ignoreNull)
                                {
                                    sb.Append(pn + "=''&");
                                }
                            }
                            else
                            {
                                sb.Append(pn + "=" + JsonConvert.SerializeObject(v) + "&");
                            }
                        }
                    }
                }
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            return sb.ToString();
        }

        public static bool IsBaseType(Type t)
        {
            return t.IsValueType || t == typeof(string);
        }


        public static NameValueCollection FillFromString(string query, bool urlencoded, Encoding encoding)
        {
            NameValueCollection queryString = new NameValueCollection();
            if (string.IsNullOrEmpty(query))
            {
                return queryString;
            }

            // 确保 查询文本首字符不是 ?
            if (query.StartsWith("?"))
            {
                query = query.Substring(1, query.Length - 1);
            }

            int num1 = (query != null) ? query.Length : 0;
            // 遍历每个字符
            for (int num2 = 0; num2 < num1; num2++)
            {
                int num3 = num2;
                int num4 = -1;
                while (num2 < num1)
                {
                    switch (query[num2])
                    {
                        case '=':
                            if (num4 < 0)
                            {
                                num4 = num2;
                            }
                            break;
                        case '&':
                            goto BREAKWHILE;
                    }
                    num2++;
                }

            BREAKWHILE:
                string name = null;
                string val = null;
                if (num4 >= 0)
                {
                    name = query.Substring(num3, num4 - num3);
                    val = query.Substring(num4 + 1, (num2 - num4) - 1);
                }
                else
                {
                    val = query.Substring(num3, num2 - num3);
                }
                if (urlencoded)
                {
                    queryString.Add(HttpUtility.UrlDecode(name, encoding), HttpUtility.UrlDecode(val, encoding));
                }
                else
                {
                    queryString.Add(name, val);
                }
                if ((num2 == (num1 - 1)) && (query[num2] == '&'))
                {
                    queryString.Add(null, string.Empty);
                }
            }
            return queryString;
        }


        public static T NameValue2Object<T>(NameValueCollection collect)
        {
            T obj = default(T);
            Type type = typeof(T);

            foreach (var item in collect.Keys)
            {
                if (item != null)
                {
                    string pn = item.ToString();
                    var pi = type.GetProperty(pn, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (pi != null)
                    {
                        pi.SetValue(obj, collect[pn]);
                    }
                }
            }
            return obj;
        }

        public static dynamic NameValue2Object(NameValueCollection collect, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var item in collect.Keys)
            {
                if (item != null)
                {
                    string pn = item.ToString();
                    var pi = type.GetProperty(pn, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (pi != null)
                    {
                        pi.SetValue(obj, HackType(collect[pn], pi.PropertyType));
                    }
                }
            }
            return obj;
        }

        public static object HackType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                    conversionType = nullableConverter.UnderlyingType;
                }
            }
            return value == null ? null : Convert.ChangeType(value, conversionType);
        }

        public static NameValueCollection ToNameValueCollection(this JObject jo)
        {
            var qs = new NameValueCollection();
            foreach (var item in jo)
            {
                qs.Add(item.Key, item.Value.ToString());
            }
            return qs;
        }

        public static NameValueCollection Combine(this NameValueCollection nvc, NameValueCollection nvc2)
        {
            foreach (string item in nvc2)
            {
                nvc.Add(item, nvc2[item]);
            }
            return nvc;
        }

        /// <summary>
        /// 获取字符串字节长度(中文双字节)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int StringTrueLength(string str)
        {
            int lenTotal = 0;
            int n = str.Length;
            string strWord = "";
            int asc;
            for (int i = 0; i < n; i++)
            {
                strWord = str.Substring(i, 1);
                asc = Convert.ToChar(strWord);
                if (asc < 0 || asc > 127)
                    lenTotal = lenTotal + 2;
                else
                    lenTotal = lenTotal + 1;
            }

            return lenTotal;

        }

        public static string GetTimeStr(DateTime time1, DateTime time2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(time1.Ticks);
                TimeSpan ts2 = new TimeSpan(time2.Ticks);
                if (ts2 > ts1)
                {
                    dateDiff = "00:00";
                }
                else
                {
                    TimeSpan ts = ts1 - ts2;
                    int hours = ts.Hours, minutes = ts.Minutes, seconds = ts.Seconds;
                    minutes = hours * 60 + minutes;
                    dateDiff = (minutes < 10 ? "0" + minutes : minutes.ToString()) + ":" + (seconds < 10 ? "0" + seconds : seconds.ToString());
                }
            }
            catch
            {

            }
            return dateDiff;
        }

        #region 获取web客户端ip
        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIP()
        {
            string userIP = "未获取用户IP";
            try
            {
                if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";

                string CustomerIP = "";

                //CDN加速后取到的IP simone 090805
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }

            return userIP;

        }
        #endregion

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static int ConvertDateTimeInt(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }  

    }



}
