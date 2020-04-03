using System;
using System.Collections.Generic;

public class Cocktail
{
    public enum CocktailType
    {
      Mojito
    };

    public enum Consumable
    {
        Rhum
    };

    public CocktailType Type
    {
        get; private set;
    }
    public Dictionary<Consumable, int> Formula
    {
        get; private set;
    }

    private Cocktail(CocktailType type, Dictionary<Consumable, int> formula)
    {
        this.Type = type;
        this.Formula = formula;
    }

    public static Cocktail Build(CocktailType type)
    {
        Dictionary<Consumable, int> dict = BuildFormula(type);
        return new Cocktail(type, dict);
    }

    public static Cocktail BuildRandom()
    {
        string[] types = Enum.GetNames(typeof(CocktailType));
        int rand = UnityEngine.Random.Range(0, types.Length);
        CocktailType type;
        Enum.TryParse(types[rand], out type);

        return Build(type);
    }

    private static Dictionary<Consumable, int> BuildFormula(CocktailType type)
    {
        Dictionary<Consumable, int> dict = new Dictionary<Consumable, int>();
        switch (type)
        {
            case CocktailType.Mojito:
                dict.Add(Consumable.Rhum, 100);
                return dict;
        }
        throw new ArgumentException("No formula for specific type " + type);
    }
}
