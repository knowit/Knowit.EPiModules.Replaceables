using System.Web.Mvc;

namespace Knowit.EPiModules.Replaceables.Sample.Controllers
{
    public class BaseReplaceablesController : Controller
    {
        protected override PartialViewResult PartialView(string viewName, object model)
        {
            viewName = "~/Views/Replaceables/" + viewName + ".cshtml";

            return base.PartialView(viewName, model);
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            viewName = "~/Views/Replaceables/" + viewName + ".cshtml";

            return base.View(viewName, masterName, model);
        }
    }
}