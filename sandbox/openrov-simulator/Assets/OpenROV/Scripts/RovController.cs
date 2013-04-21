using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using RenoSpaceApps.OpenROV;

public class RovController : MessageReceiver
{
    public PropController LeftProp;
    public PropController RightProp;
    public PropController TopProp;

    public override bool HandleMessage(string message)
    {
        var motorCommands = new List<string>();
        motorCommands.AddRange(message.Split(':'));

        var LeftPropValue = 0;
        var LeftPropValueValid = Int32.TryParse(motorCommands[0], out LeftPropValue);

        var RightPropValue = 0;
        var RightPropValueValid = false;

        var TopPropValue = 0;
        var TopPropValueValid = false;

        if (motorCommands.Count > 2)
        {
            Int32.TryParse(motorCommands[1], out RightPropValue);
        }

        if (motorCommands.Count > 3)
        {
            Int32.TryParse(motorCommands[2], out TopPropValue);
        }

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

        return true;
    } 
}
