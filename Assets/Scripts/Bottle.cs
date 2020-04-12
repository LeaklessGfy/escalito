using Core;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    private bool _isClicked;
    private bool _isFlowing;
    private ParticleSystem _particleSystem;
    private Rigidbody2D _rigidBody2D;

    public Ingredient ingredient;

    private void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var zAngle = transform.localEulerAngles.z;
        var shouldFlow = zAngle > 80 && zAngle < 280;

        if (_isFlowing == shouldFlow)
        {
            return;
        }

        if (!_isClicked)
        {
            ResetPosition();
        }

        _isFlowing = shouldFlow;
        if (_isFlowing)
        {
            _particleSystem.Play();
        }
        else
        {
            _particleSystem.Stop();
        }
    }

    private void OnMouseDown()
    {
        _isClicked = true;
    }

    private void OnMouseUp()
    {
        _isClicked = false;
        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.rotation = Quaternion.identity;
        _rigidBody2D.angularVelocity = 0f;
    }
}