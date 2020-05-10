using System.Collections.Generic;
using Cash;
using Characters.Impl;
using Ingredients;
using UnityEngine;
using UnityEngine.UI;
using Selectable = Components.Selectable;

public class MainController : MonoBehaviour
{
    private const float SecondsPerDay = 60;

    public static MainController Main;

    private float _time;
    public Text clockText;
    public Text selectedText;

    public bool BarIsOpen { get; set; }
    public int Difficulty { get; private set; } = 1;
    public int Reputation { get; private set; } = 1;
    public int PositiveCombo { get; private set; }
    public int NegativeCombo { get; private set; }
    public Selectable Selected { get; set; }
    public Dictionary<IngredientKey, bool> Ingredients { get; } = new Dictionary<IngredientKey, bool>();

    private void Awake()
    {
        Main = this;
    }

    private void Update()
    {
        selectedText.text = Selected ? Selected.name : "";
        UpdateTime();
    }

    private void UpdateTime()
    {
        _time += Time.deltaTime / SecondsPerDay;

        var timeNormalized = _time % 1f;
        var minutes = Mathf.Floor(timeNormalized * 24f % 1f * 60f);
        var hours = Mathf.Floor(timeNormalized * 24f);

        clockText.text = $"{hours:00}:{minutes:00}";
    }

    public decimal Increment(Customer customer)
    {
        return customer.Satisfied ? IncrementSuccess(customer) : IncrementFailure(customer);
    }

    private decimal IncrementSuccess(Customer customer)
    {
        PositiveCombo++;
        NegativeCombo = 0;

        var amount = CashController.Main.Bonus(customer);
        AudioController.Main.success.Play();

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

        return amount;
    }

    private decimal IncrementFailure(Customer customer)
    {
        PositiveCombo = 0;
        NegativeCombo++;

        var amount = CashController.Main.Penalty(customer);
        AudioController.Main.failure.Play();

        if (NegativeCombo % 3 == 0)
        {
            Reputation--;
        }

        if (CashController.Main.Cash < 0)
        {
            // GAME OVER
        }

        // Calculate if lost contract

        return amount;
    }
}