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

            var position = transform.position;
            transform.position = new Vector3(position.x, position.y, -2f);
        }

        private void OnMouseExit()
        {
            if (_isClicked)
            {
                return;
            }

            Controller.Main.Selected = null;
            CursorManager.Main.SetHover(false);
            
            var position = transform.position;
            transform.position = new Vector3(position.x, position.y, 0f);
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