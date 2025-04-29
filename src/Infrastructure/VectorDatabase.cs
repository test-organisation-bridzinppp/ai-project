using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace Infrastructure
{
    public class VectorDatabase : IVectorDatabase
    {
        private readonly SearchClient _searchClient;

        public VectorDatabase(Uri endpoint, AzureKeyCredential key)
        {
            _searchClient = new SearchClient(endpoint, "pdf-pages", key);
        }

        public async Task SaveDocument(string pageContent, string pagenNumber, IReadOnlyCollection<float> embeddgins)
        {
            var document = new Dictionary<string, object>
            {
                { "content", pageContent },
                { "pageNumber", pagenNumber },
                { "embeddings", embeddgins }
            };
            await _searchClient.UploadDocumentsAsync(new[] { document });
        }
        
        public async Task<List<string>> VectorSearch(IReadOnlyList<float> queryEmbeddings)
        {
            ReadOnlyMemory<float> query = queryEmbeddings.ToArray();
            var searchOptions = new SearchOptions
            {
                VectorSearch = new()
                {
                    Queries = { new VectorizedQuery(query)
                    {
                        KNearestNeighborsCount = 5,
                        Fields = { "embeddings" }
                    }
                }
                }
            };

            var searchResults = await _searchClient.SearchAsync<string>(searchOptions);
            return searchResults.Value.GetResults().Select(r => r.Document.ToString()).ToList();
        }
    }
}
