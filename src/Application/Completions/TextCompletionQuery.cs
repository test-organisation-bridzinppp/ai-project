using MediatR;

namespace Application.Completions
{
    public record TextCompletionQuery(string Prompt) : IRequest<string>;
}
