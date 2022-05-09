using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Onion
{
    // TODO: support empty snapshot
    public record EntitySnapshot<TValue>(TValue Value, IEnumerable<DomainEvent> Events) where TValue : class;

    public static class EntitySnapshotExtensions
    {
        public static EntitySnapshot<TValue> ToEntitySnapshot<TValue>(this TValue value,
            params DomainEvent[] events)
            where TValue : class
            => new(value, events);

        public static EntitySnapshot<TTarget> Select<TSource, TTarget>(this EntitySnapshot<TSource> source,
            Func<TSource, TTarget> selector)
            where TSource : class
            where TTarget : class
            => new(selector(source.Value), source.Events);

        public static EntitySnapshot<TTarget> SelectMany<TSource, TCollection, TTarget>(this EntitySnapshot<TSource> source,
            Func<TSource, EntitySnapshot<TCollection>> collectionSelector,
            Func<TSource, TCollection, TTarget> targetSelector)
            where TSource : class
            where TCollection : class
            where TTarget : class
        {
            var collection = collectionSelector(source.Value);
            return new(targetSelector(source.Value, collection.Value),
                source.Events.Concat(collection.Events).ToArray());
        }
    }
}

