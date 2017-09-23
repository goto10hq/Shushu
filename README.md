
![Shushu](https://github.com/goto10hq/Shushu/raw/master/shushu-icon.png)

# Shushu

[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](LICENSE.md)
[![Latest Version on NuGet](https://img.shields.io/nuget/v/Shushu.svg?style=flat-square)](https://www.nuget.org/packages/Shushu/)
[![NuGet](https://img.shields.io/nuget/dt/Shushu.svg?style=flat-square)](https://www.nuget.org/packages/Shushu/)
[![Visual Studio Team services](https://img.shields.io/vso/build/frohikey/c3964e53-4bf3-417a-a96e-661031ef862f/119.svg?style=flat-square)](https://github.com/goto10hq/Shushu)

## Preface

__Azure Search__ has kinda not super firendly teirs for situations when you have zillions of various document types organized in relatively small batches. In such cases it would becomes quite handy to have a possibility to just have one __universal index__. Unfortunately it's quite cumbersome to make mappings like:

``
Title -> Text0, Author -> Text1, ...
``

Here comes __Shushu__ to smooth things up.

## One index for all

[AzureSearch](https://github.com/goto10hq/Shushu/blob/master/Shushu/Tokens/AzureSearch.cs) is the main index.

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
public string Text19 { get; set; 

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

## TODO

- check types of properties being indexes
