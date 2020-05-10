using UnityEngine;

namespace Characters
{
    public struct PositionBag
    {
        public Vector2 Spawn { get; }
        public Vector2 Bar { get; }

        public PositionBag(Vector2 spawn, Vector2 bar)
        {
            Spawn = spawn;
            Bar = bar;
        }
    }
}