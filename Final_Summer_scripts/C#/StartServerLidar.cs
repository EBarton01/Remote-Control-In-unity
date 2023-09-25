using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

public class StartServerLidar : MonoBehaviour
{
    private const int port = 8888;
    private TcpListener server;

    public GameObject WallPrefab;  // Assign your prefab in the Inspector

    // Define the maximum angle to consider (e.g., 360 degrees)
    private const int maxAngle = 360;

    // Reference to the MainRover GameObject
    private GameObject mainRover;

    // Position of the MainRover GameObject
    private Vector3 mainRoverPosition;

    private void Start()
    {
        // Find and store the reference to the MainRover GameObject
        mainRover = GameObject.Find("MainRover");
        if (mainRover == null)
        {
            Debug.LogError("MainRover GameObject not found!");
        }

        // Capture the position of the MainRover on the main thread
        if (mainRover != null)
        {
            mainRoverPosition = mainRover.transform.position;
        }

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

            // Create a StringBuilder to buffer the incoming data
            StringBuilder messageBuilder = new StringBuilder();

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

                // Append the received bytes to the message builder
                messageBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                // Check if the received data contains a complete message (e.g., ends with a closing curly brace)
                string message = messageBuilder.ToString();
                if (message.EndsWith("}"))
                {
                    // Process the complete message
                    ProcessMessage(message);

                    // Clear the message builder for the next message
                    messageBuilder.Clear();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error handling incoming message: " + e.Message);
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            // Deserialize the message as JSON data into a dictionary
            Dictionary<string, int> lidarData = JsonConvert.DeserializeObject<Dictionary<string, int>>(message);

            // Check if the MainRover reference is valid
            if (mainRover == null)
            {
                Debug.LogError("MainRover reference is null!");
                return;
            }

            // Use the captured position of the MainRover GameObject
            Vector3 mainRoverPosition = this.mainRoverPosition;


        // Create a list to store positions for instantiation
        List<Vector3> spawnPositions = new List<Vector3>();

        // Process the Lidar data and add positions to the list for all angles
        for (int angle = 0; angle <= maxAngle; angle++)
        {
            // Convert the angle to a string
            string angleStr = angle.ToString();

            // If the angle is present in the received data, use its distance; otherwise, assume a default distance of 0
            int distance = lidarData.ContainsKey(angleStr) ? lidarData[angleStr] : 0;

            // Only add positions for angles where the distance is greater than zero
            if (distance > 0)
            {
                // Calculate spawn position based on angle and a fixed distance (e.g., 10 units) around the MainRover
                float angleRadians = Mathf.Deg2Rad * angle;
                float spawnDistance = distance * .008f; // Adjust this distance as needed
                Vector3 spawnOffset = new Vector3(Mathf.Cos(angleRadians) * spawnDistance, 0, Mathf.Sin(angleRadians) * spawnDistance);
                Vector3 spawnPosition = mainRoverPosition + spawnOffset;

                // Add the spawn position to the list
                spawnPositions.Add(spawnPosition);

                // Print the data to the console
                Debug.Log("Angle: " + angle + ", Distance: " + distance);
            }
        }

        // Use the MainThreadDispatcher to instantiate objects on the main thread
        MainThreadDispatcher.ExecuteInMainThread(() =>
        {
            foreach (Vector3 spawnPosition in spawnPositions)
            {
                // Instantiate the wall prefab at the calculated position
                Instantiate(WallPrefab, spawnPosition, Quaternion.identity);
            }
        });
    }
    catch (Exception e)
    {
        Debug.LogError("Error processing message: " + e.Message);
    }
}



    private void OnDestroy()
    {
        // Close the server when the script is destroyed
        if (server != null)
            server.Stop();
    }
}