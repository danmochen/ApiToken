namespace ApiServices
{
    public class SignParameterModel
    {
        public string appid { get; set; }
        public string appkey { get; set; }
        public long timestamp { get; set; }
        public string publickey { get; set; }
        public string businesskey { get; set; }
        public string sign { get; set; }
    }
}
