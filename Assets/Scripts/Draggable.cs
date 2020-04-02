using UnityEngine;

public class Draggable : MonoBehaviour
{
    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
