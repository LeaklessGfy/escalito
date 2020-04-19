using Core;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    private Vector2 _formerPosition;
    private Vector2 _initialPosition;
    private ParticleSystem _particleSystem;

    public Ingredient ingredient;

    private void Awake()
    {
        _formerPosition = transform.position;
        _particleSystem = GetComponentInChildren<ParticleSystem>();

        var main = _particleSystem.main;
        main.startColor = ingredient.color;
    }

    private void OnMouseDrag()
    {
        if (Camera.main == null)
        {
            return;
        }

        var currentPosition = transform.position;
        var pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(pointerPosition.x - _initialPosition.x, currentPosition.y, currentPosition.z);
    }

    private void OnMouseDown()
    {
        if (Camera.main == null)
        {
            return;
        }

        transform.position = new Vector2(0, 50);
        transform.Rotate(0, 0, 180);
        _initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _particleSystem.Play();
    }

    private void OnMouseUp()
    {
        transform.position = _formerPosition;
        transform.rotation = Quaternion.identity;
        _initialPosition = Vector2.zero;
        _particleSystem.Stop();
    }
}