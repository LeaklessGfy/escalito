using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Singleton
{
    [Serializable]
    public class CustomerEntry
    {
        public CustomerKey key;
        public GameObject prefab;
    }

    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Main;

        private readonly Dictionary<CustomerKey, GameObject> _prefabs = new Dictionary<CustomerKey, GameObject>();
        [SerializeField] private List<CustomerEntry> entries;
        [SerializeField] private Transform spawn;

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in entries) _prefabs.Add(spawnEntry.key, spawnEntry.prefab);
        }

        public Customer Spawn(CustomerKey key)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            var customer = Controller.CreateComponent<Customer>(prefab, spawn, prefab.name);
            customer.OrderBuilder = Order.BuildRandom;

            return customer;
        }

        public Customer SpawnRandom()
        {
            var keys = Enum.GetValues(typeof(CustomerKey))
                .Cast<CustomerKey>()
                .ToArray();
            var rand = Random.Range(0, keys.Length);
            var key = keys[rand];

            return Spawn(key);
        }
    }
}