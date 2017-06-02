// You can just copy and paste this entire function and use it as is.
// However, some of the GUI menu items will need to be customized with the proper game name such as:
// gameType and the string: "Your Game Title Here"

//GUI.contentColor = Color.white; the text in the buttons and labels



using UnityEngine;
//using System.Collections;

public class NetworkMasterServer : MonoBehaviour {

// Keep track of various menu and ingame states
public enum menustate
{
	networklobby,    // Master Game Server Lobby
	ingame,               // In a game as either the GS or Client
}
public menustate gamemenustate = menustate.networklobby;

// NOTE: gameType is used to distinguish your game from any other game that might 
//             also be registered to the MGS. I also like to add a version number as part of the 
//             game type.  This way as you release new version, the client will have to match the
//             same version in order to connect
public string gameType = "ZSF_v0.1";

// NOTE: gameName is an arbitrary name that will show in the game lobby so other players 
//             find and distinguish your game from another.  We also add a text box so the 
//             game client can customize it before creating a new game
private string gameName = "Lite: Special Forces Server";
//private string gameName = "ZSF Server";
public int serverPort = 30000;  // The port must be unique to the GS and not conflict with other
                                // apps running on the game server.  This is how clients will 
                                // connect to the GS
private string gamePort = "30000";

// NOTE: The rest of the variables below are various attributes used to automatically refresh
//              the discovered game servers, and to keep track of the testing and results when
//              checking to see if NPT is required
private float lastHostListRequest = -1000.0f;
private float hostListRefreshTimeout = 10.0f;
private ConnectionTesterStatus natCapable = ConnectionTesterStatus.Undetermined;
private bool filterNATHosts = false;
private bool probingPublicIP = false;
private bool doneTesting = false;
private float timer = 0.0f;
private string testMessage = "Testing NAT capabilities";
public GUIStyle format = new GUIStyle();
private bool useNat = false;


private string joinip = "";
private string joinport = "30000";

