using System;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    private readonly List<Action<Glass>> _collisionsListeners = new List<Action<Glass>>();

    public void AddCollisionListener(Action<Glass> action)
    {
        _collisionsListeners.Add(action);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var glass = collision.gameObject.GetComponent<Glass>();
        foreach (var listener in _collisionsListeners)
        {
            listener(glass);
        }
    }
}
