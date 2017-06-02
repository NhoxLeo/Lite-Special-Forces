using UnityEngine;
using System.Collections;

//Lock mouse in the screen

public class MenuScript : MonoBehaviour {
    bool lockcursor = true;

	// Use this for initialization
	void Start () {
        Screen.showCursor = false;
        Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
            if (lockcursor)
            {
                lockcursor = false;
                Screen.showCursor = true;
                Screen.lockCursor = false;
                
            }
            else
            {
                lockcursor = true;
                Screen.showCursor = false;
                Screen.lockCursor = true;
            }
		}
	}

    void OnGUI()
    {
        if (!lockcursor)
        {
            if (GUI.Button(new Rect(10, 10, 90, 30), "Disconnect")) //Show disconnect button
            {
                Network.Disconnect(); // Tell all the other clients you're disconnecting
                MasterServer.UnregisterHost();
                NetworkMasterServer nms = GameObject.Find("MasterServerMenu(Clone)").GetComponent("NetworkMasterServer") as NetworkMasterServer;
                nms.dc();
                Destroy(nms.gameObject);
                // Return to the Master Game Server Lobby because we pressed disconnect
                Application.LoadLevel("MasterGameServerLobby");
                
            }
        }
    }
}
