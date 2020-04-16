using System;
using System.Collections.Generic;
using Components;
using Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Selectable = Components.Selectable;

public class Controller : MonoBehaviour
{
    /* CONSTANT */
    private const int MinutesPerDay = 10;
    private const int MinDistance = 2;
    private const int MaxDistance = 20;
    public static Controller Main;

    /* DEPENDENCIES */
    [SerializeField] private Text selectedText = default;
    [SerializeField] private Text clockText = default;
    [SerializeField] private Text moneyText = default;
    [SerializeField] private Transform bar = default;
    [SerializeField] private Transform origin = default;
    [SerializeField] private GameObject clientPrefab = default;
    [SerializeField] private GameObject glassPrefab = default;
    [SerializeField] private int maxClients = 3;

    /* STATE */
    private int _cash;
    private float _nextSpawnTime;
    private float _currentSpawnTime;
    private Vector2 _spawnTimeRange = new Vector2(1f, 2);
    private bool _barIsOpen;

    private readonly LinkedList<Client> _clients = new LinkedList<Client>();
    private readonly Dictionary<Client, Cocktail> _orders = new Dictionary<Client, Cocktail>();

    /* PUBLIC */
    public Selectable Selected { get; set; }

    private void Awake()
    {
        Main = this;
    }

    private void FixedUpdate()
    {
        UpdateUi();
        UpdateSelected();
        UpdateQueue();
        UpdateSpawn();
    }

    public bool ToggleBar()
    {
        if (_barIsOpen)
        {
            return _barIsOpen = false;
        }
        return _barIsOpen = true;
    }

    private void UpdateUi()
    {
        clockText.text = GetTime();
        moneyText.text = _cash + "$";
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

            if (client.IsNear(GetPosition(client, bar), MaxDistance))
            {
                AskOrder(client);
            }
            else if (client.HasOrder() && client.IsExhausted())
            {
                LeaveAsync(client);
            }

            node = node.Previous;
        }
    }

    private void UpdateSpawn()
    {
        if (!_barIsOpen)
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

        var sprite = Instantiate(clientPrefab, origin.position, Quaternion.identity);
        var client = sprite.GetComponent<Client>();
        sprite.name = Client.GetName();
        client.CollisionListeners += ReceiveOrder;
        _clients.AddLast(client);
    }

    private void GoToBar(Client client)
    {
        client.MoveTo(GetPosition(client, bar), MinDistance);
    }

    private void GoToClient(Client client, Client leader)
    {
        client.MoveTo(GetPosition(client, leader), MinDistance);
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
            _cash += client.Pay(expected.Price, satisfaction);
        }

        Destroy(glass.gameObject);
        LeaveAsync(client, satisfaction);
        
        var newInstance = Instantiate(glassPrefab, new Vector3(0, 20, 0), Quaternion.identity);
        newInstance.name = "Glass";
        
        _spawnTimeRange.x = Mathf.Max(0, _spawnTimeRange.x - 1);
    }

    private void LeaveAsync(Client client, int satisfaction = 0)
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
        client.Leave(satisfaction);
        client.MoveTo(origin.position, MinDistance);
        // Destroy(client.gameObject); in update
    }
    
    private static Vector2 GetPosition(Client client, Component component)
    {
        return new Vector2(component.transform.position.x - client.GetOffset(), client.transform.position.y);
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