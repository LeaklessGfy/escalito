using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private float _currentTime;
    private float _spawnTime;
    private Vector2 _range = new Vector2(10f, 10f);
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
            if (node.Previous != null)
            {
                GoFollow(node.Value, node.Previous.Value);
            }
            else
            {
                GoToBarAsync(node.Value);
            }
            node = node.Previous;
        }
    }

    private void UpdateSpawn()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime < _spawnTime)
        {
            return;
        }

        _spawnTime = Random.Range(_range.x, _range.y);
        _currentTime = 0;
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
        sprite.name = client.GetName();
        client.CollisionListeners += ReceiveOrder;

        AddClient(client);
    }

    private void AddClient(Client client)
    {
        if (_clients.Count == 0)
        {
            GoToBarAsync(client);
        }
        else
        {
            GoFollow(client, _clients.Last.Value);
        }

        _clients.AddLast(client);
    }

    private async void GoToBarAsync(Client client)
    {
        var hasMoved = await client.MoveToAsync(bar.transform.position);
        if (!hasMoved)
        {
            return;
        }
        
        AskOrder(client);

        try
        {
            await client.Await();
        }
        catch (TaskCanceledException)
        {
            LeaveAsync(client);
        }
    }

    private async void GoFollow(Client client, Client leader)
    {
        var hasMoved = await client.MoveToAsync(leader.transform.position, 2);
        if (!hasMoved)
        {
            return;
        }

        /*AskOrder(client);
        
        try
        {
            await client.Await();
        }
        catch (TaskCanceledException)
        {
            LeaveAsync(client);
        }*/
    }

    private void AskOrder(Client client)
    {
        _orders[client] = client.Order();
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
        
        _range.x = Mathf.Max(0, _range.x - 1);
    }

    private async void LeaveAsync(Client client, int satisfaction = 0)
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
        await client.MoveToAsync(origin.position);
        Destroy(client.gameObject);
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