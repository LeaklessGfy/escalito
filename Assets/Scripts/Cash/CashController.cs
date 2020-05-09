﻿using System;
using System.Linq;
using System.Text;
using Cash.Effect;
using Cash.Expense;
using Characters;
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

        private const int ExpenseTime = 10;

        public static CashController Main;
        private TimingAction _expenseAction;

        public Text cashText;
        public Text expenseText;

        public decimal Cash { get; private set; } = 50;
        public ExpenseManager ExpenseManager { get; } = new ExpenseManager();
        public EffectManager Bonuses { get; } = new EffectManager();
        public EffectManager Penalties { get; } = new EffectManager();

        private void Awake()
        {
            Main = this;
            _expenseAction = new TimingAction(ExpenseTime, ExpenseCondition, ExpenseTick, ExpenseTrigger);
        }

        private void Update()
        {
            cashText.text = Cash + " $";
            _expenseAction.Tick(Time.deltaTime);
        }

        private bool ExpenseCondition()
        {
            return ExpenseManager.HasExpense();
        }
    
        private void ExpenseTick(float current, float trigger)
        {
            var diff = trigger - current;
            var percent = diff / trigger * 100;
            if (percent < 70)
            {
                expenseText.text = "";
            }
        }

        private float ExpenseTrigger()
        {
            var expensesSum = ExpenseManager.Sum();
            var text = expensesSum
                .Aggregate(new StringBuilder(), (sb, pair) => sb.AppendLine($"{pair.Key} : -{pair.Value} $"))
                .ToString();
            var total = expensesSum.Aggregate(0m, (sum, pair) => sum + pair.Value);

            expenseText.text = text;
            expenseText.color = PercentHelper.GetColor((Cash - total) / Cash * 100);

            Pay(total);
            AudioController.Main.cash.Play();

            return 10;
        }

        public void Bonus(Customer customer, decimal amount)
        {
            Cash += Bonuses.Apply(customer, amount);
        }

        public void Penalty(Customer customer)
        {
            Cash -= Penalties.Apply(customer, 0);
        }

        public void Pay(decimal price)
        {
            Cash -= price;
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