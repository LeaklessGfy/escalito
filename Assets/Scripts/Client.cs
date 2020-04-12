using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    /* CONSTANT */
    private const int AnimIdle = 0;
    private const int AnimMove = 1;
    private const float Speed = 30f;
    private const float Patience = 10f;

    private readonly List<Func<Cocktail, Cocktail, float>> _rules = new List<Func<Cocktail, Cocktail, float>>();

    /* UNITY */
    private Animator _animator;
    private Button _orderButton;
    private TextMeshProUGUI _orderText;
    public TextMeshProUGUI _cashText;
    private Slider _slider;
    private SpriteRenderer _spriteRenderer;

    /* STATE */
    private Vector2 _dst;
    private TaskCompletionSource<bool> _onMoveTask;
    private TaskCompletionSource<bool> _onWaitTask;
    private State _state = State.Idle;
    private float _timeAwaited;

    private Cocktail _order;

    public event Action<Client, Glass> CollisionListeners;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _orderButton = GetComponentInChildren<Button>();
        _orderText = _orderButton.GetComponentInChildren<TextMeshProUGUI>();
        _slider = GetComponentInChildren<Slider>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _orderButton.gameObject.SetActive(false);
        _orderButton.onClick.AddListener(() =>
        {
            _timeAwaited -= 1;
        });
        _cashText.gameObject.SetActive(false);
        _slider.minValue = 0;
        _slider.maxValue = Patience;
        _slider.gameObject.SetActive(false);

        _rules.Add(Rules.CocktailRule);
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
        _orderButton.gameObject.SetActive(true);
        _orderText.text = _order.Name.ToString();
        return _order;
    }

    public Task Await()
    {
        if (_onWaitTask != null)
        {
            throw new InvalidOperationException("Client is already awaiting");
        }

        _onWaitTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _slider.gameObject.SetActive(true);

        return _onWaitTask.Task;
    }

    public float Serve(Cocktail expected, Cocktail actual)
    {
        if (_onWaitTask == null)
        {
            throw new InvalidOperationException("Client is not awaiting");
        }

        _onWaitTask.TrySetResult(true);
        _slider.gameObject.SetActive(false);
        Clean();

        return _rules.Sum(rule => rule(expected, actual));
    }

    public void Pay(int price)
    {
        _cashText.gameObject.SetActive(true);
        _cashText.text = "+" + price + " $";
    }

    public void Satisfaction(float satisfaction)
    {
        var color = Color.green;
        if (satisfaction < 20)
        {
            color = Color.red;
        }
        else if (satisfaction < 60)
        {
            color = Color.yellow;
        }
        _spriteRenderer.color = color;
    }

    public void Leave()
    {
        _orderButton.gameObject.SetActive(false);
        _slider.gameObject.SetActive(false);
    }

    private void StepWait()
    {
        if (_onWaitTask == null)
        {
            return;
        }

        _timeAwaited += Time.deltaTime;
        _slider.value = Patience - _timeAwaited;
        if (_timeAwaited < Patience)
        {
            return;
        }

        _onWaitTask?.TrySetCanceled();
        _slider.gameObject.SetActive(false);

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

    /*private void StepFollow()
    {
        var position = transform.position;
        _dst = new Vector2(_leader.transform.position.x, position.y);

        var distance = Vector2.Distance(position, _dst);
        if (distance > _spriteRenderer.bounds.extents.x + 10)
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
    }*/

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