using Onion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SingleQueue
{
    #region models

    public record Queue
    {
        public IEnumerable<Agent> Agents { get; init; } = new Agent[] { };
        public bool IsEmpty => !Agents.Any();
    }
    public record Agent(int Id);

    #endregion

    #region events

    public record QueueCreated : DomainEvent;
    public record AgentQueued(int AgentId) : DomainEvent;
    public record AgentServed(int AgentId) : DomainEvent;

    #endregion

    public static class QueueService
    {
        public static EntitySnapshot<Queue> CreateQueue()
            => new Queue()
            .ToEntitySnapshot(new QueueCreated());

        public static EntitySnapshot<Queue> QueueAgent(this Queue queue, int agentId)
            => (queue with
            {
                Agents = queue.Agents.Append(new Agent(agentId)).ToArray()
            })
            .ToEntitySnapshot(new AgentQueued(agentId));

        public static EntitySnapshot<Queue> ServeAgent(this Queue queue)
            => queue.Agents.FirstOrDefault() switch
            {
                null => throw new InvalidOperationException("Cannot serve agent; queue is empty"),
                Agent agent => (queue with
                {
                    Agents = queue.Agents.Except(new[] { agent }).ToArray(),
                })
                .ToEntitySnapshot(new AgentServed(agent.Id)),
            };
    }
}

