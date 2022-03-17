using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class WebHelper
    {
        public HttpClient httpClient { get; set; }
        public WebHelper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        /// <summary>
        /// 常用的post请求
        /// </summary>
        /// <param name="url">路径</param>
        /// <param name="content">请求体</param>
        /// <param name="contentType">request-head的content-type,用于告知服务器request-body的类型（.net core一个确定的路由对应一种media-type,此contentType需要与请求的服务器路由media-type对应，否则报错415）</param>
        /// <returns></returns>
        public string HttpPost(string url, string body, string contentType = "application/json")
        {
            //请求路径
            Uri uri = new Uri(url);
            //请求报文体request-body
            HttpContent requestBody = new StringContent(body);
            requestBody.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            //请求报文
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = requestBody;

            HttpResponseMessage response = HttpRequest(request);

            byte[] buffer = response.Content.ReadAsByteArrayAsync().Result;
            //释放掉响应报文
            response.Dispose();
            return Encoding.UTF8.GetString(buffer);//默认响应报文就是utf8编码的字符串

        }
        /// <summary>
        /// 发送一个http请求
        /// </summary>
        /// <param name="request">请求报文</param>
        /// <returns></returns>
        public HttpResponseMessage HttpRequest(HttpRequestMessage request)
        {
            //HttpClientHandler s = new HttpClientHandler()
            //{
            //    Proxy = new WebProxy("127.0.0.1:8866", true),
            //    UseProxy = true
            //};
            HttpResponseMessage response = httpClient.Send(request);
            //释放掉请求报文，HttpClient的释放工作由外部方法自己决定
            request.Dispose();
            return response;
        }
    }
}
