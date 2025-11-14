namespace DesafioKria.Application.Interfaces;

public interface IChallengeSubmissionUseCase
{
    Task<string> ExecuteAsync();
}