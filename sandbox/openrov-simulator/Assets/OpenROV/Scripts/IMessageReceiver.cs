using UnityEngine;
using System.Collections;

namespace RenoSpaceApps.OpenROV
{
    public interface IMessageReceiver
    {
        bool HandleMessage(string message);
    }
}
