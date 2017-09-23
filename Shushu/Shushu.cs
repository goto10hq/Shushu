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
        SearchServiceClient _serviceClient;
        SearchIndexClient _searchClient;
        SearchIndexClient _indexClient;

        string _index;

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

            var ind = _serviceClient.Indexes.List().Indexes.FirstOrDefault(x => x.Name.Equals(index));

            if (ind == null)
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
        /// Indexs the document.
        /// </summary>
        /// <param name="document">Document.</param>
        public void IndexDocument<T>(T document) where T: class
        {
            AsyncTools.RunSync(() => IndexDocumentAsync(document));
        }

        /// <summary>
        /// Indexs the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="document">Document.</param>
        public async Task IndexDocumentAsync<T>(T document) where T: class
        {
            var documents = new List<AzureSearch> { document.MapIndex() };
            var batch = IndexBatch.Upload(documents);
            await _indexClient.Documents.IndexAsync(batch);
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
