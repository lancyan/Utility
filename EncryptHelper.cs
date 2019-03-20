using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
//using System.Web.Security;

namespace Utility
{
    public class EncryptHelper
    {
        private EncryptHelper()
        {
        }

        #region DES
        /// <summary>
        /// 初始向量
        /// </summary>
        private static byte[] cryptIV = new byte[] { 18, 52, 86, 120, 144, 171, 205, 239 };

        /// <summary>
        /// 8位Key  
        /// </summary>
        private static byte[] cryptKey = new byte[] { 97, 117, 116, 111, 64, 35, 36, 38 };

        private static DESCryptoServiceProvider CreateDESCrypto()
        {
            return new System.Security.Cryptography.DESCryptoServiceProvider
            {
                KeySize = 64,
                BlockSize = 64
            };
        }

        public static string DESEncrypt(string value)
        {
            return EncryptHelper.DESEncrypt(value, Encoding.Default);
        }
        public static string DESEncrypt(string value, Encoding encoding)
        {
            return EncryptHelper.DESEncrypt(value, encoding, EncryptHelper.cryptKey, EncryptHelper.cryptIV);
        }
        public static string DESEncrypt(string value, Encoding encoding, byte[] cryptKey = null, byte[] cryptIV = null)
        {
            byte[] bytes = encoding.GetBytes(value);
            byte[] inArray = EncryptHelper.DESEncrypt(bytes, cryptKey, cryptIV);
            return System.Convert.ToBase64String(inArray);
        }

        public static byte[] DESEncrypt(byte[] data, byte[] cryptKey = null, byte[] cryptIV = null)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = EncryptHelper.CreateDESCrypto();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(cryptKey, cryptIV), CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }
        public static string DESDecrypt(string value)
        {
            return EncryptHelper.DESDecrypt(value, Encoding.Default);
        }
        public static string DESDecrypt(string value, Encoding encoding)
        {
            return EncryptHelper.DESDecrypt(value, encoding, EncryptHelper.cryptKey, EncryptHelper.cryptIV);
        }
        public static string DESDecrypt(string value, Encoding encoding, byte[] cryptKey, byte[] cryptIV)
        {
            byte[] data = System.Convert.FromBase64String(value);
            byte[] bytes = EncryptHelper.DESDecrypt(data, cryptKey, cryptIV);
            return encoding.GetString(bytes);
        }
        public static byte[] DESDecrypt(byte[] data, byte[] cryptKey = null, byte[] cryptIV = null)
        {
            System.Security.Cryptography.DESCryptoServiceProvider dESCryptoServiceProvider = EncryptHelper.CreateDESCrypto();
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(cryptKey, cryptIV), System.Security.Cryptography.CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }

        #endregion

        #region TripleDES
        /// <summary>
        /// 24位Key或者16位Key
        /// </summary>
        public static string strKey = "0123456789QWEQWEEWQQ1234";
        /// <summary>
        /// 私有key
        /// </summary>
        public static string PrivateKey = "YWJjZGVmZ2hpamtsbW5vcHFy";

