using System;
using Core;
using UnityEngine;

namespace Singleton
{
    public class CashManager : MonoBehaviour
    {
        private const int GlassPrice = 5;
        private const int LemonPrice = 5;
        private const int StrawberryPrice = 5;

        private const int MojitoPrice = 5;
        private const int CubaLibrePrice = 10;
        private const int RumPrice = 5;
        private const int ColaPrice = 3;
        private const int LemonadePrice = 2;

        public static CashManager Main;

        public int Cash { get; set; }

        private void Awake()
        {
            Main = this;
        }

        public static int GetPrice(IngredientKey ingredient)
        {
            switch (ingredient)
            {
                case IngredientKey.Lemon:
                    return LemonPrice;
                case IngredientKey.Strawberry:
                    return StrawberryPrice;
                default:
                    return 0;
            }
        }

        public static int GetPrice(CocktailKey cocktail)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(cocktail), cocktail, null);
            }
        }
    }
}