using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Client : MonoBehaviour
{
    /* CONSTANT */
    private const int AnimIdle = 0;
    private const int AnimMove = 1;
    private const float Speed = 30f;
    private const float Patience = 10f;

    /* UNITY */
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    /* DEPENDENCIES */
    [SerializeField] private Button orderButton = default;
    [SerializeField] private Text orderText = default;
    [SerializeField] private Text cashText = default;
    [SerializeField] private Slider waitingSlider = default;
    [SerializeField] private Image waitingImage = default;

    /* STATE */
    private Vector2 _dst;
    private TaskCompletionSource<bool> _onMoveTask;
    private TaskCompletionSource<bool> _onWaitTask;
    private State _state = State.Idle;
    private float _timeAwaited;
    private Cocktail _order;
    private readonly List<Func<Cocktail, Cocktail, int>> _rules = new List<Func<Cocktail, Cocktail, int>>();

    /* PUBLIC */
    public event Action<Client, Glass> CollisionListeners;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rules.Add(Rules.CocktailRule);

        orderButton.gameObject.SetActive(false);
        orderButton.onClick.AddListener(() =>
        {
            _timeAwaited -= 1;
        });
        cashText.gameObject.SetActive(false);

        waitingSlider.minValue = 0;
        waitingSlider.maxValue = Patience;
        waitingSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        StepWait();

        switch (_state)
        {
            case State.Idle:
                StepIdle();
                break;
            case State.Move:
                StepMove();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var glass = collision.gameObject.GetComponent<Glass>();
        CollisionListeners?.Invoke(this, glass);
    }

    public String GetName()
    {
        return "Michel";
    }

    public Task<bool> MoveToAsync(Vector2 position, float overflow = 1f)
    {
        if (_onMoveTask != null)
        {
            //throw new InvalidOperationException("Client is already moving, can't move again");
            return Task.FromResult(false);
        }

        var x = position.x - (_spriteRenderer.sprite.bounds.extents.x * overflow);
        _dst = new Vector2(x, transform.position.y);

        if (Vector2.Distance(transform.position, _dst) < 2)
        {
            return Task.FromResult(false);
        }
        
        _state = State.Move;
        _onMoveTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        return _onMoveTask.Task;
    }

    public Cocktail Order()
    {
        if (_order != null)
        {
            return _order;
        }
        
        _order = Cocktail.BuildRandom();
        orderButton.gameObject.SetActive(true);
        orderText.text = _order.Name.ToString();

        return _order;
    }

    public Task Await()
    {
        if (_onWaitTask != null)
        {
            throw new InvalidOperationException("Client is already awaiting");
        }

        _onWaitTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        waitingSlider.gameObject.SetActive(true);

        return _onWaitTask.Task;
    }

    public int Serve(Cocktail expected, Cocktail actual)
    {
        if (_onWaitTask == null)
        {
            throw new InvalidOperationException("Client is not awaiting");
        }

        _onWaitTask.TrySetResult(true);
        waitingSlider.gameObject.SetActive(false);
        Clean();

        return _rules.Sum(rule => rule(expected, actual));
    }

    public int Pay(int expectedPrice, int satisfaction)
    {
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
        if (_onWaitTask == null)
        {
            return;
        }

        _timeAwaited += Time.deltaTime;
        var percent = 100 - ((_timeAwaited / Patience) * 100);
        waitingSlider.value = Patience - _timeAwaited;
        waitingImage.color = Satisfaction.GetColor((int) percent);

        if (_timeAwaited < Patience)
        {
            return;
        }

        _onWaitTask?.TrySetCanceled();
        waitingSlider.gameObject.SetActive(false);

        Clean();
    }

    private void StepIdle()
    {
        _animator.SetInteger("State", AnimIdle);
    }

    private void StepMove()
    {
        var distance = Vector2.Distance(transform.position, _dst);
        if (distance > 1)
        {
            _animator.SetInteger("State", AnimMove);
            Step();
        }
        else
        {
            _animator.SetInteger("State", AnimIdle);
            _onMoveTask?.TrySetResult(true);
            Clean();
        }
    }

    private void Step()
    {
        var position = transform.position;
        _spriteRenderer.flipX = _dst.x < position.x;
        transform.position = Vector2.MoveTowards(position, _dst, Speed * Time.deltaTime);
    }

    private void Clean()
    {
        _state = State.Idle;
        _dst = Vector2.zero;
        _onMoveTask = null;
        _onWaitTask = null;
        _timeAwaited = 0f;
    }

    private enum State
    {
        Idle,
        Move
    }
}