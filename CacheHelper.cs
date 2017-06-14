using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Utility
{
    public class CacheHelper
    {

        /// <summary> 
        /// 获取当前应用程序指定CacheKey的Cache对象值  
        /// </summary> 
        /// <param name="CacheKey">索引键值</param> 
        /// <returns>返回缓存对象</returns>  
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }
        /// <summary> 
        /// 设置当前应用程序指定CacheKey的Cache对象值 
        /// </summary> 
        /// <param name="CacheKey">索引键值</param> 
        /// <param name="objObject">缓存对象</param> 
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }
        /// <summary> 
        /// 设置当前应用程序指定CacheKey的Cache对象值 
        /// </summary> 
        /// <param name="CacheKey">索引键值</param> 
        /// <param name="objObject">缓存对象</param> 
        /// <param name="absoluteExpiration">绝对过期时间</param> 
        /// <param name="slidingExpiration">最后一次访问所插入对象时与该对象过期时之间的时间间隔</param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }
    }

}
