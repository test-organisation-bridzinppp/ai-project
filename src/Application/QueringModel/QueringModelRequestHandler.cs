using BuildingBlocks.Application.Ports;
using Infrastructure;
using MediatR;
using System.Text;

namespace Application.QueringModel
{
    public class QueringModelRequestHandler : IRequestHandler<QueringModelRequest, string>
    {
        private readonly ITextEmbeddingsProvider _textEmbeddingsProvider;
        private readonly IVectorDatabase _vectorDatabase;
        private readonly IChatCompletionProvider _chatCompletionProvider;

        public QueringModelRequestHandler(ITextEmbeddingsProvider textEmbeddingsProvider, IVectorDatabase vectorDatabase, IChatCompletionProvider chatCompletionProvider)
        {
            _chatCompletionProvider = chatCompletionProvider;
            _textEmbeddingsProvider = textEmbeddingsProvider;
            _vectorDatabase = vectorDatabase;
        }

        public async Task<string> Handle(QueringModelRequest request, CancellationToken cancellationToken)
        {
            var query = request.Query;
            var embedding = await _textEmbeddingsProvider.GetEmbeddings(query);
            var vectorSearchResults = await _vectorDatabase.VectorSearch(embedding);
            var sb = new StringBuilder();
            
            foreach(var document in vectorSearchResults.Take(3))
            {
                sb.Append(document);
            }
            var prompt = sb.ToString();
            await _chatCompletionProvider.SetNewContext("You are a helpful assistant. Answer the question based on the context provided.");
            return await _chatCompletionProvider.CompleteChatAsync(prompt);
        }
    }
}
