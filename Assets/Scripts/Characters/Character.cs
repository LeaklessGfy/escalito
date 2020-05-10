using System;
using System.Threading.Tasks;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public abstract class Character : MonoBehaviour
    {
        private const float Patience = 20f;
        private const int AnimIdle = 0;
        private const int AnimMove = 1;
        private const float Speed = 30f;
        private static readonly int StateKey = Animator.StringToHash("State");
        protected readonly State State = new State();

        private Animator _animator;
        private float _currentPatience;
        private float _distance;
        private Vector2 _dst;
        private TaskCompletionSource<bool> _onArriveTask;
        private Action<Character> _onLeave;
        private float _timeAwaited;
        protected PositionBag Position;
        protected SpriteRenderer SpriteRenderer;

        public Image waitingImage;
        public Slider waitingSlider;

        public float Offset => SpriteRenderer.sprite.bounds.extents.x;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            waitingSlider.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (State.Idling)
            {
                StepIdle();
            }
            else if (State.Moving)
            {
                StepMove();
            }

            if (State.Waiting)
            {
                StepWait();
            }
        }

        protected void Init(PositionBag position, Action<Character> onLeave)
        {
            Position = position;
            _onLeave = onLeave;
        }

        protected abstract bool Flip(float x);

        protected bool IsNear(Vector2 dst, float distance = 0f)
        {
            return Vector2.Distance(transform.position, OnlyX(dst)) <= distance;
        }

        protected void MoveTo(Vector2 dst, float distance = 0f)
        {
            if (dst == _dst)
            {
                return;
            }

            _dst = dst;
            _distance = distance;

            State.Move();
            SpriteRenderer.flipX = Flip(_dst.x);
        }

        protected Task<bool> MoveToAsync(Vector2 dst, float distance = 0f)
        {
            _onArriveTask?.SetResult(false);
            _onArriveTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            MoveTo(dst, distance);

            return _onArriveTask.Task;
        }

        protected void LeaveTo(Vector2 dst)
        {
            State.Leave();
            waitingSlider.gameObject.SetActive(false);
            _onLeave?.Invoke(this);
            MoveTo(dst);
        }

        protected Task<bool> LeaveToAsync(Vector2 dst)
        {
            State.Leave();
            waitingSlider.gameObject.SetActive(false);
            _onLeave?.Invoke(this);

            var pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, 10);

            return MoveToAsync(dst);
        }

        protected void Await(int difficulty)
        {
            if (State.Waiting)
            {
                throw new InvalidOperationException("Customer is already awaiting");
            }

            State.Wait();
            _timeAwaited = 0;
            _currentPatience = Patience - difficulty;

            waitingSlider.gameObject.SetActive(true);
            waitingSlider.minValue = 0;
            waitingSlider.maxValue = _currentPatience;
        }

        private void StepIdle()
        {
            _animator.SetInteger(StateKey, AnimIdle);
        }

        private void StepMove()
        {
            if (IsNear(_dst, _distance))
            {
                State.Idle();

                _distance = 0;
                _onArriveTask?.SetResult(true);
                _onArriveTask = null;
            }
            else
            {
                _animator.SetInteger(StateKey, AnimMove);
                transform.position = ComputeTransform();
            }
        }

        private void StepWait()
        {
            _timeAwaited += Time.deltaTime;
            waitingSlider.value = _currentPatience - _timeAwaited;

            var percent = 100 - _timeAwaited / _currentPatience * 100;
            waitingImage.color = PercentHelper.GetColor((int) percent);

            if (_timeAwaited < _currentPatience)
            {
                return;
            }

            State.Exhaust();
        }

        private Vector3 ComputeTransform()
        {
            var pos = transform.position;
            var vec = Vector2.MoveTowards(
                pos,
                OnlyX(_dst),
                Speed * Time.deltaTime);

            return new Vector3(vec.x, vec.y, pos.z);
        }

        private Vector2 OnlyX(Vector2 dst)
        {
            return new Vector2(dst.x, transform.position.y);
        }
    }
}