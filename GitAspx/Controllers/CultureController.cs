using System.Web.Mvc;

namespace GitAspx.Controllers
{
    public class CultureController : Controller
    {
        public ActionResult Index(string culture)
        {
            Session["culture"] = culture;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}