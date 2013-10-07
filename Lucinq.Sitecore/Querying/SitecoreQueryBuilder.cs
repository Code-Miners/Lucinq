using System;
using System.Linq.Expressions;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Extensions;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.SitecoreIntegration.Constants;
using Lucinq.SitecoreIntegration.Extensions;
using Lucinq.SitecoreIntegration.Querying.Interfaces;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.Querying
{
    public enum SitecoreMode
    {
        Lucinq,
        Sitecore7
    }

    public class SitecoreQueryBuilder : QueryBuilder, ISitecoreQueryBuilder
    {
        #region [ Properties ]

        public SitecoreMode SitecoreMode
        {
            get; private set;
        }

        new public ISitecoreQueryBuilder Parent { get; private set; }

        #endregion

        #region [ Constructors ]

        public SitecoreQueryBuilder(params Action<ISitecoreQueryBuilder>[] queries)
            : this()
        {
            AddQueries(queries);
        }
        
        public SitecoreQueryBuilder() : this(SitecoreMode.Lucinq)
        {
            
        }

        public SitecoreQueryBuilder(SitecoreMode sitecoreMode, params Action<ISitecoreQueryBuilder>[] queries)
            : this(sitecoreMode)
        {
            AddQueries(queries);
        }

        public SitecoreQueryBuilder(SitecoreMode sitecoreMode)
        {
            SitecoreMode = sitecoreMode;
        }

        public SitecoreQueryBuilder(ISitecoreQueryBuilder parentQueryBuilder) : this(parentQueryBuilder, SitecoreMode.Lucinq)
        {
        }

        public SitecoreQueryBuilder(ISitecoreQueryBuilder parentQueryBuilder, SitecoreMode sitecoreMode) : this(sitecoreMode)
        {
            Parent = parentQueryBuilder;
        }

        public SitecoreQueryBuilder(ISitecoreQueryBuilder parentQueryBuilder, SitecoreMode sitecoreMode, params Action<ISitecoreQueryBuilder>[] queries)
            : this(parentQueryBuilder, sitecoreMode)
        {
           AddQueries(queries);
        }

        #endregion

        #region [ Methods ]

        #region [ Group Setup ]

        protected void AddQueries(params Action<ISitecoreQueryBuilder>[] queries)
        {
            foreach (Action<ISitecoreQueryBuilder> item in queries)
            {
                item(this);
            }
        }

        /// <summary>
        /// A setup method to aid multiple query setup
        /// </summary>
        /// <param name="queries">Comma seperated lambda actions</param>
        /// <returns>The input querybuilder</returns>
        public virtual ISitecoreQueryBuilder Setup(params Action<ISitecoreQueryBuilder>[] queries)
        {
            AddQueries(queries);
            return this;
        }

        #endregion

        #region [ ID Extensions ]

        public virtual TermQuery Id(ID itemId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            string luceneItemId = itemId.ToLuceneId();
            return Term(SitecoreFields.Id, luceneItemId, occur, boost, key);
        }

        public virtual IQueryBuilder Ids(ID[] itemIds, float? boost = null, Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, string key = null)
        {
            var group = Group(occur);
            foreach (ID itemId in itemIds)
            {
                group.Id(itemId, childrenOccur, boost, key);
            }
            return this;
        }

        #endregion

        #region [ Grouping ]

        new protected virtual ISitecoreQueryBuilder CreateNewChildGroup(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet)
        {
            return new SitecoreQueryBuilder(this) { Occur = occur, DefaultChildrenOccur = childrenOccur };
        }

        public ISitecoreQueryBuilder Or(params Action<ISitecoreQueryBuilder>[] queries)
        {
            throw new NotImplementedException();
        }

        public ISitecoreQueryBuilder Or(Matches occur = Matches.NotSet, params Action<ISitecoreQueryBuilder>[] queries)
        {
            return Group(occur, Matches.Sometimes, queries).Parent;
        }

        public ISitecoreQueryBuilder Group(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, params Action<ISitecoreQueryBuilder>[] queries)
        {
            if (occur == Matches.NotSet)
            {
                occur = Matches.Always;
            }
            var groupedBooleanQuery = AddChildGroup(occur, childrenOccur, queries);
            if (groupedBooleanQuery == null)
            {
                throw new Exception("An error occurred creating the inner query");
            }
            return groupedBooleanQuery;
        }

        protected virtual ISitecoreQueryBuilder AddChildGroup(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, params Action<ISitecoreQueryBuilder>[] queries)
        {
            if (occur == Matches.NotSet)
            {
                occur = Matches.Always;
            }
            if (childrenOccur == Matches.NotSet)
            {
                childrenOccur = Matches.Always;
            }

            ISitecoreQueryBuilder queryBuilder = CreateNewChildGroup(occur, childrenOccur);
            foreach (var item in queries)
            {
                item(queryBuilder);
            }
            Groups.Add(queryBuilder);
            return queryBuilder;
        }

        public ISitecoreQueryBuilder And(Matches occur = Matches.NotSet, params Action<ISitecoreQueryBuilder>[] queries)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [ Template Extensions ]

        /// <summary>
        /// To find items that directly inherit from a particular template
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public TermQuery TemplateId(ID templateId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            string luceneTemplateId = templateId.ToLuceneId();
            return Term(SitecoreFields.TemplateId, luceneTemplateId, occur, boost, key);
        }

        /// <summary>
        /// Helper to add multiple templates
        /// </summary>
        /// <param name="templateIds"></param>
        /// <param name="boost"></param>
        /// <param name="occur"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public IQueryBuilder TemplateIds(ID[] templateIds, float? boost = null, Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, string key = null)
        {
            var group = Group(occur);
            foreach (ID templateId in templateIds)
            {
                group.TemplateId(templateId, childrenOccur, boost, key);
            }
            return this;
        }

        /// <summary>
        /// Get items derived from the given template at some point in their heirarchy
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public TermQuery TemplateDescendsFrom(ID templateId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            string luceneTemplateId = templateId.ToLuceneId();
            return Term(SitecoreFields.TemplatePath, luceneTemplateId, occur, boost, key);
        }

        #endregion

        #region [ Name Extensions ]

        /// <summary>
        /// Gets items by their name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Query Name(string value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            return Term(SitecoreFields.Name, value, occur, boost, key);
        }

        /// <summary>
        /// Gets items by an array of names
        /// </summary>
        /// <param name="values"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public IQueryBuilder Names(string[] values, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            foreach (string templateId in values)
            {
                Term(SitecoreFields.TemplateId, templateId, occur, boost, key);
            }
            return this;
        }

        #endregion

        #region [ Regionalisation ]

        /// <summary>
        /// Gets items by their language
        /// </summary>
        /// <param name="language"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Query Language(Language language, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            string languageString = language.CultureInfo.Name.ToLower();
            return Keyword(SitecoreFields.Language, languageString, occur, boost, key);
        }

        public IQueryBuilder Languages(Language[] languages, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            foreach (Language language in languages)
            {
                Language(language, occur, boost, key);
            }
            return this;
        }
        #endregion

        #region [ Heirarchy Extensions ]

        /// <summary>
        /// Gets a query representing 
        /// </summary>
        /// <param name="ancestorId"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public TermQuery DescendantOf(ID ancestorId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            string ancestorIdString = ancestorId.ToLuceneId();
            return Term(SitecoreFields.Path, ancestorIdString, occur, boost, key);
        }
        
        /// <summary>
        /// Gets items by their direct parent
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public TermQuery ChildOf(ID parentId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            string parentIdString = parentId.ToLuceneId();
            return Term(SitecoreFields.Parent, parentIdString, occur, boost, key);
        }

        #endregion

        #region [ Field Extensions ]

        public Query Field(string fieldName, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null, int slop = 1)
        {
            if (value.Contains("*"))
            {
                return WildCard(SitecoreQueryBuilderExtensions.GetEncodedFieldName(fieldName), value.ToLower(), occur, boost, key);
            }
            if (value.Contains(" "))
            {
                PhraseQuery phraseQuery = Phrase(slop, boost, occur, key);
                string[] valueElems = value.Split(new[] { ' ' });
                foreach (var valueElem in valueElems)
                {
                    phraseQuery.AddTerm(SitecoreQueryBuilderExtensions.GetEncodedFieldName(fieldName), valueElem.ToLower());
                }
                return phraseQuery;
            }
            return Term(SitecoreQueryBuilderExtensions.GetEncodedFieldName(fieldName), value.ToLower(), occur, boost, key);
        }

        public Query Field(string fieldName, ID value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            return Term(SitecoreQueryBuilderExtensions.GetEncodedFieldName(fieldName), value.ToLuceneId(), occur, boost, key);
        }

        #endregion

        #region [ Database ]

        /// <summary>
        /// Gets items in the given database
        /// </summary>
        /// <param name="value"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Query Database(string value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            return Term(SitecoreFields.Database, value.ToLower(), occur, boost, key);
        }

        #endregion

        #endregion
    }
}
