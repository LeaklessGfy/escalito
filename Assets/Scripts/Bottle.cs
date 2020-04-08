using Core;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Consumable consumable;

    private bool _isFlowing;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        var zAngle = transform.localEulerAngles.z;
        var shouldFlow = zAngle > 80 && zAngle < 280;

        if (_isFlowing == shouldFlow)
        {
            return;
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
}
