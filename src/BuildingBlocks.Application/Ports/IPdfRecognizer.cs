namespace BuildingBlocks.Application.Ports
{
    public interface IPdfRecognizer
    {
        Task<RecognizedDocument> Recognize(byte[] content);
    }

    public record RecognizedDocument(IEnumerable<string> Pages);
}
