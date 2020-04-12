using UnityEngine;

namespace Components
{
    public class Selectable : MonoBehaviour
    {
        private readonly Vector2 _hotpot = new Vector2(10, 5);

        private bool _isClicked;
        public Texture2D hover;
        public Texture2D normal;

        private void Awake()
        {
            Cursor.SetCursor(normal, _hotpot, CursorMode.ForceSoftware);
        }

        private void OnMouseEnter()
        {
            Controller.Main.Selected = this;
            Cursor.SetCursor(hover, _hotpot, CursorMode.Auto);
        }

        private void OnMouseExit()
        {
            if (_isClicked)
            {
                return;
            }

            Controller.Main.Selected = null;
            Cursor.SetCursor(normal, _hotpot, CursorMode.ForceSoftware);
        }

        private void OnMouseDown()
        {
            _isClicked = true;
        }

        private void OnMouseUp()
        {
            _isClicked = false;
        }
    }
}