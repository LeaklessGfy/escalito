using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Customer : MonoBehaviour
{
    private const int AnimIdle = 0;
    private const int AnimMove = 1;
    private const float Speed = 30f;
    private const float Patience = 10f;
    private readonly List<Func<Cocktail, Cocktail, int>> _rules = new List<Func<Cocktail, Cocktail, int>>();
    private readonly HashSet<State> _states = new HashSet<State> {State.Idle};

    private Animator _animator;
    private Vector2 _dst;
    private float _minDistance;
    private Order _order;
    private SpriteRenderer _spriteRenderer;
    private float _timeAwaited;

    [SerializeField] private Text cashText;

    public Func<Order> OrderBuilder = () =>
    {
        var o = new Order();
        o.Cocktails.Enqueue(Cocktail.BuildRandom());
        return o;
    };

    [SerializeField] private Button orderButton;
    [SerializeField] private Text orderText;
    [SerializeField] private Image waitingImage;
    [SerializeField] private Slider waitingSlider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rules.Add(Rules.CocktailRule);

        orderButton.gameObject.SetActive(false);
        orderButton.onClick.AddListener(() => { _timeAwaited -= 1; });
        cashText.gameObject.SetActive(false);

        waitingSlider.minValue = 0;
        waitingSlider.maxValue = Patience;
        waitingSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_states.Contains(State.Wait))
        {
            StepWait();
        }

        if (_states.Contains(State.Idle))
        {
            StepIdle();
        }

        if (_states.Contains(State.Move))
        {
            StepMove();
        }
    }

    private void OnMouseDown()
    {
        if (HasOrder())
        {
            Controller.Main.ReceiveOrder(this);
        }
    }

    public static string GetName()
    {
        return "Michel";
    }

    public float GetOffset()
    {
        return _spriteRenderer.sprite.bounds.extents.x;
    }

    private Vector2 Normalize(Vector2 dst, float offset)
    {
        return new Vector2(dst.x + offset, transform.position.y);
    }

    public bool IsNear(Vector2 dst, float offset, float minDistance)
    {
        return Vector2.Distance(transform.position, Normalize(dst, offset)) <= minDistance;
    }

    public void MoveTo(Vector2 dst, float offset, float minDistance)
    {
        if (dst == _dst || IsNear(dst, offset, minDistance))
        {
            return;
        }

        _dst = Normalize(dst, offset);
        _minDistance = minDistance;
        _states.Remove(State.Idle);
        _states.Add(State.Move);
        _spriteRenderer.flipX = _dst.x < transform.position.x;
    }

    public bool HasOrder()
    {
        return _order != null;
    }

    public Order Order()
    {
        if (_order != null)
        {
            throw new InvalidOperationException("Customer has already order");
        }

        _order = OrderBuilder();
        orderButton.gameObject.SetActive(true);
        orderText.text = _order.Cocktails.Peek().Key.ToString();

        return _order;
    }

    public void Await()
    {
        if (_states.Contains(State.Wait))
        {
            throw new InvalidOperationException("Customer is already awaiting");
        }

        _states.Add(State.Wait);
        _timeAwaited = 0;
        waitingSlider.gameObject.SetActive(true);
    }

    public bool IsExhausted()
    {
        return _states.Contains(State.Wait) && _timeAwaited >= Patience;
    }

    public int Serve(Cocktail expected, Cocktail actual)
    {
        if (!_states.Contains(State.Wait))
        {
            throw new InvalidOperationException("Customer is not awaiting");
        }

        _states.Remove(State.Wait);
        _timeAwaited = 0;
        waitingSlider.gameObject.SetActive(false);
        _order = null;

        return _rules.Sum(rule => rule(expected, actual));
    }

    public int Pay(int expectedPrice, int satisfaction)
    {
        if (satisfaction < Satisfaction.Low)
        {
            return 0;
        }

        var bonus = satisfaction > Satisfaction.High && Random.Range(0, 4) == 0;
        var price = expectedPrice + (bonus ? Random.Range(1, 5) : 0);

        cashText.gameObject.SetActive(true);
        cashText.text = "+" + price + "$";
        cashText.color = Satisfaction.GetColor(satisfaction);

        return price;
    }

    public void Leave(int satisfaction)
    {
        _spriteRenderer.color = Satisfaction.GetColor(satisfaction);
        orderButton.gameObject.SetActive(false);
        waitingSlider.gameObject.SetActive(false);
    }

    private void StepWait()
    {
        _timeAwaited += Time.deltaTime;
        waitingSlider.value = Patience - _timeAwaited;

        var percent = 100 - _timeAwaited / Patience * 100;
        waitingImage.color = Satisfaction.GetColor((int) percent);

        if (_timeAwaited < Patience)
        {
            return;
        }

        waitingSlider.gameObject.SetActive(false);
    }

    private void StepIdle()
    {
        _animator.SetInteger("State", AnimIdle);
    }

    private void StepMove()
    {
        if (Vector2.Distance(transform.position, _dst) > _minDistance)
        {
            _animator.SetInteger("State", AnimMove);
            transform.position = Vector2.MoveTowards(transform.position, _dst, Speed * Time.deltaTime);
        }
        else
        {
            _animator.SetInteger("State", AnimIdle);
            _states.Remove(State.Move);
            _states.Add(State.Idle);
            _dst = Vector2.zero;
            _minDistance = 0;
        }
    }

    private enum State
    {
        Idle,
        Move,
        Wait
    }
}