using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPiServer.Globalization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace Knowit.EPiModules.Replaceables.Helper
{
    public static class ReplaceableHelper
    {

        private static readonly ILogger Log = LogManager.GetLogger(typeof (ReplaceableHelper));

        public const string SearchTagStart = "@#";
        public const string SearchTagEnd = "##";

        public static string SetReplacables(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input)) return input;

                var replaceables = GetCurrentLocaleReplaceables();
                
                return replaceables.Aggregate(input, (current, item) =>
                    Regex.Replace(current, string.Format("{0}{1}{2}", SearchTagStart, item.Key, SearchTagEnd), item.Value, RegexOptions.IgnoreCase));

            }
            catch (Exception e)
            {
                Log.Error("Exception during SetReplacables", e);
                return input;
            }

        }

        public static string GetCurrentLocale()
        {
            var currentCulture = ContentLanguage.PreferredCulture;
            return currentCulture.Name;
        }

        public static IDictionary<string, string> GetCurrentLocaleReplaceables()
        {
            var replacebles = ServiceLocator.Current.GetInstance<IReplaceablesManager>();
            var locale = GetCurrentLocale();
            return Task.Run(() =>replacebles.GetByLanguage(locale)).Result;
        }
    }
}