using Application.Completions;
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
    }
}
