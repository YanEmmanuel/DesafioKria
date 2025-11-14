using DesafioKria.Domain.Entities;
using DesafioKria.Domain.Services;
using Xunit;

namespace DesafioKria.Tests.Services;

public class TollTransactionRulesServiceTests
{
    private readonly TollTransactionRulesService _service = new();

    [Theory]
    [InlineData(3, 1, 0.0)]
    [InlineData(3, 2, 0.5)]
    [InlineData(1, 0, 1.0)]
    [InlineData(2, 0, 2.0)]
    [InlineData(4, 0, 1.0)]
    public void CalculateFareMultiplier_ReturnsExpectedValue(int vehicleType, int exemptionIndicator, double expectedMultiplier)
    {
        var transaction = new TollTransaction
        {
            VehicleTypeCode = vehicleType,
            ExemptionIndicator = exemptionIndicator
        };

        var multiplier = (double)_service.CalculateFareMultiplier(transaction);

        Assert.Equal(expectedMultiplier, multiplier);
    }
}
