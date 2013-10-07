using System;
using System.Linq.Expressions;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.Querying.Interfaces
{
    public interface ISitecoreQueryBuilder : IQueryBuilder
    {
        new ISitecoreQueryBuilder Parent { get; }

        SitecoreMode SitecoreMode { get; }

        TermQuery Id(ID itemId, Matches occur = Matches.NotSet, float? boost = null, string key = null);
        IQueryBuilder Ids(ID[] itemIds, float? boost = null, Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, string key = null);

        /// <summary>
        /// To find items that directly inherit from a particular template
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        TermQuery TemplateId(ID templateId, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        ISitecoreQueryBuilder Setup(params Action<ISitecoreQueryBuilder>[] queries);

        /// <summary>
        /// Helper to add multiple templates
        /// </summary>
        /// <param name="templateIds"></param>
        /// <param name="boost"></param>
        /// <param name="occur"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IQueryBuilder TemplateIds(ID[] templateIds, float? boost = null, Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, string key = null);

        /// <summary>
        /// Get items derived from the given template at some point in their heirarchy
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        TermQuery TemplateDescendsFrom(ID templateId, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        /// <summary>
        /// Gets items by their name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Query Name(string value, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        /// <summary>
        /// Gets items by an array of names
        /// </summary>
        /// <param name="values"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IQueryBuilder Names(string[] values, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        /// <summary>
        /// Gets items by their language
        /// </summary>
        /// <param name="language"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Query Language(Language language, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        IQueryBuilder Languages(Language[] languages, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        /// <summary>
        /// Gets a query representing 
        /// </summary>
        /// <param name="ancestorId"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        TermQuery DescendantOf(ID ancestorId, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        TermQuery ChildOf(ID parentId, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        Query Field(string fieldName, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null, int slop = 1);
        Query Field(string fieldName, ID value, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        /// <summary>
        /// Gets items in the given database
        /// </summary>
        /// <param name="value"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Query Database(string value, Matches occur = Matches.NotSet, float? boost = null, string key = null);

        /// <summary>
        /// Creates a simple group that MUST occur, each item of which SHOULD occur by default
        /// </summary>
        /// <param name="queries">The lamdba expressions showing queries</param>
        ISitecoreQueryBuilder Or(params Action<ISitecoreQueryBuilder>[] queries);

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, each item of which SHOULD occur by default
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        ISitecoreQueryBuilder Or(Matches occur = Matches.NotSet, params Action<ISitecoreQueryBuilder>[] queries);

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, and specification of each items occurance.
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="childrenOccur">Whether the child query should occur by default</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        ISitecoreQueryBuilder Group(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, params Action<ISitecoreQueryBuilder>[] queries);

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, each item of which MUST occur by default
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        ISitecoreQueryBuilder And(Matches occur = Matches.NotSet, params Action<ISitecoreQueryBuilder>[] queries);
    }
}
