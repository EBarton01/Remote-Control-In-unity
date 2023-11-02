using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class startServerUnity : MonoBehaviour
{
    private const int port = 8888;
    private TcpListener server;

    private Queue<string> messageQueue = new Queue<string>();
    private object queueLock = new object();

    public GameObject WallPrefab;  // Assign your prefab in the Inspector

    private void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        try
        {
            // Set up the server and start listening for incoming connections
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Debug.Log("Server started. Waiting for connections...");

            // Start accepting incoming connections in the background
            server.BeginAcceptTcpClient(HandleIncomingConnection, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting the server: " + e.Message);
        }
    }

    private void HandleIncomingConnection(IAsyncResult result)
    {
        try
        {
            // Accept the incoming connection
            TcpClient client = server.EndAcceptTcpClient(result);

            // Inform that a connection has been established
            Debug.Log("Connected to client: " + client.Client.RemoteEndPoint);

            // Start listening for incoming messages from this client
            StartListeningForMessages(client);

            // Continue accepting incoming connections by recursively calling BeginAcceptTcpClient
            server.BeginAcceptTcpClient(HandleIncomingConnection, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error handling incoming connection: " + e.Message);
        }
    }

    private void StartListeningForMessages(TcpClient client)
    {
        try
        {
            // Start listening for incoming messages from the connected client
            byte[] buffer = new byte[1024];
            NetworkStream stream = client.GetStream();

            // Continuously listen for incoming messages
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                // Check if the client disconnected
                if (bytesRead == 0)
                {
                    Debug.Log("Client disconnected: " + client.Client.RemoteEndPoint);
                    client.Close();
                    break;
                }

                // Get the message from the buffer
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Print the incoming message to the console
                Debug.Log("Received message from " + client.Client.RemoteEndPoint + ": " + message);

                // Add the message to the queue for processing on the main thread
                lock (queueLock)
                {
                    messageQueue.Enqueue(message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error handling incoming message: " + e.Message);
        }
    }

    private void Update()
    {
        // Process the message queue on the main thread
        lock (queueLock)
        {
            while (messageQueue.Count > 0)
            {
                string message = messageQueue.Dequeue();

                // Instantiate the prefab when a message is received
                if (message == "SpawnPrefab")
                {
                    InstantiatePrefab();
                }
            }
        }
    }

    private void InstantiatePrefab()
    {
        // Instantiate the prefab at the center of the screen
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
        Instantiate(WallPrefab, spawnPosition, Quaternion.identity);
    }

    private void OnDestroy()
    {
        // Close the server when the script is destroyed
        if (server != null)
            server.Stop();
    }
}
