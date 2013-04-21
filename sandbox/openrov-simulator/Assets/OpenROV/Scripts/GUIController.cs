using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RenoSpaceApps.OpenROV;
using SimpleJSON;

public class GUIController : MessageReceiver
{
    protected enum GuiMode { None, SendCommand, DisplayMessages, DisplayGUI };

    protected GuiMode currentMode = GuiMode.None;

    protected List<string> messages = new List<string>();

    private string defaultCommand = "";
    private string lastCommand;

    protected float depth;
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode = GuiMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMode = GuiMode.DisplayMessages;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentMode = GuiMode.SendCommand;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentMode = GuiMode.DisplayGUI;
        }
	}

    public void OnGUI()
    {
        switch (currentMode)
        {
            case GuiMode.DisplayMessages:
                DisplayMessageGUI();
                break;

            case GuiMode.SendCommand:
                SendCommand();
                break;

            case GuiMode.DisplayGUI:
                DisplayGUI();
                break;
        }
    }

    protected void DisplayMessageGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Label(GuiMode.DisplayMessages.ToString());

        messages.ForEach( message => GUILayout.Label(message));

        if (GUILayout.Button("Clear"))
        {
            messages.Clear();
        }

        GUILayout.EndVertical();
    }

    protected void SendCommand()
    {
        GUILayout.BeginVertical();

        GUILayout.Label(GuiMode.SendCommand.ToString());

        if(string.IsNullOrEmpty(lastCommand))
        {
            lastCommand = defaultCommand;
        }

        lastCommand = GUILayout.TextArea(lastCommand);

        if (GUILayout.Button("Send Command"))
        {
            Client.Publish(lastCommand);
        }

        GUILayout.EndVertical();
    }

    protected void DisplayGUI()
    {
        GUILayout.Label("Depth: " + depth);
    }

    public override bool HandleMessage(string message)
    {
        messages.Add(message);

        var GUICommands = JSON.Parse(message);

        if (GUICommands["Depth"] != null)
        {
            depth = GUICommands["Depth"].AsFloat;
        }
        return true;
    }
}
