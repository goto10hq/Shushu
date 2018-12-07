using Microsoft.Azure.Search;
using Sushi2;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using Shushu.Tokens;
using Momo.Tokens;
using System;

namespace Shushu
{
    /// <summary>
    /// Shushu.
    /// </summary>
    public class Shushu : IDisposable
    {
        bool _disposed;

        /// <summary>
        /// The size of the batch.
        /// </summary>
        /// <remarks>Note that you can only include up to 1000 documents (or 16 MB) in a single indexing request</remarks>
        const int MaxBatchSize = 1000;

        string _indexKey = Enums.IndexField.Id.ToString().ToCamelCase();
        SearchServiceClient _serviceClient;
        SearchIndexClient _searchClient;
        SearchIndexClient _indexClient;
        string _index;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="serviceApiKey">Service API key.</param>
        /// <param name="searchApiKey">Search API key.</param>
        /// <param name="index">Index.</param>
        public Shushu(string name, string serviceApiKey, string searchApiKey, string index)
        {
            if (name == null)
                throw new System.ArgumentNullException(nameof(name));

            if (serviceApiKey == null)
                throw new System.ArgumentNullException(nameof(serviceApiKey));

            if (searchApiKey == null)
                throw new System.ArgumentNullException(nameof(searchApiKey));

            if (index == null)
                throw new System.ArgumentNullException(nameof(index));

            InitClients(new ShushuConfiguration(name, serviceApiKey, serviceApiKey, index));
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="shushuConfiguration">Shushu configuration</param>
        public Shushu(IShushuConfiguration shushuConfiguration)
        {
            if (shushuConfiguration == null)
                throw new System.ArgumentNullException(nameof(shushuConfiguration));

            InitClients(shushuConfiguration);
        }

        void InitClients(IShushuConfiguration shushuConfiguration)
        {
            _index = shushuConfiguration.Index;
            _serviceClient = new SearchServiceClient(shushuConfiguration.Name, new SearchCredentials(shushuConfiguration.ServiceApiKey));
            _searchClient = new SearchIndexClient(shushuConfiguration.Name, shushuConfiguration.Index, new SearchCredentials(shushuConfiguration.SearchApiKey));
            _indexClient = new SearchIndexClient(shushuConfiguration.Name, shushuConfiguration.Index, new SearchCredentials(shushuConfiguration.ServiceApiKey));

            var idx = _serviceClient.Indexes.List().Indexes.FirstOrDefault(x => x.Name.Equals(shushuConfiguration.Index));

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
            await _serviceClient.Indexes.DeleteAsync(_index).ConfigureAwait(false);
        }

        /// <summary>
        /// Index document.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public void IndexDocument<T>(T document, bool merge = true) where T : class
        {
            AsyncTools.RunSync(() => IndexDocumentAsync(document, merge));
        }

        /// <summary>
        /// Index document.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public async Task IndexDocumentAsync<T>(T document, bool merge = true) where T : class
        {
            var documents = new List<ShushuIndex> { document.MapToIndex() };

            if (merge)
            {
                var batch = IndexBatch.MergeOrUpload(documents);
                await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
            }
            else
            {
                var batch = IndexBatch.Upload(documents);
                await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
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
            await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
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
            await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
        }

        /// <summary>
        /// Index documents.
        /// </summary>
        /// <param name="documents">The list of documents.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public void IndexDocuments<T>(IList<T> documents, bool merge = true) where T : class
        {
            AsyncTools.RunSync(() => IndexDocumentsAsync(documents, merge));
        }

        /// <summary>
        /// Index documents.
        /// </summary>
        /// <param name="documents">The list of documents.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public async Task IndexDocumentsAsync<T>(IList<T> documents, bool merge = true) where T : class
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
                    await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
                }
                else
                {
                    var batch = IndexBatch.Upload(items);
                    await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
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
            var result = await _searchClient.Documents.CountAsync(null).ConfigureAwait(false);
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
            var shushu = await _searchClient.Documents.GetAsync<ShushuIndex>(key).ConfigureAwait(false);

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
            var originalSearchResult = await _searchClient.Documents.SearchAsync<ShushuIndex>(searchText, searchParameters.MapSearchParameters<T>()).ConfigureAwait(false);

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

                foreach (var result in originalSearchResult.Results)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _serviceClient.Dispose();
                _serviceClient = null;

                _searchClient.Dispose();
                _searchClient = null;

                _indexClient.Dispose();
                _indexClient = null;
            }

            _disposed = true;
        }

        ~Shushu()
        {
            Dispose(false);
        }
    }
}