using UnityEngine;

namespace Assets.Scripts.Components
{
    public class RotatableTransform : MonoBehaviour
    {
        public float speed = 150f;

        private bool click = false;

        private void Update()
        {
            float wheelAxis = Input.GetAxis("Mouse ScrollWheel");
            if (click && wheelAxis != 0)
            {
                Vector3 rotation = new Vector3(transform.rotation.x, transform.rotation.y, speed * wheelAxis);
                transform.Rotate(rotation);
            }
        }

        private void OnMouseDown()
        {
            click = true;
        }

        private void OnMouseUp()
        {
            click = false;
        }
    }
}
