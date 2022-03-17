using System.IO.Pipelines;
using System.Text;

namespace ApiServices
{
    /// <summary>
    /// 接口服务的客户类
    /// </summary>
    public class Client
    {
        /// <summary>
        /// 主键，客户唯一识别标志
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 客户的秘钥
        /// </summary>
        public string appkey { get; set; }
        /// <summary>
        /// 生成token时候的时间戳，用于判断token是否超时
        /// </summary>
        public long tokenTimestamp { get; set; }
    }
    /// <summary>
    /// 简单的客户集合类
    /// </summary>
    public static class ClientManager
    {
        private static List<Client> clients = new List<Client>();

        static ClientManager()
        {
            clients.Add(new Client() { appid = "张三", appkey = "123456" });
        }

        public static Client Add(string appid)
        {
            return Add(new Client() { appid = appid, appkey = Guid.NewGuid().ToString() });
        }
        public static Client Add(Client c)
        {
            clients.Add(c);
            return c;
        }
        public static Client Get(string appid)
        {
            return clients.Find(f => f.appid == appid);
        }
        public static Client Set(string appid, long tokenTimestamp)
        {
            Client c = Get(appid);
            if (c != null)
            {
                c.tokenTimestamp = tokenTimestamp;
            }
            return c;
        }
    }

}