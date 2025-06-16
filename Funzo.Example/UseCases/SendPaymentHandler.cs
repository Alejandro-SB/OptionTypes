using Funzo.Example.ThirdParty;

namespace Funzo.Example.UseCases;

internal class SendPaymentHandler
{
    private readonly PaymentProvider _paymentProvider;

    public SendPaymentHandler(PaymentProvider paymentProvider)
    {
        _paymentProvider = paymentProvider;
    }

    public Task<SendPaymentResult> Handle(PaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        return _paymentProvider.Pay(paymentRequest, cancellationToken);
    }
}

[Result]
public partial class SendPaymentResult : Result<PaymentError>;