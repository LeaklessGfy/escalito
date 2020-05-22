using System;
using System.ComponentModel;
using Cash;
using Cash.Contract;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Impl
{
    public class Sponsor : Character
    {
        private Contract _contract;
        public Text bonusText;
        public Text expenseText;

        public Button noButton;
        public GameObject panel;
        public Text penaltyText;
        public Text priceText;
        public Button yesButton;

        public new void Init(PositionBag position, Action<Character> onLeave)
        {
            base.Init(position, onLeave);

            panel.SetActive(false);
            noButton.onClick.AddListener(RefuseContract);
            yesButton.onClick.AddListener(AcceptContract);

            var t = transform;
            var p = t.position;
            t.position = new Vector3(p.x, p.y + 5, p.z);
        }

        protected override bool Flip(float x)
        {
            return x > transform.position.x;
        }

        public void Behave(Vector2 next)
        {
            if (!State.Moving)
            {
                MoveTo(next);
            }
            
            if (State.Leaving)
            {
                throw new InvalidAsynchronousStateException("Should not behave if leave");
            }

            if (_contract == null && IsNear(Position.Bar))
            {
                AskContract();
                Await(MainController.Main.Difficulty);
            }
            else if (State.Exhausted)
            {
                RefuseContract();
            }
        }

        private void AskContract()
        {
            _contract = ContractBuilder.Build(MainController.Main);

            yesButton.interactable = CashController.Main.Cash >= _contract.Price;
            priceText.text = $"Price : - {_contract.Price} $";
            expenseText.text = $"Tax : - {_contract.CashTrigger.Amount} $ / Day";
            bonusText.text = $"Bonus : {_contract.Bonus.Amount} x Combo";
            penaltyText.text = $"Penalty : - {_contract.Penalty.Amount} $";

            panel.SetActive(true);
        }

        private void RefuseContract()
        {
            Leave();
        }

        private void AcceptContract()
        {
            CashController.Main.AddContract(_contract);
            Leave();
        }

        private async void Leave()
        {
            panel.SetActive(false);

            if (!await LeaveToAsync(Position.Spawn))
            {
                throw new InvalidAsynchronousStateException("LeaveTo is called again");
            }

            Destroy(gameObject);
        }
    }
}