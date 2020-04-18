﻿using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Singleton
{
    [Serializable]
    public class SpawnEntry
    {
        public Spawnable spawnable;
        public GameObject prefab;
        public Transform spawn;
    }
        
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Main;

        [SerializeField] private SpawnEntry[] spawnEntries;

        private readonly Dictionary<Spawnable, SpawnEntry> _prefabFactory = new Dictionary<Spawnable, SpawnEntry>();
        private readonly Dictionary<Spawnable, Func<SpawnEntry, MonoBehaviour>> _scriptFactories = new Dictionary<Spawnable, Func<SpawnEntry, MonoBehaviour>>();

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in spawnEntries)
            {
                _prefabFactory.Add(spawnEntry.spawnable, spawnEntry);
            }
            _scriptFactories.Add(Spawnable.Client, CreateClient);
            _scriptFactories.Add(Spawnable.Glass, CreateGlass);
        }

        public T Spawn<T>(Spawnable spawnable) where T : MonoBehaviour
        {
            if (!_prefabFactory.TryGetValue(spawnable, out var entry))
            {
                throw new InvalidOperationException();
            }
            if (!_scriptFactories.TryGetValue(spawnable, out var factory))
            {
                throw new InvalidOperationException();
            }
            return factory(entry) as T;
        }

        public void Spawn(Spawnable spawnable)
        {
            if (!_prefabFactory.TryGetValue(spawnable, out var entry))
            {
                throw new InvalidOperationException();
            }
            if (!_scriptFactories.TryGetValue(spawnable, out var factory))
            {
                throw new InvalidOperationException();
            }
            factory(entry);
        }
        
        private Client CreateClient(SpawnEntry spawnEntry)
        {
            return Create<Client>(spawnEntry.prefab, spawnEntry.spawn, c => Client.GetName());
        }

        private Glass CreateGlass(SpawnEntry spawnEntry)
        {
            return Create<Glass>(spawnEntry.prefab, spawnEntry.spawn, g => "Glass");
        }

        private T Create<T>(GameObject prefab, Transform spawn, Func<T, string> nameProvider) where T : MonoBehaviour
        {
            var impl = Instantiate(prefab, spawn.position, Quaternion.identity);
            var component = impl.GetComponent<T>();
            impl.name = nameProvider(component);
            return component;
        }
    }
}