using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Cache;
using Knowit.EPiModules.Replaceables.Models;
using Knowit.EPiModules.Replaceables.Storage;

namespace Knowit.EPiModules.Replaceables
{
    /// <summary>
    /// Manages replaceables, and the cache for them.
    /// </summary>
    public interface IReplaceablesManager
    {
        Task<Dictionary<string, string>> GetByLanguage(string languageCode);
        Task Save(ReplaceableModel replaceable);
        Task Delete(string key);
    }

    internal class ReplaceablesManager : IReplaceablesManager
    {
        private readonly IReplaceableStorageProvider _replaceableRepository;
        private readonly ILanguageBranchRepository _languageBranchRepository;
        private readonly ISynchronizedObjectInstanceCache _cacheManager;

        public ReplaceablesManager(
            IReplaceableStorageProvider replaceableRepository, 
            ILanguageBranchRepository languageBranchRepository, 
            ISynchronizedObjectInstanceCache cacheManager)
        {
            _replaceableRepository = replaceableRepository;
            _languageBranchRepository = languageBranchRepository;
            _cacheManager = cacheManager;
        }

        private const string CacheGuid = "747B2B52-A9B6-4621-8DD1-363ECFB783C3";

        public async Task<Dictionary<string, string>> GetByLanguage(string languageCode)
        {
            var cacheKey = GetCacheKey(languageCode);

            var cached = _cacheManager.Get(cacheKey) as Dictionary<string, string>;
            if (cached != null && cached.Any()) return cached;

            var fromDb = await _replaceableRepository.FindAllByLanguage(languageCode);
            if (fromDb == null || !fromDb.Any()) return new Dictionary<string, string>();

            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (ReplaceableModel model in fromDb)
                dictionary.Add(model.Key, model.Value);
            UpdateCache(cacheKey, dictionary);
            return dictionary;
        }

        public Task Save(ReplaceableModel replaceable)
        {
            _replaceableRepository.Save(replaceable);

            var cacheKey = GetCacheKey(replaceable.LanguageCode);
            var cached = _cacheManager.Get(cacheKey) as Dictionary<string, string>;
            if (cached == null) return Task.FromResult(0);

            if (cached.ContainsKey(replaceable.Key))
            {
                cached[replaceable.Key] = replaceable.Value;
            }
            else
            {
                cached.Add(replaceable.Key, replaceable.Value);
            }

            UpdateCache(cacheKey, cached);

            return Task.FromResult(0);
        }

        public Task Delete(string key)
        {
            _replaceableRepository.Delete(key);

            foreach (var languageBranch in _languageBranchRepository.ListEnabled())
            {
                var cacheKey = GetCacheKey(languageBranch.Culture.Name);
                DeleteKeyFromCache(key, cacheKey);
            }

            return Task.FromResult(0);
        }

        private void DeleteKeyFromCache(string key, string cacheKey)
        {
            var cache = _cacheManager.Get(cacheKey) as Dictionary<string, string>;
            if (cache != null && cache.ContainsKey(key))
            {
                cache.Remove(key);
                UpdateCache(cacheKey, cache);
            }

        }

        private string GetCacheKey(string languageCode)
        {
            return string.Format("{0}-{1}", CacheGuid, languageCode);
        }

        private void UpdateCache(string cacheKey, Dictionary<string, string> replaceables)
        {
            _cacheManager.Remove(cacheKey);
            _cacheManager.Insert(cacheKey, replaceables, CacheEvictionPolicy.Empty);
        }
    }
}