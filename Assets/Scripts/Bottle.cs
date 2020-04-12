using System;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bottle : MonoBehaviour, IPointerDownHandler
{
    private bool _isFlowing;
    private ParticleSystem _particleSystem;
    public Ingredient ingredient;

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

    private void OnMouseUp()
    {
        transform.rotation = Quaternion.identity;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.clickCount < 2)
        {
            return;
        }

        transform.rotation = Quaternion.identity;
    }
}