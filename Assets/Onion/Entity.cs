using System;
using System.Reactive.Subjects;

namespace Onion
{
    public class Entity<TValue>
        where TValue : class
    {
        public TValue Value => ValueSource.Value;

        public IObservable<TValue> ValueStream => ValueSource;
        private BehaviorSubject<TValue> ValueSource { get; } = new(null);

        public IObservable<DomainEvent> EventStream => EventSource;
        private ReplaySubject<DomainEvent> EventSource { get; } = new();

        public EntitySnapshot<TValue> Snapshot
            => new(Value, new DomainEvent[] { });

        public void Save(EntitySnapshot<TValue> snapshot)
        {
            if (snapshot is EmptyEntitySnapshot<TValue>) { return; }

            ValueSource.OnNext(snapshot.Value);
            foreach (var @event in snapshot.Events)
            {
                EventSource.OnNext(@event);
            }
        }
    }

    public static class EntityExtensions
    {
        public static EntitySnapshot<TSource> Where<TSource>(this Entity<TSource> entity,
            Func<TSource, bool> predicate)
            where TSource : class
            => predicate(entity.Value) ? entity.Snapshot : EntitySnapshot<TSource>.Empty;
    }
}

