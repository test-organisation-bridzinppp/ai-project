using MediatR;

namespace Application.QueringModel
{
    public record QueringModelRequest(string Query) : IRequest<string>;
}
