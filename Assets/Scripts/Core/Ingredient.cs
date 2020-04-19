using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class Ingredient
    {
        public Color color;
        public IngredientKey key;
        public int stock;
    }
}