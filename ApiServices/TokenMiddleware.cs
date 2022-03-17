using Common;
using Microsoft.AspNetCore.Server.HttpSys;
using Newtonsoft.Json;
using System.Buffers;
using System.Collections;
using System.IO.Pipelines;
using System.Text;
using System.Text.RegularExpressions;

namespace ApiServices
{
    public class TokenMiddleware
    {
        private static readonly int signtimeout = 10;//签名过期时间，单位秒
        private static readonly int tokentimeout = 60;//token过期时间，单位分

        private readonly RequestDelegate _next;
        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            //简单api请求中间件 一般情况由过滤器实现该功能
            var path = context.Request.Path;

            if (path.HasValue && Regex.IsMatch(path.Value, "^/api/"))
            {

                //对request.body进行缓存
                context.Request.EnableBuffering();
                string bodyStr =await context.Request.Body.ReadEndAsync();
                //读完重置起始点
                context.Request.Body.Position = 0;
                var model = JsonConvert.DeserializeObject<SignParameterModel>(bodyStr);
                if (model == null)
                {
                    return;
                }
                //根据appid找到具体的客户
                Client c = ClientManager.Get(model.appid);
                //
                if (TimeHelper.StampToDateTime(model.timestamp).AddSeconds(signtimeout) <= DateTime.UtcNow)
                {

                    string msg = JsonConvert.SerializeObject(new ApiResult(300, "签名超时"));
                    context.Response.StatusCode = 300;
                    context.Response.ContentType = "text/plain";
                    await PipeHelper.WriteToPipeAsync(context.Response.BodyWriter, msg);
                    return;
                }
                //token超时
                if (TimeHelper.StampToDateTime(c.tokenTimestamp).AddMinutes(tokentimeout) <= DateTime.UtcNow)
                {
                    string msg = JsonConvert.SerializeObject(new ApiResult(302, "token超时"));
                    context.Response.StatusCode = 302;
                    context.Response.ContentType = "text/plain";
                    await PipeHelper.WriteToPipeAsync(context.Response.BodyWriter, msg);
                    return;
                }
                //客户token（也可以在客户请求token时候存储起来，这里选择相同方法再次生成。）
                SortedDictionary<string, string> tokenSortDic = new SortedDictionary<string, string>();
                tokenSortDic.Add("appid", model.appid);
                tokenSortDic.Add("appkey", c.appkey);
                tokenSortDic.Add("timestamp", c.tokenTimestamp.ToString());
                string token = SecurityHelper.Token(tokenSortDic);
                //签名
                SortedDictionary<string, string> signSortDic = new SortedDictionary<string, string>();
                signSortDic.Add("appid", model.appid);
                signSortDic.Add("appkey", c.appkey);
                signSortDic.Add("timestamp", model.timestamp.ToString());
                signSortDic.Add("token", token);
                signSortDic.Add("businesskey", model.businesskey);
                string serviceCalculateSign = SecurityHelper.Sign(signSortDic);
                if (model.sign != serviceCalculateSign)
                {
                    string msg = JsonConvert.SerializeObject(new ApiResult(301, "签名错误"));
                    context.Response.StatusCode = 301;
                    context.Response.ContentType = "text/plain";
                    await PipeHelper.WriteToPipeAsync(context.Response.BodyWriter, msg);
                    return;
                }
            }

            await _next(context);
        }

    }
}
