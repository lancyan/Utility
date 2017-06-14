using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Utility
{
    public class SQLReinject
    {
        //// < summary>
        /// 处理用户提交的请求
        /// < /summary>
        public static void StartProcessRequest()
        {
            try
            {
                string getkeys = "";
                var request = HttpContext.Current.Request;
                if (request.QueryString != null)
                {
                    for (int i = 0; i < request.QueryString.Count; i++)
                    {
                        getkeys = request.QueryString.Keys[i];
                        if (!ProcessSqlStr(request.QueryString[getkeys], 0))
                        {
                            HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请勿非法提交！');history.back();< /script>");
                            HttpContext.Current.Response.End();
                        }
                    }
                }
                if (request.Form != null)
                {
                    for (int i = 0; i < request.Form.Count; i++)
                    {
                        getkeys = request.Form.Keys[i];
                        if (!ProcessSqlStr(request.Form[getkeys], 1))
                        {
                            HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请勿非法提交！');history.back();< /script>");
                            HttpContext.Current.Response.End();
                        }
                    }
                }
            }
            catch
            {
                // 错误处理： 处理用户提交信息！
            }
        }

        /// < summary>
        /// 分析用户请求是否正常
        /// < /summary>
        /// < param name="Str">传入用户提交数据< /param>
        /// < returns>返回是否含有SQL注入式攻击代码< /returns>
        public static bool ProcessSqlStr(string str, int type = 1)
        {
            string SqlStr;
            if (type == 1)
                SqlStr = "exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
            else
                SqlStr = "'|and|exec|insert|select|delete|update|count|*|chr|mid|master|truncate|char|declare";
            bool returnValue = true;
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string s in anySqlStr)
                    {
                        if (str.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            returnValue = false;
                        }
                    }
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }
    }
}
