using System.Security.Cryptography;
using System.Text;
using XSystem.Security.Cryptography;

namespace Common
{
    public static class SecurityHelper
    {
        /// <summary>
        /// MD5加密（获取32位字符串）
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string MD5(string source)
        {
            if (string.IsNullOrEmpty(source)) { return string.Empty; }

            byte[] sourceBytes = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Bytes = new XSystem.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(sourceBytes);

            return BitConverter.ToString(md5Bytes).Replace("-", "");
        }
        /// <summary>
        /// SHA1 加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string SHA1(string source)
        {
            byte[] StrRes = Encoding.Default.GetBytes(source);
            XSystem.Security.Cryptography.HashAlgorithm iSHA = new XSystem.Security.Cryptography.SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
        /// <summary>
        /// 参数签名方法
        /// </summary>
        /// <param name="partials">有序字典</param>
        /// <returns></returns>
        public static string Sign(SortedDictionary<string, string> partials)
        {
            string v = partials.Select(s => s.Value).Aggregate((a1, a2) => a1 + a2);
            return MD5(v);
        }
        /// <summary>
        /// token生成算法
        /// </summary>
        /// <param name="partials">有序字典</param>
        /// <returns></returns>
        public static string Token(SortedDictionary<string, string> partials)
        {
            string v = string.Empty;
            string[] arr = new string[4];
            int[] sums = new int[4];

            //1.求md5 hash值
            v = partials.Select(s => s.Value).Aggregate((a1, a2) => a1 + a2);
            v = MD5(v);
            //2.算法处理
            arr[0] = v.Substring(0, 8);
            arr[1] = v.Substring(8, 8);
            arr[2] = v.Substring(16, 8);
            arr[3] = v.Substring(24, 8);
            for (int i = 0; i < arr.Length; i++)
            {
                int[] nums = new int[8];
                for (int j = 0; j < arr[i].Length; j++)
                {
                    char c = arr[i][j];
                    if (c == 'a' || c == 'A')
                    { nums[j] = 10; }
                    else if (c == 'b' || c == 'B')
                    { nums[j] = 11; }
                    else if (c == 'c' || c == 'C')
                    { nums[j] = 12; }
                    else if (c == 'd' || c == 'D')
                    { nums[j] = 13; }
                    else if (c == 'e' || c == 'E')
                    { nums[j] = 14; }
                    else if (c == 'f' || c == 'F')
                    { nums[j] = 15; }
                    else
                    { nums[j] = Convert.ToInt32(c.ToString()); }
                }
                sums[i] = nums.Sum();
            }
            sums[0] *= 17;//乘以特定质数，此质数规则可以自行配置
            sums[1] *= 37;
            sums[2] *= 23;
            sums[3] *= 73;
            sums = sums.Select(s => s % 25).ToArray();

            //3.对应字母表
            string words = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] keys = words.AsEnumerable().ToArray();

            string token = string.Join("", sums.Select(s => keys[s]));
            //4.sha1加密
            token = SHA1(token);
            return token;
        }
        /// <summary>
        /// RSA使用公钥加密
        /// </summary>
        /// <param name="publickey">公钥</param>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey, string source)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsaProvider = new System.Security.Cryptography.RSACryptoServiceProvider();
            rsaProvider.FromXmlString(publickey);
            byte[] encryptedBytes = rsaProvider.Encrypt(Encoding.UTF8.GetBytes(source), false);
            rsaProvider.Dispose();
            return Convert.ToBase64String(encryptedBytes);
        }
        /// <summary>
        /// RSA使用私钥解密
        /// </summary>
        /// <param name="privatekey">私钥</param>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey, string source/*密文为base64格式*/)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsaProvider = new System.Security.Cryptography.RSACryptoServiceProvider();
            rsaProvider.FromXmlString(privatekey);
            byte[] decryptedBytes = rsaProvider.Decrypt(Convert.FromBase64String(source), false);
            rsaProvider.Dispose();
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        /// <summary>
        /// 生成一对RSA秘钥
        /// </summary>
        /// <returns>元组第一个元素为私钥，第二个元素为公钥</returns>
        public static Tuple<string, string> RSA()
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsaProvider = new System.Security.Cryptography.RSACryptoServiceProvider();
            string privatekey = rsaProvider.ToXmlString(true);
            string publickey = rsaProvider.ToXmlString(false);
            rsaProvider.Dispose();
            return new Tuple<string, string>(privatekey, publickey);
        }
    }
}