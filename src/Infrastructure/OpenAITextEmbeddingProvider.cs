using BuildingBlocks.Application.Ports;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace Infrastructure
{

    public class OpenAITextEmbeddingProvider : ITextEmbeddingsProvider
    {
        
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
       
        public OpenAITextEmbeddingProvider(Kernel kernel)
        {
            _textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        }

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma 

        public async Task<IReadOnlyList<float>> GetEmbeddings(string text)
        {
            var result = await _textEmbeddingGenerationService.GenerateEmbeddingAsync("text-embedding-ada-002");
            return result.ToArray();
        }        
    }
}
