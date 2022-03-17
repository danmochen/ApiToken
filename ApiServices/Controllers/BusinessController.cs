using Common;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;

namespace ApiServices.Controllers
{
    public class BusinessController : Controller
    {
        [Route("api/test")]
        public IActionResult Index1()
        {
            return Content("test");
        }
        [Route("/")]
        public IActionResult Index()
        {
            return Json(new ApiResult(200,"这是一个公开接口"));
        }
        [HttpPost]
        [Route("token")]
        public IActionResult BuildToken([FromBody] SignParameterModel model)
        {
            /*FromForm小惊喜，请求体被做了安全处理，如果接口用FromForm模式传输，会产生问题*/

            //根据appid找到具体的客户
            Client c = ClientManager.Get(model.appid);
            //验证时间戳
            if (TimeHelper.StampToDateTime(model.timestamp).AddSeconds(20) <= DateTime.UtcNow)
            {
                return Json(new ApiResult(300, "签名超时"));
            }
            //验证签名
            SortedDictionary<string, string> signSortDic = new SortedDictionary<string, string>();
            signSortDic.Add("appid", model.appid);
            signSortDic.Add("appkey", c.appkey);
            signSortDic.Add("timestamp", model.timestamp.ToString());
            signSortDic.Add("publickey", model.publickey);
            string serviceCalculateSign = SecurityHelper.Sign(signSortDic);
            if (model.sign != serviceCalculateSign)
            {
                return Json(new ApiResult(301, "签名错误"));
            }

            //生产token
            SortedDictionary<string, string> tokenSortDic = new SortedDictionary<string, string>();
            tokenSortDic.Add("appid", model.appid);
            tokenSortDic.Add("appkey", c.appkey);
            tokenSortDic.Add("timestamp", model.timestamp.ToString());
            string token = SecurityHelper.Token(tokenSortDic);
            //保存生成token时所使用的时间戳到客户
            ClientManager.Set(model.appid, model.timestamp);
            //用客户的公钥加密
            string encryptedToken = SecurityHelper.RSAEncrypt(model.publickey, token);


            return Json(new ApiResult(200, "签名正确", encryptedToken));
        }
        [Route("api/order")]
        public IActionResult Order([FromBody] SignParameterModel model)
        {
            ////根据appid找到具体的客户
            //Client c = ClientManager.Get(model.appid);
            ////
            //if (TimeHelper.StampToDateTime(model.timestamp).AddSeconds(signtimeout) <= DateTime.UtcNow)
            //{
            //    return Json(new ApiResult(300, "签名超时"));
            //}
            ////token超时
            //if (TimeHelper.StampToDateTime(c.tokenTimestamp).AddMinutes(tokentimeout) <= DateTime.UtcNow)
            //{
            //    return Json(new ApiResult(302, "token超时"));
            //}
            ////客户token（也可以在客户请求token时候存储起来，这里选择相同方法再次生成。）
            //SortedDictionary<string, string> tokenSortDic = new SortedDictionary<string, string>();
            //tokenSortDic.Add("appid", model.appid);
            //tokenSortDic.Add("appkey", c.appkey);
            //tokenSortDic.Add("timestamp", c.tokenTimestamp.ToString());
            //string token = SecurityHelper.Token(tokenSortDic);
            ////签名
            //SortedDictionary<string, string> signSortDic = new SortedDictionary<string, string>();
            //signSortDic.Add("appid", model.appid);
            //signSortDic.Add("appkey", c.appkey);
            //signSortDic.Add("timestamp", model.timestamp.ToString());
            //signSortDic.Add("token", token);
            //signSortDic.Add("businesskey", model.businesskey);
            //string serviceCalculateSign = SecurityHelper.Sign(signSortDic);
            //if (model.sign != serviceCalculateSign)
            //{
            //    return Json(new ApiResult(301, "签名错误"));
            //}

            /*业务代码......*/

            return Json(new ApiResult(200, $"业务处理成功businesskey:{model.businesskey}"));
        }
    }
}
