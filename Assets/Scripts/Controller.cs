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
    public static Controller Main;

    private readonly LinkedList<Client> _clients = new LinkedList<Client>();
    private readonly Dictionary<IngredientKey, int> _inventory = new Dictionary<IngredientKey, int>();
    private readonly Queue<Client> _leavingClients = new Queue<Client>();
    private readonly Dictionary<Client, Cocktail> _orders = new Dictionary<Client, Cocktail>();

    private float _currentSpawnTime;
    private float _nextSpawnTime;
    private Vector2 _spawnTimeRange = new Vector2(1f, 2);
    private int _combo;

    [SerializeField] private Transform bar;
    [SerializeField] private Text cashText;
    [SerializeField] private Text clockText;
    [SerializeField] private int maxClients = 3;
    [SerializeField] private Text selectedText;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform spawn;

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
        for (var node = _clients.Last; node != null;)
        {
            var client = node.Value;

            if (node.Previous == null)
            {
                GoToBar(client);
            }
            else
            {
                GoToClient(client, node.Previous.Value);
            }

            if (client.IsExhausted())
            {
                Leave(client);
            }

            if (!client.HasOrder() && client.IsNear(bar.position, -client.GetOffset(), MaxDistance))
            {
                AskOrder(client);
            }

            node = node.Previous;
        }
    }

    private void UpdateLeaving()
    {
        while (_leavingClients.Count > 0)
        {
            var leavingClient = _leavingClients.Peek();
            if (leavingClient.IsNear(spawn.position, 0, MinDistance))
            {
                _leavingClients.Dequeue();
                Destroy(leavingClient.gameObject);
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
        CreateNewClient();
    }

    private void CreateNewClient()
    {
        if (_clients.Count >= maxClients)
        {
            return;
        }

        var client = SpawnManager.Main.Spawn<Client>(Spawnable.Client);
        client.CollisionListeners += ReceiveOrder;
        _clients.AddLast(client);
    }

    private void GoToBar(Client client)
    {
        client.MoveTo(bar.position, -client.GetOffset(), MinDistance);
    }

    private void GoToClient(Client client, Client leader)
    {
        client.MoveTo(leader.transform.position, -client.GetOffset(), MinDistance);
    }

    private void AskOrder(Client client)
    {
        _orders[client] = client.Order();
        client.Await();
    }

    private void ReceiveOrder(Client client, Glass glass)
    {
        if (!_orders.TryGetValue(client, out var expected))
        {
            return;
        }

        _orders.Remove(client);
        var actual = Cocktail.BuildCustom(glass.Recipe);
        var satisfaction = client.Serve(expected, actual);

        if (satisfaction > Satisfaction.Low)
        {
            var price = CashManager.Main.GetPrice(expected.Key);
            CashManager.Main.Cash += client.Pay(price, satisfaction);
            _combo++;

            if (_combo % 3 == 0)
            {
                AudioManager.Main.PlayLaugh();
            }
        }

        Destroy(glass.gameObject);
        Leave(client, satisfaction);

        SpawnManager.Main.Spawn(Spawnable.Glass);
        _spawnTimeRange.x = Mathf.Max(0, _spawnTimeRange.x - 1);
    }

    private void Leave(Client client, int satisfaction = 0)
    {
        var node = _clients.Find(client);

        if (node == null)
        {
            throw new InvalidOperationException("Client couldn't be found in clients queue");
        }

        if (satisfaction > Satisfaction.Low)
        {
            AudioManager.Main.PlaySuccess();
        }
        else
        {
            AudioManager.Main.PlayFailure();
        }

        _clients.Remove(node);
        _leavingClients.Enqueue(client);
        client.Leave(satisfaction);
        client.MoveTo(spawn.position, 0, MinDistance);
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
}