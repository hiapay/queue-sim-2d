using Onion;

namespace SingleQueue
{
    public class SingleQueueService
    {
        public Entity<Queue> QueueEntity = new();

        public void Initialize()
        {
            var snapshot = from q0 in QueueService.CreateQueue()
                           from q1 in QueueService.QueueAgent(q0, 0)
                           from q2 in QueueService.QueueAgent(q1, 1)
                           from q3 in QueueService.QueueAgent(q2, 2)
                           from q4 in QueueService.QueueAgent(q3, 3)
                           select q4;
            QueueEntity.Save(snapshot);
        }

        public void Next()
        {
            // TODO: support where !q0.IsEmpty
            if (QueueEntity.Value.IsEmpty) { return; }

            var snapshot = from q0 in QueueEntity
                           from q1 in QueueService.ServeAgent(q0)
                           select q1;
            QueueEntity.Save(snapshot);
        }
    }
}

