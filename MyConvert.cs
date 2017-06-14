using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Globalization;

namespace Utility
{
    public static class MyConvert
    {
        public static T ReaderToModel<T>(IDataReader dr)
        {
            try
            {
                using (dr)
                {
                    if (dr.Read())
                    {
                        List<string> list = new List<string>(dr.FieldCount);
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            list.Add(dr.GetName(i).ToLower());
                        }
                        T model = Activator.CreateInstance<T>();
                        foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (list.Contains(pi.Name.ToLower()))
                            {
                                if (!IsNullOrDBNull(dr[pi.Name]))
                                {
                                    pi.SetValue(model, HackType(dr[pi.Name], pi.PropertyType), null);
                                }
                            }
                        }
                        return model;
                    }
                }
                return default(T);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //where字句后面有new()约束的话，T类型必须有公有的无参的构造函数。
        public static T TableToModel<T>(DataTable dt) where T : class
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    int cc = dt.Columns.Count;
                    List<string> list = new List<string>();
                    for (int i = 0; i < cc; i++)
                    {
                        list.Add(dt.Columns[i].ColumnName.ToLower());
                    }
                    T model = Activator.CreateInstance<T>();
                    DataRow dr = dt.Rows[0];
                    foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (list.Contains(pi.Name.ToLower()))
                        {
                            if (!IsNullOrDBNull(dr[pi.Name]))
                            {
                                pi.SetValue(model, HackType(dr[pi.Name], pi.PropertyType), null);
                            }
                        }
                    }
                    return model;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<T> DataTableToList<T>(DataTable dt)
        {
            int len = dt.Columns.Count;
            string[] field = new string[len];
            for (int i = 0; i < len; i++)
            {
                field[i] = dt.Columns[i].ColumnName;
            }
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T model = Activator.CreateInstance<T>();
                foreach (PropertyInfo p in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    string pn = p.Name;
                    if (field.Contains(pn))
                    {
                        if (!IsNullOrDBNull(dr[pn]))
                        {
                            object o = HackType(dr[pn], p.PropertyType);
                            p.SetValue(model, o, null);
                        }
                    }
                }
                list.Add(model);
            }
            return list;
        }

        public static List<T> ReaderToList<T>(IDataReader dr)
        {
            int len = dr.FieldCount;
            string[] field = new string[len];
            for (int i = 0; i < len; i++)
            {
                field[i] = dr.GetName(i);
            }
            List<T> list = new List<T>();
            while (dr.Read())
            {
                T model = Activator.CreateInstance<T>();
                foreach (PropertyInfo p in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    string pn = p.Name;
                    if (field.Contains(pn))
                    {
                        if (!IsNullOrDBNull(dr[pn]))
                        {
                            object o = HackType(dr[pn], p.PropertyType);
                            p.SetValue(model, o, null);
                        }
                    }
                }
                list.Add(model);
            }
            if (!dr.IsClosed)
                dr.Close();
            return list;
        }

        public static DataTable ListToDataTable<T>(List<T> list)
        {
            if (list == null || list.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            Type entityType = list[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                dt.Columns.Add(entityProperties[i].Name);
            }
            foreach (object entity in list)
            {
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }

        public static Dictionary<string, string> ObjectToDict(object model, Type type)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.GetProperty | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < propertys.Length; i++)
            {
                PropertyInfo property = propertys[i];
                if (property.PropertyType.IsValueType)
                {
                    object o = property.GetValue(model, null);
                    if (!IsNullOrDBNull(o))
                    {
                        string value = String.Empty;
                        switch (property.PropertyType.Name)
                        {
                            case "String":
                                value = o.ToString();
                                break;
                            case "Boolean":
                                //value = String.Concat(SqlBuilder.NoQuotationFlag, Convert.ToInt16(o));
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
                                    //value = String.Concat(SqlBuilder.NoQuotationFlag, o.ToString());
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                        }
                        data.Add(String.Concat("[", property.Name, "]"), value);
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
            if (value == DBNull.Value)
            {
                return null;
            }
            var valueAsString = value as string;
            if (valueAsString != null && valueAsString.Length == 0)
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

        private static T HackType<T>(object value)
        {
            Type conversionType = typeof(T);
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            return (T)Convert.ChangeType(value, conversionType);
        }

        private static object HackType(object value, Type conversionType)
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
            return Convert.ChangeType(value, conversionType);
        }

        private static bool IsNullOrDBNull(object obj)
        {
            return (obj is DBNull) || string.IsNullOrEmpty(obj.ToString());
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