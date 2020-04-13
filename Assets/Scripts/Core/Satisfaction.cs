using UnityEngine;

namespace Core
{
    public static class Satisfaction
    {
        public const int High = 100;
        public const int Medium = 70;
        public const int Low = 40;

        public static Color GetColor(int satisfaction)
        {
            if (satisfaction < Low)
            {
                return Color.red;
            }
            if (satisfaction < Medium)
            {
                return Color.yellow;
            }
            return Color.green;
        }
    }
}