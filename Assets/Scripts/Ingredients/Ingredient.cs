using System;
using UnityEngine;

namespace Ingredients
{
    [Serializable]
    public class Ingredient
    {
        public Color color;
        public IngredientKey key;
        public float stock;
    }
}