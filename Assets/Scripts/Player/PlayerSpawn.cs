using UnityEngine;
//using System.Collections;

public class PlayerSpawn : MonoBehaviour {

	void OnNetworkInstantiate (NetworkMessageInfo msg) 
	{
		// I like to setup a local variable to our local gameObject so the code is a little more readable
		GameObject localplayer = this.gameObject;
		networkView.group = 1; // send all RPC messages for group 1, so only the player instantiate is on group 0

		
		if (networkView.isMine)
		{
			// On the original player since we control the player with the keyboard controls
			// so we don't want to use the NetworkRigidbody script which was specific for the prediction and smoothing
			// of remote networked character avatars.  Therefore, let's find that component on the new player and
			// disable it
			//NetworkRigidbody _NetworkRigidbody = (NetworkRigidbody) localplayer.GetComponent("NetworkRigidbody");	
			//_NetworkRigidbody.enabled = false;
			
			AudioListener al = (AudioListener)localplayer.GetComponentInChildren(typeof(AudioListener));
			al.enabled = true;

		}
		else
		{
			name += "Remote";
			
			// Since this player object is a remote avatar, we wont be performing any manual controls to this avatar.  
			// Instead we want all updates to come from network updates.  The NetworkView will send those updates 
			// automatically.  Therefore we want to enable "NetworkRigidbody" component which will process all
			// the prediction and smoothing for our avatar
			NetworkRigidbody _NetworkRigidbody = (NetworkRigidbody) localplayer.GetComponent("NetworkRigidbody");
			_NetworkRigidbody.enabled = true;
			
		}
		//*/
	}
}
