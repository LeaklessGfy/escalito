using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class Sponsor : Character
    {
        private readonly Contract _contract = new Contract();

        public Button noButton;
        public Button yesButton;
        public GameObject panel;

        protected new void Awake()
        {
            base.Awake();
            panel.SetActive(false);
        }

        public void AskContract()
        {
            yesButton.interactable = CashController.Main.Cash >= _contract.Price;
            panel.SetActive(true);
        }
        
        public void RefuseContract()
        {
            panel.SetActive(false);
        }

        public void AcceptContract()
        {
            CashController.Main.Pay(_contract.Price);
            CashController.Main.Expenses.Add(_contract.Expense);
            CashController.Main.Bonuses.Add(_contract.Bonus);
            CashController.Main.Penalties.Add(_contract.Penalty);
            panel.SetActive(false);
        }
        
        protected override bool Flip(float x)
        {
            return x > transform.position.x;
        }
    }
}