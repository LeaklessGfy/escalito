using UnityEngine;

namespace Components
{
    public class Controller : MonoBehaviour
    {
        protected static T CreateComponent<T>(GameObject prefab, Transform spawn, string name) where T : MonoBehaviour
        {
            var impl = Instantiate(prefab, spawn.position, Quaternion.identity);
            var component = impl.GetComponent<T>();
            impl.name = name;
            return component;
        }

        protected static void CreateObject(GameObject prefab, Transform spawn, string name)
        {
            var impl = Instantiate(prefab, spawn.position, Quaternion.identity);
            impl.name = name;
        }
    }
}