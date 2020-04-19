using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.UI;

public class Shaker : MonoBehaviour
{
    private const float MixTime = 100f;

    private float _currentMix;
    private Glass _glass;
    private bool _isClicked;
    private bool _isMixInit;
    private Vector3 _lastPosition = Vector3.zero;
    private LineRenderer _mixLineRenderer;
    private bool _shouldMix;

    [SerializeField] private Image mixImage;
    [SerializeField] private Slider mixSlider;

    private void Awake()
    {
        mixSlider.minValue = 0;
        mixSlider.maxValue = MixTime;
        mixSlider.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!_isClicked || !_shouldMix)
        {
            return;
        }

        var delta = Input.mousePosition - _lastPosition;
        var speed = Mathf.Abs(delta.x + delta.y);

        if (speed < 1)
        {
            return;
        }

        if (!_isMixInit)
        {
            CreateMix();
        }
        else
        {
            UpdateMix(speed);
        }

        _lastPosition = Input.mousePosition;
    }

    private void OnMouseDown()
    {
        _isClicked = true;
        _lastPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        _isClicked = false;
        _lastPosition = Vector3.zero;
    }

    public void Attach(Glass glass)
    {
        _glass = glass;
        _shouldMix = glass.NeedMix();
    }

    private void CreateMix()
    {
        _isMixInit = true;

        var lr = _glass.LineRenderers;
        var gradient = BuildGradient(lr);

        _mixLineRenderer = _glass.CreateLineRenderer(Color.clear, null);
        _mixLineRenderer.colorGradient = gradient;
        _mixLineRenderer.SetPosition(1, lr.Last.Value.GetPosition(1));

        mixSlider.gameObject.SetActive(true);
    }

    private void UpdateMix(float speed)
    {
        _currentMix += speed / 50;
        mixSlider.value = _currentMix;
        mixImage.color = Satisfaction.GetColor(GetMixPercent());

        if (_currentMix < MixTime)
        {
            return;
        }

        var colorGradient = _mixLineRenderer.colorGradient;
        var mixedColor = colorGradient.colorKeys
            .Select(key => key.color)
            .Aggregate(new Color(), (acc, c) => acc + c) / colorGradient.colorKeys.Length;

        _mixLineRenderer.colorGradient = new Gradient();
        _mixLineRenderer.startColor = mixedColor;
        _mixLineRenderer.endColor = mixedColor;

        mixSlider.gameObject.SetActive(false);
    }

    private static Gradient BuildGradient(IReadOnlyCollection<LineRenderer> lr)
    {
        var gradient = new Gradient();
        var gradientColorKeys = new GradientColorKey[lr.Count];
        var gradientAlphaKeys = new GradientAlphaKey[lr.Count];

        var i = 0;
        foreach (var lineRenderer in lr)
        {
            lineRenderer.gameObject.SetActive(false);
            var time = (float) (i + 1) / lr.Count;
            var colorKey = new GradientColorKey {time = time, color = lineRenderer.startColor};
            var alphaKey = new GradientAlphaKey {time = time, alpha = 1.0f};
            gradientColorKeys[i] = colorKey;
            gradientAlphaKeys[i] = alphaKey;
            i++;
        }

        gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);

        return gradient;
    }

    public int GetMixPercent()
    {
        return (int) (_currentMix / MixTime * 100);
    }
}