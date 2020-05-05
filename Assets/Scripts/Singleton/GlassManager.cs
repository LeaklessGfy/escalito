using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Singleton
{
    [Serializable]
    public class GlassEntry
    {
        public GlassKey key;
        public GameObject prefab;
    }

    public class GlassManager : MonoBehaviour
    {
        public static GlassManager Main;

        private readonly Dictionary<GlassKey, GameObject> _prefabs = new Dictionary<GlassKey, GameObject>();
        [SerializeField] private List<GlassEntry> entries;
        [SerializeField] private Transform spawn;

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in entries) _prefabs.Add(spawnEntry.key, spawnEntry.prefab);
        }

        public GlassSprite Spawn(GlassKey key = GlassKey.Default)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            return Controller.CreateComponent<GlassSprite>(prefab, spawn, prefab.name);
        }
    }
}