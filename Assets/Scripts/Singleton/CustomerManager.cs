using System.Collections.Generic;
using UnityEngine;

namespace Singleton
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Main;

        [SerializeField] private Transform spawnCustomer;
        [SerializeField] private List<GameObject> prefabs;

        private void Awake()
        {
            Main = this;
        }

        public Customer Spawn()
        {
            var rand = Random.Range(0, prefabs.Count);
            var prefab = prefabs[rand];
            return Create(prefab);
        }

        private Customer Create(GameObject prefab)
        {
            var impl = Instantiate(prefab, spawnCustomer.position, Quaternion.identity);
            var customer = impl.GetComponent<Customer>();
            impl.name = Customer.GetName();
            return customer;
        }
    }
}