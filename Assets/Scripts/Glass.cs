using Assets.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private LineRenderer lineRenderer;
    private ParticleSystem fullParticleSystem;

    public Dictionary<Cocktail.Consumable, int> Consumables
    {
        get; private set;
    } = new Dictionary<Cocktail.Consumable, int>();

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        fullParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        float zAngle = transform.localEulerAngles.z;
        bool shouldFlow = zAngle > 80 && zAngle < 280;

        if (shouldFlow && !IsEmpty())
        {
            ThrowConsumable();
        }
    }

    private void OnParticleCollision(GameObject origin)
    {
        ParticleSystem particleSystem = origin.GetComponent<ParticleSystem>();
        Color color = particleSystem.main.startColor.color;

        //lineRenderer.startColor = new Color(color.r, color.g, color.b, 0.60f);
        //lineRenderer.endColor = new Color(color.r, color.g, color.b, 0.70f);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        if (!IsFull())
        {
            AddConsumable(origin);
        }
        else if (!fullParticleSystem.isPlaying)
        {
            var main = fullParticleSystem.main;
            main.startColor = color;
            fullParticleSystem.Play();
        }
    }

    private void AddConsumable(GameObject origin)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(0);
        Vector3 stepPosition = Vector3.up * 0.1f;
        Vector3 newPosition = currentPosition + stepPosition;
        lineRenderer.SetPosition(0, newPosition);

        Bottle bottle = origin.GetComponentInParent<Bottle>();
        Consumables.TryGetValue(bottle.consumable, out var prev);
        Consumables[bottle.consumable] = prev + 1;
    }

    private void ThrowConsumable()
    {
        Vector3 currentPosition = lineRenderer.GetPosition(0);
        Vector3 stepPosition = Vector3.up * 0.1f;
        Vector3 newPosition = currentPosition - stepPosition;
        lineRenderer.SetPosition(0, newPosition);

        List<Cocktail.Consumable> keys = new List<Cocktail.Consumable>(Consumables.Keys);
        foreach (Cocktail.Consumable key in keys)
        {
            Consumables[key] = Consumables[key] - 1;
        }
    }

    private bool IsFull()
    {
        return lineRenderer.GetPosition(0).y >= boxCollider.size.y;
    }

    private bool IsEmpty()
    {
        return lineRenderer.GetPosition(0).y <= 0;
    }
}
