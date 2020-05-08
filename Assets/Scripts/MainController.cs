using System.Collections.Generic;
using Characters;
using Ingredients;
using UnityEngine;
using UnityEngine.UI;
using Selectable = Components.Selectable;

public class MainController : MonoBehaviour
{
    public static MainController Main;
    public Text selectedText;

    public bool BarIsOpen { get; set; }
    public int Difficulty { get; private set; } = 1;
    public int Reputation { get; private set; } = 11;
    public int PositiveCombo { get; private set; } = 0;
    public int NegativeCombo { get; private set; } = 0;
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

    public void IncrementSuccess(Customer customer, int cash)
    {
        PositiveCombo++;
        NegativeCombo = 0;

        CashController.Main.Bonus(customer, cash);

        if (PositiveCombo % 3 == 0)
        {
            AudioController.Main.laugh.Play();
        }
        
        if (PositiveCombo % 10 == 0)
        {
            Difficulty++;
        }

        if (PositiveCombo % 20 == 0)
        {
            Reputation++;
        }
    }

    public void IncrementFailure(Customer customer)
    {
        PositiveCombo = 0;
        NegativeCombo++;

        CashController.Main.Penalty(customer);

        if (NegativeCombo % 3 == 0)
        {
            Reputation--;
        }

        if (CashController.Main.Cash < 0)
        {
            // GAME OVER
        }
        
        
        // Calculate if lost contract
    }
}