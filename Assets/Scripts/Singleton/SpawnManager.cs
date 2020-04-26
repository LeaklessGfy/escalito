using System;
using System.Collections.Generic;
using Components;
using Core;
using UnityEngine;

namespace Singleton
{
    [Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        public Transform spawn;
        public Spawnable spawnable;
    }

    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Main;

        private readonly Dictionary<Spawnable, SpawnEntry> _prefabFactory = new Dictionary<Spawnable, SpawnEntry>();

        private readonly Dictionary<Spawnable, Func<SpawnEntry, MonoBehaviour>> _scriptFactories =
            new Dictionary<Spawnable, Func<SpawnEntry, MonoBehaviour>>();

        private int _id;

        [SerializeField] private SpawnEntry[] spawnEntries;

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in spawnEntries) _prefabFactory.Add(spawnEntry.spawnable, spawnEntry);
            _scriptFactories.Add(Spawnable.Customer, CreateCustomer);
            _scriptFactories.Add(Spawnable.Glass, CreateGlass);
            _scriptFactories.Add(Spawnable.Lemon, CreateConsumable);
            _scriptFactories.Add(Spawnable.Strawberry, CreateConsumable);
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

        private static Customer CreateCustomer(SpawnEntry spawnEntry)
        {
            return Create<Customer>(spawnEntry.prefab, spawnEntry.spawn, _ => Customer.GetName());
        }

        private Glass CreateGlass(SpawnEntry spawnEntry)
        {
            _id++;
            return Create<Glass>(spawnEntry.prefab, spawnEntry.spawn, _ => "Glass " + _id);
        }

        private static Consumable CreateConsumable(SpawnEntry spawnEntry)
        {
            return Create<Consumable>(spawnEntry.prefab, spawnEntry.spawn, _ => spawnEntry.spawnable.ToString());
        }

        private static T Create<T>(GameObject prefab, Transform spawn, Func<T, string> nameProvider)
            where T : MonoBehaviour
        {
            var impl = Instantiate(prefab, spawn.position, Quaternion.identity);
            var component = impl.GetComponent<T>();
            impl.name = nameProvider(component);
            return component;
        }
    }
}