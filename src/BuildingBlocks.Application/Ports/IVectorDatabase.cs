
namespace Infrastructure
{
    public interface IVectorDatabase
    {
        Task SaveDocument(string pageContent, string pagenNumber, IReadOnlyCollection<float> embeddgins);
        Task<List<string>> VectorSearch(IReadOnlyList<float> queryEmbeddings);
    }
}