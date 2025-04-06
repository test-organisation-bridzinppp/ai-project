using BuildingBlocks.Application.Ports;
using Microsoft.SemanticKernel.Embeddings;

namespace Infrastructure
{
    class OpenAITextEmbeddingProvider : ITextEmbeddingsProvider
    {
        
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
        public OpenAITextEmbeddingProvider(ITextEmbeddingGenerationService textEmbeddingGenerationService)
        {
            _textEmbeddingGenerationService = textEmbeddingGenerationService;
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
