using Funzo.Example.ThirdParty;
using Funzo.Example.UseCases;

namespace Funzo.Example;

internal class Program
{
    static async Task Main(string[] args)
    {
        var paymentProvider = new PaymentProvider();
        var sendPaymentHandler = new SendPaymentHandler(paymentProvider);
        var totalToSend = 125M;
        var request = new PaymentRequest("AccountId", totalToSend);

        var paymentResult = await sendPaymentHandler.Handle(request, CancellationToken.None);

        var response = paymentResult.Match(
            ok: _ => "Payment successful", 
            err: err => err.Match(
                (InvalidAccountError invalidAccount) => $"The account {invalidAccount.AccountId} does not exist",
                (InsufficientFundsError insufficientFunds) => $"Your account has {insufficientFunds.CurrentFunds:C2}. You need {totalToSend - insufficientFunds.CurrentFunds} more in your account",
                (InvalidEffectiveDateError invalidDate) => $"The date {invalidDate.SuppliedDate:o} is not valid")
        );

        Console.WriteLine(response);
    }
}
