using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Characters
{
    public abstract class Character : MonoBehaviour
    {
        private const int AnimIdle = 0;
        private const int AnimMove = 1;
        private const float Speed = 30f;
        private static readonly int StateKey = Animator.StringToHash("State");

        protected readonly HashSet<State> States = new HashSet<State> {State.Idle};

        private Animator _animator;
        private float _distance;
        private Vector2 _dst;
        private TaskCompletionSource<bool> _onArriveTask;
        protected SpriteRenderer SpriteRenderer;

        public float Offset => SpriteRenderer.sprite.bounds.extents.x;

        protected void Awake()
        {
            _animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected void Update()
        {
            if (States.Contains(State.Idle))
            {
                StepIdle();
            }

            if (States.Contains(State.Move))
            {
                StepMove();
            }
        }

        public bool IsNear(Vector2 dst, float offset, float distance)
        {
            return Vector2.Distance(transform.position, Normalize(dst, offset)) <= distance;
        }

        public void MoveTo(Vector2 dst, float offset = 0f, float distance = 0f)
        {
            if (dst == _dst || IsNear(dst, offset, distance))
            {
                return;
            }

            _dst = Normalize(dst, offset);
            _distance = distance;
            States.Remove(State.Idle);
            States.Add(State.Move);
            SpriteRenderer.flipX = Flip(_dst.x);
        }

        public Task<bool> MoveToAsync(Vector2 dst, float offset = 0f, float distance = 0f)
        {
            if (dst == _dst || IsNear(dst, offset, distance))
            {
                return Task.FromResult(true);
            }

            _onArriveTask?.SetResult(false);
            _onArriveTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            MoveTo(dst, offset, distance);

            return _onArriveTask.Task;
        }

        private Vector2 Normalize(Vector2 dst, float offset)
        {
            return new Vector2(dst.x + offset, transform.position.y);
        }

        private void StepIdle()
        {
            _animator.SetInteger(StateKey, AnimIdle);
        }

        private void StepMove()
        {
            if (Vector2.Distance(transform.position, _dst) > _distance)
            {
                _animator.SetInteger(StateKey, AnimMove);
                transform.position = Vector2.MoveTowards(transform.position, _dst, Speed * Time.deltaTime);
            }
            else
            {
                States.Remove(State.Move);
                States.Add(State.Idle);

                _animator.SetInteger(StateKey, AnimIdle);
                _dst = Vector2.zero;
                _distance = 0;

                _onArriveTask?.SetResult(true);
                _onArriveTask = null;
            }
        }

        protected abstract bool Flip(float x);

        protected enum State
        {
            Idle,
            Move,
            Wait,
            Exhausted
        }
    }
}