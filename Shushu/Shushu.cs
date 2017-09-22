using Microsoft.Azure.Search;
using Sushi2;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace Shushu
{
    public class Shushu
    {
        public readonly SearchServiceClient ServiceClient;
        public readonly SearchIndexClient SearchClient;
        public readonly SearchIndexClient IndexClient;

        string _name;
        string _serviceApiKey;
        string _searchApiKey;
        string _index;

        public Shushu(string name, string serviceApiKey, string searchApiKey, string index)
        {
            _name = name;
            _serviceApiKey = serviceApiKey;
            _searchApiKey = searchApiKey;
            _index = index;

            ServiceClient = new SearchServiceClient(name, new SearchCredentials(serviceApiKey));
            SearchClient = new SearchIndexClient(name, index, new SearchCredentials(searchApiKey));
            IndexClient = new SearchIndexClient(name, index, new SearchCredentials(serviceApiKey));

            var ind = ServiceClient.Indexes.List().Indexes.FirstOrDefault(x => x.Name.Equals(index));

            if (ind == null)
                CreateIndex();            
        }
        
        Index CreateIndex()
        {
            var result = ServiceClient.Indexes.List();
            var indexes = result.Indexes;
            
            var definition = new Index
            {
                Name = _index,
                Fields = FieldBuilder.BuildForType<Tokens.AzureSearch>()
            };

            return ServiceClient.Indexes.Create(definition);
        }        

        public void DeleteIndex()
        {
            AsyncTools.RunSync(DeleteIndexAsync);
        }

        public async Task DeleteIndexAsync()
        {
            await ServiceClient.Indexes.DeleteAsync(_index);
        }

        public void IndexDocument(object document)
        {
            AsyncTools.RunSync(() => IndexDocumentAsync(document));
        }

        public async Task IndexDocumentAsync(object document)
        {
            var documents = new List<Tokens.AzureSearch> { document.MapIndex() };
            var batch = IndexBatch.Upload(documents);
            await IndexClient.Documents.IndexAsync(batch);
        }
    }
}
