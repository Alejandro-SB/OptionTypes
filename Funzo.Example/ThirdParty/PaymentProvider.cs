using Funzo.Example.UseCases;

namespace Funzo.Example.ThirdParty;

public class PaymentProvider
{
    private decimal _balance = 5000M;
    private readonly string[] _validAccounts = ["1", "2", "3"];

    public async Task<SendPaymentResult> Pay(PaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        if(!_validAccounts.Contains(paymentRequest.DestinationAccountId))
        {
            return SendPaymentResult.Err(new InvalidAccountError(paymentRequest.DestinationAccountId));
        }

        if (_balance < paymentRequest.Amount)
        {
            return SendPaymentResult.Err(new InsufficientFundsError(_balance));
        }

        var now = DateTimeOffset.UtcNow;
        var nextMonth = now.AddMonths(1);

        var effectiveDate = paymentRequest.EffectiveDate.ValueOr(now);

        if(effectiveDate > nextMonth)
        {
            return SendPaymentResult.Err(new InvalidEffectiveDateError(effectiveDate));
        }

        _balance -= paymentRequest.Amount;

        return SendPaymentResult.Ok();
    }
}
