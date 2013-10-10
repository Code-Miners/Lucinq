using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucinq.Building;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Field = Sitecore.Data.Fields.Field;

namespace Lucinq.SitecoreIntegration.Indexing
{
    public class IndexOperations : IndexingHelper, IIndexOperations
    {
        // Based upon 
        // Sitecore.ContentSearch.SitecoreItemCrawler

        #region [ Fields ]

        private readonly ILuceneProviderIndex index;

        private readonly string[] rootPaths;

        #endregion

        #region [ Constructors ]

        public IndexOperations(ILuceneProviderIndex index, string[] rootPaths)
        {
            Assert.ArgumentNotNull(index, "index");
            Assert.IsNotNull(index.Schema, "Index schema not available.");
            this.index = index;
            this.rootPaths = rootPaths;
        }

        #endregion

        #region [ Methods ]

        public virtual void Update(IIndexable indexable, IProviderUpdateContext context, ProviderIndexConfiguration indexConfiguration)
        {
            Document document = GetIndexData(indexable, context);

            bool valid = ItemIsValid(indexable);

            if (!valid)
            {
                return;
            }

            if (document == null)
            {
                CrawlingLog.Log.Warn(
                    string.Format(
                        "LuceneIndexOperations.Update(): IndexVersion produced a NULL doc for version {0}. Skipping.",
                        indexable.UniqueId));
                return;
            }
            var updateTerm = new Term("_uniqueid", indexable.UniqueId.Value.ToString());
            context.UpdateDocument(document, updateTerm, indexable.Culture != null ? new CultureExecutionContext(indexable.Culture) : null);
        }

        public virtual void Delete(IIndexable indexable, IProviderUpdateContext context)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            Delete(indexable.UniqueId, context);
        }

        public virtual void Delete(IIndexableId id, IProviderUpdateContext context)
        {
            Assert.ArgumentNotNull(id, "id");
            context.Delete(id);
        }

        public virtual void Delete(IIndexableUniqueId indexableUniqueId, IProviderUpdateContext context)
        {
            Assert.ArgumentNotNull(indexableUniqueId, "indexableUniqueId");
            context.Delete(indexableUniqueId);
        }

        public virtual void Add(IIndexable indexable, IProviderUpdateContext context, ProviderIndexConfiguration indexConfiguration)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            Assert.ArgumentNotNull(context, "context");

            bool valid = ItemIsValid(indexable);

            if (!valid)
            {
                return;
            }

            Document document = GetIndexData(indexable, context);
            if (document == null)
            {
                return;
            }

            context.AddDocument(document, indexable.Culture != null ? new CultureExecutionContext(indexable.Culture) : null);
        }

        public virtual Document GetIndexData(IIndexable indexable, IProviderUpdateContext context)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            Assert.ArgumentNotNull(context, "context");
            Assert.Required(index.Configuration.DocumentOptions as LuceneDocumentBuilderOptions, "IDocumentBuilderOptions of wrong type for this crawler");
            SitecoreIndexableItem sitecoreIndexableItem = indexable as SitecoreIndexableItem;
            if (indexable.Id.ToString() == string.Empty || sitecoreIndexableItem == null)
            {
                return null;
            }

            Document document = new Document();

            AddFields(document, sitecoreIndexableItem.Item);
            
            // add the unique identifier field
            document.AddAnalysedField("_uniqueid", indexable.UniqueId.Value.ToString());

            return document; 
        }

        private bool ItemIsValid(IIndexable indexable)
        {
            SitecoreIndexableItem sitecoreIndexableItem = indexable as SitecoreIndexableItem;
            if(sitecoreIndexableItem == null)
            {
                return false;
            }

            bool valid;
            if (rootPaths == null || rootPaths.Length == 0)
            {
                valid = true;
            }
            else
            {
                valid = rootPaths.Any(indexingRootPath => sitecoreIndexableItem.Item.Paths.FullPath.IndexOf(indexingRootPath, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            return valid;
        }

        public virtual void AddFields(Document document, Item item)
        {
            AddLucinqFields(document, item);
            AddCustomFields(document, item);
            AddOtherFields(document, item);
        }

        protected virtual void AddOtherFields(Document document, Item item)
        {
            List<Field> fields = GetFields(item);
            fields.ForEach(field => AddField(document, field));
        }

        protected List<Field> GetFields(Item item)
        {
            return item.Fields.ToList();
        }

        #endregion
    }
}

