using System;
using Cocktails;
using Ingredients;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private Text cashText;

    public int Cash { get; set; } = 50;

    private void Awake()
    {
        Main = this;
    }

    private void Update()
    {
        cashText.text = Cash + " $";
    }

    public static int GetPrice(IngredientKey ingredient)
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
            case CocktailKey.Custom:
                return 0;
            default:
                throw new ArgumentOutOfRangeException(nameof(cocktail), cocktail, null);
        }
    }
}