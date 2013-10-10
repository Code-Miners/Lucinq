using Sitecore.ContentSearch;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Maintenance;

namespace Lucinq.SitecoreIntegration.Indexing
{
    public class LucinqIndex : LuceneIndex
    {
        #region [ Fields ]

        private string[] rootPaths;

        #endregion

        #region [ Constructors ]

        public LucinqIndex(string name, string folder, IIndexPropertyStore propertyStore, string rootPaths) : base(name, folder, propertyStore)
        {
            SetRootPaths(rootPaths);
        }

        #endregion

        #region [ Properties ]

        public override IIndexOperations Operations
        {
            get { return new IndexOperations(this, rootPaths); }
        }

        #endregion

        #region [ Methods ]

        protected void SetRootPaths(string input)
        {
            if (input.Contains("|"))
            {
                rootPaths = input.Split('|');
                return;
            }
            rootPaths = new[] {input};
        }

        
        #endregion
    }
}
