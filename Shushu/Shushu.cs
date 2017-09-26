using Microsoft.Azure.Search;
using Sushi2;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using Shushu.Tokens;

namespace Shushu
{
    /// <summary>
    /// Shushu.
    /// </summary>
    public class Shushu
    {        
        /// <summary>
        /// The size of the batch.
        /// </summary>
        /// <remarks>Note that you can only include up to 1000 documents (or 16 MB) in a single indexing request</remarks>
        const int MaxBatchSize = 1000;
        readonly string _indexKey = Enums.IndexField.Id.ToString().ToCamelCase();
        readonly SearchServiceClient _serviceClient;
        SearchIndexClient _searchClient;
        SearchIndexClient _indexClient;
        readonly string _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Shushu.Shushu"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="serviceApiKey">Service API key.</param>
        /// <param name="searchApiKey">Search API key.</param>
        /// <param name="index">Index.</param>
        public Shushu(string name, string serviceApiKey, string searchApiKey, string index)
        {
            _index = index;

            _serviceClient = new SearchServiceClient(name, new SearchCredentials(serviceApiKey));
            _searchClient = new SearchIndexClient(name, index, new SearchCredentials(searchApiKey));
            _indexClient = new SearchIndexClient(name, index, new SearchCredentials(serviceApiKey));

            var idx = _serviceClient.Indexes.List().Indexes.FirstOrDefault(x => x.Name.Equals(index));

            if (idx == null)
                CreateIndex();            
        }

        /// <summary>
        /// Deletes the index.
        /// </summary>
        public void DeleteIndex()
        {
            AsyncTools.RunSync(DeleteIndexAsync);
        }

        /// <summary>
        /// Deletes the index.
        /// </summary>
        /// <returns>The index.</returns>
        public async Task DeleteIndexAsync()
        {
            await _serviceClient.Indexes.DeleteAsync(_index);
        }

        /// <summary>
        /// Index document.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public void IndexDocument<T>(T document, bool merge = true) where T: class
        {
            AsyncTools.RunSync(() => IndexDocumentAsync(document, merge));
        }

        /// <summary>
        /// Index document.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public async Task IndexDocumentAsync<T>(T document, bool merge = true) where T: class
        {
            var documents = new List<ShushuIndex> { document.MapToIndex() };

            if (merge)
            {
                var batch = IndexBatch.MergeOrUpload(documents);
                await _indexClient.Documents.IndexAsync(batch);
            }
            else
            {
                var batch = IndexBatch.Upload(documents);
                await _indexClient.Documents.IndexAsync(batch);
            }
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="id">Identifier (key).</param>
        public void DeleteDocument(string id)
        {
            var batch = IndexBatch.Delete(_indexKey, new List<string> { id });
            _indexClient.Documents.Index(batch);
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="id">Identifier (key).</param>
        public async Task DeleteDocumentAsync(string id)
        {
            var batch = IndexBatch.Delete(_indexKey, new List<string> { id });
            await _indexClient.Documents.IndexAsync(batch);
        }

        /// <summary>
        /// Deletes the documents.
        /// </summary>
        /// <param name="ids">Identifiers (keys).</param>
        public void DeleteDocuments(IEnumerable<string> ids)
        {
            var batch = IndexBatch.Delete(_indexKey, ids);
            _indexClient.Documents.Index(batch);
        }

        /// <summary>
        /// Deletes the documents.
        /// </summary>
        /// <param name="ids">Identifiers (keys).</param>
        public async Task DeleteDocumentsAsync(IEnumerable<string> ids)
        {
            var batch = IndexBatch.Delete(_indexKey, ids);
            await _indexClient.Documents.IndexAsync(batch);
        }

        /// <summary>
        /// Index documents.
        /// </summary>
        /// <param name="documents">The list of documents.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public void IndexDocuments<T>(IList<T> documents, bool merge = true) where T: class
        {
            AsyncTools.RunSync(() => IndexDocumentsAsync(documents, merge));
        }

        /// <summary>
        /// Index documents.
        /// </summary>
        /// <param name="documents">The list of documents.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public async Task IndexDocumentsAsync<T>(IList<T> documents, bool merge = true) where T: class
        {
            var max = documents.Count();

            for (var i = 0; i < max; i += MaxBatchSize)
            {
                var items = new List<ShushuIndex>();

                for (var i2 = 0; i2 < MaxBatchSize && i + i2 < max; i2++)
                {
                    var item = documents[i + i2].MapToIndex();
                    items.Add(item);    
                }

                if (merge)
                {
                    var batch = IndexBatch.MergeOrUpload(items);
                    await _indexClient.Documents.IndexAsync(batch);
                }
                else
                {
                    var batch = IndexBatch.Upload(items);
                    await _indexClient.Documents.IndexAsync(batch);
                }
            }
        }

        /// <summary>
        /// Counts all documents.
        /// </summary>
        /// <returns>The number of all documents in index.</returns>
        public long CountAllDocuments()
        {
            var result = _searchClient.Documents.Count(null);
            return result;
        }

        /// <summary>
        /// Counts all documents.
        /// </summary>
        /// <returns>The number of all documents in index.</returns>
        public async Task<long> CountAllDocumentsAsync()
        {
            var result = await _searchClient.Documents.CountAsync(null);
            return result;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="id">Identifier (key).</param>
        /// <typeparam name="T">The type of object.</typeparam>
        public T GetDocument<T>(string id) where T : class, new()
        {
            return AsyncTools.RunSync(() => GetDocumentAsync<T>(id));
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="key">Key.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        public async Task<T> GetDocumentAsync<T>(string key) where T : class, new()
        {
            var shushu = await _searchClient.Documents.GetAsync<ShushuIndex>(key);

            return shushu.MapFromIndex<T>();
        }

        /// <summary>
        /// Searchs the documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="searchText">Search text.</param>
        /// <param name="searchParameters">Search parameters.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        public DocumentSearchResult<T> SearchDocuments<T>(string searchText, SearchParameters searchParameters) where T : class, new()
        {
            return AsyncTools.RunSync(() => SearchDocumentsAsync<T>(searchText, searchParameters));
        }

        /// <summary>
        /// Searchs the documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="searchText">Search text.</param>
        /// <param name="searchParameters">Search parameters.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        public async Task<DocumentSearchResult<T>> SearchDocumentsAsync<T>(string searchText, SearchParameters searchParameters) where T : class, new()
        {
            var originalSearchResult = await _searchClient.Documents.SearchAsync<ShushuIndex>(searchText, searchParameters.MapSearchParameters<T>());

            var documentSearchResult = new DocumentSearchResult<T>
            {
                 ContinuationToken = originalSearchResult.ContinuationToken,
                 Count = originalSearchResult.Count,
                 Coverage = originalSearchResult.Coverage,
                 Facets = originalSearchResult.Facets,
            };

            if (originalSearchResult != null)
            {
                var searchResults = new List<SearchResult<T>>();

                foreach(var result in originalSearchResult.Results)
                {
                    var newResult = new SearchResult<T>
                    {
                        Highlights = result.Highlights,
                        Score = result.Score,
                        Document = result.Document.MapFromIndex<T>()
                    };

                    searchResults.Add(newResult);
                }

                documentSearchResult.Results = searchResults;
            }

            return documentSearchResult;
        }

        Index CreateIndex()
        {
            var result = _serviceClient.Indexes.List();
            var indexes = result.Indexes;
            
            var definition = new Index
            {
                Name = _index,
                Fields = FieldBuilder.BuildForType<ShushuIndex>()
            };

            return _serviceClient.Indexes.Create(definition);
        }        
    }
}