using DesafioKria.Application.Interfaces;
using DesafioKria.Application.UseCases;
using DesafioKria.Domain.Interfaces;
using DesafioKria.Domain.Services;
using DesafioKria.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ITollTransactionRulesService, TollTransactionRulesService>();

builder.Services.AddScoped<ITollTransactionRepository, TollTransactionRepository>();

builder.Services.AddHttpClient<IChallengeSubmissionUseCase, ChallengeSubmissionUseCase>();

var app = builder.Build();

app.MapControllers();

app.Run();
