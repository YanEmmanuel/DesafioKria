using System.Collections.Generic;

namespace DesafioKria.Application.DTOs;

public class ChallengeSubmissionEnvelopeDto
{
    public string Candidato { get; init; } = string.Empty;

    public string DataReferencia { get; init; } = string.Empty;

    public int Motivo { get; init; }

    public string CnpjConcessionaria { get; init; } = string.Empty;

    public int NumeroArquivo { get; init; }

    public List<TollTransactionResponseDto> Registros { get; init; } = new();
}