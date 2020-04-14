using UnityEngine;

namespace Components
{
    internal class DraggableTransform : MonoBehaviour
    {
        private Vector3 _screenBounds;

        private void Awake()
        {
            if (Camera.main == null)
            {
                return;
            }

            var mainCamera = Camera.main;
            var screen = new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z);
            _screenBounds = mainCamera.ScreenToWorldPoint(screen);
        }

        private void OnMouseDrag()
        {
            if (Camera.main == null)
            {
                return;
            }

            Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (pointerPosition.x < _screenBounds.x * -1 || pointerPosition.x > _screenBounds.x || pointerPosition.y < _screenBounds.y * -1 ||
                pointerPosition.y > _screenBounds.y)
            {
                return;
            }
            transform.position = pointerPosition;
        }
    }
}