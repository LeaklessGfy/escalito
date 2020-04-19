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

        [SerializeField] private Spawnable spawnable;

        private void Awake()
        {
            _itemText = GetComponentInChildren<Text>();
            _itemButton = GetComponentInChildren<Button>();
            _itemText.text = spawnable.ToString();
            _itemButton.onClick.AddListener(Buy);
        }

        private void Update()
        {
            _itemButton.interactable = CashManager.Main.GetPrice(spawnable) <= CashManager.Main.Cash;
        }

        private void Buy()
        {
            if (CashManager.Main.GetPrice(spawnable) > CashManager.Main.Cash)
            {
                return;
            }

            CashManager.Main.Cash -= CashManager.Main.GetPrice(spawnable);
            SpawnManager.Main.Spawn(spawnable);
        }
    }
}