using System;
using System.Collections.Generic;
using Core;
using Singleton;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Selectable = Components.Selectable;

public class Controller : MonoBehaviour
{
    private const int MinutesPerDay = 10;
    private const int MinDistance = 2;
    private const int MaxDistance = 3;
    private const int Combo = 3;
    public static Controller Main;

    private readonly LinkedList<Customer> _customers = new LinkedList<Customer>();
    private readonly Dictionary<IngredientKey, int> _inventory = new Dictionary<IngredientKey, int>();
    private readonly Queue<Customer> _leavingCustomers = new Queue<Customer>();
    private int _combo;

    private float _currentSpawnTime;
    private float _nextSpawnTime;
    private Vector2 _spawnTimeRange = new Vector2(1f, 2);

    [SerializeField] private Transform bar;
    [SerializeField] private Text cashText;
    [SerializeField] private Text clockText;
    [SerializeField] private int maxCustomers = 3;
    [SerializeField] private Text selectedText;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform spawnCustomer;

    public bool BarIsOpen { get; set; }
    public Selectable Selected { get; set; }

    public void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }

    private void Awake()
    {
        Main = this;

        shopPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateUi();
        UpdateSelected();
        UpdateQueue();
        UpdateLeaving();
        UpdateSpawn();
    }

    private void UpdateUi()
    {
        clockText.text = GetTime();
        cashText.text = CashManager.Main.Cash + "$";
    }

    private void UpdateSelected()
    {
        selectedText.text = Selected ? Selected.name : "";
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
                _leavingCustomers.Dequeue();
                Destroy(leavingCustomer.gameObject);
            }
            else
            {
                break;
            }
        }
    }

    private void UpdateSpawn()
    {
        if (!BarIsOpen)
        {
            return;
        }

        _currentSpawnTime += Time.deltaTime;

        if (_currentSpawnTime < _nextSpawnTime)
        {
            return;
        }

        _currentSpawnTime = 0;
        _nextSpawnTime = Random.Range(_spawnTimeRange.x, _spawnTimeRange.y);

        if (_customers.Count >= maxCustomers)
        {
            return;
        }

        var customer = CustomerManager.Main.SpawnRandom();
        _customers.AddLast(customer);
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
        var order = customer.AskOrder();
        GlassManager.Main.Spawn();
        customer.Await();
    }

    public void ReceiveOrder(Customer customer, GlassSprite glass)
    {
        var actual = glass.Cocktail;
        Destroy(glass.gameObject);

        if (!customer.Serve(actual))
        {
            GlassManager.Main.Spawn();
            return;
        }

        var cash = customer.Pay();
        // TODO: Add cash to cashManager

        HandleCombo(customer);
        Leave(customer);
        IncreaseDifficulty();
    }

    private void HandleCombo(Customer customer)
    {
        if (!customer.IsSatisfied())
        {
            _combo = 0;
            return;
        }

        _combo++;

        if (_combo != Combo)
        {
            return;
        }

        AudioManager.Main.PlayLaugh();
        _combo = 0;
    }

    private void IncreaseDifficulty()
    {
        _spawnTimeRange.x = Mathf.Max(0, _spawnTimeRange.x - 1);
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
    }

    private static string GetTime()
    {
        var now = DateTime.Now;
        var hours = now.TimeOfDay.TotalMinutes % MinutesPerDay;
        var minutes = hours % 1 * 60;
        var timeStamp = new TimeSpan((int) hours, (int) minutes, 0);

        var hoursString = timeStamp.Hours.ToString().PadLeft(2, '0');
        var minutesString = timeStamp.Minutes.ToString().PadLeft(2, '0');

        return hoursString + ":" + minutesString;
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