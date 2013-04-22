using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using RenoSpaceApps.OpenROV;
using SimpleJSON;

public class RovController : MessageReceiver
{
    public PropController LeftProp;
    public PropController RightProp;
    public PropController TopProp;

    public Transform Light1;
    public Transform Light2;

    public Transform LeftThrustPoint;
    public Transform RightThrustPoint;
    public Transform TopThrustPoint;

    protected int LeftPropValue = 0;
    protected int RightPropValue = 0;
    protected int TopPropValue = 0;

    public override bool HandleMessage(string message)
    {
        var motorCommands = JSON.Parse(message);

        
        var LeftPropValueValid = false;
        var RightPropValueValid = false;
        var TopPropValueValid = false;

        if (motorCommands["Motor1"] != null)
        {
            LeftPropValueValid = true;
            LeftPropValue = motorCommands["Motor1"].AsInt;
        }

        if (motorCommands["Motor2"] != null)
        {
            RightPropValueValid = true;
            RightPropValue = motorCommands["Motor2"].AsInt;
        }

        if (motorCommands["Motor3"] != null)
        {
            TopPropValueValid = true;
            TopPropValue = motorCommands["Motor3"].AsInt;
        }


        if(motorCommands["Camera"] != null)
        {
            var cameraValue = motorCommands["Camera"].AsInt;

            switch (cameraValue)
            {
                case 1:
                    Light1.localRotation = Quaternion.Euler(-25.0f, 0.0f, 0.0f);
                    Light2.localRotation = Quaternion.Euler(-25.0f, 0.0f, 0.0f);
                    break;
                case 0:
                    Light1.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    Light2.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    break;
                case -1:
                    Light1.localRotation = Quaternion.Euler(25.0f, 0.0f, 0.0f);
                    Light2.localRotation = Quaternion.Euler(25.0f, 0.0f, 0.0f);
                    break;
            }
        }

        if(motorCommands["Brightness"] != null)
        {
            var brightness = motorCommands["Brightness"].AsInt;

            brightness = Mathf.Clamp(brightness, 0, 8);

            Light1.light.intensity = brightness;
            Light2.light.intensity = brightness;
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LeftPropValue = 0;
            RightPropValue = 0;
            TopPropValue = 0;
        }
    }

    public void FixedUpdate()
    {
        rigidbody.AddForceAtPosition(transform.forward * LeftPropValue, LeftThrustPoint.position, ForceMode.Force);
        rigidbody.AddForceAtPosition(transform.forward * RightPropValue, RightThrustPoint.position, ForceMode.Force);
        rigidbody.AddForceAtPosition(transform.up * TopPropValue, TopThrustPoint.position, ForceMode.Force);
    }
}
