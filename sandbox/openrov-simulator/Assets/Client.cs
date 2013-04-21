using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Client : MonoSingleton<Client>
{
    public List<MessageReceiver> Receivers = new List<MessageReceiver>();

    public void HandleMessage(string message)
    {
        var messageHandled = false;
        Receivers.ForEach(receivers => messageHandled |= receivers.HandleMessage(message));

        if (!messageHandled)
        {
            Debug.Log("Message: " + message + " was not handled!!!");
        }
    }

    public static void Publish(string message)
    {
#if !UNITY_WEBPLAYER
        instance.HandleMessage(message);
#else
        Application.ExternalCall("PublishMessage", message);
#endif
    }
}
