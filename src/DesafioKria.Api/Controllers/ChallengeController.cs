using System.Threading.Tasks;
using DesafioKria.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DesafioKria.Api.Controllers;

[ApiController]
[Route("challenge")]
public class ChallengeController : ControllerBase
{
    private readonly IChallengeSubmissionUseCase _challengeSubmissionUseCase;

    public ChallengeController(IChallengeSubmissionUseCase challengeSubmissionUseCase)
    {
        _challengeSubmissionUseCase = challengeSubmissionUseCase;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteAsync()
    {
        var response = await _challengeSubmissionUseCase.ExecuteAsync();
        return Ok(response);
    }
}