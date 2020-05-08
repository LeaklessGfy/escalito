using System.Collections.Generic;
using Cocktails;
using Ingredients;
using UnityEngine;
using UnityEngine.UI;
using Selectable = Components.Selectable;

public class MainController : MonoBehaviour
{
    public static MainController Main;

    private int _currentCombo;
    private Glass _glass;

    public Text selectedText;

    public bool BarIsOpen { get; set; }
    public int Difficulty { get; set; } = 1;
    public int Reputation { get; set; } = 11;
    public Selectable Selected { get; set; }
    public Dictionary<IngredientKey, bool> Ingredients { get; } = new Dictionary<IngredientKey, bool>();

    private void Awake()
    {
        Main = this;
    }

    private void Update()
    {
        selectedText.text = Selected ? Selected.name : "";
    }
}