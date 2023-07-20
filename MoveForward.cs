using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class MoveForward : MonoBehaviour
{
    public Button buttonF;
    public Button buttonR;
    public Button buttonL;
    public Button buttonRight;

    public float movementSpeed = 100f;
    float rotationAngle = 10f;
    public Transform objectToMove;
    public Transform cubeTransform;
    public float maxDistanceFromCube = 5f;

    private TcpClient client;
    private NetworkStream networkStream;
    private Thread listeningThread;
    //private string raspberryPiIP = "45.152.182.93";
    private string raspberryPiIP = "192.168.1.25";
    private bool buttonPressed = false;

    private void Start()
    {
        try 
        {
            Debug.Log("1");
            client = new TcpClient(raspberryPiIP, 12345);
            Debug.Log("2");
            networkStream = client.GetStream();
            Debug.Log("3");
            listeningThread = new Thread(ListenForMessages);
            Debug.Log("4");
            listeningThread.Start();

            buttonF.onClick.AddListener(OnButtonClickF);
            buttonR.onClick.AddListener(OnButtonClickR);
            buttonL.onClick.AddListener(OnButtonClickL);
            buttonRight.onClick.AddListener(OnButtonClickRight);
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
        }
    }

    private void OnButtonClickF()
    {
        Debug.Log("Forward");
        buttonPressed = true;
        //Debug.Log(buttonPressed);

        SendMessageToServer("Forward");
        buttonPressed = false;

        objectToMove.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
        // Constrain the object's position within the cube
        Vector3 constrainedPosition = objectToMove.position;
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, cubeTransform.position.x - maxDistanceFromCube, cubeTransform.position.x + maxDistanceFromCube);
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, cubeTransform.position.y - maxDistanceFromCube, cubeTransform.position.y + maxDistanceFromCube);
        constrainedPosition.z = Mathf.Clamp(constrainedPosition.z, cubeTransform.position.z - maxDistanceFromCube, cubeTransform.position.z + maxDistanceFromCube);
        objectToMove.position = constrainedPosition;
    }

    private void OnButtonClickR()
    {
        Debug.Log("Revese");
        buttonPressed = true;
        //Debug.Log(buttonPressed);

        SendMessageToServer("Reverse");
        buttonPressed = false;

        objectToMove.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        // Constrain the object's position within the cube
        Vector3 constrainedPosition = objectToMove.position;
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, cubeTransform.position.x - maxDistanceFromCube, cubeTransform.position.x + maxDistanceFromCube);
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, cubeTransform.position.y - maxDistanceFromCube, cubeTransform.position.y + maxDistanceFromCube);
        constrainedPosition.z = Mathf.Clamp(constrainedPosition.z, cubeTransform.position.z - maxDistanceFromCube, cubeTransform.position.z + maxDistanceFromCube);
        objectToMove.position = constrainedPosition;
    }

    private void OnButtonClickL()
    {
        Debug.Log("Left");
        buttonPressed = true;
        //Debug.Log(buttonPressed);

        SendMessageToServer("Left");
        buttonPressed = false;

        objectToMove.Rotate(Vector3.up * rotationAngle);
        // Constrain the object's position within the cube
        Vector3 constrainedPosition = objectToMove.position;
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, cubeTransform.position.x - maxDistanceFromCube, cubeTransform.position.x + maxDistanceFromCube);
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, cubeTransform.position.y - maxDistanceFromCube, cubeTransform.position.y + maxDistanceFromCube);
        constrainedPosition.z = Mathf.Clamp(constrainedPosition.z, cubeTransform.position.z - maxDistanceFromCube, cubeTransform.position.z + maxDistanceFromCube);
        objectToMove.position = constrainedPosition;
    }

    private void OnButtonClickRight()
    {
        Debug.Log("Right");
        buttonPressed = true;
        //Debug.Log(buttonPressed);

        SendMessageToServer("Right");
        buttonPressed = false;

        objectToMove.Rotate(-Vector3.up * rotationAngle);
        // Constrain the object's position within the cube
        Vector3 constrainedPosition = objectToMove.position;
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, cubeTransform.position.x - maxDistanceFromCube, cubeTransform.position.x + maxDistanceFromCube);
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, cubeTransform.position.y - maxDistanceFromCube, cubeTransform.position.y + maxDistanceFromCube);
        constrainedPosition.z = Mathf.Clamp(constrainedPosition.z, cubeTransform.position.z - maxDistanceFromCube, cubeTransform.position.z + maxDistanceFromCube);
        objectToMove.position = constrainedPosition;
    }

    private void SendMessageToServer(string message)
    {
        if (client == null)
        {
            Debug.LogError("Not connected to server");
            return;
        }

        if (networkStream.CanWrite)
        {
            byte[] messageAsByteArray = Encoding.ASCII.GetBytes(message);
            networkStream.Write(messageAsByteArray, 0, messageAsByteArray.Length);
            //Debug.Log("MSG sentS");
        }
    }

    private void ListenForMessages()
    {
        byte[] buffer = new byte[1024];
        int byteCount;
        while (client.Connected)
        {

            if ((byteCount = networkStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string receivedMessage = Encoding.ASCII.GetString(buffer, 0, byteCount);
                Debug.Log($"Received message: {receivedMessage}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        SendMessageToServer("exit");
        client.Close();
        if (listeningThread != null)
            listeningThread.Abort();
    }
}
