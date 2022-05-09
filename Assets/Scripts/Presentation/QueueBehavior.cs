using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using UnityEngine;

public class QueueBehavior : MonoBehaviour
{
    public GameObject AgentPrefab;
    private LinkedList<GameObject> AgentGameObjects { get; } = new();

    private SingleQueueService SingleQueueService { get; } = new();
    private IDisposable QueueValueSubscription { get; set; }
    private IDisposable QueueEventSubscription { get; set; }

    public IEnumerator Start()
    {
        QueueValueSubscription = SingleQueueService.QueueEntity.ValueStream.Subscribe(Draw);
        QueueEventSubscription = SingleQueueService.QueueEntity.EventStream.Subscribe(Debug.Log);

        SingleQueueService.Initialize();
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SingleQueueService.Next();
        }
    }

    public void OnDisable()
    {
        QueueValueSubscription.Dispose();
        QueueEventSubscription.Dispose();
    }

    private void Draw(Queue queue)
    {
        if (queue is null) { return; }

        while (AgentGameObjects.Count() < queue.Agents.Count())
        {
            var agentGameObject = Instantiate(AgentPrefab,
                new Vector3(AgentGameObjects.Count(), 0, 0),
                Quaternion.identity);
            AgentGameObjects.AddFirst(agentGameObject);
        }

        while (queue.Agents.Count() < AgentGameObjects.Count())
        {
            Destroy(AgentGameObjects.Last());
            AgentGameObjects.RemoveLast();
        }
    }
}

