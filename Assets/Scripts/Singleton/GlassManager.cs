using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace Singleton
{
    public class GlassManager : MonoBehaviour
    {
        public static GlassManager Main;
        
        private List<Glass> _glasses;

        [SerializeField] private Transform spawnAwaitingGlass;
        [SerializeField] private Transform spawnInProgressGlass;

        private void Awake()
        {
            Main = this;
        }

        public void Spawn(Order order)
        {
            var isFirst = true;
            
            _glasses = order.Cocktails.Select(cocktail =>
            {
                var glass = SpawnManager.Main.Spawn<Glass>(Spawnable.Glass);
                
                if (isFirst)
                {
                    GoToProgress(glass);
                    isFirst = false;
                }
                else
                {
                    GoToAwait(glass);
                }

                return glass;
            }).ToList();
        }

        public void Clean()
        {
            foreach (var glass in _glasses)
            {
                Destroy(glass.gameObject);
            }
        }

        private void GoToAwait(Glass glass)
        {
            var glassTransform = glass.transform;

            var position = spawnAwaitingGlass.position;
            var x = position.x;
            var y = position.y;
            var z = glassTransform.position.z;

            glassTransform.position = new Vector3(x, y, z);
        }

        private void GoToProgress(Glass glass)
        {
            var glassTransform = glass.transform;

            var position = spawnInProgressGlass.position;
            var x = position.x;
            var y = position.y;
            var z = glassTransform.position.z;

            glassTransform.position = new Vector3(x, y, z);
        }
    }
}