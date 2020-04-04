using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Draggable : MonoBehaviour
    {
        private void OnMouseDrag()
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.position = currentPosition;
            GetComponent<Rigidbody2D>().velocity = (currentPosition - (Vector2)transform.position) * 5;
        }
    }
}
