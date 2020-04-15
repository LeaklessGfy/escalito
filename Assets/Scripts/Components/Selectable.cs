using UnityEngine;

namespace Components
{
    public class Selectable : MonoBehaviour
    {
        private bool _isClicked;

        private void OnMouseEnter()
        {
            Controller.Main.Selected = this;
            CursorManager.Main.SetHover(true);
        }

        private void OnMouseExit()
        {
            if (_isClicked)
            {
                return;
            }

            Controller.Main.Selected = null;
            CursorManager.Main.SetHover(false);
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