using Application.Completions;
using Application.Embeddings;
using Application.EmbeddingsComputing;
using Application.QueringModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ai_api.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IMediator _mediator;
        public TestController(ILogger<TestController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test(string prompt)
        {
            var response = await _mediator.Send(new TextCompletionQuery(prompt));
            return Ok(response);
        }

        [HttpGet("embeddgins")]
        public async Task<IActionResult> GetEmbeddings(string text)
        {
            var response = await _mediator.Send(new TextEmbeddingQuery(text));
            return Ok(response);
        }

        [HttpPost("/embeddings")]
        public async Task<ActionResult> GenerateEmbeddings()
        {
            var response = await _mediator.Send(new ComputeEmbeddingsCommand());
            return Ok(response);
        }

        [HttpGet("/rag-resources")]
        public async Task<IActionResult> Get(string prompt)
        {
            var response = await _mediator.Send(new QueringModelRequest(prompt));
            return Ok(response);
        }
    }
}
