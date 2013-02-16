Lucinq
======

Expressive API for Lucene.Net, currently in alpha stage. 

The aim of the project is to give a relatively fluent API to Lucene.Net. The primary purpose of which is to make Lucene a little less verbose 
to accomplish most tasks whilst retaining the power and speed of Lucene.

Example Syntax
==============

Further examples can be found in the integration tests, however here is a quick overview of how the syntax looks

LuceneSearch search = new LuceneSearch(indexPath));

BooleanQuery query = new BooleanQuery();

query.Term("field", "value");
query.Or
	(
		x => x.Term("_name", "work"),
		x => x.Term("_name", "text")
	);

TopDocs results = search.Execute(query, 20);	
foreach (Document document in Search.GetTopDocuments(results))
{
	Console.WriteLine(document.GetValues("field")[0]);
}

OR

LuceneSearch search = new LuceneSearch(indexPath));

BooleanQuery query = new BooleanQuery();

query.Setup(
	x => x.Term("field", "value"),
	x => x.Or
			(
				y => y.Term("_name", "work"),
				y => y.Term("_name", "text")
			)
);

TopDocs results = search.Execute(query, 20);	
foreach (Document document in Search.GetTopDocuments(results))
{
	Console.WriteLine(document.GetValues("field")[0]);
}


Further Projects
================

The intention at present is to allow the use of raw lucene with a view to writing overlays for Sitecore and potentially umbraco.