using System;
using System.Collections.Generic;
using Components;
using UnityEngine;

namespace Cocktails
{
    [Serializable]
    public class GlassEntry
    {
        public GlassKey key;
        public GameObject prefab;
    }

    public class GlassController : Controller
    {
        public static GlassController Main;

        private readonly Dictionary<GlassKey, GameObject> _prefabs = new Dictionary<GlassKey, GameObject>();
        public List<GlassEntry> entries;
        public Transform spawn;

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in entries) _prefabs.Add(spawnEntry.key, spawnEntry.prefab);
        }

        public Glass Spawn(GlassKey key = GlassKey.Default)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            return CreateComponent<Glass>(prefab, spawn, prefab.name);
        }
    }
}