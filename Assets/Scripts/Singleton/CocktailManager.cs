using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Singleton
{
    [Serializable]
    public class CocktailEntry
    {
        public CocktailKey key;
        public Sprite sprite;
    }

    public class CocktailManager : MonoBehaviour
    {
        public static CocktailManager Main;

        private readonly Dictionary<CocktailKey, Sprite> _mapping = new Dictionary<CocktailKey, Sprite>();

        [SerializeField] private CocktailEntry[] sprites;

        private void Awake()
        {
            Main = this;
            foreach (var sprite in sprites) _mapping.Add(sprite.key, sprite.sprite);
        }

        public Sprite GetSprite(CocktailKey key)
        {
            return _mapping.TryGetValue(key, out var sprite) ? sprite : null;
        }
    }
}