	// Enable this if not running a client on the server machine
	// MasterServer.dedicatedServer = true;
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		//Debug.Log(info);
	}
	
	void OnFailedToConnect(NetworkConnectionError info) {
		//Debug.Log(info);
	}
	
	void OnGUI () {
		ShowGUI();
	}
	
	void Awake () {
		DontDestroyOnLoad(this);

		// Start connection test
		natCapable = Network.TestConnection();

		// What kind of IP does this machine have? TestConnection also indicates
		// this in the test results
		//if (Network.HavePublicAddress())
		//	Debug.Log("This machine has a public IP address");
		//else
		//	Debug.Log("This machine has a private IP address");

                           // The game can have several menus and states, so I like to use 
                           // an enum to keep track of what state we’re in.  In this case we’re
                           // in the MGS lobby
		gamemenustate = menustate.networklobby;
	}
		
	void Update() {
		// If test is undetermined, keep running
		if (!doneTesting) {
			TestConnection(); // Testing is to evaluate the IP's of the client and server to determine if we need NPT to connect
		}

		if (Time.realtimeSinceStartup > lastHostListRequest + hostListRefreshTimeout)
		{
			MasterServer.ClearHostList();			  // Clear the current local list of GS's prior to refreshing it
			MasterServer.RequestHostList (gameType);  // Get an update list of available GS's from the MGS
			lastHostListRequest = Time.realtimeSinceStartup;
			//Debug.Log("Refresh Available GS List");
		}
	}
	
	
	// Test Connection is used to check the connection to the Game Server (GS) to determine if it's behind a router/FW
	// with a different public IP and private IP.  This helps us determine if we need to try using NAT Punch Through to connect
	// Network.useNat will be set based on the tested findings.  This is used on the client to figure out how to connect
	// On the GS we check !Network.HavePublicAddress() and pass it into the MGS when we register so it know if NPT is required
	void TestConnection() {
		// Start/Poll the connection test, report the results in a label and react to the results accordingly
		natCapable = Network.TestConnection();
	
		switch (natCapable) 
		{
			case ConnectionTesterStatus.Error:
				testMessage = "Problem determining NAT capabilities";
				doneTesting = true;
				break;
			case ConnectionTesterStatus.Undetermined:
				testMessage = "Testing NAT capabilities";
				doneTesting = false;
				break;
			case ConnectionTesterStatus.PublicIPIsConnectable:
				testMessage = "Directly connectable public IP address.";
				useNat = false;
				doneTesting = true;
				break;
	
			// This case is a bit special as we now need to check if we can
			// use the blocking by using NAT punchthrough
			case ConnectionTesterStatus.PublicIPPortBlocked:
				testMessage = "Non-connectable public IP address (port " + serverPort +"	blocked),"
							+" running a server is impossible.";
				useNat = false;
				// If no NAT punchthrough test has been performed on this public IP, force a test
				if (!probingPublicIP)
				{
					//Debug.Log("Testing if firewall can be circumnvented");
					natCapable = Network.TestConnectionNAT();
					probingPublicIP = true;
					timer = Time.time + 10;
				}
				// NAT punchthrough test was performed but we still get blocked
				else if (Time.time > timer)
				{
					probingPublicIP = false; // reset
					useNat = true;
					doneTesting = true;
				}
				break;
			case ConnectionTesterStatus.PublicIPNoServerStarted:
				testMessage = "Public IP address but server not initialized,"
							+"it must be started to check server accessibility. Restart connection test when ready.";
				doneTesting = true;
				break;
			default:
				testMessage = "Error in test routine, got " + natCapable;
				if ( string.Compare("Limited",0,natCapable.ToString(),0,7) == 0 )
					useNat = true;
				doneTesting = true;
				break;
		} // end switch
		//Debug.Log(testMessage);
	}
	
	// Here we paint the menu screen so the player can choose to join a game or start a new game
	void ShowGUI() 
	{
		// in the AWAKE method we used: DontDestroyOnLoad(this);
		// so when we load our game level this ShowGUI is still running
		// so here we can see what menu we should be showing and display the appropriate menu
		
		// When we're ingame we want to show a Disconnect button to quit the game and return to the lobby
		if ( gamemenustate == menustate.ingame )
		{
				/*if (GUI.Button (new Rect(10,10,90,30),"Disconnect"))
				{
					Network.Disconnect(); // Tell all the other clients you're disconnecting
					MasterServer.UnregisterHost();
					gamemenustate = menustate.networklobby;
					// Return to the Master Game Server Lobby because we pressed disconnect
					Application.LoadLevel("MasterGameServerLobby");
				}*/
		}
		else if ( gamemenustate == menustate.networklobby ) // ensure we're in the lobby, and not off somewhere unexpected
		{
			

			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				format.fontSize = 40;
                format.normal.textColor = Color.white;
                GUI.Label(new Rect(((Screen.width / 2) - 80) * 0.2f, (Screen.height / 2) - 200, 400, 50), "Lite: Special Forces", format);
                //GUI.Label(new Rect(((Screen.width / 2) - 80) * 0.2f, (Screen.height / 2) - 200, 400, 50), "Lite: Special Forces", format);
				
				gameName = GUI.TextField(new Rect((((Screen.width/2)) * 0.2f)+200,
										 (Screen.height/2)-100,
										200,
										20),
										gameName);

                gamePort = GUI.TextField(new Rect((((Screen.width / 2)) * 0.2f) + 400,
                                         (Screen.height / 2) - 100,
                                        50,
                                        20),
                                        gamePort);
                /*
                joinip = GUI.TextField(new Rect((((Screen.width / 2)) * 0.2f) + 200,
                                         (Screen.height / 2) - 75,
                                        200,
                                        20),
                                        joinip);

                joinport = GUI.TextField(new Rect((((Screen.width / 2)) * 0.2f) + 400,
                                         (Screen.height / 2) - 75,
                                        50,
                                        20),
                                        joinport);
                 */
                if (GUI.Button(new Rect(0, 0, 150, 20),
                                        "Quit"))
                {
                    Application.Quit();
                }
				// Start a new server
				if (GUI.Button(new Rect(((Screen.width/2)-100) * 0.2f,
										 (Screen.height/2)-100,
										150,
										20),
										"Start Server"))
				{

                
					// If start server is chosen then we want to initialize ourselves as a server
					// and then register with the master server so other players will see us
					// as a choice in the list of game servers
					
					// We'll use Network.InitializeServer to enable our Game Server (GS) functionality
					// but first we need to determine if we'll require clients to connect to us directly
					// or by using NPT.  
					
					// The first two parameters to InitializeServer are how many clients can connect and the port to use
					// The port needs to be unique, but on desktop/laptop pc's it's not usually an issue as there aren't a 
					// lot of network listeners running to contend
					// Once the game starts you can consider using Network.maxConnections to stop any new players
					// from connecting

                    if (doneTesting)
                    { // If done testing use the more thourough results of the testing to setup the server
                        serverPort = int.Parse(gamePort);
                        Network.InitializeServer(15, serverPort, useNat);
                    }
                    else // otherwise setup the server and specific NPT based on public/private IP
                        // this is not as accurate as the full test, but is sufficient for most needs
                        Network.InitializeServer(15, serverPort, !Network.HavePublicAddress());
					MasterServer.updateRate = 3;  
					MasterServer.RegisterHost(gameType, gameName, "lsf_desert");
				}
                /*
                if (GUI.Button(new Rect(((Screen.width / 2) - 100) * 0.2f,
                                         (Screen.height / 2) - 75,
                                        150,
                                        20),
                                        "Direct Connect"))
                {
                    HostData[] data1 = MasterServer.PollHostList();	// Extract the list of available GS's into a local variable array for processing
                    int _cnt1 = 0;

                    // Loop through all the available GS's provided by the MGS and display each one so we can choose one to joing
                    foreach (HostData gs in data1)
                    {
                        // Do not display NAT enabled games if we cannot do NAT punchthrough
                        if (!(filterNATHosts && gs.useNat))
                        {
                            // Build a name string to use for displaying the GS in the list

                            if (gs.ip[_cnt1] == joinip)
                            {
                                // Enable NAT functionality based on GS host we're connecting to
                                useNat = gs.useNat;
                                if (useNat)
                                {
                                    print("Using Nat punchthrough to connect");
                                    Network.Connect(gs.guid);
                                }
                                else
                                {
                                    print("Connecting directly to host");
                                    Network.Connect(gs.ip, gs.port);
                                }
                            }
                        }
                        _cnt1++;
                    }
                    Network.Connect(joinip, joinport);
                }
			*/
				HostData[] data = MasterServer.PollHostList();	// Extract the list of available GS's into a local variable array for processing
				int _cnt = 0;
				
				// Loop through all the available GS's provided by the MGS and display each one so we can choose one to joing
				foreach (HostData gs in data)
				{
					// Do not display NAT enabled games if we cannot do NAT punchthrough
					if ( !(filterNATHosts && gs.useNat) )
					{
						// Build a name string to use for displaying the GS in the list
                        string name = gs.gameName + " (" + gs.ip[_cnt] +") " + gs.comment + " (" + gs.connectedPlayers + " / " + gs.playerLimit + ")";
						
						if (GUI.Button(new Rect(((Screen.width/2)-100) * 0.2f,
										 (Screen.height/2)+(40*_cnt),
										550,
										30),
										name )) 
						{
							// Enable NAT functionality based on GS host we're connecting to
							useNat = gs.useNat;
							if (useNat)
							{
								print("Using Nat punchthrough to connect");
								Network.Connect(gs.guid);
							}
							else
							{
								print("Connecting directly to host");
								Network.Connect(gs.ip, gs.port);
							}
						}
					}
                    _cnt++;
				}
			}
		}
	}
    public void dc(){
        gamemenustate = menustate.networklobby;
    }
}

