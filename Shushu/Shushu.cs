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
        const int BatchSize = 1000;

        SearchServiceClient _serviceClient;
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
            var documents = new List<AzureSearch> { document.MapToIndex() };

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
        /// Index documents.
        /// </summary>
        /// <param name="documents">The collection of documents.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public void IndexDocuments<T>(IEnumerable<T> documents, bool merge = true) where T: class
        {
            AsyncTools.RunSync(() => IndexDocumentAsync(documents, merge));
        }

        /// <summary>
        /// Index documents.
        /// </summary>
        /// <param name="documents">The collection of documents.</param>
        /// <param name="merge">If set to <c>true</c> merge otherwise just upload.</param>
        /// <typeparam name="T">The document type.</typeparam>
        public async Task IndexDocumentsAsync<T>(IEnumerable<T> documents, bool merge = true) where T: class
        {
            var chunks = documents.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / BatchSize).Select(x => x.Select(v => v.Value.MapToIndex()).ToList()).ToList();

            foreach(var chunk in chunks)
            {
                if (merge)
                {
                    var batch = IndexBatch.MergeOrUpload(chunk);
                    await _indexClient.Documents.IndexAsync(batch);
                }
                else
                {
                    var batch = IndexBatch.MergeOrUpload(chunk);
                    await _indexClient.Documents.IndexAsync(batch);
                }
            }
        }

        Index CreateIndex()
        {
            var result = _serviceClient.Indexes.List();
            var indexes = result.Indexes;
            
            var definition = new Index
            {
                Name = _index,
                Fields = FieldBuilder.BuildForType<AzureSearch>()
            };

            return _serviceClient.Indexes.Create(definition);
        }        
    }
}
