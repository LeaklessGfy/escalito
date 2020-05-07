using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public abstract class Item : MonoBehaviour
    {
        private Button _itemButton;
        private Text _itemButtonText;
        private Image _itemImage;
        private Text _itemText;

        protected int Price { get; set; }

        private void Awake()
        {
            _itemButton = GetComponentInChildren<Button>();
            _itemButtonText = _itemButton.GetComponentInChildren<Text>();
            _itemText = GetComponentInChildren<Text>();
            _itemImage = GetComponent<Image>();
            _itemButton.onClick.AddListener(Buy);
        }

        private void Update()
        {
            _itemButton.interactable = Price <= CashController.Main.Cash && !ForbidBuy();
        }

        protected void SetName(string name)
        {
            _itemText.text = name;
        }

        protected void SetPrice(int price)
        {
            Price = price;
            _itemButtonText.text = price + " $";
        }

        protected abstract void Buy();
        protected abstract bool ForbidBuy();
    }
}