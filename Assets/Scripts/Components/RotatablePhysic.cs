using UnityEngine;

namespace Assets.Scripts.Components
{
    class RotatablePhysic : MonoBehaviour
    {
        public float speed = 150f;

        private float torque = 0;
        private static readonly float CONSTANT_FORCE = 2000;

        private void FixedUpdate()
        {
            float wheelAxis = Input.GetAxis("Mouse ScrollWheel");

            if (wheelAxis == 0)
            {
                return;
            }

            torque = wheelAxis * speed * CONSTANT_FORCE;
            GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Force);
        }
    }
}
