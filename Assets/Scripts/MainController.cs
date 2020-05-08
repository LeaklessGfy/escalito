using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonuses;
using Cocktails;
using Core;
using Ingredients;
using UnityEngine;
using UnityEngine.UI;
using Selectable = Components.Selectable;

public class MainController : MonoBehaviour
{
    public static MainController Main;
    private readonly TimingAction _expenseAction;

    private int _currentCombo;
    private readonly int _expenseTime = 10;
    private Glass _glass;

    [SerializeField] private Text expenseText;
    [SerializeField] private Text selectedText;

    public MainController()
    {
        _expenseAction = new TimingAction(_expenseTime, ExpenseActionTick, ExpenseActionTrigger);
    }

    public bool BarIsOpen { get; set; }
    public int Difficulty { get; set; } = 1;
    public int Reputation { get; set; } = 11;
    public Selectable Selected { get; set; }
    public Expenses Expenses { get; } = new Expenses();
    public IBonus Bonuses { get; } = new CompositeBonus();
    public Dictionary<IngredientKey, bool> Ingredients { get; } = new Dictionary<IngredientKey, bool>();

    private void Awake()
    {
        Main = this;
    }

    private void Update()
    {
        selectedText.text = Selected ? Selected.name : "";
        if (Expenses.HasExpense())
        {
            _expenseAction.Tick(Time.deltaTime);
        }
    }

    private void ExpenseActionTick(float current, float trigger)
    {
        var diff = trigger - current;
        var percent = diff / trigger * 100;
        if (percent < 70)
        {
            expenseText.text = "";
        }
    }

    private float ExpenseActionTrigger()
    {
        var expensesSum = Expenses.Sum();
        var text = expensesSum
            .Aggregate(new StringBuilder(), (sb, pair) => sb.AppendLine($"{pair.Key} : -{pair.Value} $"))
            .ToString();
        var total = expensesSum.Aggregate(0, (sum, pair) => sum + pair.Value);
        var currentCash = CashController.Main.Cash;

        expenseText.text = text;
        expenseText.color = PercentHelper.GetColor((currentCash - total) / currentCash * 100);

        CashController.Main.Cash -= total;
        AudioController.Main.cash.Play();
        // TODO : Check if bankrupt, if it's the case, lose game
        return 10;
    }
}