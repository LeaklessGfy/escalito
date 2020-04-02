using UnityEngine;

public class Draggable : MonoBehaviour
{
    private void OnMouseDrag()
    {
        transform.position = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
