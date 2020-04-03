using UnityEngine;

public class Draggable : MonoBehaviour
{
    private void OnMouseDrag()
    {
        Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = currentPosition;
    }
}
