using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;
using Knowit.EPiModules.Replaceables.Filters;
using Knowit.EPiModules.Replaceables.Helper;
using Knowit.EPiModules.Replaceables.Models;

namespace Knowit.EPiModules.Replaceables.Sample.Controllers
{
    [Component(PlugInAreas = PlugInArea.Navigation, 
           Categories = "cms", // For edit mode
           WidgetType = "knowit.widgets.replaceables", // Made-up namespace and name for our Dojo component (a.k.a dijit)
           Title = "Replaceables",// Path to translations in a standard language file
           SortOrder = 100,
           Description = "A editable list of replacable values")]
    [Authorize(Roles = "WebEditors, WebAdmins, Administrators")]
    [Route("episerver/widgets/replaceables")]
    [ReplaceablesBypass]
    public class ReplaceablesWidgetController : BaseReplaceablesController
    {
        private readonly IReplaceablesManager _replaceables;

        public ReplaceablesWidgetController(IReplaceablesManager replaceables)
        {
            _replaceables = replaceables;
        }

        // GET: ReplacablesWidget
        public async Task<ActionResult> Index()
        {
            var languageCode = ReplaceableHelper.GetCurrentLocale();
            var replaceables = (await _replaceables.GetByLanguage(languageCode))
                .Select(x => new ReplaceableModel
                {
                    Key = x.Key,
                    LanguageCode = languageCode,
                    Value = x.Value
                }).OrderBy(x => x.Key).ToList();

            return PartialView("Widget", replaceables);
        }


       
    }
}