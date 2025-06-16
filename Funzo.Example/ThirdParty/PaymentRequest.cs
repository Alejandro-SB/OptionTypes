namespace Funzo.Example.ThirdParty;

public record PaymentRequest(string DestinationAccountId, decimal Amount)
{
    public Option<DateTimeOffset> EffectiveDate { get; set; } = Option<DateTimeOffset>.None();
}

[Union]
public partial class PaymentError : Union<InvalidAccountError, InsufficientFundsError, InvalidEffectiveDateError>;

public record InvalidAccountError(string AccountId);
public record InsufficientFundsError(decimal CurrentFunds);
public record InvalidEffectiveDateError(DateTimeOffset SuppliedDate);