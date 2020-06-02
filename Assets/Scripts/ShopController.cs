using System;
using System.Linq;
using Components;
using Ingredients;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public VerticalLayoutGroup layout;
    public GameObject prefab;
    public GameObject shopPanel;
    public Button shopButton;

    private void Awake()
    {
        shopPanel.SetActive(false);
        shopButton.onClick.AddListener(ToggleShop);
    }

    private void Start()
    {
        foreach (var ingredient in Enum.GetValues(typeof(IngredientKey)).Cast<IngredientKey>())
        {
            var go = Instantiate(prefab, transform);
            var payable = go.GetComponent<IngredientItem>();
            payable.IngredientKey = ingredient;
            go.transform.SetParent(layout.transform);
        }
    }

    private void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }
}