        public static string TripleDESEncrypt(string pToEncrypt, string key = "")
        {
            string result;
            try
            {
                key = string.IsNullOrWhiteSpace(key) ? EncryptHelper.strKey : key;
                StringBuilder stringBuilder = new StringBuilder();
                TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
                tripleDESCryptoServiceProvider.Key = System.Text.Encoding.ASCII.GetBytes(key);
                tripleDESCryptoServiceProvider.Mode = System.Security.Cryptography.CipherMode.ECB;
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(pToEncrypt);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] array = memoryStream.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    byte b = array[i];
                    stringBuilder.AppendFormat("{0:X2}", b);
                }
                result = stringBuilder.ToString();
            }
            catch (System.Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }
        public static string TripleDESDecrypt(string pToDecrypt, string key = "")
        {
            string result;
            try
            {
                key = (string.IsNullOrWhiteSpace(key) ? EncryptHelper.strKey : key);
                byte[] array = new byte[pToDecrypt.Length / 2];
                for (int i = 0; i < pToDecrypt.Length / 2; i++)
                {
                    int num = System.Convert.ToInt32(pToDecrypt.Substring(i * 2, 2), 16);
                    array[i] = (byte)num;
                }
                TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
                tripleDESCryptoServiceProvider.Key = System.Text.Encoding.ASCII.GetBytes(key);
                tripleDESCryptoServiceProvider.Mode = System.Security.Cryptography.CipherMode.ECB;
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                cryptoStream.Write(array, 0, array.Length);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
                memoryStream.Close();
                result = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (System.Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        #endregion

        #region AES
        /// <summary>
        /// 16位密钥
        /// </summary>
        private static string AESKey
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["aesKey"].ToString();    ////必须是16位
            }
        }
        //默认密钥向量 
        private static byte[] _key1 = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// AES加密算法
        /// </summary>
        /// <param name="plainText">明文字符串</param>
        /// <returns>将加密后的密文转换为Base64编码，以便显示</returns>
        public static string AESEncrypt(string plainText, bool isEncode = true)
        {
            //分组加密算法
            SymmetricAlgorithm des = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组 
            //设置密钥及密钥向量
            des.Key = Encoding.UTF8.GetBytes(AESKey);
            des.IV = _key1;
            byte[] cipherBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();//得到加密后的字节数组
                }
            }
            string result = Convert.ToBase64String(cipherBytes);

            return isEncode ? result.Replace("+", "%2B") : result;

        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">密文字符串</param>
        /// <returns>返回解密后的明文字符串</returns>
        public static string AESDecrypt(string showText)
        {
            byte[] cipherText = Convert.FromBase64String(showText);
            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = Encoding.UTF8.GetBytes(AESKey);
            des.IV = _key1;
            byte[] decryptBytes = new byte[cipherText.Length];
            try
            {
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cs.Read(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Encoding.UTF8.GetString(decryptBytes).Replace("\0", "");   ///将字符串后尾的'\0'去掉
        }
        #endregion

        #region Base64
        public static string Base64Encode(string value)
        {
            return EncryptHelper.Base64Encode(value, System.Text.Encoding.Default);
        }
        public static string Base64Encode(string value, System.Text.Encoding encoding)
        {
            string result = "";
            try
            {
                byte[] bytes = encoding.GetBytes(value);
                result = System.Convert.ToBase64String(bytes);
            }
            catch
            {
                result = value;
            }
            return result;
        }
        public static string Base64Decode(string value)
        {
            return EncryptHelper.Base64Decode(value, System.Text.Encoding.Default);
        }
        public static string Base64Decode(string value, System.Text.Encoding encoding)
        {
            string result = "";
            try
            {
                byte[] bytes = System.Convert.FromBase64String(value);
                result = encoding.GetString(bytes);
            }
            catch
            {
                result = value;
            }
            return result;
        }

        #endregion

        #region Shift
        public static string ShiftEncrypt(string value)
        {
            string text = "";
            for (int i = 0; i < value.Length; i++)
            {
                int value2 = (int)(value[i] + '\u0003');
                text += System.Convert.ToChar(value2);
            }
            return text;
        }
        public static string ShiftDecrypt(string value)
        {
            string text = "";
            for (int i = 0; i < value.Length; i++)
            {
                int value2 = (int)(value[i] - '\u0003');
                text += System.Convert.ToChar(value2);
            }
            return text;
        }
        #endregion

        #region RSA
        private static string m_PrivateKey = @"<RSAKeyValue><Modulus>MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAJTgtiHhyl01AbzF/DnDTRX5PYVkHgxHuy8fXOCnPN1F2QrKUaER9a0HRJd+WcQI8oYBHWf7GmlfTN8x6m14l1AuUlTXCljgZRcNEZma0apSW5tu94R8balEtJaPfO0i1tHFc47fEYhnYfzktVsxt2YpVN/S6q/J+PeM4Dvy+yNtAgMBAAECgYBEzdVes0si6Gx1Icr/pxLpJNcZ3rtEUaJglM4HxUKLwMweAILZPcOcw88fdHVn8/qhk8JTW+lI6ZJNVHRTQ3gqEcfA8jjh+9T/Z34UWhgkVw4/5np2M28G7+RF+NDDteW7qFMw4gKY9slMw4Gck4sob+kV3TvkAPaCPWFXQZ/4AQJBAMWoI1U2xCP48C2Ho152Ap4BvfyggtoHSYshMFhAjz9zlgeylzxFNzZ8gbWJmcEP8t3GhReu9fv42AIRM0e+wc0CQQDA0psKme9wXIE9ac+f67GVXD2PDq8GBiJWWo0NhsaA/e3cu3rnvRP8RVU1z6jrPzfVWQXitQVtfTBHS9EP7MghAkAJBIjIJH2CXqMmkJ+leaDY8J9oXTJbHCYA0PzRqfBfJrjblQxNVaMVO0z3qVV4d2/PKnV8BSF343yHa515UnypAkEAi4ZZhdxJc+ab5hJwmGl2AHvUV3Xqk9NQeWfgdQ83CBO2UGig0JryoTKSK/PtaPw/rHNUXO1b1hQmIRDtYDaXwQJACEYOiwLx3TNFVc25q3ZrWmRLWCERPSjqcfesX7dMLkcePOoVS+of18iv3xhBoWmY1RAAg9g87ir64IOUCMs/5A==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static string m_PublicKey = @"<RSAKeyValue><Modulus>MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCU4LYh4cpdNQG8xfw5w00V+T2FZB4MR7svH1zgpzzdRdkKylGhEfWtB0SXflnECPKGAR1n+xppX0zfMepteJdQLlJU1wpY4GUXDRGZmtGqUlubbveEfG2pRLSWj3ztItbRxXOO3xGIZ2H85LVbMbdmKVTf0uqvyfj3jOA78vsjbQIDAQAB</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        //private static string m_ProductModule = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCjTuSRErEquzrKcGt35XdOtHWqWB94XU70XZs2qb+Ae4A+21qPW2kD0yRggyiY53DjX/jfzZD4r4Nj8qcBeuXr4dUh2bLcQRHfMdV/5fg3jLmLjhSHabVVarURc0HoFOJuXxjSPrn/2r/l5O42x0UszhyWpIigj4QKNp8MD3JGRQIDAQAB";
        /// <summary>   
        /// RSA encrypt   
        /// </summary>   
        /// <param name="publickey"></param>   
        /// <param name="content"></param>   
        /// <returns></returns>   
        public static string RSAEncrypt(string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(m_PublicKey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
            //return cipherbytes;
        }

        /// <summary>   
        /// RSA decrypt   
        /// </summary>   
        /// <param name="privatekey"></param>   
        /// <param name="content"></param>   
        /// <returns></returns>   
        public static string RSADecrypt(string content) //byte[] content
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(m_PrivateKey);
            //cipherbytes = rsa.Decrypt(content, false);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);
            return Encoding.UTF8.GetString(cipherbytes);
        }
        #endregion

        #region Md5
        public static string Md5(string input, int code = 32, Encoding encoding = null)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] buffer;
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            if (code == 16)
            {
                buffer = md5.ComputeHash(encoding.GetBytes(input), 4, 8);
            }
            else
            {
                buffer = md5.ComputeHash(encoding.GetBytes(input));
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Md5(Stream input, int code = 32, Encoding encoding = null)
        {
            string result;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            if (code == 16)
            {
                result = BitConverter.ToString(md5.ComputeHash(input), 4, 8);
            }
            else
            {
                result = BitConverter.ToString(md5.ComputeHash(input));
            }
            result = result.Replace("-", "");
            return result;
        }
        #endregion

        #region SHA
        /// <summary>
        /// SHA256加密（64位字符）
        /// </summary>
        /// /// <param name="str">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string SHAEncrypt(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }

       
        #endregion

        #region SHA1 (数字签名)

        public static string SHA1(Stream stream, int code = 32)
        {
            string result;
            System.Security.Cryptography.HashAlgorithm hash;
            hash = System.Security.Cryptography.SHA1.Create();


            if (code == 16)
            {
                result = BitConverter.ToString(hash.ComputeHash(stream), 4, 8);
            }
            else
            {
                result = BitConverter.ToString(hash.ComputeHash(stream));
            }
            result = result.Replace("-", "");
            return result;
        }

        /// <summary>
        /// SHA1加密（40位字符）
        /// </summary>
        /// <param name="strSource">源字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密后的字符串</returns>
        public static string GetSHA1(string strSource, System.Text.Encoding encoding = null)
        {
            StringBuilder strResult = new StringBuilder();
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] bytResult = sha.ComputeHash(encoding.GetBytes(strSource));
            for (int i = 0, len = bytResult.Length; i < len; i++)
            {
                strResult.Append(bytResult[i].ToString("X2"));
            }
            return strResult.ToString();
        }

        public static byte[] GetSHA1(byte[] value)
        {
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            return sha.ComputeHash(value);
        }

        #endregion SHA1

        #region HMAC
        public static string ToHMACSHA1(string encryptText, string encryptKey)
        {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(encryptKey);
            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(encryptText);
            using (HMACSHA1 hmacsha1 = new HMACSHA1(key))
            {
                byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
                return Convert.ToBase64String(hashBytes);
            }
        }
        private string ToHMACSHA256(string message, string secret = "")
        {
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        #endregion
    }
}
