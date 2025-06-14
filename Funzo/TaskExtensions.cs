namespace Funzo;
internal static class TaskExtensions
{
    internal static Task<TOut> Then<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> func)
        => task.ContinueWith(t => func(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
}
