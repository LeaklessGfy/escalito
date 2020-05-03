using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Singleton
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Main;
        [SerializeField] private List<GameObject> prefabs;

        [SerializeField] private Transform spawn;

        private void Awake()
        {
            Main = this;
        }

        public Customer Spawn()
        {
            var rand = Random.Range(0, prefabs.Count);
            var prefab = prefabs[rand];
            var customer = Controller.CreateComponent<Customer>(prefab, spawn, "Michel");
            customer.OrderBuilder = Order.BuildRandom;

            return customer;
        }
    }
}