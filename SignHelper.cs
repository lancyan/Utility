
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
//using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class SignHelper
    {
        public const string appId = "appId";
        public const string appKey = "appKey";
        public const string appSecret = "appSecret";

        public const string userId = "userId";
        public const string token = "token";
        public const string sign = "sign";
        public static string[] confuseArr = new string[] { "!#%&(", "@$^*&", "&*(%)" };
        //appKey Arrays
        public static string[] keyArr = new string[] { "687D7C5DD80E4F0588D5FC2327DE248A", "A35D05A7E929497BA325EA2029C04DBC", "83EF032868A84459B98B987E5686F9AD" };
        //appSecret Arrays
        public static string[] valueArr = new string[] { "D24E0AEF6ED947359EE92E068E38F518", "4AF86EBAAA3D420ABB48B4103B1C3531", "4336444F21AA4DB48EE51368BB5C1092" };

       

       

        static SignHelper()
        {
            
        }

        public static bool IsPassVerify(NameValueCollection dict1, Dictionary<string, object> dict2)
        {
            bool re = false;
            string appId = GetSign(SignHelper.appId, dict1, dict2);
            string clientToken = GetSign(SignHelper.token, dict1, dict2);
            string serverToken = "";

            switch (appId)
            {
                case "687D7C5DD80E4F0588D5FC2327DE248A":
                    serverToken = CreateToken(valueArr[0], confuseArr[0], DateTime.Now.ToString("yyyy/MM/d/HH"));
                    re = !string.IsNullOrEmpty(clientToken) && serverToken.Equals(clientToken, StringComparison.OrdinalIgnoreCase);
                    break;
                case "A35D05A7E929497BA325EA2029C04DBC":
                    serverToken = CreateToken(valueArr[1], confuseArr[1], DateTime.Now.ToString("M/d/yyyy/HH"));
                    re = !string.IsNullOrEmpty(clientToken) && serverToken.Equals(clientToken, StringComparison.OrdinalIgnoreCase);
                    break;
                case "83EF032868A84459B98B987E5686F9AD":
                    serverToken = CreateToken(valueArr[2], confuseArr[2], DateTime.Now.ToString("d/M/yyyy/HH"));
                    re = !string.IsNullOrEmpty(clientToken) && serverToken.Equals(clientToken, StringComparison.OrdinalIgnoreCase);
                    break;
            }
            return re;
        }

        public static string CreateToken(string s1, string s2, string s3)
        {
            return EncryptHelper.Md5(string.Concat(s1, s2, s3), 32, System.Text.Encoding.UTF8);
        }

        private static string GetSign(string key, NameValueCollection dict1, Dictionary<string, object> dict2)
        {
            string val = "";

            if (dict1.Count > 0)
            {
                if (dict1[key] != null)
                {
                    val = dict1[key];
                    return val;
                }

            }
            if (dict2.Count > 0)
            {
                if (dict2[key] != null)
                {
                    val = (dict2[key]).ToString();
                    return val;
                }
            }
            return val;
        }

    }




  

    
}
