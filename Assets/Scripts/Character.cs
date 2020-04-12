using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    /* Constants */
    private const int AnimIdle = 0;
    private const int AnimMove = 1;
    private const float Speed = 30f;
    private const float Patience = 10000f;

    private readonly List<Func<Cocktail, Cocktail, float>> _rules = new List<Func<Cocktail, Cocktail, float>>();

    private Animator _animator;
    private Vector2 _dst;
    private Character _leader;
    private TaskCompletionSource<bool> _onMoveTask;
    private TaskCompletionSource<bool> _onWaitTask;
    private Slider _slider;
    private SpriteRenderer _spriteRenderer;
    private State _state = State.Idle;
    private float _timeAwaited;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _slider = GetComponentInChildren<Slider>();
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
            case State.Follow:
                StepFollow();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Task MoveToAsync(Vector2 position)
    {
        if (_onMoveTask != null)
        {
            throw new InvalidOperationException("Client is already moving, can't move again");
        }

        _dst = new Vector2(position.x, transform.position.y);
        _state = State.Move;
        _onMoveTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        return _onMoveTask.Task;
    }

    public void Follow(Character character)
    {
        if (_onMoveTask != null)
        {
            throw new InvalidOperationException("Client is moving, can't follow");
        }

        _leader = character;
        _state = State.Follow;
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

    public void PatienceBonus(int bonus)
    {
        _timeAwaited -= bonus;
    }

    public void Satisfaction(float satisfaction)
    {
        _spriteRenderer.color = satisfaction <= 0 ? Color.red : Color.green;
    }

    private void StepWait()
    {
        if (_onWaitTask == null || Patience < -1)
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

    private void StepFollow()
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
        _leader = null;
        _onMoveTask = null;
        _onWaitTask = null;
        _timeAwaited = 0f;
    }

    private enum State
    {
        Idle,
        Move,
        Follow
    }
}