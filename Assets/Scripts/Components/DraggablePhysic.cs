using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DraggablePhysic : MonoBehaviour
    {
        private void OnMouseDrag()
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetComponent<Rigidbody2D>().velocity = (currentPosition - (Vector2)transform.position) * 5;
        }
    }
}
