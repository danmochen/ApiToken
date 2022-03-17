using Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;

namespace WebClients
{
    public class TokenManager
    {
        private string _appid;
        private string _appkey;
        private string _token;
        private string _tokenurl;
        private long _timestamp;//生成token的时间戳
        private int _tokentimeout;//token有效时间，单位分钟
        public readonly IConfiguration _configuration;
        public HttpClient _httpClient;
        public readonly Tuple<string, string> _rsakey;//rsa秘钥
        public TokenManager(IConfiguration configuration, HttpClient httpClient)
        {
            this._httpClient = httpClient;
            this._configuration = configuration;
            IConfigurationSection section = configuration.GetSection("api");
            this._appid = section.GetValue<string>("appid");
            this._appkey = section.GetValue<string>("appkey");
            this._tokenurl = section.GetValue<string>("tokenurl");
            this._tokentimeout = section.GetValue<int>("tokentimeout");
            _rsakey = SecurityHelper.RSA();
        }
        public string GetToken()
        {
            if (string.IsNullOrEmpty(_token) || TimeHelper.StampToDateTime(_timestamp).AddMinutes(_tokentimeout) <= DateTime.UtcNow)
            {
                //时间戳
                _timestamp = TimeHelper.GetTimestamp();
                //公钥
                string publickey = _rsakey.Item2;
                //生成参数签名，与服务器端方法必须一致，使用对称加密
                SortedDictionary<string, string> sortDic = new SortedDictionary<string, string>();
                sortDic.Add("appid", _appid);
                sortDic.Add("appkey", _appkey);
                sortDic.Add("publickey", publickey/*公钥*/);
                sortDic.Add("timestamp", _timestamp.ToString());
                string sign = SecurityHelper.Sign(sortDic);
                //拼接请求报文body，拼接方式必须与请求报文header的content-type保持一致
                string content = JsonConvert.SerializeObject(new
                {
                    sign = sign,
                    appid = _appid,
                    publickey = publickey,
                    timestamp = _timestamp,
                });
                //从服务器获取token
                string responseToken = new WebHelper(_httpClient).HttpPost(_tokenurl, content);
                ApiResult api = JsonConvert.DeserializeObject<ApiResult>(responseToken);
                if (api.code == 200)
                {
                    string encryptedToken = api.data?.ToString();
                    //用私钥解密
                    _token = SecurityHelper.RSADecrypt(_rsakey.Item1, encryptedToken);
                }
            }
            return _token;
        }
        public string RequestUrl(string url, string businesskey)
        {
            long timestamp = TimeHelper.GetTimestamp();
            string token = GetToken();
            SortedDictionary<string, string> sortDic = new SortedDictionary<string, string>();
            sortDic.Add("appid", _appid);
            sortDic.Add("appkey", _appkey);
            sortDic.Add("timestamp", timestamp.ToString());
            sortDic.Add("token", token);
            sortDic.Add("businesskey", businesskey);
            string sign = SecurityHelper.Sign(sortDic);
            
            string content = //$"sign={sign}&appid={_appid}&timestamp={timestamp}&{businesskeyQuery}";
                JsonConvert.SerializeObject(new
                {
                    sign = sign,
                    appid = _appid,
                    timestamp = timestamp,
                    businesskey = businesskey
                });
            return new WebHelper(_httpClient).HttpPost(url, content);
        }
    }
}
