using Core;
using UnityEngine;
using UnityEngine.UI;

public class Bottle : MonoBehaviour
{
    private Vector2 _formerPosition;
    private Vector2 _initialPosition;
    private ParticleSystem _particleSystem;

    public Ingredient ingredient;

    [SerializeField] private Image stockImage;
    [SerializeField] private Slider stockSlider;

    private void Awake()
    {
        _formerPosition = transform.position;
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        var main = _particleSystem.main;
        main.startColor = ingredient.color;
    }

    private void Update()
    {
        if (ingredient.stock <= 0)
        {
            _particleSystem.Stop();
        }

        if (_particleSystem.isPlaying)
        {
            ingredient.stock -= 5 * Time.deltaTime;
        }

        stockSlider.value = ingredient.stock;
        stockImage.color = SatisfactionHelper.GetColor((int) ingredient.stock);
    }

    private void OnMouseDrag()
    {
        if (Camera.main == null)
        {
            return;
        }

        var currentPosition = transform.position;
        var pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(pointerPosition.x - _initialPosition.x, currentPosition.y, currentPosition.z);
    }

    private void OnMouseDown()
    {
        if (Camera.main == null)
        {
            return;
        }

        transform.position = new Vector2(0, 70);
        transform.Rotate(0, 0, 180);
        stockSlider.transform.Rotate(0, 0, 180);
        _initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ingredient.stock > 0)
        {
            _particleSystem.Play();
        }
    }

    private void OnMouseUp()
    {
        transform.position = _formerPosition;
        transform.Rotate(0, 0, 180);
        stockSlider.transform.Rotate(0, 0, 180);
        _initialPosition = Vector2.zero;
        _particleSystem.Stop();
    }
}