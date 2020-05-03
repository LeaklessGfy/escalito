using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Singleton
{
    public class GlassManager : MonoBehaviour
    {
        public static GlassManager Main;

        private List<GlassSprite> _glasses;

        [SerializeField] private List<GameObject> prefabs;
        [SerializeField] private Transform spawnAwaiting;
        [SerializeField] private Transform spawnInProgress;

        private void Awake()
        {
            Main = this;
        }

        public void Spawn(Order order)
        {
            _glasses = new List<GlassSprite>();
            for (var i = 0; i < order.Count; i++)
            {
                var glass = Controller.CreateComponent<GlassSprite>(prefabs[0], spawnAwaiting, "Glass " + i);

                if (i == 0)
                {
                    GoToProgress(glass);
                }
                else
                {
                    GoToAwait(glass);
                }

                _glasses.Add(glass);
            }
        }

        private void GoToAwait(Component glass)
        {
            var glassTransform = glass.transform;

            var position = spawnAwaiting.position;
            var x = position.x;
            var y = position.y;
            var z = glassTransform.position.z;

            glassTransform.position = new Vector3(x, y, z);
        }

        private void GoToProgress(Component glass)
        {
            var glassTransform = glass.transform;

            var position = spawnInProgress.position;
            var x = position.x;
            var y = position.y;
            var z = glassTransform.position.z;

            glassTransform.position = new Vector3(x, y, z);
        }
    }
}