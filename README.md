Lucinq - A Fluent / Expressive API for Lucene.Net
=================================================

This project gives fluent style API to Lucene.Net. The primary purpose of which is to make Lucene a little less verbose 
to accomplish most tasks whilst retaining the power and speed of Lucene. It has been designed with the goal with driving the
existing lucene API, and keeping the abstraction to a minimum.

Wiki
====

More complete documentation can be found in the wiki which can be found here:
https://github.com/cardinal252/Lucinq/wiki

Features
========

- A fluent style api for working with lucene queries for most of the query types and grouping.
- Paging
- Sorting
- Query manipulation - remove terms and re-run for example.
- Collecting

Get it with NuGet
=================

You can find the Nuget Package here: http://nuget.org/packages/Lucinq/

PM> Install-Package Lucinq

Example Syntax
==============

Further examples can be found in the integration tests, however here is a quick overview of how the syntax looks
```C#
LuceneSearch search = new LuceneSearch(indexPath));

IQueryBuilder query = new QueryBuilder();

query.Term("field", "value");
query.Or
	(
		x => x.Term("_name", "work"),
		x => x.Term("_name", "text")
	);

LuceneSearchResult result = search.Execute(query.Build(), 20);	
foreach (Document document in result.GetTopDocuments())
{
	Console.WriteLine(document.GetValues("field")[0]);
}
```

OR

```C#
LuceneSearch search = new LuceneSearch(indexPath));

IQueryBuilder query = new IQueryBuilder();

query.Setup(
	x => x.Term("field", "value"),
	x => x.Or
			(
				y => y.Term("_name", "work"),
				y => y.Term("_name", "text")
			)
);

LuceneSearchResult results = search.Execute(query.Build(), 20);	
foreach (Document document in result.GetTopDocuments())
{
	Console.WriteLine(document.GetValues("field")[0]);
}
```

Sitecore Adaptor
================

The Lucinq Solr adaptor was incompatible with Sitecore 10 onwards. This is becuase Lucinq uses ServiceLocator via Solr.Net to retrive Solr Operations. Sitecore Content API also initalises a custom locator provider, and this results in a "first past the post" situation which will cause errors with either Lucinq or Sitecore Indexing.

To resolve this an adaptor has been created where Lucinq generates the Solr.Net queries, then passes these queries to the Sitecore Content Search Api to execute


Further Projects
================

The intention at present is to allow the use of raw lucene with a view to potentially writing overlays for umbraco.



License
=======
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.