using System.Collections.Generic;
using System.Linq;

#region models

public record Queue(IEnumerable<Agent> Agents);
public record Agent(int Id);

#endregion

#region events

public record AgentQueued(int AgentId) : DomainEvent;
public record AgentServed(int AgentId) : DomainEvent;

#endregion

public class QueueService
{
    public Result<Queue> CreateQueue() =>
        Result.Success(new Queue(new List<Agent>()));

    public Result<Queue> QueueAgent(Queue queue, int agentId) =>
        Result.Success(queue with
        {
            Agents = queue.Agents.Append(new Agent(agentId))
        })
        .WithEvent(new AgentQueued(agentId));

    public Result<Queue> ServeAgent(Queue queue) =>
        queue.Agents.FirstOrDefault() switch
        {
            null => Result.Success(queue),
            Agent agent => Result.Success(queue with
                {
                    Agents = queue.Agents.Except(new[] { agent })
                })
                .WithEvent(new AgentServed(agent.Id)),
        };
}

