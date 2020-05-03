using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Rendering;

public class Glass : MonoBehaviour, IComparable<Glass>
{
    private BoxCollider2D _boxCollider;
    private ParticleSystem _fullParticleSystem;
    private IngredientKey _last;

    [SerializeField] private Material material;

    public LinkedList<LineRenderer> LineRenderers { get; } = new LinkedList<LineRenderer>();
    public Cocktail Cocktail { get; } = Cocktail.BuildEmpty();
    public float OverflowX => _boxCollider.size.x;
    public float OverflowY => _boxCollider.size.y;
    public bool Served { get; set; }

    public int CompareTo(Glass other)
    {
        return 0;
    }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _fullParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject origin)
    {
        var bottle = origin.GetComponentInParent<Bottle>();
        var key = bottle.ingredient.key;
        var color = bottle.ingredient.color;

        if (LineRenderers.Count < 1 || key != _last)
        {
            LineRenderers.AddLast(CreateLineRenderer(color, LineRenderers.Last?.Value));
            _last = key;
        }

        AddIngredient(key);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var origin = collision.gameObject;
        var consumable = origin.GetComponent<Consumable>();
        if (consumable == null)
        {
            return;
        }

        origin.transform.SetParent(transform);
        Destroy(origin.GetComponent<Rigidbody2D>());
        Cocktail.AddIngredient(consumable.ingredient.key);
    }

    public bool NeedMix()
    {
        return LineRenderers.Count > 1;
    }

    public LineRenderer CreateLineRenderer(Color color, LineRenderer last)
    {
        var line = new GameObject();
        line.transform.SetParent(gameObject.transform);
        line.transform.position = Vector3.zero;
        line.transform.localPosition = Vector3.zero;

        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 14;
        lineRenderer.endWidth = 14;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.material = material;
        lineRenderer.sortingLayerName = "Clients";
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.allowOcclusionWhenDynamic = false;
        lineRenderer.useWorldSpace = false;

        var position = last != null ? last.GetPosition(1) : Vector3.zero;
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, position);

        return lineRenderer;
    }

    private void AddIngredient(IngredientKey ingredientKey)
    {
        if (IsFull())
        {
            PlayFull();
            return;
        }

        var currentPosition = LineRenderers.Last.Value.GetPosition(1);
        var stepPosition = Vector3.up * 0.1f;
        LineRenderers.Last.Value.SetPosition(1, currentPosition + stepPosition);
        Cocktail.AddIngredient(ingredientKey);
    }

    private bool IsFull()
    {
        return LineRenderers.Last.Value.GetPosition(1).y >= _boxCollider.size.y - 1;
    }

    private void PlayFull()
    {
        if (_fullParticleSystem.isPlaying)
        {
            return;
        }

        var lastColor = LineRenderers.Last.Value.startColor;
        var main = _fullParticleSystem.main;
        main.startColor = lastColor;

        _fullParticleSystem.Play();
    }
}