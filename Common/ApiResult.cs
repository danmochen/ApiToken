namespace Common
{
    public class ApiResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
        public ApiResult()
        {
            this.code = 200; this.msg = "";
        }
        public ApiResult(int code, string msg) : this(code, msg, null)
        {

        }
        public ApiResult(int code, string msg, object data)
        {
            this.code = code;
            this.msg = msg;
            this.data = data;
        }
    }
}
