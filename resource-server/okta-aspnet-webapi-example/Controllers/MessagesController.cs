using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace okta_aspnet_webapi_example.Controllers
{
    public class MessagesController : ApiController
    {
        [HttpGet]
        [Route("~/api/messages")]
        public IHttpActionResult Get()
        {
            var principal = RequestContext.Principal.Identity as ClaimsIdentity;

            var login = principal.Claims
                .SingleOrDefault(c => c.Type == System.IdentityModel.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            return Json(new
            {
                messages = new dynamic[]
                {
                    new { date = DateTime.Now, text = "I am a Robot." },
                    new { date = DateTime.Now, text = "Hello, world!" },
                }
            });
        }
    }
}
