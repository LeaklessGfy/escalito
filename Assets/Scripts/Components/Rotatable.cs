using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Rotatable : MonoBehaviour
    {
        public float speed = 150f;

        private float torque = 0;
        private static readonly float CONSTANT_FORCE = 2000;

        private void FixedUpdate()
        {
            float wheelAxis = Input.GetAxis("Mouse ScrollWheel");
            Vector3 rotation = new Vector3(transform.rotation.x, transform.rotation.y, speed * wheelAxis);
            transform.Rotate(rotation);

            if (wheelAxis == 0)
            {
                return;
            }

            //torque = wheelAxis * speed * CONSTANT_FORCE;
            //GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Force);
        }
    }
}
