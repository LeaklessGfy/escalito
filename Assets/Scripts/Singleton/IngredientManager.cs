using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Singleton
{
    [Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        public IngredientKey key;
    }

    public class IngredientManager : MonoBehaviour
    {
        public static IngredientManager Main;

        private readonly Dictionary<IngredientKey, GameObject> _prefabFactory = new Dictionary<IngredientKey, GameObject>();

        private int _id;

        [SerializeField] private Transform spawn;
        [SerializeField] private SpawnEntry[] spawnEntries;

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