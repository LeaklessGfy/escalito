using Core;
using Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class Payable : MonoBehaviour
    {
        private Button _itemButton;
        private Text _itemText;

        [SerializeField] private IngredientKey spawnable;

        private void Awake()
        {
            _itemText = GetComponentInChildren<Text>();
            _itemButton = GetComponentInChildren<Button>();
            _itemText.text = spawnable.ToString();
            _itemButton.onClick.AddListener(Buy);
        }

        private void Update()
        {
            _itemButton.interactable = CashManager.GetPrice(spawnable) <= CashManager.Main.Cash;
        }

        private void Buy()
        {
            if (CashManager.GetPrice(spawnable) > CashManager.Main.Cash)
            {
                return;
            }

            CashManager.Main.Cash -= CashManager.GetPrice(spawnable);
            IngredientManager.Main.Spawn(spawnable);
        }
    }
}