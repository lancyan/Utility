using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Utility
{
   public class EncodeHelper
    {
        public static Encoding GetEncoding(byte[] bytes)
        {
            Encoding encoding = Encoding.GetEncoding("gb18030");
            try
            {
                if (IsUTF8Code(bytes))
                {
                    return Encoding.GetEncoding("utf-8");
                }
            }
            catch
            {
            }
            try
            {
                if (IsGB2312Code(bytes))
                {
                    return Encoding.GetEncoding("gb2312");
                }
            }
            catch
            {
            }
            try
            {
                if (IsBIG5Code(bytes))
                {
                    return Encoding.GetEncoding("big5");
                }
            }
            catch
            {
            }
            try
            {
                if (IsGBKCode(bytes))
                {
                    return Encoding.GetEncoding("gbk");
                }
            }
            catch
            {
            }
            return encoding;
        }

        //http://blog.chinaunix.net/uid-14348211-id-2821150.html
        static bool IsUTF8SpecialByte(byte c)
        {
            int special_byte = 0X02; //binary 00000010    
            if (c >> 6 == special_byte)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool IsUTF8Code(byte[] str)
        {
            int one_byte = 0X00; //binary 00000000
            int two_byte = 0X06; //binary 00000110
            int three_byte = 0X0E; //binary 00001110  
            int four_byte = 0X1E; //binary 00011110
            int five_byte = 0X3E; //binary 00111110
            int six_byte = 0X7E; //binary 01111110
            int utf8_yes = 0;
            int utf8_no = 0;
            byte k = 0;
            byte m = 0;
            byte n = 0;
            byte p = 0;
            byte q = 0;
            byte c = 0;
            for (uint i = 0; i < str.Length; )
            {
                c = (byte)str[i];
                if (c >> 7 == one_byte)
                {
                    i++;
                    continue;
                }
                else if (c >> 5 == two_byte)
                {
                    k = (byte)str[i + 1];
                    if (IsUTF8SpecialByte(k))
                    {
                        utf8_yes++;
                        i += 2;
                        continue;
                    }
                }
                else if (c >> 4 == three_byte)
                {
                    m = (byte)str[i + 1];
                    n = (byte)str[i + 2];
                    if (IsUTF8SpecialByte(m) && IsUTF8SpecialByte(n))
                    {
                        utf8_yes++;
                        i += 3;
                        continue;
                    }
                }
                else if (c >> 3 == four_byte)
                {
                    k = (byte)str[i + 1];
                    m = (byte)str[i + 2];
                    n = (byte)str[i + 3];
                    if (IsUTF8SpecialByte(k) && IsUTF8SpecialByte(m) && IsUTF8SpecialByte(n))
                    {
                        utf8_yes++;
                        i += 4;
                        continue;
                    }
                }
                else if (c >> 2 == five_byte)
                {
                    k = (byte)str[i + 1];
                    m = (byte)str[i + 2];
                    n = (byte)str[i + 3];
                    p = (byte)str[i + 4];
                    if (IsUTF8SpecialByte(k) && IsUTF8SpecialByte(m) && IsUTF8SpecialByte(n) && IsUTF8SpecialByte(p))
                    {
                        utf8_yes++;
                        i += 5;
                        continue;
                    }
                }
                else if (c >> 1 == six_byte)
                {
                    k = (byte)str[i + 1];
                    m = (byte)str[i + 2];
                    n = (byte)str[i + 3];
                    p = (byte)str[i + 4];
                    q = (byte)str[i + 5];
                    if (IsUTF8SpecialByte(k) && IsUTF8SpecialByte(m) && IsUTF8SpecialByte(n) && IsUTF8SpecialByte(p) && IsUTF8SpecialByte(q))
                    {
                        utf8_yes++;
                        i += 6;
                        continue;
                    }
                }
                utf8_no++;
                i++;
            }
            //Console.WriteLine("%d %d\n", utf8_yes, utf8_no);
            int ret = (100 * utf8_yes) / (utf8_yes + utf8_no);
            if (ret > 90)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool IsGB2312Code(byte[] str)
        {
            int one_byte = 0X00; //binary 00000000
            int gb2312_yes = 0;
            int gb2312_no = 0;
            byte k = 0;
            byte c = 0;
            for (uint i = 0; i < str.Length; )
            {
                c = (byte)str[i];
                if (c >> 7 == one_byte)
                {
                    i++;
                    continue;
                }
                else if (c >= 0XA1 && c <= 0XF7)
                {
                    k = (byte)str[i + 1];
                    if (k >= 0XA1 && k <= 0XFE)
                    {
                        gb2312_yes++;
                        i += 2;
                        continue;
                    }
                }
                gb2312_no++;
                i += 2;
            }

            //Console.WriteLine("%d %d\n", gb2312_yes, gb2312_no);
            int ret = (100 * gb2312_yes) / (gb2312_yes + gb2312_no);
            if (ret > 90)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool IsBIG5Code(byte[] str)
        {
            int one_byte = 0X00; //binary 00000000
            int big5_yes = 0;
            int big5_no = 0;
            byte k = 0;
            byte c = 0;
            for (uint i = 0; i < str.Length; )
            {
                c = (byte)str[i];
                if (c >> 7 == one_byte)
                {
                    i++;
                    continue;
                }
                else if (c >= 0XA1 && c <= 0XF9)
                {
                    k = (byte)str[i + 1];
                    if (k >= 0X40 && k <= 0X7E || k >= 0XA1 && k <= 0XFE)
                    {
                        big5_yes++;
                        i += 2;
                        continue;
                    }
                }

                big5_no++;
                i += 2;
            }
            //Console.WriteLine("%d %d\n", big5_yes, big5_no);
            int ret = (100 * big5_yes) / (big5_yes + big5_no);
            if (ret > 90)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool IsGBKCode(byte[] str)
        {
            int one_byte = 0X00; //binary 00000000
            int gbk_yes = 0;
            int gbk_no = 0;
            byte k = 0;
            byte c = 0;
            for (uint i = 0; i < str.Length; )
            {
                c = (byte)str[i];
                if (c >> 7 == one_byte)
                {
                    i++;
                    continue;
                }
                else if (c >= 0X81 && c <= 0XFE)
                {
                    k = (byte)str[i + 1];
                    if (k >= 0X40 && k <= 0XFE)
                    {
                        gbk_yes++;
                        i += 2;
                        continue;
                    }
                }
                gbk_no++;
                i += 2;
            }

            //Console.WriteLine("%d %d\n", gbk_yes, gbk_no);
            int ret = (100 * gbk_yes) / (gbk_yes + gbk_no);
            if (ret > 90)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
