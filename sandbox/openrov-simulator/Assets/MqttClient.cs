using UnityEngine;
using System.Collections;
using MqttLib;
using System;
using System.Collections.Generic;
using RenoSpaceApps.OpenROV;
using System.Linq;

public class MqttClient : MonoSingleton<MqttClient>
{
    public string ConnectionString;
    public string UserName;
    public string Password;
    public string ClientId;
    public string Topic;

    public List<MessageReceiver> Receivers = new List<MessageReceiver>();

    protected IMqtt _client;
    
    protected Queue<string> messageQueue = new Queue<string>();

    // Use this for initialization
    protected void Start()
    {
        Debug.Log("Starting");
        // Instantiate client using MqttClientFactory
        _client = MqttClientFactory.CreateClient(ConnectionString, ClientId, UserName, Password);

        Debug.Log(_client);
        // Setup some useful client delegate callbacks
        _client.Connected += new ConnectionDelegate(client_Connected);
        _client.ConnectionLost += new ConnectionDelegate(_client_ConnectionLost);
        _client.PublishArrived += new PublishArrivedDelegate(client_PublishArrived);


        Connect();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Connect();
        }

        var proccessed = 0;
        while(messageQueue.Count > 0 || proccessed > 10)
        {
            var currentCommand = messageQueue.Dequeue();

            HandleMessage(currentCommand);
            proccessed++;
        }
    }


    #region MQTT

    private void Connect()
    {
        Debug.Log("Connecting......");
        _client.Connect();
    }

    protected void client_Connected(object sender, EventArgs e)
    {
        Debug.Log("Client connected\n");
        RegisterOurSubscriptions();
    }

    protected void _client_ConnectionLost(object sender, EventArgs e)
    {
        Debug.Log("Client connection lost\n");
    }

    protected void RegisterOurSubscriptions()
    {
        Debug.Log("Subscribing to " + Topic);
        _client.Subscribe(Topic, QoS.BestEfforts);
    }

    protected bool client_PublishArrived(object sender, PublishArrivedArgs e)
    {
        Debug.Log("Received Message");
        Debug.Log("Topic: " + e.Topic);
        Debug.Log("Payload: " + e.Payload);


        var message = e.Payload.ToString().Trim('\"');

        messageQueue.Enqueue(message);

        return true;
    }

    public void PublishSomething(object message)
    {
        Debug.Log("Publishing on " + Topic);
        _client.Publish(Topic, message.ToString(), QoS.BestEfforts, false);
    }

    protected void HandleMessage(string message)
    {
        var messageHandled = false;
        Receivers.ForEach( receivers => messageHandled |= receivers.HandleMessage(message));

        if (!messageHandled)
        {
            Debug.Log("Message: " + message + " was not handled!!!");
        }
    }

    #endregion
}
