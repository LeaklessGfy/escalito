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

    private void Awake()
    {
        gameObject.SetActive(false);
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

    public void ToggleShop()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}