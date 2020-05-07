using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Singleton;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Customer : Character
{
    private const float Patience = 10f;
    private readonly List<Func<Cocktail, Cocktail, int>> _rules = new List<Func<Cocktail, Cocktail, int>>();

    private Order _order;
    private int _satisfaction;
    private int _servedCount;
    private float _timeAwaited;
    private float _currentPatience;

    [SerializeField] private Text cashText;
    [SerializeField] private Image orderImage;
    [SerializeField] private Image waitingImage;
    [SerializeField] private Slider waitingSlider;

    public Func<Order> OrderBuilder { private get; set; }
    public bool HasOrder => _order != null;

    protected new void Awake()
    {
        base.Awake();
        _rules.Add(Rules.CocktailRule);
        cashText.gameObject.SetActive(false);
        orderImage.gameObject.SetActive(false);
        waitingSlider.gameObject.SetActive(false);
    }

    protected new void Update()
    {
        base.Update();
        if (States.Contains(State.Wait))
        {
            StepWait();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!HasOrder)
        {
            return;
        }

        var glass = collision.gameObject.GetComponent<GlassSprite>();

        if (glass == null || glass.Served)
        {
            return;
        }

        glass.Served = true;
        Controller.Main.ReceiveOrder(this, glass);
    }

    public void AskOrder()
    {
        if (_order != null)
        {
            throw new InvalidOperationException("Customer has already order");
        }

        _order = OrderBuilder();
        orderImage.sprite = CocktailManager.Main.GetSprite(_order.Cocktail.Key);
        orderImage.gameObject.SetActive(true);
    }

    public void Await(int difficulty)
    {
        if (States.Contains(State.Wait))
        {
            throw new InvalidOperationException("Customer is already awaiting");
        }

        States.Add(State.Wait);
        _currentPatience = Patience / difficulty;
        _timeAwaited = 0;

        waitingSlider.gameObject.SetActive(true);
        waitingSlider.minValue = 0;
        waitingSlider.maxValue = _currentPatience;
    }

    public bool IsExhausted()
    {
        return States.Contains(State.Exhausted);
    }

    private int Try(Cocktail expected, Cocktail actual)
    {
        return _rules.Sum(rule => rule(expected, actual)) / _rules.Count;
    }

    public void Serve(Cocktail actual)
    {
        if (!States.Contains(State.Wait))
        {
            throw new InvalidOperationException("Customer is not awaiting");
        }

        States.Remove(State.Wait);
        _timeAwaited = 0;
        _satisfaction = Try(_order.Cocktail, actual);

        orderImage.gameObject.SetActive(false);
        waitingSlider.gameObject.SetActive(false);
    }

    public bool IsSatisfied()
    {
        return _satisfaction > SatisfactionHelper.Low;
    }

    public int Pay()
    {
        if (!IsSatisfied())
        {
            return 0;
        }

        var bonus = _satisfaction > SatisfactionHelper.High && Random.Range(0, 4) == 0;
        var price = _order.Cocktail.Price + (bonus ? Random.Range(1, 5) : 0);

        cashText.gameObject.SetActive(true);
        cashText.text = "+" + price + "$";
        cashText.color = SatisfactionHelper.GetColor(_satisfaction);

        return price;
    }

    public void LeaveTo(Vector2 dst)
    {
        SpriteRenderer.color = SatisfactionHelper.GetColor(_satisfaction);
        orderImage.gameObject.SetActive(false);
        waitingSlider.gameObject.SetActive(false);
        MoveTo(dst, 0.0f, 0.0f);
    }

    private void StepWait()
    {
        _timeAwaited += Time.deltaTime;
        waitingSlider.value = _currentPatience - _timeAwaited;

        var percent = 100 - _timeAwaited / _currentPatience * 100;
        waitingImage.color = SatisfactionHelper.GetColor((int) percent);

        if (_timeAwaited < _currentPatience)
        {
            return;
        }

        States.Remove(State.Wait);
        States.Add(State.Exhausted);
    }
}