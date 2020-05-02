using System.Collections.Generic;
using UnityEngine;

namespace Singleton
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Main;

        [SerializeField] private Transform spawn;
        [SerializeField] private List<GameObject> prefabs;

        private void Awake()
        {
            Main = this;
        }

        public Customer Spawn()
        {
            var rand = Random.Range(0, prefabs.Count);
            var prefab = prefabs[rand];
            return Controller.CreateComponent<Customer>(prefab, spawn, "Michel");
        }
    }
}