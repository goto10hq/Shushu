
![Shushu](https://github.com/goto10hq/Shushu/raw/master/shushu-icon.png)

# Shushu

[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](LICENSE.md)
[![Latest Version on NuGet](https://img.shields.io/nuget/v/Shushu.svg?style=flat-square)](https://www.nuget.org/packages/Shushu/)
[![NuGet](https://img.shields.io/nuget/dt/Shushu.svg?style=flat-square)](https://www.nuget.org/packages/Shushu/)
[![Visual Studio Team services](https://img.shields.io/vso/build/frohikey/c3964e53-4bf3-417a-a96e-661031ef862f/119.svg?style=flat-square)](https://github.com/goto10hq/Shushu)

## Preface

__Azure Search__ has kinda not super firendly teirs for situations when you have zillions of various document types organized in relatively small batches. 
In such cases it would becomes quite handy to have a possibility to just have one __universal index__. Unfortunately it's quite cumbersome to make mappings like:

```
Title -> Text0, Author -> Text1, ...
```

Here comes __Shushu__ to smooth things up.

## One index for all

[ShushuIndex](https://github.com/goto10hq/Shushu/blob/master/Shushu/Tokens/ShushuIndex.cs) is the main index.

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

`T GetDocument<T>(string key)`

Get one document by identifier (key)

`IndexDocument<T>(T document, bool merge)`

Upload or upload+merge and index one document.

`IndexDocuments<T>(IList<T> documents, bool merge)` 

Upload or upload+merge and index documents (automatically served in chunks divided into 1000).

`DeleteIndex()`

Delete index.

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

## TODO

- check types of properties being indexed (is conversion valid?)

## License

MIT © [Joey Frohikey](http://frohikey.com)