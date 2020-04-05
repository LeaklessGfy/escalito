using UnityEngine;

namespace Assets.Scripts
{
    class Bar : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Glass glass = collision.gameObject.GetComponent<Glass>();
            foreach (var value in glass.Consumables)
            {
                print(value.Key + " " + value.Value);
            }
        }
    }
}
