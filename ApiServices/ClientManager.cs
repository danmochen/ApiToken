using System.IO.Pipelines;
using System.Text;

namespace ApiServices
{
    /// <summary>
    /// �ӿڷ���Ŀͻ���
    /// </summary>
    public class Client
    {
        /// <summary>
        /// �������ͻ�Ψһʶ���־
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// �ͻ�����Կ
        /// </summary>
        public string appkey { get; set; }
        /// <summary>
        /// ����tokenʱ���ʱ����������ж�token�Ƿ�ʱ
        /// </summary>
        public long tokenTimestamp { get; set; }
    }
    /// <summary>
    /// �򵥵Ŀͻ�������
    /// </summary>
    public static class ClientManager
    {
        private static List<Client> clients = new List<Client>();

        static ClientManager()
        {
            clients.Add(new Client() { appid = "����", appkey = "123456" });
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