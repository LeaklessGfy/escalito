using System.Collections.Generic;
using Components;
using Core;
using UnityEngine;
using UnityEngine.Rendering;

public class Glass : MonoBehaviour
{
    private readonly Dictionary<IngredientKey, int> _recipe = new Dictionary<IngredientKey, int>();

    private BoxCollider2D _boxCollider;
    private ParticleSystem _fullParticleSystem;
    private IngredientKey _last;

    public bool hasCollide;

    [SerializeField] private Material material;

    public IReadOnlyDictionary<IngredientKey, int> Recipe => _recipe;
    public LinkedList<LineRenderer> LineRenderers { get; } = new LinkedList<LineRenderer>();

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
        _recipe.TryGetValue(consumable.ingredient.key, out var prev);
        _recipe[consumable.ingredient.key] = prev + 1;
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

        _recipe.TryGetValue(ingredientKey, out var prev);
        _recipe[ingredientKey] = prev + 1;
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