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
    private const int MinutesPerDay = 10;
    public static Controller Main;

    private readonly LinkedList<Character> _clients = new LinkedList<Character>();
    private int _cash;

    private SpriteRenderer _clientSpriteRenderer;
    private Order _currentOrder;
    private float _currentTime;
    private Text _orderAskText;

    private Text _selectedText;
    private float _spawnTime;

    public Bar bar;
    public GameObject clientPrefab;
    public Text clock;
    public int maxClients = 3;
    public Text money;
    public Button orderAskButton;
    public Transform origin;

    public Selectable Selected { get; set; }

    private void Awake()
    {
        Main = this;

        _clientSpriteRenderer = clientPrefab.GetComponent<SpriteRenderer>();
        _orderAskText = orderAskButton.GetComponentInChildren<Text>();
        _selectedText = GameObject.Find("Selected").GetComponent<Text>();
        bar.CollisionListeners += ReceiveOrder;
    }

    private void Update()
    {
        UpdateClock();
        UpdateMoney();
        UpdateSelected();
        UpdateSpawn();
    }

    private void UpdateClock()
    {
        var now = GetTime();
        var hours = now.Hours.ToString().PadLeft(2, '0');
        var minutes = now.Minutes.ToString().PadLeft(2, '0');
        clock.text = hours + ":" + minutes;
    }

    private void UpdateMoney()
    {
        money.text = _cash + "$";
    }

    private void UpdateSelected()
    {
        _selectedText.text = Selected ? Selected.name : "";
    }

    private void UpdateSpawn()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime < _spawnTime)
        {
            return;
        }

        _spawnTime = Random.Range(5.0f, 30.0f);
        _currentTime = 0;
        CreateNewClient();
    }

    private void CreateNewClient()
    {
        if (_clients.Count >= maxClients)
        {
            return;
        }

        var originPosition = origin.position;
        var sprite = Instantiate(clientPrefab, originPosition, Quaternion.identity);
        var client = sprite.GetComponent<Character>();

        AddClient(client);
    }

    private void AddClient(Character client)
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

    private async void GoToBarAsync(Character client)
    {
        var position = bar.transform.position;
        var point = new Vector2(position.x - _clientSpriteRenderer.sprite.bounds.extents.x, position.y);
        await client.MoveToAsync(point);
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

    private void GoFollow(Character client, Character leader)
    {
        client.Follow(leader);
        // on progress, increment exhaust ?
        // run exhaust parallel
    }

    private void AskOrder(Character client)
    {
        var expected = Cocktail.BuildRandom();
        _currentOrder = new Order(client, expected);

        orderAskButton.gameObject.SetActive(true);
        orderAskButton.transform.position = client.transform.position +
                                            new Vector3(0, _clientSpriteRenderer.sprite.bounds.extents.y, 0);
        _orderAskText.text = expected.Name.ToString();

        orderAskButton.onClick.AddListener(() =>
        {
            client.PatienceBonus(2);
        });
    }

    private void ReceiveOrder(Glass glass)
    {
        if (_currentOrder == null)
        {
            return;
        }

        var client = _currentOrder.Client;
        var expected = _currentOrder.Cocktail;
        var actual = Cocktail.BuildCustom(glass.Recipe);

        var satisfaction = client.Serve(expected, actual);
        LeaveAsync(client, satisfaction);

        if (satisfaction > 0)
        {
            _cash += expected.Price;
        }

        glass.Drain();
        CleanOrderAsk();
    }

    private void CleanOrderAsk()
    {
        orderAskButton.onClick.RemoveAllListeners();
        orderAskButton.gameObject.SetActive(false);
        _currentOrder = null;
    }

    private async void LeaveAsync(Character client, float satisfaction = 0f)
    {
        var node = _clients.Find(client);

        if (node == null)
        {
            throw new InvalidOperationException("Client couldn't be found in clients queue");
        }

        var follower = node.Next?.Value;
        var leader = node.Previous?.Value;
        _clients.Remove(node);
        CleanOrderAsk();

        if (follower)
        {
            if (leader)
            {
                GoFollow(follower, leader);
            }
            else
            {
                GoToBarAsync(follower);
            }

            // Dispatch bonus time awaited to all awaiting clients (different waiting ?) ?
        }

        client.Satisfaction(satisfaction);
        await client.MoveToAsync(origin.position);
        Destroy(client.gameObject);
    }

    private static TimeSpan GetTime()
    {
        var now = DateTime.Now;
        var timeNow = now.TimeOfDay;
        var hours = timeNow.TotalMinutes % MinutesPerDay;
        var minutes = hours % 1 * 60;
        var seconds = minutes % 1 * 60;

        return new TimeSpan((int) hours, (int) minutes, (int) seconds);
    }
}