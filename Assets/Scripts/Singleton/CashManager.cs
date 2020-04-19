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

        public int GetPrice(Spawnable spawnable)
        {
            switch (spawnable)
            {
                case Spawnable.Glass:
                    return GlassPrice;
                case Spawnable.Lemon:
                    return LemonPrice;
                case Spawnable.Strawberry:
                    return StrawberryPrice;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spawnable), spawnable, null);
            }
        }

        public int GetPrice(CocktailKey cocktail)
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