using UnityEngine;

namespace Components
{
    internal class RotatablePhysic : MonoBehaviour
    {
        private static readonly float CONSTANT_FORCE = 2000;
        private Rigidbody2D _rigidBody2D;

        private float _torque;
        public float speed = 150f;

        private void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var wheelAxis = Input.GetAxis("Mouse ScrollWheel");

            if (wheelAxis == 0)
            {
                return;
            }

            _torque = wheelAxis * speed * CONSTANT_FORCE;
            _rigidBody2D.AddTorque(_torque, ForceMode2D.Force);
        }
    }
}