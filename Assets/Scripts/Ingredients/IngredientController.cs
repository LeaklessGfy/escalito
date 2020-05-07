using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ingredients
{
    [Serializable]
    public class IngredientEntry
    {
        public IngredientKey key;
        public GameObject prefab;
    }

    public class IngredientController : MonoBehaviour
    {
        public static IngredientController Main;

        private readonly Dictionary<IngredientKey, GameObject> _prefabs = new Dictionary<IngredientKey, GameObject>();
        [SerializeField] private IngredientEntry[] entries;
        [SerializeField] private Transform spawn;

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in entries) _prefabs.Add(spawnEntry.key, spawnEntry.prefab);
        }

        public void Spawn(IngredientKey key)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            MainController.CreateObject(prefab, spawn, prefab.name);
        }
    }
}