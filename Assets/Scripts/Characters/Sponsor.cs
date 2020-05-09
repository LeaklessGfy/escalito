using Cash;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class Sponsor : Character
    {
        private Contract _contract;

        public Button noButton;
        public Button yesButton;
        public GameObject panel;
        public Text priceText;
        public Text expenseText;
        public Text bonusText;
        public Text penaltyText;

        protected new void Awake()
        {
            base.Awake();
            panel.SetActive(false);
        }

        public void AskContract()
        {
            _contract = Contract.Build(MainController.Main);

            yesButton.interactable = CashController.Main.Cash >= _contract.Price;
            priceText.text = $"Price : - {_contract.Price} $";
            expenseText.text =  $"Tax : - {_contract.Expense.Amount} $ / Day";
            bonusText.text =  $"Bonus : {_contract.Bonus.Amount} x Combo";
            penaltyText.text =  $"Penalty : - {_contract.Penalty.Amount} $";;
            
            panel.SetActive(true);
        }
        
        public void RefuseContract()
        {
            panel.SetActive(false);
        }

        public void AcceptContract()
        {
            CashController.Main.Pay(_contract.Price);
            CashController.Main.ExpenseManager.Add(_contract.Expense);
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