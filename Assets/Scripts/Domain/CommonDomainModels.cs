using System;
using System.Collections.Generic;
using System.Linq;

public record DomainEvent;

// TODO: support failure results
public record Result<T>(T Value)
{
    public IEnumerable<DomainEvent> Events { get; init; } = new List<DomainEvent>();
}

public static class Result
{
    public static Result<T> Success<T>(T value) => new(value);

    public static Result<T> WithEvent<T>(this Result<T> result, DomainEvent @event) =>
        result with
        {
            Events = result.Events.Append(@event)
        };

    public static Result<TResult> Select<TSource, TResult>(this Result<TSource> source, Func<TSource, TResult> selector)
    {
        var value = selector(source.Value);
        return new Result<TResult>(value) { Events = source.Events };
    }

    public static Result<TResult> SelectMany<TSource, TCollection, TResult>(this Result<TSource> source,
        Func<TSource, Result<TCollection>> collectionSelector,
        Func<TSource, TCollection, TResult> resultSelector)
    {
        var result = collectionSelector(source.Value);
        var value = resultSelector(source.Value, result.Value);
        return new Result<TResult>(value) { Events = source.Events.Concat(result.Events) };
    }
}

