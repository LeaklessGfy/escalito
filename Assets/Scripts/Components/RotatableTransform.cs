using UnityEngine;

namespace Components
{
    public class RotatableTransform : MonoBehaviour
    {
        private Rigidbody2D _rigidBody2D;
        private bool _isClicked;

        public float speed = 150f;

        private void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var wheelAxis = Input.GetAxis("Mouse ScrollWheel");

            if (!_isClicked || wheelAxis == 0)
            {
                return;
            }

            var currentRotation = transform.rotation;
            var rotation = new Vector3(currentRotation.x, currentRotation.y, speed * wheelAxis);
            transform.Rotate(rotation);

            _rigidBody2D.velocity = Vector2.zero;
            _rigidBody2D.angularVelocity = 0f;
        }

        private void OnMouseDown()
        {
            _isClicked = true;
        }

        private void OnMouseUp()
        {
            _isClicked = false;
        }
    }
}