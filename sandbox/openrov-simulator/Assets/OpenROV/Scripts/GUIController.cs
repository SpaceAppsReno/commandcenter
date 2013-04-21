using UnityEngine;
using System.Collections;
using RenoSpaceApps.OpenROV;

public class GUIController : MessageReceiver
{

    protected enum GuiMode { None, SendCommand, DisplayData };

    protected GuiMode currentMode = GuiMode.None;
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode = GuiMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMode = GuiMode.DisplayData;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentMode = GuiMode.SendCommand;
        }
	}

    public void OnGUI()
    {
        switch (currentMode)
        {
            case GuiMode.DisplayData:
                DisplayDataGUI();
                break;

            case GuiMode.SendCommand:
                SendCommand();
                break;
        }
    }

    protected void DisplayDataGUI()
    {
        GUILayout.Label(GuiMode.DisplayData.ToString());
    }

    protected void SendCommand()
    {
        GUILayout.Label(GuiMode.SendCommand.ToString());
    }

    public override bool HandleMessage(string message)
    {
        return false;
    }
}
