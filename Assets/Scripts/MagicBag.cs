using Cash;
using Cocktails;
using UnityEngine;

public class MagicBag : MonoBehaviour
{
    public static MagicBag Bag;

    private MagicBag()
    {
        Bag = this;
    }
    
    public MainController main;
    public CashController cash;
    public ClockController clock;
    public AudioController audio;
    public CursorController cursor;
    public CocktailController cocktail;
    public GlassController glass;
}