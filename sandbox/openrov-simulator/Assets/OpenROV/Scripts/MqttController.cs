using UnityEngine;
using System.Collections;
using MqttLib;
using System;
using System.Collections.Generic;

public class MqttController : MonoBehaviour
{

    public PropController LeftProp;
    public PropController RightProp;
    public PropController TopProp;

    public string ConnectionString;
    public string UserName;
    public string Password;
    public string ClientId;
    public string Topic;

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


        Debug.Log("Connecting......");
        _client.Connect();
    }


    public void Update()
    {
        while(messageQueue.Count > 0)
        {
            var currentCommand = messageQueue.Dequeue();

            HandlePropCommand(currentCommand);
        }
    }

    protected void client_Connected(object sender, EventArgs e)
    {
        Debug.Log("Client connected\n");
        RegisterOurSubscriptions();
        //PublishSomething("Hello MQTT World");
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

    public void HandlePropCommand(string message)
    {
        var motorCommands = message.Split(':');

        var LeftPropValue = 0;
        var LeftPropValueValid = Int32.TryParse(motorCommands[0], out LeftPropValue);

        var RightPropValue = 0;
        var RightPropValueValid = Int32.TryParse(motorCommands[1], out RightPropValue);

        var TopPropValue = 0;
        var TopPropValueValid = Int32.TryParse(motorCommands[2], out TopPropValue);

        if (LeftPropValueValid)
        {
            if (LeftPropValue > 0)
            {
                LeftProp.MoveForward(Mathf.Abs(LeftPropValue));
            }
            else if (LeftPropValue < 0)
            {
                LeftProp.MoveReverse(Mathf.Abs(LeftPropValue));
            }
            else
            {
                LeftProp.Stop();
            }
        }

        if (RightPropValueValid)
        {
            if (RightPropValue > 0)
            {
                RightProp.MoveForward(Mathf.Abs(RightPropValue));
            }
            else if (RightPropValue < 0)
            {
                RightProp.MoveReverse(Mathf.Abs(RightPropValue));
            }
            else
            {
                RightProp.Stop();
            }
        }

        if (TopPropValueValid)
        {
            if (TopPropValue > 0)
            {
                TopProp.MoveForward(Mathf.Abs(TopPropValue));
            }
            else if (TopPropValue < 0)
            {
                TopProp.MoveReverse(Mathf.Abs(TopPropValue));
            }
            else
            {
                TopProp.Stop();
            }
        }
    }
}
