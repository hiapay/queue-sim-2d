using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

public class SingleQueueService
{
    public IObservable<DomainEvent> EventStream => EventSource;
    private Subject<DomainEvent> EventSource { get; } = new();

    private QueueService QueueService { get; } = new();
    public Queue Queue { get; private set; }

    public void Init()
    {
        var queue = from q0 in QueueService.CreateQueue()
                    from q1 in QueueService.QueueAgent(q0, 0)
                    from q2 in QueueService.QueueAgent(q1, 2)
                    from q3 in QueueService.QueueAgent(q2, 1)
                    select q3;

        Queue = queue.Value;
        PublishEvents(queue.Events);
    }

    public void Next()
    {
        var queue = QueueService.ServeAgent(Queue);

        Queue = queue.Value;
        PublishEvents(queue.Events);
    }

    private void PublishEvents(IEnumerable<DomainEvent> events)
    {
        foreach (var @event in events)
        {
            EventSource.OnNext(@event);
        }
    }
}

