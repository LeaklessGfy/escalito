using UnityEngine;

namespace Singleton
{
    public class CursorManager : MonoBehaviour
    {
        private static readonly Vector2 HotSpot = new Vector2(10, 5);

        public static CursorManager Main;

        [SerializeField] private Texture2D hover;
        [SerializeField] private Texture2D normal;

        private void Awake()
        {
            Main = this;
            Cursor.SetCursor(normal, HotSpot, CursorMode.ForceSoftware);
        }

        public void SetHover(bool isHover)
        {
            Cursor.SetCursor(isHover ? hover : normal, HotSpot, CursorMode.ForceSoftware);
        }
    }
}