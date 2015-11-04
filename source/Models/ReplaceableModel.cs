using System;

namespace Knowit.EPiModules.Replaceables.Models
{
    public class ReplaceableModel
    {
        public string Key { get; set; }
        public string LanguageCode { get; set; }
        public string Value { get; set; }

        public static T Map<T>(ReplaceableModel model) where T : ReplaceableModel
        {
            if (model == null) return null;

            var entity = Activator.CreateInstance<T>();
            entity.Key = model.Key;
            entity.LanguageCode = model.LanguageCode;
            entity.Value = model.Value;
            return entity;
        }
    }
}