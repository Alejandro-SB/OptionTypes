using Funzo.Example.UseCases;

namespace Funzo.Example.ThirdParty;
internal interface IPaymentProvider
{
    Task<SendPaymentResult> Pay(PaymentRequest paymentRequest, CancellationToken cancellationToken);
}
