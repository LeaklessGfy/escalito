using UnityEngine;

namespace Components
{
    public class RotatableTransform : MonoBehaviour
    {
        private bool _isClicked;
        private Rigidbody2D _rigidBody2D;

        public float speed = 150f;

        private void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var wheelAxis = Input.GetAxis("Mouse ScrollWheel");

            if (!_isClicked)
            {
                return;
            }

            if (wheelAxis != 0)
            {
                Rotate(wheelAxis);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
            {
                Rotate(10f);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                Rotate(-10f);
            }
        }

        private void Rotate(float axis)
        {
            _rigidBody2D.velocity = Vector2.zero;
            _rigidBody2D.angularVelocity = 0f;
            
            var currentRotation = transform.rotation;
            var rotation = new Vector3(currentRotation.x, currentRotation.y, speed * axis);
            transform.Rotate(rotation);
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