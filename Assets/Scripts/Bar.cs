using System;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public event Action<Glass> CollisionListeners;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var glass = collision.gameObject.GetComponent<Glass>();
        CollisionListeners?.Invoke(glass);
    }
}