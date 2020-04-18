using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Rendering;

public class Glass : MonoBehaviour
{
    /* UNITY */
    private BoxCollider2D _boxCollider;
    private ParticleSystem _fullParticleSystem;
    
    /* DEPENDENCIES */
    [SerializeField] private Material material = default;

    /* STATE */
    private IngredientKey _last;

    private readonly LinkedList<LineRenderer> _lineRenderers = new LinkedList<LineRenderer>();
    private readonly Dictionary<IngredientKey, int> _recipe = new Dictionary<IngredientKey, int>();

    /* PUBLIC */
    public IReadOnlyDictionary<IngredientKey, int> Recipe => _recipe;
    public LinkedList<LineRenderer> LineRenderers => _lineRenderers;

    public bool hasCollide;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _fullParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject origin)
    {
        var liquid = origin.GetComponent<ParticleSystem>();
        var color = liquid.main.startColor.color;
        var bottle = origin.GetComponentInParent<Bottle>();

        if (_lineRenderers.Count < 1 || bottle.IngredientKey != _last)
        {
            _lineRenderers.AddLast(CreateLineRenderer(color, _lineRenderers.Last?.Value));
            _last = bottle.IngredientKey;
        }

        AddIngredient(bottle.IngredientKey);
    }

    public bool NeedMix()
    {
        return _lineRenderers.Count > 1;
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
        
        var currentPosition = _lineRenderers.Last.Value.GetPosition(1);
        var stepPosition = Vector3.up * 0.1f;
        _lineRenderers.Last.Value.SetPosition(1, currentPosition + stepPosition);

        _recipe.TryGetValue(ingredientKey, out var prev);
        _recipe[ingredientKey] = prev + 1;
    }

    private bool IsFull()
    {
        return _lineRenderers.Last.Value.GetPosition(1).y >= _boxCollider.size.y - 1;
    }

    private void PlayFull()
    {
        if (_fullParticleSystem.isPlaying)
        {
            return;
        }

        var lastColor = _lineRenderers.Last.Value.startColor;
        var main = _fullParticleSystem.main;
        main.startColor = lastColor;

        _fullParticleSystem.Play();
    }
}