using DesafioKria.Domain.Entities;

namespace DesafioKria.Domain.Services;

public interface ITollTransactionRulesService
{
    decimal CalculateFareMultiplier(TollTransaction transaction);
}