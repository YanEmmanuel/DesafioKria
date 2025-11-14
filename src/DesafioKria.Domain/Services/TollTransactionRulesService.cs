using DesafioKria.Domain.Entities;

namespace DesafioKria.Domain.Services;

public class TollTransactionRulesService : ITollTransactionRulesService
{
    public decimal CalculateFareMultiplier(TollTransaction transaction)
    {
        if (transaction.VehicleTypeCode == 3 && transaction.ExemptionIndicator == 1)
        {
            return 0m;
        }

        if (transaction.VehicleTypeCode == 3 && transaction.ExemptionIndicator == 2)
        {
            return 0.5m;
        }

        if (transaction.VehicleTypeCode == 1)
        {
            return 1.0m;
        }

        if (transaction.VehicleTypeCode == 2)
        {
            return 2.0m;
        }

        return 1.0m;
    }
}
