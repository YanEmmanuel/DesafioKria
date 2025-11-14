using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using DesafioKria.Application.UseCases;
using DesafioKria.Domain.Entities;
using DesafioKria.Domain.Interfaces;
using DesafioKria.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesafioKria.Tests.UseCases;

public class ChallengeSubmissionUseCaseTests
{
    private readonly Mock<ITollTransactionRepository> _repositoryMock = new();
    private readonly Mock<ITollTransactionRulesService> _rulesServiceMock = new();
    private readonly Mock<ILogger<ChallengeSubmissionUseCase>> _loggerMock = new();

    [Fact]
    public async Task ExecuteAsync_PostsTransformedPayloadAndReturnsSuccessMessage()
    {
        var transaction = new TollTransaction
        {
            TollPlazaCode = "1001",
            TollBoothCode = 5,
            EventTimestamp = "2024-01-01T00:00:00Z",
            DirectionCode = 1,
            VehicleTypeCode = 2,
            ExemptionIndicator = 0,
            EvasionIndicator = 1,
            ChargeTypeCode = 2,
            AmountDue = 25.5m,
            AmountCollected = 25.5m
        };

        _repositoryMock
            .Setup(r => r.GetTransactionsAsync(int.MaxValue))
            .ReturnsAsync(new List<TollTransaction> { transaction });

        _rulesServiceMock
            .Setup(s => s.CalculateFareMultiplier(transaction))
            .Returns(2m);

        var handler = new StubHttpMessageHandler(HttpStatusCode.OK);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test")
        };

        var useCase = new ChallengeSubmissionUseCase(
            _repositoryMock.Object,
            _rulesServiceMock.Object,
            httpClient,
            _loggerMock.Object);

        var result = await useCase.ExecuteAsync();

        Assert.Equal("Challenge submission completed successfully.", result);
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal(
            "https://contratacaosirapi.azurewebsites.net/Candidato/PublicarDesafio",
            handler.LastRequest.RequestUri?.ToString());

        Assert.False(string.IsNullOrWhiteSpace(handler.LastPayload));
        using var document = JsonDocument.Parse(handler.LastPayload!);
        var root = document.RootElement;

    Assert.Equal("Yan Emmanuel Costa Soares", root.GetProperty("candidato").GetString());
    Assert.Equal(1, root.GetProperty("numeroArquivo").GetInt32());
    Assert.Equal(1, root.GetProperty("registros").GetArrayLength());

    var record = root.GetProperty("registros")[0];
    Assert.Equal("1001", record.GetProperty("codigoPracaPedagio").GetString());
    Assert.Equal("5", record.GetProperty("codigoCabine").GetString());
    Assert.Equal("Crescente", record.GetProperty("sentido").GetString());
    Assert.Equal("Comercial", record.GetProperty("tipoVeiculo").GetString());
    Assert.Equal("Não", record.GetProperty("isento").GetString());
    Assert.Equal("Sim", record.GetProperty("evasao").GetString());
    Assert.Equal("TAG", record.GetProperty("tipoCobrancaEfetuada").GetString());

    var amountDueRaw = record.GetProperty("valorDevido").GetString();
    Assert.True(decimal.TryParse(amountDueRaw, NumberStyles.Number, CultureInfo.CurrentCulture, out var amountDue));
    Assert.Equal(25.5m, amountDue);

    var amountCollectedRaw = record.GetProperty("valorArrecadado").GetString();
    Assert.True(decimal.TryParse(amountCollectedRaw, NumberStyles.Number, CultureInfo.CurrentCulture, out var amountCollected));
    Assert.Equal(25.5m, amountCollected);
    Assert.Equal("2", record.GetProperty("multiplicadorTarifa").GetString());
    Assert.False(string.IsNullOrWhiteSpace(record.GetProperty("guid").GetString()));
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;

        public StubHttpMessageHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastPayload { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;

            if (request.Content is not null)
            {
                LastPayload = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent("ok")
            };
        }
    }
}
