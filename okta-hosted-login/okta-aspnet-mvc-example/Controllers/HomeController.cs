using System.Web;
using System.Web.Mvc;

namespace okta_aspnet_mvc_example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [Authorize]
        public ActionResult Profile()
        {
            var user = HttpContext.GetOwinContext().Authentication.User;
            var claims = user.Claims;
            var accessToken = user.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            var idToken = user.Claims.FirstOrDefault(c => c.Type == "id_token")?.Value;
            Console.WriteLine("This is accessToken: " + accessToken); // this will be null
            Console.WriteLine("This is idToken: " + idToken); // this will be null

            ViewBag.AccessToken = accessToken;
            ViewBag.IdToken = idToken;

            return View(claims);
        }

    }
}