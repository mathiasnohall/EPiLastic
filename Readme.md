EPiLastic

A project to index episerver pages and blocks into an elastic server cluster or node.
provides filtering on type and subtypes. Autocompletion. 

The solution consists of two main projects. A indexer and a queruyer. The indexer has a reindex job and an event handler wich listens for content events. The queryer is the ISearchClient in the query project. It has some basic methods for querying Nest and Elastic Search.

For full implementation of Searches. See example project EPiLastic.API here: https://github.com/mathiasnohall/EPiLastic.Api

### Installing

From the package manager console:

	PM> Install-Package EPiLastic

or by simply searching for `EPiLastic` in the package manager UI.

### Usage:

	Install an elastic search 2.x node

	<appSettings>
		<add key="IndexAliasName" value="YouElasticSearchAliasName" />
		<add key="ElasticSearchUrl" value="http://localhost:9200" />
	</appSettings>

Add interface ISearchablePage to pagetypes you want to be indexed.
Add interface ISearchableBlock to block types you want to be indexed.
Add interface ISearchableBlockContainer to blocks that contains searchable block types, i.eg. blocks with a contentarea.


Map properties with attributes.

[TitleAttribute]
i.eg. the title of the block or page. Only one allowed. string property.

[TextAttribute]
the maincontent of a block or a page. Multiple allowed. strings and xhtml properties.

[HiddenKeywordsAttribute]
content that is indexed and searchable. But will not appear in highlighting. i.eg. a property that exists on the pagetype but is not visible to visitors.

[TeaserTextAttribute]
Text that could appear in the search result teaser/description text

[TeaserImageAttribute]
The search result teaser image

[TypeAttribute]
For filtering on types

[SubTypeAttribute]
For filtering on subtypes

i.eg. blocktype:

```C#
public class SearchableBlock : BlockData, ISearchableBlock
{
    [Title]
    public string Title { get; set; }

    [Text]
    public XhtmlString MainBody { get; set; }

    [Text]
    public string SecondBody { get; set; }

    [Text]
    public XhtmlString ThirdBody { get; set; }	
}
```

i.eg. pagetype:

```C#
public class SearchablePage : PageData, ISearchablePage
{
    [HiddenKeywords]
    public string HiddenKeyWord { get; set; }

    [Title]
    public string Title { get; set; }

    [Text]
    public XhtmlString MainBody { get; set; }

    [Type]
    public string Type { get; set; }

    [SubType]
    public string SubType { get; set; }

    [TeaserText]
    public string TeaserText { get; set; }

    [TeaserImage]
    public ContentReference TeaserImage { get; set; }

    public bool ExcludeFromSearch { get; set; }
}
```
