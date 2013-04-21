using UnityEngine;
using System.Collections;
using MqttLib;
using System;

public class MqttTest : MonoBehaviour 
{
    public string ConnectionString;
    public string UserName;
    public string Password;
    public string ClientId;
    public string Topic;

    protected IMqtt _client;

	// Use this for initialization
	protected void Start () 
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
		return true;
	}

    public void PublishSomething(object message)
    {
        Debug.Log("Publishing on " + Topic);
        _client.Publish(Topic, message.ToString(), QoS.BestEfforts, false);
    }
}
