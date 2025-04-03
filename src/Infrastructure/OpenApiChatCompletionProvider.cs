using BuildingBlocks.Application.Ports;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Infrastructure
{
    public class OpenApiChatCompletionProvider : IChatCompletionProvider
    {        
        private readonly IChatCompletionService _chatCompletionService;
        private ChatHistory _chatHistory = new ChatHistory();
        private readonly Kernel _kernel;

        public OpenApiChatCompletionProvider(Kernel kernel)
        {
            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            _kernel = kernel;
        }
        public async Task<string> CompleteChatAsync(string prompt)
        {            
            _chatHistory.AddUserMessage(prompt);
            var response = await _chatCompletionService.GetChatMessageContentAsync(
                chatHistory: _chatHistory,                
                kernel: _kernel);

            if (response.Content == null)
            {
                return string.Empty;
            }
            _chatHistory.AddAssistantMessage(response.Content);
            return response.Content;
        }

        public Task SetNewContext(string prompt)
        {            
            _chatHistory.Clear();
            _chatHistory.AddSystemMessage(prompt);
            return Task.CompletedTask;
        }
    }
}
