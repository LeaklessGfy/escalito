using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public float speed = 150f;

    private void Update()
    {
        OnMouseWheel();
    }

    private void OnMouseWheel()
    {
        float wheelAxis = Input.GetAxis("Mouse ScrollWheel");
        Vector3 rotation = new Vector3(transform.rotation.x, transform.rotation.y, speed * wheelAxis);
        transform.Rotate(rotation);
    }
}
