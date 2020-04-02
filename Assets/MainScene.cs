using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public GameObject clientPrefab;
    public Transform origin;
    public Transform barPosition;
    public int maxClients = 3;

    private readonly LinkedList<Character> clients = new LinkedList<Character>();

    private void Start()
    {
        InvokeRepeating("CreateNewClient", 1f, 2f);
        InvokeRepeating("TestFollow", 7f, 7f);
    }

    private void TestFollow()
    {
        Leave(clients.First.Next.Value);
    }

    private void CreateNewClient()
    {
        if (clients.Count >= maxClients) {
            return;
        }
        Vector3 position = new Vector3(origin.position.x, origin.position.y, Camera.main.transform.position.z);
        GameObject gameObject = Instantiate(clientPrefab, position, Quaternion.identity);
        Character client = gameObject.GetComponent<Character>();
        AddClient(client);
    }

    private void AddClient(Character client)
    {
        // print("New client");
        if (clients.Count == 0) {
            GoToBar(client);
        } else {
            GoFollow(client, clients.Last.Value);
        }
        clients.AddLast(client);
    }

    private async void GoToBar(Character client)
    {
        Task move = client.MoveTo(barPosition.position); // should await here, and wait prepare order
        Task exhausted = Task.Delay(15000); // Client.Wait();
        Task first = await Task.WhenAny(move, exhausted);
        if (first == move && !first.IsCanceled) {
            Order(client);
        } else if (first == exhausted) {
            Leave(client);
        }
    }

    private void GoFollow(Character client, Character leader)
    {
        client.Follow(leader);
        // on progress, increment exhaust ?
        // run exhaust parallel
    }

    private void Order(Character client)
    {
        // print("Arrive");
    }

    private async void Leave(Character client)
    {
        print("Leave");
        LinkedListNode<Character> node = clients.Find(client);

        if (node == null) {
            return;
        }

        Character leader = node.Previous != null ? node.Previous.Value : null;
        Character follower = node.Next != null ? node.Next.Value : null;

        if (follower) {
            if (leader) {
                GoFollow(follower, leader);
            } else {
                GoToBar(node.Next.Value);
            }
            // Dispatch add exhausted time ?
        }

        clients.Remove(node);
        try {
            await client.MoveTo(origin.position);
            Destroy(client.gameObject);
        } catch (Exception e) {
            print(e);
        }
    }
}
