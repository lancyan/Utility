using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility
{
    public static class FastProperty
    {
        public delegate T GetPropertyValue<T>();
        public delegate void SetPropertyValue<T>(T Value);

        private static ConcurrentDictionary<string, Delegate> myDelegateCache = new ConcurrentDictionary<string, Delegate>();

        public static GetPropertyValue<T> CreateGetPropertyValueDelegate<TSource, T>(this PropertyInfo propertyInfo, TSource obj)
        {
            string propertyName = propertyInfo.Name;
            string key = string.Format("Delegate-GetProperty-{0}-{1}", typeof(TSource).FullName, propertyName);
            GetPropertyValue<T> result = (GetPropertyValue<T>)myDelegateCache.GetOrAdd(
                key,
                newkey =>
                {
                    return Delegate.CreateDelegate(typeof(GetPropertyValue<T>), obj, propertyInfo.GetGetMethod());
                });

            return result;
        }
        public static SetPropertyValue<T> CreateSetPropertyValueDelegate<TSource, T>(this PropertyInfo propertyInfo, TSource obj)
        {
            string propertyName = propertyInfo.Name;
            string key = string.Format("Delegate-SetProperty-{0}-{1}", typeof(TSource).FullName, propertyName);
            SetPropertyValue<T> result = (SetPropertyValue<T>)myDelegateCache.GetOrAdd(
               key,
               newkey =>
               {
                   return Delegate.CreateDelegate(typeof(SetPropertyValue<T>), obj, propertyInfo.GetSetMethod());
               });
            return result;
        }

        //public static GetPropertyValue<T> CreateGetPropertyValueDelegateNoCache<TSource, T>(TSource obj, string propertyName)
        //{
        //    return (GetPropertyValue<T>)Delegate.CreateDelegate(typeof(GetPropertyValue<T>), obj, typeof(TSource).GetProperty(propertyName).GetGetMethod()); ;
        //}
        //public static SetPropertyValue<T> CreateSetPropertyValueDelegateNoCache<TSource, T>(TSource obj, string propertyName)
        //{
        //    return (SetPropertyValue<T>)Delegate.CreateDelegate(typeof(SetPropertyValue<T>), obj, typeof(TSource).GetProperty(propertyName).GetSetMethod()); ;
        //}

    }
}
