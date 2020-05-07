using System;
using System.Collections.Generic;
using System.Linq;
using Cocktails;
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

    public class CharacterController : MonoBehaviour
    {
        private const int MinDistance = 2;
        private const int MaxDistance = 3;
        private const int MaxCombo = 3;

        public static CharacterController Main;
        private readonly Queue<Customer> _customersLeave = new Queue<Customer>();
        private readonly LinkedList<Customer> _customersQueue = new LinkedList<Customer>();

        private readonly Dictionary<CharacterKey, GameObject> _prefabs = new Dictionary<CharacterKey, GameObject>();
        private readonly TimingAction _spawnAction;
        private readonly Vector2 _spawnRange = new Vector2(2, 5);
        private int _combo;
        private readonly int _customerLimit = 3;
        private Glass _glass;
        [SerializeField] private Transform bar;

        [SerializeField] private List<CustomerEntry> entries;
        [SerializeField] private Transform spawn;

        public CharacterController()
        {
            _spawnAction = new TimingAction(0, SpawnActionTrigger);
        }

        private void Awake()
        {
            Main = this;
            foreach (var spawnEntry in entries) _prefabs.Add(spawnEntry.key, spawnEntry.prefab);
        }

        private void Update()
        {
            UpdateQueue();
            UpdateLeaving();
            UpdateSpawn();
        }

        private void UpdateQueue()
        {
            for (var node = _customersQueue.Last; node != null;)
            {
                var customer = node.Value;

                if (node.Previous == null)
                {
                    GoToBar(customer);
                }
                else
                {
                    GoToCustomer(customer, node.Previous.Value);
                }

                if (customer.IsExhausted())
                {
                    Leave(customer);
                }

                if (!customer.HasOrder && customer.IsNear(bar.position, -customer.Offset, MaxDistance))
                {
                    AskOrder(customer);
                }

                node = node.Previous;
            }
        }

        private void UpdateLeaving()
        {
            while (_customersLeave.Count > 0)
            {
                var leavingCustomer = _customersLeave.Peek();
                if (leavingCustomer.IsNear(spawn.position, 0, MinDistance))
                {
                    Destroy(_customersLeave.Dequeue().gameObject);
                }
                else
                {
                    break;
                }
            }
        }

        private void UpdateSpawn()
        {
            if (!MainController.Main.BarIsOpen || _customersQueue.Count >= _customerLimit)
            {
                return;
            }

            _spawnAction.Tick(Time.deltaTime);
        }

        private void GoToBar(Customer customer)
        {
            customer.MoveTo(bar.position, -customer.Offset, MinDistance);
        }

        private void GoToCustomer(Customer customer, Customer leader)
        {
            customer.MoveTo(leader.transform.position, -customer.Offset, MinDistance);
        }

        private void AskOrder(Customer customer)
        {
            customer.AskOrder();
            customer.Await(MainController.Main.Difficulty);
            _glass = GlassController.Main.Spawn();
        }

        public void ReceiveOrder(Customer customer, Glass glass)
        {
            var actual = glass.Cocktail;
            Destroy(glass.gameObject);

            customer.Serve(actual);
            var cash = customer.Pay();
            CashController.Main.Cash += cash * (_combo + 1);

            MainController.Main.Difficulty++;
            HandleCombo(customer);
            Leave(customer);
        }

        private void HandleCombo(Customer customer)
        {
            if (!customer.IsSatisfied())
            {
                _combo = 0;
                return;
            }

            _combo++;

            if (_combo != MaxCombo)
            {
                return;
            }

            AudioController.Main.laugh.Play();
            _combo = 0;
        }

        private void Leave(Customer customer)
        {
            var node = _customersQueue.Find(customer);

            if (node == null)
            {
                throw new InvalidOperationException("Customer couldn't be found in customers queue");
            }

            if (customer.IsSatisfied())
            {
                AudioController.Main.success.Play();
            }
            else
            {
                AudioController.Main.failure.Play();
            }

            _customersQueue.Remove(node);
            _customersLeave.Enqueue(customer);
            customer.LeaveTo(spawn.position);
            Destroy(_glass.gameObject);
            _glass = null;
        }

        private float SpawnActionTrigger()
        {
            var customer = SpawnRandomCustomer();
            _customersQueue.AddLast(customer);
            return Random.Range(_spawnRange.x, _spawnRange.y);
        }

        private Customer SpawnCustomer(CharacterKey key)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                throw new InvalidOperationException();
            }

            var customer = MainController.CreateComponent<Customer>(prefab, spawn, prefab.name);
            customer.OrderBuilder = Order.BuildRandom;

            return customer;
        }

        private Customer SpawnRandomCustomer()
        {
            var keys = Enum.GetValues(typeof(CharacterKey))
                .Cast<CharacterKey>()
                .ToArray();
            var rand = Random.Range(0, keys.Length);
            var key = keys[rand];

            return SpawnCustomer(key);
        }
    }
}