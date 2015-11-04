using System;
using System.Web.Mvc;

namespace Knowit.EPiModules.Replaceables.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ReplaceablesBypass : FilterAttribute
    {
        protected bool Equals(ReplaceablesBypass other)
        {
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetType() == obj.GetType();
        }
    }
}