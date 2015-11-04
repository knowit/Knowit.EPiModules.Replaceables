using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.DataAbstraction;
using EPiServer.Shell.Navigation;
using Knowit.EPiModules.Replaceables.Filters;
using Knowit.EPiModules.Replaceables.Models;

namespace Knowit.EPiModules.Replaceables.Sample.Controllers
{
    [Authorize(Roles = "WebEditors, WebAdmins, Administrators")]
    [RoutePrefix("episerver/replaceables")]
    [ReplaceablesBypass]
    public class ReplaceablesEditorController : BaseReplaceablesController
    {
        private readonly IReplaceablesManager _replaceablesManager;
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public ReplaceablesEditorController(IReplaceablesManager replaceablesManager, ILanguageBranchRepository languageBranchRepository)
        {
            _replaceablesManager = replaceablesManager;
            _languageBranchRepository = languageBranchRepository;
        }

        [Route("editor")]
        [MenuItem("/global/cmd/edit", Text = "Replacables", Url = "/episerver/replaceables/editor")]
        public ActionResult Index()
        {
            return View("Editor");
        }

        [Route("")]
        public async Task<ActionResult> ViewModel()
        {
            var enabledLanguages = _languageBranchRepository.ListEnabled().Select(x =>
                new LanguageViewModel
                {
                    DisplayName = x.Name,
                    LanguageCode = x.Culture.Name
                }).ToArray();

            var languageTasks = enabledLanguages.ToDictionary(language => language.LanguageCode, language => _replaceablesManager.GetByLanguage(language.LanguageCode));
            await Task.WhenAll(languageTasks.Values);

            var replaceables = from languageTask in languageTasks 
                               from replaceable in languageTask.Value.Result 
                               select new ReplaceableModel {Key = replaceable.Key, Value = replaceable.Value, LanguageCode = languageTask.Key};

            var viewModel = new ReplacablesViewModel
            {
                Languages = enabledLanguages,
                Replaceables = replaceables.OrderBy(x => x.Key).ToArray()
            };


            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Route("")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ReplaceableModel model)
        {
            ValidateRequired("Key", model.Key);
            ValidateAlphanumeric("Key", model.Key);
            ValidateRequired("LanguageCode", model.LanguageCode);
            ValidateRequired("Value", model.Value);

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            await _replaceablesManager.Save(model);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string key)
        {
            _replaceablesManager.Delete(key);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void ValidateRequired(string fieldName, string fieldValue)
        {
            if (string.IsNullOrEmpty(fieldValue))
            {
                ModelState.AddModelError(fieldName, "Missing " + fieldName);
            }
        }

        private void ValidateAlphanumeric(string fieldName, string fieldValue)
        {
            var r = new Regex("^[a-zA-Z0-9]+$");
            if (!r.IsMatch(fieldValue))
                ModelState.AddModelError(fieldName, string.Format("Only letters or numbers allowed for {0}.", fieldName));
        }

        public class ReplacablesViewModel
        {
            public LanguageViewModel[] Languages { get; set; }
            public ReplaceableModel[] Replaceables { get; set; }

        }

        public class LanguageViewModel
        {
            public string DisplayName { get; set; }
            public string LanguageCode { get; set; }
        }

    }
}