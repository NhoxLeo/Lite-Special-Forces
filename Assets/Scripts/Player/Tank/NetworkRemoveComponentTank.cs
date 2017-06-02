using UnityEngine;
using System.Collections;

public class NetworkRemoveComponentTank : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (!networkView.isMine)
        {
            this.gameObject.GetComponentInChildren<Camera>().enabled = false;
            this.gameObject.GetComponentInChildren<AudioListener>().enabled = false;
            this.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
