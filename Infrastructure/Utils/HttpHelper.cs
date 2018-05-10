using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Infrastructure.Utils
{
    public class HttpHelper
    {
        public static string Post(string url, string param)
        {
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 60000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] paramsArr = Encoding.UTF8.GetBytes(param);
            request.ContentLength = paramsArr.Length;
            request.GetRequestStream().Write(paramsArr, 0, paramsArr.Length);

            WebResponse response = request.GetResponse();
            var resp = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                var streamReader = new System.IO.StreamReader(stream, Encoding.UTF8);
                resp = streamReader.ReadToEnd().Replace("\n", "");
            }

            return resp;
        }

        public static string Get(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 60000;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();
            var resp = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                var streamReader = new System.IO.StreamReader(stream, Encoding.UTF8);
                resp = streamReader.ReadToEnd().Replace("\n", "");
            }

            return resp;
        }
    }
}
