using System;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.ContentSearch.Utilities;

namespace Lucinq.SitecoreIntegration.Indexing
{
    public class LucinqIndex : LuceneIndex
    {
        private string[] rootPaths;

        public LucinqIndex(string name, string folder, IIndexPropertyStore propertyStore, string rootPaths) : base(name, folder, propertyStore)
        {
            SetRootPaths(rootPaths);
        }

        protected void SetRootPaths(string input)
        {
            if (input.Contains("|"))
            {
                rootPaths = input.Split('|').Select(GetPath).ToArray();
                return;
            }
            rootPaths = new[] {GetPath(input)};
        }

        private string GetPath(string input)
        {
            if (input.IndexOf("/sitecore/content", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return input.Substring(17);
            }
            return input;
        }
        
        public override IIndexOperations Operations
        {
            get { return new IndexOperations(this, rootPaths); }
        }
    }
}
