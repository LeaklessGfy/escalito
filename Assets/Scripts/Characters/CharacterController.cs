using System;
using System.Collections.Generic;
using System.Linq;
using Characters.Impl;
using Cocktails;
using Components;
using Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters
{
    [Serializable]
    public class CustomerEntry
    {
        public CharacterKey key;
        public GameObject prefab;
    }

    public class CharacterController : Controller
    {
        private const int ReputationThreshold = 10;

        private readonly LinkedList<Customer> _customerQueue = new LinkedList<Customer>();
        private readonly Dictionary<CharacterKey, GameObject> _prefabs = new Dictionary<CharacterKey, GameObject>();
        private readonly Vector2 _spawnRange = new Vector2(2, 5);
        private readonly HashSet<Sponsor> _sponsors = new HashSet<Sponsor>();
        private readonly TimeActionManager _timeAction = new TimeActionManager();

        public Transform bar;
        public List<CustomerEntry> entries;
        public Transform spawn;

        public CharacterController()
        {
            _timeAction.Add(new DelegateTimeAction(5, TimeUnit.Second, SpawnCustomerCondition, SpawnCustomerTrigger));
            _timeAction.Add(new DelegateTimeAction(10, TimeUnit.Minute, SpawnSponsorCondition, SpawnSponsorTrigger));
        }

        private void Awake()
        {
            foreach (var spawnEntry in entries) _prefabs.Add(spawnEntry.key, spawnEntry.prefab);
        }

        private void Update()
        {
            for (var node = _customerQueue.Last; node != null;)
            {
                var customer = node.Value;
                var next = bar.position;

                if (node.Previous != null)
                {
                    var leader = node.Previous.Value.transform.position;
                    next = new Vector2(leader.x - customer.Offset, leader.y);
                }
                
                customer.Behave(next);
                node = node.Previous;
            }

            foreach (var sponsor in _sponsors) sponsor.Behave(bar.position);

            _timeAction.Tick(ClockController.Main.CurrentTime);
        }

        private void Remove(Customer customer)
        {
            var node = _customerQueue.Find(customer);

            if (node == null)
            {
                throw new InvalidOperationException("Customer couldn't be found in customers queue");
            }

            _customerQueue.Remove(node);
        }

        private void Remove(Sponsor sponsor)
        {
            _sponsors.Remove(sponsor);
        }

        private bool SpawnCustomerCondition()
        {
            return MainController.Main.BarIsOpen && _customerQueue.Count < MainController.Main.Difficulty;
        }

        private void SpawnCustomerTrigger()
        {
            var customer = SpawnCustomer();
            _customerQueue.AddLast(customer);
            //return Random.Range(_spawnRange.x, _spawnRange.y);
        }

        private bool SpawnSponsorCondition()
        {
            return MainController.Main.Reputation > ReputationThreshold && _sponsors.Count < 1;
        }

        private void SpawnSponsorTrigger()
        {
            var sponsor = SpawnSponsor();
            _sponsors.Add(sponsor);
        }

        private Customer SpawnRandomCustomer()
        {
            var keys = Enum.GetValues(typeof(CharacterKey))
                .Cast<CharacterKey>()
                .Where(c => c != CharacterKey.Sponsor)
                .ToArray();
            var rand = Random.Range(0, keys.Length);
            var key = keys[rand];

            return SpawnCustomer();
        }

        private Customer SpawnCustomer()
        {
            var customer = SpawnCharacter<Customer>(CharacterKey.Default);

            void OnLeave(Character c)
            {
                Remove(customer);
            }

            customer.Init(new PositionBag(spawn.position, bar.position), OnLeave, Order.BuildRandom);

            return customer;
        }

        private Sponsor SpawnSponsor()
        {
            var sponsor = SpawnCharacter<Sponsor>(CharacterKey.Sponsor);

            void OnLeave(Character c)
            {
                Remove(sponsor);
            }

            sponsor.Init(new PositionBag(spawn.position, bar.position), OnLeave);

            return sponsor;
        }

        private T SpawnCharacter<T>(CharacterKey key) where T : MonoBehaviour
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            return CreateComponent<T>(prefab, spawn, prefab.name);
        }
    }
}