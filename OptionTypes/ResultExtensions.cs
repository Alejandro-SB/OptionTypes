namespace OptionTypes;

/// <summary>
/// Extensions for result type
/// </summary>
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
        return task.Then(resultSelector);
    }

    /// <summary>
    /// Converts the result of a task to a <see cref="Maybe{T}"/>
    /// </summary>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TErr"></typeparam>
    /// <param name="task">The task to extract the result from</param>
    /// <returns>A task wrapping the result</returns>
    public static Task<Maybe<TOk>> Ok<TOk, TErr>(this Task<Result<TOk, TErr>> task)
    {
        return task.Then(t => t.Ok());
    }

    /// <summary>
    /// Applies a predicate to the result of a task
    /// </summary>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TErr"></typeparam>
    /// <param name="task">The task to create the continuation on</param>
    /// <param name="ok">The action to execute if the result is Ok</param>
    /// <param name="fail">The action to execute if the result fails</param>
    /// <returns>A wrapper around the original task</returns>
    public static Task Match<TOk, TErr>(this Task<Result<TOk, TErr>> task, Action<TOk> ok, Action<TErr> fail)
    {
        return task.Then(t => t.Match(ok, fail));
    }
}
