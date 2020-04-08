using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Core;

public class Character : MonoBehaviour
{
    private enum State
    {
        Idle,
        Move,
        Follow
    };

    public float speed = 30f;
    public float patience = 10f;
    public int Id { get; set; }

    /* Properties */
    private State _state = State.Idle;
    private Vector2 _dst;
    private Character _leader;
    private TaskCompletionSource<bool> _onMoveTask;
    private TaskCompletionSource<Cocktail> _onWaitTask;
    private float _timeAwaited = 0f;
    private readonly List<Func<Cocktail, Cocktail, float>> _rules = new List<Func<Cocktail, Cocktail, float>>();

    /* Unity Injection */
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    /* Constants */
    private const int AnimIdle = 0;
    private const int AnimMove = 1;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rules.Add(Rules.BaseRule);
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
            throw new InvalidOperationException("Client is alreay moving, can't move again");
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
            throw new InvalidOperationException("Client is already moving, can't follow");
        }
        _leader = character;
        _state = State.Follow;
    }

    public Task<Cocktail> Await()
    {
        if (_onWaitTask != null)
        {
            throw new InvalidOperationException("Client is already awaiting for something");
        }
        _onWaitTask = new TaskCompletionSource<Cocktail>(TaskCreationOptions.RunContinuationsAsynchronously);
        return _onWaitTask.Task;
    }

    public void Serve(Cocktail cocktail)
    {
        if (_onWaitTask == null)
        {
            throw new InvalidOperationException("Client is not awaiting for something");
        }
        _onWaitTask.TrySetResult(cocktail);
        Clean();
    }

    public int Check(Cocktail expected, Cocktail actual)
    {
        return (int)_rules.Sum(rule => rule(expected, actual));
    }

    private void StepWait()
    {
        if (_onWaitTask == null || Math.Abs(patience - (-1)) < 0.1)
        {
            return;
        }

        _timeAwaited += Time.deltaTime;
        if (!(_timeAwaited > patience))
        {
            return;
        }

        _onWaitTask?.TrySetCanceled();
        _spriteRenderer.color = Color.red;
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
        _spriteRenderer.flipX = _dst.x < position.x ? true : false;
        transform.position = Vector2.MoveTowards(position, _dst, speed * Time.deltaTime);
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
}
