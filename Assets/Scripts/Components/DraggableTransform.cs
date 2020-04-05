using UnityEngine;

namespace Assets.Scripts.Components
{
    class DraggableTransform : MonoBehaviour
    {
        private void OnMouseDrag()
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = currentPosition;
        }
    }
}
