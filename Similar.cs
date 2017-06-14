using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace Utility
{
    public class Similar
    {
        public static string GetMaxSameLengthString(string s1, string s2)
        {
            int len = s1.Length;
            string previousNode = string.Empty;
            for (int i = 0; i < len; i++)
            {
                for (int j = i, k = 1; j < len; j++)
                {
                    string ss = s1.Substring(i, k);
                    if (s2.Contains(ss))
                    {
                        if (ss.Length > previousNode.Length)
                        {
                            previousNode = ss;
                        }
                        if (j == len - 1)
                        {
                            i = len - 1;
                            break;
                        }
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return previousNode;
        }

        public static double GetSimilar(string s1, string s2)
        {
            int len = s1.Length;
            string previousNode = string.Empty;
            for (int i = 0; i < len; i++)
            {
                for (int j = i, k = 1; j < len; j++)
                {
                    string ss = s1.Substring(i, k);
                    if (s2.Contains(ss))
                    {
                        if (ss.Length > previousNode.Length)
                        {
                            previousNode = ss;
                        }
                        if (j == len - 1)
                        {
                            i = len - 1;
                            break;
                        }
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return previousNode.Length * 2 / (s1.Length + s2.Length);
        }

        public static List<string> GetAllSameLengthString(string s1, string s2)
        {
            List<string> list = new List<string>();
            int len = s1.Length;
            for (int i = 0; i < len; i++)
            {
                string previousNode = string.Empty;
                for (int j = i, k = 1; j < len; j++)
                {
                    string ss = s1.Substring(i, k);
                    if (s2.Contains(ss))
                    {
                        previousNode = ss;
                        if (j == len - 1)
                        {
                            i = len - 1;
                            break;
                        }
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (previousNode.Length > 0)
                {
                    if (!list.Contains(previousNode))
                    {
                        list.Add(previousNode);
                    }
                }
            }
            return list;
        }

        public static double getSimilarity(string doc1, string doc2)
        {
            if (!string.IsNullOrEmpty(doc1) && !string.IsNullOrEmpty(doc2))
            {
                Dictionary<char, int[]> dict = new Dictionary<char, int[]>();
                for (int i = 0, len = doc1.Length; i < len; i++)
                {
                    char key = doc1[i];
                    if (!dict.ContainsKey(key))
                    {
                        int[] arr = new int[2];
                        arr[0] = 1;
                        dict.Add(key, arr);
                    }
                    else
                    {
                        int[] arr = dict[key];
                        arr[0]++;
                        dict[key] = arr;
                    }
                }
                for (int i = 0, len = doc2.Length; i < len; i++)
                {
                    char key = doc2[i];
                    if (!dict.ContainsKey(key))
                    {
                        int[] arr = new int[2];
                        arr[1] = 1;
                        dict.Add(key, arr);
                    }
                    else
                    {
                        int[] arr = dict[key];
                        arr[1]++;
                        dict[key] = arr;
                    }
                }
                double sqdoc1 = 0;
                double sqdoc2 = 0;
                double denominator = 0;
                foreach (KeyValuePair<char, int[]> kvp in dict)
                {
                    int[] c = kvp.Value;
                    denominator += c[0] * c[1];
                    sqdoc1 += c[0] * c[0];
                    sqdoc2 += c[1] * c[1];
                }
                return denominator / Math.Sqrt(sqdoc1 * sqdoc2);
            }
            else
            {
                return 0;
            }
        }

        public static double levenshtein(string str1, string str2)
        {
            //计算两个字符串的长度。  
            int len1 = str1.Length;
            int len2 = str2.Length;
            //建立上面说的数组，比字符长度大一个空间  
            int[,] dif = new int[len1 + 1, len2 + 1];
            //赋初值，步骤B。  
            for (int a = 0; a <= len1; a++)
            {
                dif[a, 0] = a;
            }
            for (int a = 0; a <= len2; a++)
            {
                dif[0, a] = a;
            }
            //计算两个字符是否一样，计算左上的值  
            int temp;
            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    int[] arr = new int[] { dif[i - 1, j - 1] + temp, dif[i, j - 1] + 1, dif[i - 1, j] + 1 };
                    int min = arr[0];
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (arr[k] < min)
                        {
                            min = arr[k];
                        }
                    }
                    dif[i, j] = min;
                }
            }
            //Console.WriteLine("字符串\"" + str1 + "\"与\"" + str2 + "\"的比较");
            //取数组右下角的值，同样不同位置代表不同字符串的比较  
            //Console.WriteLine("差异步骤：" + dif[len1, len2]);
            //计算相似度  
            double similarity = 1 - (double)dif[len1, len2] / (double)Math.Max(str1.Length, str2.Length);
            //Console.WriteLine("相似度：" + similarity);
            return similarity;
        }

    }
}
