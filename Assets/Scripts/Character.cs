using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Core;
using System.Linq;

public class Character : MonoBehaviour
{
    enum State
    {
        Idle,
        Move,
        Follow
    };

    public float speed = 30f;
    public float patience = 10f;
    public int id;

    /* Properties */
    private State state = State.Idle;
    private Vector2 dst;
    private Character leader;
    private TaskCompletionSource<bool> onMoveTask;
    private TaskCompletionSource<Cocktail> onWaitTask;
    private float timeAwaited = 0f;
    private readonly List<Func<Cocktail, Cocktail, float>> rules = new List<Func<Cocktail, Cocktail, float>>();

    /* Unity Injection */
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    /* Constants */
    private static readonly int ANIM_IDLE = 0;
    private static readonly int ANIM_MOVE = 1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rules.Add(Rules.BaseRule);
    }

    private void Update()
    {
        StepWait();
        switch (state)
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
        }
    }

    public Task MoveTo(Vector2 position)
    {
        if (onMoveTask != null)
        {
            throw new InvalidOperationException("Client is alreay moving, can't move again");
        }
        dst = new Vector2(position.x, transform.position.y);
        state = State.Move;
        onMoveTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        return onMoveTask.Task;
    }

    public void Follow(Character character)
    {
        if (onMoveTask != null)
        {
            throw new InvalidOperationException("Client is already moving, can't follow");
        }
        leader = character;
        state = State.Follow;
    }

    public Task<Cocktail> Await()
    {
        if (onWaitTask != null)
        {
            throw new InvalidOperationException("Client is already awaiting for something");
        }
        onWaitTask = new TaskCompletionSource<Cocktail>(TaskCreationOptions.RunContinuationsAsynchronously);
        return onWaitTask.Task;
    }

    public void Serve(Cocktail cocktail)
    {
        if (onWaitTask == null)
        {
            throw new InvalidOperationException("Client is not awaiting for something");
        }
        onWaitTask.TrySetResult(cocktail);
        Clean();
    }

    public int Check(Cocktail expected, Cocktail actual)
    {
        return (int)rules.Sum(rule => rule(expected, actual));
    }

    private void StepWait()
    {
        if (onWaitTask == null || patience == -1)
        {
            return;
        }
        timeAwaited += Time.deltaTime;
        if (timeAwaited > patience)
        {
            onWaitTask?.TrySetCanceled();
            spriteRenderer.color = Color.red;
            Clean();
        }
    }

    private void StepIdle()
    {
        animator.SetInteger("State", ANIM_IDLE);
    }

    private void StepMove()
    {
        float distance = Vector2.Distance(transform.position, dst);
        if (distance > 1)
        {
            animator.SetInteger("State", ANIM_MOVE);
            Step();
        }
        else
        {
            animator.SetInteger("State", ANIM_IDLE);
            onMoveTask?.TrySetResult(true);
            Clean();
        }
    }

    private void StepFollow()
    {
        if (leader == null)
        {
            throw new InvalidOperationException("Client should have a leader to follow");
        }

        dst = new Vector2(leader.transform.position.x, transform.position.y);

        float distance = Vector2.Distance(transform.position, dst);
        if (distance > spriteRenderer.bounds.extents.x + 10)
        {
            animator.SetInteger("State", ANIM_MOVE);
            Step();
        }
        else
        {
            animator.SetInteger("State", ANIM_IDLE);
        }
    }

    private void Step()
    {
        spriteRenderer.flipX = dst.x < transform.position.x ? true : false;
        transform.position = Vector2.MoveTowards(transform.position, dst, speed * Time.deltaTime);
    }

    private void Clean()
    {
        state = State.Idle;
        dst = Vector2.zero;
        leader = null;
        onMoveTask = null;
        onWaitTask = null;
        timeAwaited = 0f;
    }
}
