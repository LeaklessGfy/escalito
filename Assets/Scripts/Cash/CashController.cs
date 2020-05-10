using System;
using System.Linq;
using System.Text;
using Cash.Effect;
using Cash.Expense;
using Characters.Impl;
using Cocktails;
using Core;
using Ingredients;
using UnityEngine;
using UnityEngine.UI;

namespace Cash
{
    public class CashController : MonoBehaviour
    {
        private const int LemonPrice = 5;
        private const int StrawberryPrice = 5;
        private const int BottleRumPrice = 10;
        private const int BottleColaPrice = 10;
        private const int BottleLemonadePrice = 10;

        private const int MojitoPrice = 5;
        private const int CubaLibrePrice = 10;
        private const int RumPrice = 5;
        private const int ColaPrice = 3;
        private const int LemonadePrice = 2;

        public static CashController Main;

        public Text cashText;
        public Text expenseText;

        public decimal Cash { get; private set; } = 50;
        public ExpenseManager ExpenseManager { get; } = new ExpenseManager();
        public EffectManager Bonuses { get; } = new EffectManager();
        public EffectManager Penalties { get; } = new EffectManager();

        private void Awake()
        {
            Main = this;
            Bonuses.Add(new SatisfactionBonus());
        }

        private void Update()
        {
            cashText.text = Cash + " $";
        }

        public void Expense()
        {
            if (!ExpenseManager.HasExpense())
            {
                return;
            }
            
            var expensesSum = ExpenseManager.Sum();

            var text = expensesSum
                .Aggregate(new StringBuilder(), (sb, pair) => sb.AppendLine($"{pair.Key} : -{pair.Value} $"))
                .ToString();
            var total = expensesSum.Aggregate(0m, (sum, pair) => sum + pair.Value);

            expenseText.text = text;
            expenseText.color = PercentHelper.GetColor((Cash - total) / Cash * 100);

            Pay(total);

            AudioController.Main.cash.Play();

            Invoke(nameof(HideExpense), 2);
        }

        private void HideExpense()
        {
            expenseText.text = "";
        }

        public decimal Bonus(Customer customer)
        {
            var finalAmount = Bonuses.Apply(customer, customer.Order.Price);
            Cash += finalAmount;

            return finalAmount;
        }

        public decimal Penalty(Customer customer)
        {
            var finalAmount = Penalties.Apply(customer, customer.Order.Price);
            Pay(-finalAmount);

            return finalAmount;
        }

        public void Pay(decimal price)
        {
            Cash -= price;
            
            if (Cash < 0)
            {
                print("Game over");
            }
        }

        public static decimal GetPrice(IngredientKey ingredient)
        {
            switch (ingredient)
            {
                case IngredientKey.Rum:
                    return BottleRumPrice;
                case IngredientKey.Cola:
                    return BottleColaPrice;
                case IngredientKey.Lemonade:
                    return BottleLemonadePrice;
                case IngredientKey.Lemon:
                    return LemonPrice;
                case IngredientKey.Strawberry:
                    return StrawberryPrice;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ingredient), ingredient, null);
            }
        }

        public static decimal GetPrice(CocktailKey cocktail)
        {
            switch (cocktail)
            {
                case CocktailKey.Mojito:
                    return MojitoPrice;
                case CocktailKey.CubaLibre:
                    return CubaLibrePrice;
                case CocktailKey.Rum:
                    return RumPrice;
                case CocktailKey.Cola:
                    return ColaPrice;
                case CocktailKey.Lemonade:
                    return LemonadePrice;
                case CocktailKey.Custom:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cocktail), cocktail, null);
            }
        }
    }
}