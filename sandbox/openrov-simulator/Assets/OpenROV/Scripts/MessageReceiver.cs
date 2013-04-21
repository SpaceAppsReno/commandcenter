using UnityEngine;
using System.Collections;
using RenoSpaceApps.OpenROV;

public abstract class MessageReceiver : MonoBehaviour, IMessageReceiver
{
    public abstract bool HandleMessage(string message);
}
