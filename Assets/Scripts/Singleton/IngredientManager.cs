using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Singleton
{
    [Serializable]
    public class IngredientEntry
    {
        public IngredientKey key;
        public GameObject prefab;
    }

    public class IngredientManager : MonoBehaviour
    {
        public static IngredientManager Main;

        private readonly Dictionary<IngredientKey, GameObject> _prefabFactory =
            new Dictionary<IngredientKey, GameObject>();

        private int _id;

        [SerializeField] private Transform spawn;
        [SerializeField] private IngredientEntry[] spawnEntries;

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in spawnEntries) _prefabFactory.Add(spawnEntry.key, spawnEntry.prefab);
        }

        public void Spawn(IngredientKey key)
        {
            if (!_prefabFactory.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            Controller.CreateObject(prefab, spawn, prefab.name);
        }
    }
}