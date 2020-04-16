using UnityEngine;
using UnityEngine.EventSystems;

namespace Components
{
    public class Selectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _isClicked;

        private void OnMouseDown()
        {
            _isClicked = true;
        }

        private void OnMouseUp()
        {
            _isClicked = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Controller.Main.Selected = this;
            CursorManager.Main.SetHover(true);

            var position = transform.position;
            transform.position = new Vector3(position.x, position.y, -2f);
        }

        public void OnPointerExit(PointerEventData eventData)
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
    }
}