using UnityEngine;
//using System.Collections;

public class InstantiatePlayer : MonoBehaviour {

	public Transform PlayerAvatar;	
	
	/****************************************************************
	 * 
	 * 		All Code Below Here is only executed on the SERVER
	 * 
	 * 
	 *****************************************************************/
	
	// When a new player connects the GS will receive this callback. At that time we'll
	// instantiate the player on all clients, with it facing the block in the center of the level
	// at a randomly select spawn point
	void InstantiatePlayerOnNetworkLoadedLevel () 
	{
		
		Debug.Log("Instantiate a new player");

        Instantiate(PlayerAvatar, Vector3.zero, Quaternion.identity);
	}

	// Called on the GS when a remote client connects
	// Technically we could probably spawn the player here, or spawn the balls above, 
	// but I'm breaking it out to show various techniques 
	void OnPlayerConnected (NetworkPlayer player) 
	{

	}
	
	// Called on the GS when a remote client disconnects
	void OnPlayerDisconnected (NetworkPlayer player) 
	{

		// Removing player if Network is disconnected
		Debug.Log("Server destroying player");
		
		// When a player disconnects we need to cleanup the player from all other clients.  But before we do that we need to make
		// sure we reset the state of everything back correctly.
		// If the Player was holding a ball, then we want to player to drop the ball before cleaning up  To do that we need to first
		// associate the NetworkPlayer to the ingame object that represents that player.  To do so we'll loop through all Player objects and match 
		// the player passed in to the GameObject.networkView.owner
		//
		// Loop through all GameObjects of type Player
		foreach(GameObject _player in GameObject.FindGameObjectsWithTag("Player"))
		{
			// Match the player to the NetworkPlayer and if we have a match then this is the player we're cleaning up
			if ( _player.networkView.owner == player )
			{
				Debug.Log ("Found player");
				break; // Done cleaning up, so get out
			}
		}
		
		// remove any pending Buffered RPC's left from the disconnected player for group 0 
		Network.RemoveRPCs( player, 0); 
		
		// Cleanup just the player and all the objects the player might have instantiated
		Network.DestroyPlayerObjects(player); // send a request to all the remaining clients to have them remove the disconnected player
		
		// Cleanup just the player but not all the objects the player might have instantiated
		//Network.Destroy( _playerGO  ); // send a request to all the remaining clients to have them remove the disconnected player
	}
}