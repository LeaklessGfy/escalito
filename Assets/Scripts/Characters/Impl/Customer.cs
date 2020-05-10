using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cocktails;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Impl
{
    public class Customer : Character
    {
        private readonly List<Func<Cocktail, Cocktail, int>> _rules = new List<Func<Cocktail, Cocktail, int>>();

        private Func<Order> _orderFactory;
        private Glass _glass;

        public Text cashText;
        public Image orderImage;

        public bool Satisfied => Satisfaction > PercentHelper.Low;
        public int Satisfaction { get; private set; }
        public Order Order { get; private set; }

        public void Init(PositionBag position, Action<Character> onLeave, Func<Order> orderFactory)
        {
            Init(position, onLeave);

            _orderFactory = orderFactory;
            _rules.Add(Rules.CocktailRule);

            cashText.gameObject.SetActive(false);
            orderImage.gameObject.SetActive(false);
        }

        public void Behave(Vector2 next)
        {
            if (!State.Moving && !State.Waiting && !IsNear(next, 4))
            {
                MoveTo(next);
            }

            if (State.Leaving)
            {
                throw new InvalidAsynchronousStateException("Should not behave if leave");
            }

            if (Order == null && IsNear(Position.Bar))
            {
                AskOrder();
                Await(MainController.Main.Difficulty);
                _glass = GlassController.Main.Spawn();
            }
            else if (State.Exhausted)
            {
                Leave();
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (Order == null)
            {
                return;
            }

            var glass = collision.gameObject.GetComponent<Glass>();

            if (glass == null || glass.Served)
            {
                return;
            }

            glass.Served = true;
            var actual = glass.Cocktail;
            Serve(actual);
            Leave();
        }

        protected override bool Flip(float x)
        {
            return x < transform.position.x;
        }

        private void AskOrder()
        {
            if (Order != null)
            {
                throw new InvalidOperationException("Customer has already order");
            }

            Order = _orderFactory();
            orderImage.sprite = CocktailController.Main.GetSprite(Order.Cocktail.Key);
            orderImage.gameObject.SetActive(true);
        }

        private void Serve(Cocktail actual)
        {
            if (!State.Waiting)
            {
                throw new InvalidOperationException("Customer is not awaiting");
            }

            State.UnWait();
            Satisfaction = _rules.Sum(rule => rule(Order.Cocktail, actual)) / _rules.Count;
            orderImage.gameObject.SetActive(false);
        }

        private void Pay(decimal price)
        {
            cashText.gameObject.SetActive(true);
            cashText.text = $"{(Satisfied ? '+' : '-')} {price} $";
            cashText.color = PercentHelper.GetColor(Satisfaction);
        }

        private async void Leave()
        {
            var amount = MainController.Main.Increment(this);
            SpriteRenderer.color = PercentHelper.GetColor(Satisfaction);
            orderImage.gameObject.SetActive(false);
            Pay(amount);
            Destroy(_glass.gameObject);

            if (!await LeaveToAsync(Position.Spawn))
            {
                throw new InvalidAsynchronousStateException("LeaveTo is called again");
            }

            Destroy(gameObject);
        }
    }
}