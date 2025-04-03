namespace BuildingBlocks.Application.Ports
{
    public interface ITextEmbeddingsProvider
    {
        IReadOnlyList<float> GetEmbeddings(string text);
    }
}
