using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Onion
{
    public record EntitySnapshot<TValue>(TValue Value, IEnumerable<DomainEvent> Events) where TValue : class
    {
        public static EntitySnapshot<TValue> Empty => new EmptyEntitySnapshot<TValue>();
    }
    internal record EmptyEntitySnapshot<TValue>() : EntitySnapshot<TValue>(null, null) where TValue : class;

    public static class EntitySnapshotExtensions
    {
        public static EntitySnapshot<TValue> ToEntitySnapshot<TValue>(this TValue value,
            params DomainEvent[] events)
            where TValue : class
            => new(value, events);

        public static EntitySnapshot<TSource> Where<TSource>(this EntitySnapshot<TSource> snapshot,
            Func<TSource, bool> predicate)
            where TSource : class
            => snapshot switch
            {
                EmptyEntitySnapshot<TSource> _ => EntitySnapshot<TSource>.Empty,
                EntitySnapshot<TSource> source => predicate(source.Value) ? source : EntitySnapshot<TSource>.Empty,
            };

        public static EntitySnapshot<TTarget> Select<TSource, TTarget>(this EntitySnapshot<TSource> snapshot,
            Func<TSource, TTarget> selector)
            where TSource : class
            where TTarget : class
            => snapshot switch
            {
                EmptyEntitySnapshot<TSource> _ => EntitySnapshot<TTarget>.Empty,
                EntitySnapshot<TSource> source => new(selector(source.Value), source.Events),
            };

        public static EntitySnapshot<TTarget> SelectMany<TSource, TCollection, TTarget>(this EntitySnapshot<TSource> snapshot,
            Func<TSource, EntitySnapshot<TCollection>> collectionSelector,
            Func<TSource, TCollection, TTarget> targetSelector)
            where TSource : class
            where TCollection : class
            where TTarget : class
            => snapshot switch
            {
                EmptyEntitySnapshot<TSource> _ => EntitySnapshot<TTarget>.Empty,
                EntitySnapshot<TSource> source => collectionSelector(source.Value) switch
                {
                    EmptyEntitySnapshot<TCollection> _ => EntitySnapshot<TTarget>.Empty,
                    EntitySnapshot<TCollection> collection => new(
                        targetSelector(source.Value, collection.Value),
                        source.Events.Concat(collection.Events).ToArray())
                },
            };
    }
}

