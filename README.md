Lucinq
======

Expressive API for Lucene.Net, currently in alpha stage. 

The aim of the project is to give a relatively fluent API to Lucene.Net. The primary purpose of which is to make Lucene a little less verbose 
to accomplish most tasks whilst retaining the power and speed of Lucene.

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

Further Projects
================

The intention at present is to allow the use of raw lucene with a view to writing overlays for Sitecore and potentially umbraco.

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