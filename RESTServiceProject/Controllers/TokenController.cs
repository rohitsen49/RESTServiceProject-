using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTServiceProject.Controllers
{
    public class TokenRequest
    {
        public string userEmail { get; set; }
        public string userPassword { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        // Returns an encypted token
        [HttpPost]
        public dynamic Post([FromBody] TokenRequest tokenRequest)
        {
            var token = TokenHelper.GetToken(tokenRequest.userEmail, tokenRequest.userPassword);
            return new { Token = token };
        }
    }
}
