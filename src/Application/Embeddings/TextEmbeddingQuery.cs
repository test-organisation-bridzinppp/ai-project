using MediatR;

namespace Application.Embeddings
{
    public record TextEmbeddingQuery(string Text) : IRequest<IReadOnlyList<float>> { }
}