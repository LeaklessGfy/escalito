using Core;
using Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class Payable : MonoBehaviour
    {
        [SerializeField] private int price;
        [SerializeField] private Spawnable spawnable;

        private Text _itemText;
        private Button _itemButton;

        private void Awake()
        {
            _itemText = GetComponentInChildren<Text>();
            _itemButton = GetComponentInChildren<Button>();

            _itemText.text = spawnable.ToString();
            _itemButton.onClick.AddListener(Buy);
        }

        private void Update()
        {
            _itemButton.interactable = price <= Controller.Main.Cash;
        }

        private void Buy()
        {
            if (price > Controller.Main.Cash)
            {
                return;
            }
            Controller.Main.Cash -= price;
            SpawnManager.Main.Spawn(spawnable);
        }
    }
}