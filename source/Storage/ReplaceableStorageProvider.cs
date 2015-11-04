using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using Knowit.EPiModules.Replaceables.Models;

namespace Knowit.EPiModules.Replaceables.Storage
{
    internal interface IReplaceableStorageProvider
    {
        Task Save(ReplaceableModel model);

        Task<ReplaceableModel> Find(string key, string languageCode);
        Task<IEnumerable<ReplaceableModel>> FindAllByLanguage(string languageCode);

        Task Delete(string key);

    }

    internal class ReplaceableStorageProvider : IReplaceableStorageProvider
    {
        private readonly DynamicDataStore _replaceableStore;
        private static readonly Dictionary<string, Identity> IdentityMap;

        static ReplaceableStorageProvider()
        {
            IdentityMap = new Dictionary<string, Identity>();

            var store = DynamicDataStoreFactory.Instance.GetStore(typeof (Replaceable));
            var replaceables = store.LoadAll<Replaceable>();
            foreach (var replaceable in replaceables)
            {
                IdentityMap.Add(replaceable.GetLocator(), replaceable.Id);
            }
        }

        public ReplaceableStorageProvider()
        {
            _replaceableStore = DynamicDataStoreFactory.Instance.GetStore(typeof(Replaceable));
        }

        public Task Save(ReplaceableModel model)
        {
            var mapped = ReplaceableModel.Map<Replaceable>(model);

            if (IdentityMap.ContainsKey(mapped.GetLocator()))
            {
                mapped.Id = IdentityMap[mapped.GetLocator()];
                _replaceableStore.Save(mapped, mapped.Id);
            }
            else
            {
                var id = _replaceableStore.Save(mapped);
                IdentityMap.Add(mapped.GetLocator(), id);
            }

            return Task.FromResult(0);
        }

        public Task<ReplaceableModel> Find(string key, string languageCode)
        {
            var locator = StorageHelper.GetLocator(key, languageCode);
            var id = IdentityMap.ContainsKey(locator) ? IdentityMap[locator] : null;

            Replaceable replaceable;

            if (id != null)
            {
                replaceable = _replaceableStore.Load<Replaceable>(id);
            }
            else
            {
                replaceable = _replaceableStore.LoadAll<Replaceable>().FirstOrDefault(x
                    => x.Key.ToLower().Equals(key.ToLower())
                       && x.LanguageCode.ToLower().Equals(languageCode.ToLower()));
            }

            return replaceable != null
                ? Task.FromResult(ReplaceableModel.Map<ReplaceableModel>(replaceable))
                : Task.FromResult<ReplaceableModel>(null);
        }

        public Task<IEnumerable<ReplaceableModel>> FindAllByLanguage(string languageCode)
        {
            var result = _replaceableStore.LoadAll<Replaceable>()
                .Where(x => x.LanguageCode.ToLower().Equals(languageCode.ToLower()))
                .Select(ReplaceableModel.Map<ReplaceableModel>);

            return Task.FromResult(result);
        }

        public Task Delete(string key)
        {
            var toDelete = _replaceableStore.LoadAll<Replaceable>()
                .Where(x => x.Key.ToLower().Equals(key.ToLower()));
            
            foreach (var replaceable in toDelete)
            {
                var locator = StorageHelper.GetLocator(replaceable.Key, replaceable.LanguageCode);
                if(IdentityMap.ContainsKey(locator))
                    IdentityMap.Remove(locator);

                _replaceableStore.Delete(replaceable.Id);
            }

            return Task.FromResult(0);
        }

        
    }
}