using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CrawlClient
{
    public static class StringHelper
    {
        public static Stream ToStream(this string str)
        {
            byte[] array = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new MemoryStream(array);
            return stream;
        }

        public static string HtmlDecode(this string str)
        {
            return WebUtility.HtmlDecode(str);
        }
        public static string HtmlEncode(this string str)
        {
            return WebUtility.HtmlEncode(str);
        }
        public static string UrlEncode(this string str)
        {
            return WebUtility.UrlEncode(str);
        }
        public static string UrlDecode(this string str)
        {
            return WebUtility.UrlDecode(str);
        }
    }
}
