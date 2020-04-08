using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Core;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject clientPrefab;
    public Button button;
    public Text hour;
    public Text money;
    public Transform origin;
    public Transform barPosition;
    public int maxClients = 3;

    private SpriteRenderer _spriteRenderer;
    private readonly LinkedList<Character> _clients = new LinkedList<Character>();
    private readonly List<Order> _orders = new List<Order>();

    private void Awake()
    {
        _spriteRenderer = clientPrefab.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // InvokeRepeating("CreateNewClient", 0f, 2f);
        // InvokeRepeating("TestFollow", 7f, 7f);
    }

    private void Update()
    {
        hour.text = Time.time.ToString(CultureInfo.InvariantCulture);
    }

    private void TestFollow()
    {
        if (_clients.First.Next != null)
        {
            LeaveAsync(_clients.First.Next.Value);
        }
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
        client.Id = _clients.Count;

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
        var position = barPosition.position;
        var point = new Vector2(position.x - _spriteRenderer.sprite.bounds.extents.x, position.y);
        await client.MoveToAsync(point);
        var expected = Order(client);

        try
        {
            var actual = await client.Await();
            var satisfaction = client.Check(expected, actual);
            print(satisfaction);
            LeaveAsync(client);
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

    private Cocktail Order(Character client)
    {
        var expected = Cocktail.BuildRandom();

        button.gameObject.SetActive(true);
        button.transform.position = client.transform.position + new Vector3(0, _spriteRenderer.sprite.bounds.extents.y, 0);
        button.GetComponentInChildren<Text>().text = expected.Recipe.ToString();

        button.onClick.AddListener(() =>
        {
            // add in cocktails queue ...
            // on an another trigger
            // queue of order, that contain : cocktail asked and client
            var formula = new Dictionary<Consumable, int>
            {
                [Consumable.Rum] = 50
            };
            var actual = Cocktail.BuildCustom(formula);

            client.Serve(actual);
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        });

        _orders.Add(new Order(client, expected));

        return expected;
    }

    private async void LeaveAsync(Character client)
    {
        var node = _clients.Find(client);

        if (node == null)
        {
            throw new InvalidOperationException("Client couldn't be found in clients queue");
        }

        var follower = node.Next?.Value;
        var leader = node.Previous?.Value;
        _clients.Remove(node);

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
            // Dispatch minus time awaited to all awaiting clients (different waiting ?) ?
        }

        await client.MoveToAsync(origin.position);
        Destroy(client.gameObject);
    }
}
