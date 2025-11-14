using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DesafioKria.Application.DTOs;
using DesafioKria.Application.Interfaces;
using DesafioKria.Domain.Entities;
using DesafioKria.Domain.Interfaces;
using DesafioKria.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DesafioKria.Application.UseCases;

public class ChallengeSubmissionUseCase : IChallengeSubmissionUseCase
{
    private const int MaximumRecordsPerFile = 1000;

    private readonly ITollTransactionRepository _transactionRepository;
    private readonly ITollTransactionRulesService _transactionRulesService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChallengeSubmissionUseCase> _logger;

    public ChallengeSubmissionUseCase(
        ITollTransactionRepository transactionRepository,
        ITollTransactionRulesService transactionRulesService,
        HttpClient httpClient,
        ILogger<ChallengeSubmissionUseCase> logger)
    {
        _transactionRepository = transactionRepository;
        _transactionRulesService = transactionRulesService;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync()
    {
        var transactions = await _transactionRepository.GetTransactionsAsync(int.MaxValue);
        var baseAddress = _httpClient.BaseAddress?.ToString() ?? "not configured";
        // _logger.LogInformation(
        //     "Preparing to process {TotalTransactions} transactions. Max records per file: {MaxRecordsPerFile}. HttpClient base address: {BaseAddress}.",
        //     transactions.Count,
        //     MaximumRecordsPerFile,
        //     baseAddress);

        foreach (var transaction in transactions)
        {
            transaction.FareMultiplier = _transactionRulesService.CalculateFareMultiplier(transaction);
        }

        var fileSequence = 1;
        for (var index = 0; index < transactions.Count; index += MaximumRecordsPerFile)
        {
            var transactionBatch = transactions.Skip(index).Take(MaximumRecordsPerFile).ToList();
            var records = transactionBatch.Select(CreateResponseRecord).ToList();
            var envelope = CreateEnvelope(records, fileSequence);

            var serializedPayload = JsonSerializer.Serialize(
                envelope,
                new JsonSerializerOptions { WriteIndented = true });

            // _logger.LogInformation("Payload sent to the API:\n{Payload}", serializedPayload);

            var response = await _httpClient.PostAsJsonAsync(
                        "https://contratacaosirapi.azurewebsites.net/Candidato/PublicarDesafio",
                        envelope);
            fileSequence++;
            if (!response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return "Challenge submission completed successfully.";
    }

    private static TollTransactionResponseDto CreateResponseRecord(TollTransaction transaction)
    {
        return new TollTransactionResponseDto
        {
            GUID = Guid.NewGuid().ToString(),
            CodigoPracaPedagio = ParseInteger(transaction.TollPlazaCode).ToString(),
            CodigoCabine = transaction.TollBoothCode.ToString(),
            Instante = transaction.EventTimestamp,
            Sentido = MapDirection(transaction.DirectionCode),
            TipoVeiculo = MapVehicleType(transaction.VehicleTypeCode),
            Isento = transaction.ExemptionIndicator == 1 ? "Sim" : "Não",
            Evasao = IsAffirmative(transaction.EvasionIndicator) ? "Sim" : "Não",
            TipoCobrancaEfetuada = MapChargeType(transaction.ChargeTypeCode),
            ValorDevido = transaction.AmountDue.ToString(),
            ValorArrecadado = transaction.AmountCollected.ToString() ?? "0",
            MultiplicadorTarifa = transaction.FareMultiplier.ToString() ?? "1"
        };
    }

    private static ChallengeSubmissionEnvelopeDto CreateEnvelope(List<TollTransactionResponseDto> records, int fileSequence)
    {
        return new ChallengeSubmissionEnvelopeDto
        {
            Candidato = "Yan Emmanuel Costa Soares",
            DataReferencia = DateTime.Now.ToString("dd/MM/yyyy"),
            Motivo = 1,
            CnpjConcessionaria = "12345678000190",
            NumeroArquivo = fileSequence,
            Registros = records
        };
    }

    private static string MapDirection(int directionValue)
    {
        return directionValue switch
        {
            1 => "Crescente",
            2 => "Decrescente",
            _ => "Unknown"
        };
    }

    private static string MapVehicleType(int vehicleType)
    {
        return vehicleType switch
        {
            1 => "Passeio",
            2 => "Comercial",
            3 => "Moto",
            _ => "Unknown"
        };
    }

    private static bool IsAffirmative(int value)
    {
        return value == 1;
    }

    private static string MapChargeType(int paymentType)
    {
        return paymentType switch
        {
            2 => "TAG",
            3 => "OCR/Placa",
            _ => "Manual"
        };
    }

    private static int ParseInteger(string numericText)
    {
        return int.TryParse(numericText, out var value) ? value : 0;
    }
}