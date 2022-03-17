using Common;
using Microsoft.AspNetCore.Mvc;

namespace WebClients
{
    public class WebController : Controller
    {
        public TokenManager tokenServices;
        public WebController(TokenManager tokenManager)
        {
            tokenServices=tokenManager;
        }
        [Route("order")]
        public IActionResult Index([FromQuery]string businesskey)
        {
            string url = "http://localhost:5114/api/order";
            return Content(tokenServices.RequestUrl(url, businesskey));
        }
    }
}
