using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject clientPrefab;
    public Button button;
    public Transform origin;
    public Transform barPosition;
    public int maxClients = 3;

    private SpriteRenderer spriteRenderer;
    private readonly LinkedList<Character> clients = new LinkedList<Character>();
    private readonly List<Order> orders = new List<Order>();

    public void OrderTrigger()
    {
    }

    private void Awake()
    {
        spriteRenderer = clientPrefab.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // InvokeRepeating("CreateNewClient", 0f, 2f);
        // InvokeRepeating("TestFollow", 7f, 7f);
    }

    private void TestFollow()
    {
        Leave(clients.First.Next.Value);
    }

    private void CreateNewClient()
    {
        if (clients.Count >= maxClients)
        {
            return;
        }

        Vector3 position = new Vector3(origin.position.x, origin.position.y, 0);
        GameObject gameObject = Instantiate(clientPrefab, position, Quaternion.identity);
        Character client = gameObject.GetComponent<Character>();
        client.id = clients.Count;

        AddClient(client);
    }

    private void AddClient(Character client)
    {
        if (clients.Count == 0)
        {
            GoToBar(client);
        }
        else
        {
            GoFollow(client, clients.Last.Value);
        }
        clients.AddLast(client);
    }

    private async void GoToBar(Character client)
    {
        Vector2 point = new Vector2(barPosition.position.x - spriteRenderer.sprite.bounds.extents.x, barPosition.position.y);
        await client.MoveTo(point);
        Cocktail expected = Order(client);

        try
        {
            Cocktail actual = await client.Await();
            int satisfaction = client.Check(expected, actual);
            print(satisfaction);
            Leave(client);
        }
        catch (TaskCanceledException)
        {
            Leave(client);
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
        Cocktail expected = Cocktail.BuildRandom();

        button.gameObject.SetActive(true);
        button.transform.position = client.transform.position + new Vector3(0, spriteRenderer.sprite.bounds.extents.y, 0);
        button.GetComponentInChildren<Text>().text = expected.Type.ToString();

        button.onClick.AddListener(() =>
        {
            // add in cocktails queue ...
            // on an another trigger
            // queue of order, that contain : cocktail asked and client
            var formula = new Dictionary<Cocktail.Consumable, int>
            {
                [Cocktail.Consumable.Rhum] = 50
            };
            var actual = Cocktail.BuildCustom(formula);

            client.Serve(actual);
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        });

        orders.Add(new Order(client, expected));

        return expected;
    }

    private async void Leave(Character client)
    {
        LinkedListNode<Character> node = clients.Find(client);

        if (node == null)
        {
            throw new InvalidOperationException("Client couldn't be found in clients queue");
        }

        Character follower = node.Next != null ? node.Next.Value : null;
        Character leader = node.Previous != null ? node.Previous.Value : null;
        clients.Remove(node);

        if (follower)
        {
            if (leader)
            {
                GoFollow(follower, leader);
            }
            else
            {
                GoToBar(follower);
            }
            // Dispatch minus time awaited to all awaiting clients (different waiting ?) ?
        }

        await client.MoveTo(origin.position);
        Destroy(client.gameObject);
    }
}
