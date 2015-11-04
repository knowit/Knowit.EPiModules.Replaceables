using EPiServer.Data;
using EPiServer.Data.Dynamic;
using Knowit.EPiModules.Replaceables.Models;

namespace Knowit.EPiModules.Replaceables.Storage
{
    [EPiServerDataStore(AutomaticallyCreateStore = true)]
    internal class Replaceable : ReplaceableModel, IDynamicData
    {       
        public Identity Id { get; set; }
    }
}