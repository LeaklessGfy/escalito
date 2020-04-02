using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Character : MonoBehaviour
{
    enum CharacterState
    {
        Idle = 0,
        Move = 1,
        Follow = 2
    };

    public Animator animator;
    public float speed = 3f;

    private CharacterState state { get; set; } = CharacterState.Idle;
    private Vector2 dst;
    private Character leader;
    private SpriteRenderer spriteRenderer;

    private TaskCompletionSource<bool> task;

    private static readonly int ANIM_IDLE = 0;
    private static readonly int ANIM_MOVE = 1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        switch (state) {
            case CharacterState.Idle:
                StepIdle();
                break;
            case CharacterState.Move:
                StepMove();
                break;
            case CharacterState.Follow:
                StepFollow();
                break;
        }
    }

    public Task MoveTo(Vector2 position)
    {
        if (task != null) {
            task.TrySetCanceled();
        }

        dst = new Vector2(position.x, transform.position.y);
        state = CharacterState.Move;
        task = new TaskCompletionSource<bool>();

        return task.Task;
    }

    public void Follow(Character character)
    {
        if (task != null) {
            task.TrySetCanceled();
        }
        leader = character;
        state = CharacterState.Follow;
    }

    public IEnumerable<int> Follow2(Character character)
    {
        if (task != null) {
            task.TrySetCanceled();
        }

        // yield progress, we calculate a progress by : change of leader, new position in queue (should be aware of its position ?)
        // break when finish : finished when no more leader to follow (again, should character be aware of its leader ?)
        yield return 1;
    }

    private void StepIdle()
    {
        animator.SetInteger("State", ANIM_IDLE);
    }

    private void StepMove()
    {
        float distance = Vector2.Distance(transform.position, dst);
        if (distance > 1) {
            animator.SetInteger("State", ANIM_MOVE);
            Step();
        } else {
            animator.SetInteger("State", ANIM_IDLE);
            state = CharacterState.Idle;
            dst = Vector2.zero;
            if (task != null) {
                task.TrySetResult(true);
                task = null;
            }
        }
    }

    private void StepFollow()
    {
        if (leader == null) {
            state = CharacterState.Idle;
            dst = Vector2.zero;
            return;
        }

        dst = new Vector2(leader.transform.position.x, transform.position.y);
        
        float distance = Vector2.Distance(transform.position, dst);
        if (distance > spriteRenderer.bounds.size.x / 2) {
            animator.SetInteger("State", ANIM_MOVE);
            Step();
        } else {
            animator.SetInteger("State", ANIM_IDLE);
        }
    }

    private void Step()
    {
        spriteRenderer.flipX = dst.x < transform.position.x ? true : false;
        transform.position = Vector2.MoveTowards(transform.position, dst, speed * Time.deltaTime);
    }
}
