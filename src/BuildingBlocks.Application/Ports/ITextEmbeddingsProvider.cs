namespace BuildingBlocks.Application.Ports
{
    public interface ITextEmbeddingsProvider
    {
        Task<IReadOnlyList<float>> GetEmbeddings(string text);
    }
}
