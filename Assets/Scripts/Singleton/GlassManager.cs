using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace Singleton
{
    public class GlassManager : MonoBehaviour
    {
        public static GlassManager Main;
        
        private SortedSet<Glass> _ready;
        private List<Glass> _glasses;

        [SerializeField] private Transform spawnAwaitingGlass;
        [SerializeField] private Transform spawnInProgressGlass;
        [SerializeField] private Transform spawnReadyGlass;

        public Queue<Glass> Ready => new Queue<Glass>(_ready);

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
                    glass.InternalState = Glass.State.InProgress;
                    isFirst = false;
                }
                else
                {
                    GoToAwait(glass);
                    glass.InternalState = Glass.State.Awaiting;
                }

                return glass;
            }).ToList();

            _ready = new SortedSet<Glass>();
        }

        public void NextStep(Glass glass)
        {
            switch (glass.InternalState)
            {
                case Glass.State.InProgress:
                    AddToReady(glass);
                    _ready.Add(glass);
                    break;
                case Glass.State.Ready:
                    AddToProgress(glass);
                    _ready.Remove(glass);
                    break;
                case Glass.State.Awaiting:
                    AddToProgress(glass);
                    break;
            }
        }

        public void Clean()
        {
            foreach (var glass in _glasses)
            {
                Destroy(glass.gameObject);
            }
        }

        private void AddToReady(Glass glass)
        {
            glass.InternalState = Glass.State.Ready;

            var currentAwaiting = _glasses.FirstOrDefault(g => g.InternalState == Glass.State.Awaiting);
            if (currentAwaiting != null)
            {
                GoToProgress(currentAwaiting);
                currentAwaiting.InternalState = Glass.State.InProgress;
            }

            GoToReady(glass);
            glass.InternalState = Glass.State.Ready;
        }

        private void AddToProgress(Glass glass)
        {
            var currentInProgress = _glasses.FirstOrDefault(g => g.InternalState == Glass.State.InProgress);
            if (currentInProgress != null)
            {
                GoToAwait(currentInProgress);
                currentInProgress.InternalState = Glass.State.Awaiting;
            }

            GoToProgress(glass);
            glass.InternalState = Glass.State.InProgress;
        }

        private void GoToAwait(Glass glass)
        {
            var glassTransform = glass.transform;

            var position = spawnAwaitingGlass.position;
            var x = position.x; // offset
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


        private void GoToReady(Glass glass)
        {
            var glassTransform = glass.transform;

            var position = spawnReadyGlass.position;
            var x = position.x + glass.OverflowX * _ready.Count;
            var y = position.y + glass.OverflowY;
            var z = glassTransform.position.z;

            glassTransform.position = new Vector3(x, y, z);
        }
    }
}