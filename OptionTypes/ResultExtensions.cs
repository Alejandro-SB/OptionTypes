namespace OptionTypes;
public static class ResultExtensions
{
    /// <summary>
    /// Converts a task into a <see cref="Result{TOk, TErr}"/> using a selector
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TErr"></typeparam>
    /// <param name="task">The task to convert from</param>
    /// <param name="resultSelector">The function that converts the result of the task to <see cref="Result{TOk, TErr}"></see></param>
    /// <returns>A task wrapping the result of the computation</returns>
    public static Task<Result<TOk, TErr>> AsResult<TIn, TOk, TErr>(this Task<TIn> task, Func<TIn, Result<TOk, TErr>> resultSelector)
    {
        return task.ContinueWith(t => resultSelector(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
    }
}
