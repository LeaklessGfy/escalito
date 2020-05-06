using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Singleton;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Selectable = Components.Selectable;

public class Controller : MonoBehaviour
{
    private const int MinDistance = 2;
    private const int MaxDistance = 3;
    private const int MaxCombo = 3;
    public static Controller Main;

    private readonly LinkedList<Customer> _customers = new LinkedList<Customer>();
    private readonly Dictionary<IngredientKey, int> _inventory = new Dictionary<IngredientKey, int>();
    private readonly Queue<Customer> _leavingCustomers = new Queue<Customer>();
    private readonly Vector2 _spawnRange = new Vector2(2, 5);
    private int _currentCombo;
    private int _currentDifficulty = 1;
    private TimingAction _spawnAction;
    private TimingAction _expenseAction;
    private GlassSprite _glass;

    [SerializeField] private Transform bar;
    [SerializeField] private Text cashText;
    [SerializeField] private int maxCustomers = 3;
    [SerializeField] private Text selectedText;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform spawnCustomer;
    [SerializeField] private Text expenseText;

    public bool BarIsOpen { get; set; }
    public Selectable Selected { get; set; }
    public List<Expense> IngredientsExpense => new List<Expense>();

    public void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }

    private void Awake()
    {
        Main = this;

        shopPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        _spawnAction = new TimingAction(0, () =>
        {
            var customer = CustomerManager.Main.SpawnRandom();
            _customers.AddLast(customer);
            return Random.Range(_spawnRange.x, _spawnRange.y);
        });
        _expenseAction = new TimingAction(10,
            (current, trigger) =>
            {
                var diff = trigger - current;
                var percent = (diff / trigger) * 100;
                expenseText.text = diff.ToString("0.0");
                expenseText.color = SatisfactionHelper.GetColor((int) percent);
            },
            () =>
            {
                // TODO: Do some ui stuff such as : Appending expenses summary to top right screen
                var total = IngredientsExpense.Sum(expense => expense.Amount);
                CashManager.Main.Cash -= total;
                AudioManager.Main.PlayCash();
                // TODO : Check if bankrupt, if it's the case, lose game
                return 10;
            }
        );
    }

    private void Update()
    {
        cashText.text = CashManager.Main.Cash + " $";
        selectedText.text = Selected ? Selected.name : "";

        UpdateQueue();
        UpdateLeaving();
        UpdateSpawn();
        UpdateExpenses();
    }

    private void UpdateQueue()
    {
        for (var node = _customers.Last; node != null;)
        {
            var customer = node.Value;

            if (node.Previous == null)
            {
                GoToBar(customer);
            }
            else
            {
                GoToCustomer(customer, node.Previous.Value);
            }

            if (customer.IsExhausted())
            {
                Leave(customer);
            }

            if (!customer.HasOrder && customer.IsNear(bar.position, -customer.Offset, MaxDistance))
            {
                AskOrder(customer);
            }

            node = node.Previous;
        }
    }

    private void UpdateLeaving()
    {
        while (_leavingCustomers.Count > 0)
        {
            var leavingCustomer = _leavingCustomers.Peek();
            if (leavingCustomer.IsNear(spawnCustomer.position, 0, MinDistance))
            {
                Destroy(_leavingCustomers.Dequeue().gameObject);
            }
            else
            {
                break;
            }
        }
    }

    private void UpdateSpawn()
    {
        if (!BarIsOpen || _customers.Count >= maxCustomers)
        {
            return;
        }
        _spawnAction.Tick(Time.deltaTime);
    }

    private void UpdateExpenses()
    {
        _expenseAction.Tick(Time.deltaTime);
    }

    private void GoToBar(Customer customer)
    {
        customer.MoveTo(bar.position, -customer.Offset, MinDistance);
    }

    private void GoToCustomer(Customer customer, Customer leader)
    {
        customer.MoveTo(leader.transform.position, -customer.Offset, MinDistance);
    }

    private void AskOrder(Customer customer)
    {
        customer.AskOrder();
        customer.Await(_currentDifficulty);
        _glass = GlassManager.Main.Spawn();
    }

    public void ReceiveOrder(Customer customer, GlassSprite glass)
    {
        var actual = glass.Cocktail;
        Destroy(glass.gameObject);

        customer.Serve(actual);
        var cash = customer.Pay();
        CashManager.Main.Cash += cash * (_currentCombo + 1);

        _currentDifficulty++;
        HandleCombo(customer);
        Leave(customer);
    }

    private void HandleCombo(Customer customer)
    {
        if (!customer.IsSatisfied())
        {
            _currentCombo = 0;
            return;
        }

        _currentCombo++;

        if (_currentCombo != MaxCombo)
        {
            return;
        }

        AudioManager.Main.PlayLaugh();
        _currentCombo = 0;
    }

    private void Leave(Customer customer)
    {
        var node = _customers.Find(customer);

        if (node == null)
        {
            throw new InvalidOperationException("Customer couldn't be found in customers queue");
        }

        if (customer.IsSatisfied())
        {
            AudioManager.Main.PlaySuccess();
        }
        else
        {
            AudioManager.Main.PlayFailure();
        }

        _customers.Remove(node);
        _leavingCustomers.Enqueue(customer);
        customer.LeaveTo(spawnCustomer.position);
        Destroy(_glass.gameObject);
        _glass = null;
    }

    public static T CreateComponent<T>(GameObject prefab, Transform spawn, string name) where T : MonoBehaviour
    {
        var impl = Instantiate(prefab, spawn.position, Quaternion.identity);
        var component = impl.GetComponent<T>();
        impl.name = name;
        return component;
    }

    public static void CreateObject(GameObject prefab, Transform spawn, string name)
    {
        var impl = Instantiate(prefab, spawn.position, Quaternion.identity);
        impl.name = name;
    }
}