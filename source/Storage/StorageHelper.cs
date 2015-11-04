namespace Knowit.EPiModules.Replaceables.Storage
{
    internal static class StorageHelper
    {
        public static string GetLocator(this Replaceable replaceable)
        {
            return GetLocator(replaceable.Key, replaceable.LanguageCode);
        }

        public static string GetLocator(string key, string languageCode)
        {
            return string.Format("{0}_{1}", key, languageCode);
        }
    }
}