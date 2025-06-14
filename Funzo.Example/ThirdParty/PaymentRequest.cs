namespace Funzo.Example.ThirdParty;

public record PaymentRequest(string DestinationAccountId, decimal Amount)
{
    public Option<DateTimeOffset> EffectiveDate { get; set; } = Option<DateTimeOffset>.None();
}

public class PaymentError : Union<InvalidAccountError, InsufficientFundsError, InvalidEffectiveDateError>
{
    public PaymentError(InvalidAccountError value) : base(value)
    {
    }

    public PaymentError(InsufficientFundsError value) : base(value)
    {
    }

    public PaymentError(InvalidEffectiveDateError value) : base(value)
    {
    }
}

public record InvalidAccountError(string AccountId);
public record InsufficientFundsError(decimal CurrentFunds);
public record InvalidEffectiveDateError(DateTimeOffset SuppliedDate);