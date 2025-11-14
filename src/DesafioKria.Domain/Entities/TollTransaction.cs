using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DesafioKria.Domain.Entities;

public class TollTransaction
{
    [BsonId]
    public ObjectId DocumentId { get; set; }

    [BsonElement("IdTransacao")]
    public int TransactionSequenceId { get; set; }

    [BsonElement("DtCriacao")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("CodigoPracaPedagio")]
    public string TollPlazaCode { get; set; } = string.Empty;

    [BsonElement("CodigoCabine")]
    public int TollBoothCode { get; set; }

    [BsonElement("Instante")]
    public string EventTimestamp { get; set; } = string.Empty;

    [BsonElement("Sentido")]
    public int DirectionCode { get; set; }

    [BsonElement("QuantidadeEixosVeiculo")]
    public int VehicleAxleCount { get; set; }

    [BsonElement("Rodagem")]
    public int WheelConfigurationCode { get; set; }

    [BsonElement("Isento")]
    public int ExemptionIndicator { get; set; }

    [BsonElement("MotivoIsencao")]
    public int ExemptionReasonCode { get; set; }

    [BsonElement("Evasao")]
    public int EvasionIndicator { get; set; }

    [BsonElement("EixoSuspenso")]
    public int SuspendedAxleIndicator { get; set; }

    [BsonElement("QuantidadeEixosSuspensos")]
    public int SuspendedAxleCount { get; set; }

    [BsonElement("TipoCobranca")]
    public int ChargeTypeCode { get; set; }

    [BsonElement("Placa")]
    public string LicensePlate { get; set; } = string.Empty;

    [BsonElement("LiberacaoCancela")]
    public int BarrierReleaseIndicator { get; set; }

    [BsonElement("ValorDevido")]
    public decimal AmountDue { get; set; }

    [BsonElement("ValorArrecadado")]
    public decimal? AmountCollected { get; set; }

    [BsonElement("CnpjAmap")]
    public string AmapCompanyTaxId { get; set; } = string.Empty;

    [BsonElement("MultiplicadorTarifa")]
    public decimal? FareMultiplier { get; set; }

    [BsonElement("VeiculoCarregado")]
    public int VehicleLoadIndicator { get; set; }

    [BsonElement("IdTag")]
    public string TagIdentifier { get; set; } = string.Empty;

    [BsonElement("TipoVeiculo")]
    public int VehicleTypeCode { get; set; }
}
