using System;
using System.Collections.Generic;
using System.Linq;
using Cocktails;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class Customer : Character
    {
        private const float Patience = 10f;
        private readonly List<Func<Cocktail, Cocktail, int>> _rules = new List<Func<Cocktail, Cocktail, int>>();
        private float _currentPatience;

        private Order _order;
        private int _servedCount;
        private float _timeAwaited;

        public Text cashText;
        public Image orderImage;
        public Image waitingImage;
        public Slider waitingSlider;

        public Func<Order> OrderBuilder { private get; set; }
        public bool HasOrder => _order != null;
        public bool Satisfied => Satisfaction > PercentHelper.Low;
        public int Satisfaction { get; private set; }

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

            var glass = collision.gameObject.GetComponent<Glass>();

            if (glass == null || glass.Served)
            {
                return;
            }

            glass.Served = true;
            CharacterController.Main.ReceiveOrder(this, glass);
        }

        public void AskOrder()
        {
            if (_order != null)
            {
                throw new InvalidOperationException("Customer has already order");
            }

            _order = OrderBuilder();
            orderImage.sprite = CocktailController.Main.GetSprite(_order.Cocktail.Key);
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


        private int Try(Cocktail expected, Cocktail actual)
        {
            return _rules.Sum(rule => rule(expected, actual)) / _rules.Count;
        }

        public decimal Serve(Cocktail actual)
        {
            if (!States.Contains(State.Wait))
            {
                throw new InvalidOperationException("Customer is not awaiting");
            }

            States.Remove(State.Wait);
            _timeAwaited = 0;
            Satisfaction = Try(_order.Cocktail, actual);

            orderImage.gameObject.SetActive(false);
            waitingSlider.gameObject.SetActive(false);
            
            if (!Satisfied)
            {
                return 0;
            }

            var price = _order.Cocktail.Price;
            cashText.gameObject.SetActive(true);
            cashText.text = "+" + price + "$";
            cashText.color = PercentHelper.GetColor(Satisfaction);

            return price;
        }


        public new void LeaveTo(Vector2 dst)
        {
            base.LeaveTo(dst);
            SpriteRenderer.color = PercentHelper.GetColor(Satisfaction);
            orderImage.gameObject.SetActive(false);
            waitingSlider.gameObject.SetActive(false);
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

            States.Remove(State.Wait);
            States.Add(State.Exhausted);
        }

        protected override bool Flip(float x)
        {
            return x < transform.position.x;
        }
    }
}