using System;
using UnityEngine;

namespace Components
{
    public class DraggablePhysic : MonoBehaviour
    {
        private Rigidbody2D _rigidBody2D;
        
        private void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        private void OnMouseDrag()
        {
            if (Camera.main == null)
            {
                return;
            }

            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _rigidBody2D.velocity = (currentPosition - (Vector2) transform.position) * 5;
        }
    }
}