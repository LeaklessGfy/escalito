using UnityEngine;

namespace Components
{
    internal class DraggableTransform : MonoBehaviour
    {
        private void OnMouseDrag()
        {
            if (Camera.main == null)
            {
                return;
            }

            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = currentPosition;
        }
    }
}