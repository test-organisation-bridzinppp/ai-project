namespace BuildingBlocks.Application.Ports
{
    public interface IChatCompletionProvider
    {
        Task SetNewContext(string prompt);
        Task<string> CompleteChatAsync(string prompt);
    }
}
