using System;
using System.Collections.Generic;
using System.Linq;
using Cash.Effect;
using Cash.Trigger;
using Characters.Impl;
using Cocktails;
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

        private readonly HashSet<Contract.Contract> _contracts = new HashSet<Contract.Contract>();
        private readonly HashSet<IEffect> _bonuses = new HashSet<IEffect>();
        private readonly HashSet<IEffect> _penalties = new HashSet<IEffect>();
        private readonly HashSet<CashTrigger> _cashTriggers = new HashSet<CashTrigger>();

        public Text cashText;
        public Text expenseText;

        public decimal Cash
        {
            get => _cash;
            set
            {
                _cash = value;
                if (_cash < 0)
                {
                    print("Game over");
                }
            }
        }

        private decimal _cash = 50;

        private void Awake()
        {
            _bonuses.Add(new SatisfactionBonus());
        }

        private void Update()
        {
            cashText.text = Cash + " $";
        }

        public void AddContract(Contract.Contract contract)
        { 
            Cash -= contract.Price;
            _bonuses.Add(contract.Bonus);
            _penalties.Add(contract.Penalty);
            _cashTriggers.Add(contract.CashTrigger);
            _contracts.Add(contract);

            MagicBag.Bag.clock.TimeActions.Add(new CashTriggerAction(contract.CashTrigger));
        }

        public void RemoveContracts()
        {
            foreach (var contract in _contracts)
            {
                _bonuses.Remove(contract.Bonus);
                _penalties.Remove(contract.Penalty);
            }
        }

        public decimal ApplyBonuses(Customer customer)
        {
            var finalAmount = _bonuses.Aggregate(customer.Order.Price, (current, effect) => effect.Apply(customer, current));
            Cash += finalAmount;

            return finalAmount;
        }

        public decimal ApplyPenalty(Customer customer)
        {
            var finalAmount = _penalties.Aggregate(customer.Order.Price, (current, effect) => effect.Apply(customer, current));
            Cash -= finalAmount;

            return finalAmount;
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