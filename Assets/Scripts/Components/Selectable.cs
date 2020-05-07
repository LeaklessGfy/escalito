using UnityEngine;
using UnityEngine.EventSystems;

namespace Components
{
    public class Selectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _isClicked;

        public void OnPointerEnter(PointerEventData eventData)
        {
            MainController.Main.Selected = this;
            CursorController.Main.SetHover(true);

            var position = transform.position;
            transform.position = new Vector3(position.x, position.y, -2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isClicked)
            {
                return;
            }

            MainController.Main.Selected = null;
            CursorController.Main.SetHover(false);

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