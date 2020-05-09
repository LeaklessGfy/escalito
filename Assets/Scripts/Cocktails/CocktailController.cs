using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cocktails
{
    [Serializable]
    public class CocktailEntry
    {
        public CocktailKey key;
        public Sprite sprite;
    }

    public class CocktailController : MonoBehaviour
    {
        public static CocktailController Main;

        private readonly Dictionary<CocktailKey, Sprite> _mapping = new Dictionary<CocktailKey, Sprite>();

        public CocktailEntry[] entries;

        private void Awake()
        {
            Main = this;
            foreach (var sprite in entries) _mapping.Add(sprite.key, sprite.sprite);
        }

        public Sprite GetSprite(CocktailKey key)
        {
            return _mapping.TryGetValue(key, out var sprite) ? sprite : null;
        }
    }
}