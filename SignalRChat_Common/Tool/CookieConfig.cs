using SignalRChat_Model;
using System;
using System.ComponentModel;
using System.Web;
using System.Web.Script.Serialization;

namespace SignalRChat_Common.Tool
{
    public static class CookieConfig
    {

        /// <summary>
        /// 清除指定Cookie
        /// </summary>
        /// <param name="cookiename">cookie名称</param>
        [Description("清除指定Cookie")]
        public static void ClearCookie(string cookiename)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
            if (cookie == null) return;
            cookie.Expires = DateTime.Now.AddYears(-3);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 获取指定Cookie值
        /// </summary>
        /// <param name="cookiename">cookie名称</param>
        /// <returns></returns>
        [Description("获取指定Cookie")]
        public static UserInfo GetCookieValue(string cookiename)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
            if (cookie == null) return null;
            var principalUser = (new JavaScriptSerializer()).Deserialize<UserInfo>(cookie.Value);
            return principalUser;
        }

        /// <summary>
        /// 添加一个Cookie（24小时过期）
        /// </summary>
        /// <param name="cookiename">Cookie 名称</param>
        /// <param name="cookievalue">写人Cookie的值</param>
        [Description("添加Cookie并默认过期时间")]
        public static void SetCookie(string cookiename, UserInfo cookievalue)
        {
            SetCookie(cookiename, cookievalue, DateTime.Now.AddDays(1.0));
        }

        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookiename">cookie名</param>
        /// <param name="cookievalue">cookie值</param>
        /// <param name="expires">过期时间 DateTime</param>
        [Description("添加Cookie并设置过期时间")]
        public static void SetCookie(string cookiename, UserInfo cookievalue, DateTime expires)
        {
            string principalUser = (new JavaScriptSerializer()).Serialize(cookievalue);
            HttpCookie cookie = new HttpCookie(cookiename)
            {
                Value = principalUser,
                Expires = expires
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}