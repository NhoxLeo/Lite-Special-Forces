using UnityEngine;
using System.Collections;

public class NetworkRemoveComponentScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        if (!networkView.isMine)
        {
            this.gameObject.GetComponentInChildren<Camera>().enabled = false;
            this.gameObject.GetComponentInChildren<AudioListener>().enabled = false;
            CharacterMotor cm = (CharacterMotor)this.gameObject.GetComponent("CharacterMotor");
            cm.enabled = false;
            MouseLook ml = (MouseLook)this.gameObject.GetComponent("MouseLook");
            ml.enabled = false;
            FPSInputController fps = (FPSInputController)this.gameObject.GetComponent("FPSInputController");
            fps.enabled = false;
            
        }
	
	}
	
	// Update is called once per frame
	void Update () {

        
	}
}
