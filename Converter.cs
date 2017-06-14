using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Globalization;

namespace Utility
{
    public static class Converter
    {
        public static T ToModel<T>(this IDataReader dr)
        {
            try
            {
                using (dr)
                {
                    if (dr.Read())
                    {
                        int len = dr.FieldCount;
                        string[] fields = new string[len];
                        for (int i = 0; i < len; i++)
                        {
                            fields[i] = dr.GetName(i);
                        }
                        T model = SetObject<T>(fields, dr);
                        return model;
                    }
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T ToModel<T>(this DataTable dt)
        {
            try
            {
                T model;
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    int len = dt.Columns.Count;
                    string[] fields = new string[len];
                    for (int i = 0; i < len; i++)
                    {
                        fields[i] = dt.Columns[i].ColumnName;
                    }
                    model = SetObject<T>(fields, dr);
                    return model;
                }
                return default(T);         
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<T> ToList<T>(this DataTable dt)
        {
            int len = dt.Columns.Count;
            string[] fields = new string[len];
            List<T> list = new List<T>();
            for (int i = 0; i < len; i++)
            {
                fields[i] = dt.Columns[i].ColumnName;
            }
            foreach (DataRow dr in dt.Rows)
            {
                T model = SetObject<T>(fields, dr);
                list.Add(model);
            }
            return list;
        }

        public static List<T> ToList<T>(this IDataReader dr)
        {
            using (dr)
            {
                int len = dr.FieldCount;
                string[] fields = new string[len];
                List<T> list = new List<T>();
                for (int i = 0; i < len; i++)
                {
                    fields[i] = dr.GetName(i);
                }
                while (dr.Read())
                {
                    T model = SetObject<T>(fields, dr);
                    list.Add(model);
                }
                return list;
            }
        }

        public static bool Contains2(string[] arr, string a)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                string ar = arr[i];
                if (ar.Equals(a, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        public static bool IsBaseType(Type t)
        {
            return t.IsValueType || t == typeof(string);
        }

        private static T SetObject<T>(string[] fields, object dr)
        {
            Type type = typeof(T);
            T model = IsBaseType(type) ? default(T) : Activator.CreateInstance<T>();
            var pis = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            if (fields.Length > 1 && pis.Length > 0)
            {
                foreach (PropertyInfo pi in pis)
                {
                    string pn = pi.Name;
                    if (Contains2(fields, pn))
                    {
                        object obj = dr is DataRow ? ((DataRow)dr)[pn] : ((IDataReader)dr)[pn];
                        if (!IsNullOrDBNull(obj))
                        {
                            pi.SetValue(model, HackType(obj, pi.PropertyType), null);
                        }
                    }
                }
            }
            else
            {
                foreach (string pn in fields)
                {
                    object obj = dr is DataRow ? ((DataRow)dr)[pn] : ((IDataReader)dr)[pn];
                    if (!IsNullOrDBNull(obj))
                    {
                        model = (T)HackType(obj, type);
                        break;
                    }
                }
            }
            return model;
        }

        public static DataTable ToDataTable<T>(List<T> list)
        {
            if (list == null || list.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }

            Type entityType = typeof(T);
            PropertyInfo[] entityProperties = entityType.GetProperties();
            DataTable dt = new DataTable();
            int eLen = entityProperties.Length;
            for (int i = 0; i < eLen; i++)
            {
                dt.Columns.Add(entityProperties[i].Name);
            }
            foreach (object entity in list)
            {
                object[] entityValues = new object[eLen];
                for (int i = 0; i < eLen; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }

        public static Dictionary<string, string> ObjectToDict(object model)
        {
            Type type = model.GetType();
            Dictionary<string, string> data = new Dictionary<string, string>();
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.GetProperty | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0, len = propertys.Length; i < len; i++)
            {
                PropertyInfo pi = propertys[i];
                if (pi.PropertyType.IsValueType)
                {
                    object o = pi.GetValue(model, null);
                    if (!IsNullOrDBNull(o))
                    {
                        string value = String.Empty;
                        switch (pi.PropertyType.Name)
                        {
                            case "String":
                                value = o.ToString();
                                break;
                            case "Boolean":
                                value = o.ToString();
                                break;
                            case "DateTime":
                                DateTime dt = (DateTime)o;
                                if (dt.Year != 1)
                                {
                                    value = dt.ToString();
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            default:
                                if (i != 0)
                                {
                                    value = o.ToString();
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                        }
                        data.Add(String.Concat("[", pi.Name, "]"), value);
                    }
                }
            }
            return data;
        }

        public static object ConvertSimpleType(CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType.IsInstanceOfType(value))
            {
                return value;
            }
            //var valueAsString = value as string;
            //valueAsString != null && valueAsString.Length == 0
            if (value == DBNull.Value || (string.IsNullOrEmpty(value.ToString())))
            {
                return null;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
            if (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) && value.GetType() != typeof(string))
            {
                var nulla = new NullableConverter(destinationType);
                destinationType = nulla.UnderlyingType;
                if (destinationType.IsEnum)
                {
                    return Enum.ToObject(destinationType, value);
                }
            }

            bool canConvertFrom = converter.CanConvertFrom(value.GetType());
            if (!canConvertFrom)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }

            if (!(canConvertFrom || converter.CanConvertTo(destinationType)))
            {
                if (destinationType.IsEnum)
                {
                    return Enum.ToObject(destinationType, value);
                }
            }

            try
            {
                object convertedValue = (canConvertFrom) ? converter.ConvertFrom(null, culture, value) : converter.ConvertTo(null, culture, value, destinationType);
                return convertedValue;
            }
            catch (Exception ex)
            {
                string message = String.Format(CultureInfo.CurrentUICulture, value.GetType().FullName, destinationType.FullName);
                throw new InvalidOperationException(message, ex);
            }
        }

        private static object HackType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }

        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null) || (obj is DBNull) || string.IsNullOrEmpty(obj.ToString()) ? true : false;
        }

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 格式化字符型、日期型、布尔型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string StringFormat(string str, Type type)
        {
            if (type != typeof(string) && string.IsNullOrEmpty(str))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + str.Split(' ')[0] + "\"";
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }

            return str;
        }




    }


 
}