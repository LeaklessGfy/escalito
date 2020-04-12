using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private readonly LinkedList<Client> _clients = new LinkedList<Client>();
    private readonly Dictionary<Client, Cocktail> _orders = new Dictionary<Client, Cocktail>();

    /* UNITY */
    private AudioSource _audioSource;
    private Text _selectedText;
    private Text _clockText;
    private Text _moneyText;

    /* STATE */
    private int _cash;
    private float _currentTime;
    private float _spawnTime;
    private Vector2 _range = new Vector2(10f, 10f);

    public Transform bar;
    public Transform origin;
    public GameObject clientPrefab;
    public GameObject glassPrefab;
    public int maxClients = 3;

    public Selectable Selected { get; set; }

    private void Awake()
    {
        Main = this;

        _audioSource = GetComponent<AudioSource>();
        _selectedText = GameObject.Find("Selected").GetComponent<Text>();
        _clockText = GameObject.Find("Clock").GetComponent<Text>();
        _moneyText = GameObject.Find("Money").GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        UpdateUI();
        UpdateSelected();
        UpdateQueue();
        UpdateSpawn();
    }

    private void UpdateUI()
    {
        _clockText.text = GetTime();
        _moneyText.text = _cash + "$";
    }

    private void UpdateSelected()
    {
        _selectedText.text = Selected ? Selected.name : "";
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
        
        if (satisfaction > 20)
        {
            var price = expected.Price + (satisfaction > 100 ? Random.Range(0, 5) : 0);
            client.Pay(price);
            _cash += price;
            _audioSource.Play();
        }
        else
        {
            client.GetComponent<AudioSource>().Play();
        }

        Destroy(glass.gameObject);
        LeaveAsync(client, satisfaction);
        Instantiate(glassPrefab, new Vector3(0, 20, 0), Quaternion.identity);
        _range.x = Mathf.Max(0, _range.x - 1);
    }

    private async void LeaveAsync(Client client, float satisfaction = 0f)
    {
        var node = _clients.Find(client);

        if (node == null)
        {
            throw new InvalidOperationException("Client couldn't be found in clients queue");
        }
        
        _clients.Remove(node);
        client.Leave();
        client.Satisfaction(satisfaction);
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