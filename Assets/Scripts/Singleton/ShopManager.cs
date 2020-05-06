using System;
using System.Linq;
using Components;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Singleton
{
    [Serializable]
    public class ShopEntry
    {
    }
    
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private VerticalLayoutGroup layout;

        private void Start()
        {
            foreach (var ingredient in Enum.GetValues(typeof(IngredientKey)).Cast<IngredientKey>())
            {
                var go = Instantiate(prefab, transform);
                var payable = go.GetComponent<PayableIngredient>();
                payable.IngredientKey = ingredient;
                go.transform.SetParent(layout.transform);
            }
        }
    }
}