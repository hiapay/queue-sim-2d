using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using UnityEngine;

public class QueueBehavior : MonoBehaviour
{
    public GameObject AgentPrefab;
    private Dictionary<int, GameObject> AgentGameObjects { get; } = new();

    private SingleQueueService SingleQueueService { get; } = new();

    private IDisposable subscription;

    public IEnumerator Start()
    {
        subscription = SingleQueueService.EventStream.Subscribe(_ => Draw());

        SingleQueueService.Init();
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SingleQueueService.Next();
        }
    }

    public void OnDisable()
    {
        subscription.Dispose();
    }

    private void Draw()
    {
        var agentIds = SingleQueueService.Queue.Agents.Select(it => it.Id);
        var removingAgentIds = AgentGameObjects.Keys.Except(agentIds).ToList();
        var addingAgentIds = agentIds.Except(AgentGameObjects.Keys).ToList();

        foreach (var agentId in removingAgentIds)
        {
            Destroy(AgentGameObjects[agentId]);
            AgentGameObjects.Remove(agentId);
        }

        foreach (var agentId in addingAgentIds)
        {
            AgentGameObjects[agentId] = Instantiate(AgentPrefab, new Vector3(agentId, 0, 0), Quaternion.identity);
        }
    }
}

