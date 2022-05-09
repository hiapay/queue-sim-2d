using System;
using System.Reactive.Subjects;

public class Entity<TValue>
    where TValue : class
{
    public TValue Value => ValueSource.Value;

    public IObservable<TValue> ValueStream => ValueSource;
    private BehaviorSubject<TValue> ValueSource { get; } = new(null);

    public IObservable<DomainEvent> EventStream => EventSource;
    private ReplaySubject<DomainEvent> EventSource { get; } = new();

    public void Save(EntitySnapshot<TValue> snapshot)
    {
        ValueSource.OnNext(snapshot.Value);
        foreach (var @event in snapshot.Events)
        {
            EventSource.OnNext(@event);
        }
    }
}

public static class EntityExtensions
{
    public static EntitySnapshot<TTarget> SelectMany<TSource, TCollection, TTarget>(this Entity<TSource> source,
        Func<TSource, EntitySnapshot<TCollection>> collectionSelector,
        Func<TSource, TCollection, TTarget> targetSelector)
        where TSource : class
        where TCollection : class
        where TTarget : class
    {
        var collection = collectionSelector(source.Value);
        return new(targetSelector(source.Value, collection.Value), collection.Events);
    }
}

