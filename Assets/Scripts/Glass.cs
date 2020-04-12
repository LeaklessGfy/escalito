using System.Collections.Generic;
using Core;
using UnityEngine;

public class Glass : MonoBehaviour
{
    /* UNITY */
    private BoxCollider2D _boxCollider;
    private ParticleSystem _fullParticleSystem;
    private LineRenderer _lineRenderer;
    
    private readonly Dictionary<Ingredient, int> _recipe = new Dictionary<Ingredient, int>();

    public IReadOnlyDictionary<Ingredient, int> Recipe => _recipe;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _fullParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        var zAngle = transform.localEulerAngles.z;
        var shouldFlow = zAngle > 80 && zAngle < 280;
        if (shouldFlow && !IsEmpty())
        {
            ThrowConsumable();
        }
    }

    private void OnParticleCollision(GameObject origin)
    {
        var liquid = origin.GetComponent<ParticleSystem>();
        var color = liquid.main.startColor.color;

        _lineRenderer.startColor = color; // new Color(color.r, color.g, color.b, 0.60f);
        _lineRenderer.endColor = color; // new Color(color.r, color.g, color.b, 0.60f);

        if (!IsFull())
        {
            AddConsumable(origin);
        }
        else if (!_fullParticleSystem.isPlaying)
        {
            var main = _fullParticleSystem.main;
            main.startColor = color;
            _fullParticleSystem.Play();
        }
    }

    private void AddConsumable(GameObject origin)
    {
        var currentPosition = _lineRenderer.GetPosition(0);
        var stepPosition = Vector3.up * 0.1f;
        var newPosition = currentPosition + stepPosition;
        _lineRenderer.SetPosition(0, newPosition);

        var bottle = origin.GetComponentInParent<Bottle>();
        _recipe.TryGetValue(bottle.ingredient, out var prev);
        _recipe[bottle.ingredient] = prev + 1;
    }

    private void ThrowConsumable()
    {
        var currentPosition = _lineRenderer.GetPosition(0);
        var stepPosition = Vector3.up * 0.1f;
        var newPosition = currentPosition - stepPosition;
        _lineRenderer.SetPosition(0, newPosition);

        var keys = new List<Ingredient>(_recipe.Keys);
        foreach (var key in keys) _recipe[key] = Recipe[key] - 1;
    }

    private bool IsFull()
    {
        return _lineRenderer.GetPosition(0).y >= _boxCollider.size.y;
    }

    private bool IsEmpty()
    {
        return _lineRenderer.GetPosition(0).y <= 0;
    }
}