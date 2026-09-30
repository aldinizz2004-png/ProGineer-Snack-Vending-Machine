using SnackVendingMachine.Exceptions;
using SnackVendingMachine.Payment;

namespace SnackVendingMachine.Tests;

public class CashVaultTests
{
	[Fact]
	public void Deposit_InvalidDenomination_ThrowsVendingException()
	{
		CashVault cashVault = new CashVault();

		Assert.Throws<VendingException>(() => cashVault.Deposit(0.25m, 1));
	}

	[Fact]
	public void MakeChange_AvailableAmount_ReturnsUsedDenominations()
	{
		CashVault cashVault = new CashVault();

		Dictionary<decimal, int> result = cashVault.MakeChange(0.50m);

		Assert.Equal(1, result[0.50m]);
	}

	[Fact]
	public void MakeChange_UnavailableAmount_ThrowsWithoutRemovingAvailableCash()
	{
		CashVault cashVault = new CashVault();

		Assert.Throws<VendingException>(() => cashVault.MakeChange(0.05m));
		Assert.True(cashVault.CanMakeChange(0.10m));
	}

	[Fact]
	public void MakeChange_GreedyChoiceWouldFail_FindsExactCombination()
	{
		CashVault cashVault = new CashVault();

		for (int i = 0; i < 10; i++)
		{
			cashVault.MakeChange(0.10m);
		}

		Dictionary<decimal, int> result = cashVault.MakeChange(0.60m);

		Assert.Equal(3, result[0.20m]);
		Assert.False(result.ContainsKey(0.50m));
	}
}
