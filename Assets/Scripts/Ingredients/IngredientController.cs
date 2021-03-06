﻿using System;
using System.Collections.Generic;
using Components;
using UnityEngine;

namespace Ingredients
{
    [Serializable]
    public class IngredientEntry
    {
        public IngredientKey key;
        public GameObject prefab;
    }

    public class IngredientController : Controller
    {
        private readonly Dictionary<IngredientKey, GameObject> _prefabs = new Dictionary<IngredientKey, GameObject>();
        public IngredientEntry[] entries;
        public Transform spawn;

        private void Awake()
        {
            foreach (var spawnEntry in entries) _prefabs.Add(spawnEntry.key, spawnEntry.prefab);
        }

        public void Spawn(IngredientKey key)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            CreateObject(prefab, spawn, prefab.name);
        }
    }
}