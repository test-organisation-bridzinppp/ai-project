using MediatR;

namespace Application.EmbeddingsComputing
{
    public record ComputeEmbeddingsCommand : IRequest<int>
    {
    }
}
