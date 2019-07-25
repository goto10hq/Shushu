
![Shushu](https://github.com/goto10hq/Shushu/raw/master/shushu-icon.png)

# Shushu

[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](LICENSE.md)
[![Latest Version on NuGet](https://img.shields.io/nuget/v/Shushu.svg)](https://www.nuget.org/packages/Shushu/)
[![NuGet](https://img.shields.io/nuget/dt/Shushu.svg)](https://www.nuget.org/packages/Shushu/)
[![.NETStandard 2.0](https://img.shields.io/badge/.NETStandard-2.0-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
[![Build status](https://frohikey.visualstudio.com/Goto10/_apis/build/status/NUGET/shushu)](https://frohikey.visualstudio.com/Goto10/_build/latest?definitionId=119)

## Preface

__Azure Search__ has kinda not super friendly tiers for situations when you have zillions of various document types organized in relatively small batches. 
In such cases it would becomes quite handy to have a possibility to just have one __universal index__. Unfortunately it's quite cumbersome to make mappings like:

```
Title -> Text0, Author -> Text1, ...
```

Here comes __Shushu__ to make things much more easier for you.

## One index to rule'em all

[ShushuIndex](https://github.com/goto10hq/Shushu/blob/master/Shushu/Tokens/ShushuIndex.cs) is the main index.

### SushuIndex

```csharp
[IsFilterable]
public string Id { get; set; }

[IsFilterable]
public string Entity { get; set; }
                
[IsSearchable]
[IsSortable]
[Analyzer("standardasciifolding.lucene")]
[IsFilterable]        
public string Text0 { get; set; }
public string Text19 { get; set; }

[IsSortable]
[IsFilterable]
[IsFacetable]
public DateTimeOffset? Date0 { get; set; }
public DateTimeOffset? Date4 { get; set; }

[IsSearchable]
[IsFilterable]
[IsFacetable]
public List<string> Tags0 { get; set; }
public List<string> Tags9 { get; set; }

[IsFilterable]
[IsSortable]
[IsFacetable]
public int? Number0 { get; set; }
public int? Number9 { get; set; }

[IsFilterable]
[IsSortable]
[IsFacetable]
public double? Double0 { get; set; }
public double? Double9 { get; set; }

[IsFilterable]
[IsFacetable]
public bool? Flag0 { get; set; }
public bool? Flag9 { get; set; }

[IsFilterable]
[IsSortable]
public GeographyPoint Point0 { get; set; }

[IsFilterable]
[IsSortable]        
public IList<ComplexItem> Complex0 { get; set; }
public IList<ComplexItem> Complex1 { get; set; }
public IList<ComplexItem> Complex2 { get; set; }
```

### ComplexItem

```csharp
public string Text { get; set; }
public DateTimeOffset? Date { get; set; }
public IList<string> Tags { get; set; }
public long? Number { get; set; }
public double? Double { get; set; }
public bool? Flag { get; set; }
public GeographyPoint Point { get; set; }
```

## Shushu object

There's Shushu object for some basic operations or you can you Azure Search SDK directly of course.
Just create an instance:

```csharp
var shushu = new Shushu(name_of_the_service, service_api_key, search_api_key, name_of_the_index);
```

Note:
- Shushu tries to create index if it does not exist

### Methods

`long CountAllDocuments()`

Count all documents in index.

`DeleteDocument(string id)`

Delete document from index.

`DeleteDocuments(IEnumerable<string> ids)`

Delete documents from index.

`DeleteIndex()`

Delete index.

`T GetDocument<T>(string id)`

Get one document by identifier (key)

`IndexDocument<T>(T document, bool merge)`

Upload or upload+merge and index one document.

`IndexDocuments<T>(IList<T> documents, bool merge)` 

Upload or upload+merge and index documents (automatically served in chunks divided into 1000).

`DocumentSearchResult<T> SearchDocuments<T>(string searchText, SearchParameters searchParameters)`

Search documents.

Note:
- all methods have `async` variants

## Mapping

We have two mapping attributes:
- `ClassMapping` - global settings basically for constant values 
- `PropertyMapping` - mapping property to __ShushuIndex__ and vice versa

Decorate your POCO with attributes e.g.

```csharp
[ClassMapping(Enums.IndexField.Entity, "shushu")]    
[ClassMapping(Enums.IndexField.Flag0, true)] 
public class Shu
{
 [PropertyMapping(Enums.IndexField.Id)]
 public string Id { get; set; }

 [PropertyMapping(Enums.IndexField.Text0)]
 public string Name { get; set; }

 [PropertyMapping(Enums.IndexField.Number0)]
 public int Iq { get; set; }

 public Shu() {}
 public Shu(string id, string name, int iq) { Id = id; Name = name; Iq = iq; }
}
``` 

Notes:
- you have to set a mapping for Id field
- duplicated mappings are not allowed

You can try mapping: `var search = a.MapToIndex();`

Or just create Shushu instance and index it and it will be mapped automatically:

```csharp
var shu = new Shu("1", "nene", 130);
shushu.IndexDocument(shu);
```

If you get document(s) from Azure Search they are mapped back to your POCO.

```csharp
var shu = shushu.GetDocument<Shu>("1");
```

You can try mapping: `var poco = shushu.MapFromIndex<Poco>();`

There's also handy POCO ShushuEntity if you want just map all the properties.

```csharp
var full = shushu.GetDocument<Tokens.ShushuEntity>("1");
```

And of course the most import part... searching itself. Again search parameters are replaced accordingly just use `@Property`.

```csharp
var sp = new SearchParameters
{
 Filter = "entity eq 'shushu'",
 Top = 5,
 OrderBy = new List<string> { "@Iq desc" }
};

var result = _shushu.SearchDocuments<Shu>("*", sp);
```

## Tests

Azure Search can be tricky because indexing itself takes some time. That's why I was forced to use `Thread.Sleep()`.

If you want to run tests on your own, you have to set `AppSettings.json` or provide `secrets` named `shushu`.

## License

MIT © [frohikey](http://frohikey.com) / [Goto10 s.r.o.](http://www.goto10.cz)
