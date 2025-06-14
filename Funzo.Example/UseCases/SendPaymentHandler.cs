using Funzo.Example.ThirdParty;

namespace Funzo.Example.UseCases;

internal class SendPaymentHandler
{
    private readonly IPaymentProvider _paymentProvider;

    public SendPaymentHandler(IPaymentProvider paymentProvider)
    {
        _paymentProvider = paymentProvider;
    }

    public Task<SendPaymentResult> Handle(PaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        return _paymentProvider.Pay(paymentRequest, cancellationToken);
    }
}

public class SendPaymentResult : Result<Unit, PaymentError>
{
    public SendPaymentResult(Unit ok) : base(ok)
    {
    }

    public SendPaymentResult(PaymentError err) : base(err)
    {
    }

    public new static SendPaymentResult Ok() => new(Unit.Default);
    public new static SendPaymentResult Err(PaymentError err) => new(err);

}