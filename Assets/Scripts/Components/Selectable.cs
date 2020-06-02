using UnityEngine;
using UnityEngine.EventSystems;

namespace Components
{
    public class Selectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _isClicked;

        public void OnPointerEnter(PointerEventData eventData)
        {
            MagicBag.Bag.main.Selected = this;
            MagicBag.Bag.cursor.SetHover(true);

            //var position = transform.position;
            //transform.position = new Vector3(position.x, position.y, -2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isClicked)
            {
                return;
            }

            MagicBag.Bag.main.Selected = null;
            MagicBag.Bag.cursor.SetHover(false);

            //var position = transform.position;
            //transform.position = new Vector3(position.x, position.y, 0f);
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