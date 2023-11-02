using UnityEngine;
using System;
using System.Collections.Generic;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private static UnityMainThreadDispatcher _instance = null;

    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<UnityMainThreadDispatcher>();
            if (_instance == null)
            {
                GameObject obj = new GameObject("UnityMainThreadDispatcher");
                _instance = obj.AddComponent<UnityMainThreadDispatcher>();
            }
        }

        return _instance;
    }

    private void Update()
    {
        while (_executionQueue.Count > 0)
        {
            Action action = null;
            lock (_executionQueue)
            {
                action = _executionQueue.Dequeue();
            }
            action.Invoke();
        }
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}
