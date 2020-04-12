using Core;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    /* UNITY */
    private Vector2 _formerPosition;
    private ParticleSystem _particleSystem;

    /* STATE */
    public Ingredient ingredient;

    private void Awake()
    {
        _formerPosition = transform.position;
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnMouseDown()
    {
        transform.position = new Vector2(0, 50);
        transform.Rotate(0, 0, 180);
        _particleSystem.Play();
    }

    private void OnMouseUp()
    {
        transform.position = _formerPosition;
        transform.rotation = Quaternion.identity;
        _particleSystem.Stop();
    }
}