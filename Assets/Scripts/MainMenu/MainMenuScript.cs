using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

    public Transform menu;

	// Use this for initialization
	void Start () {
        Screen.showCursor = true;
        Screen.lockCursor = false;
        /*foreach (GameObject m in GameObject.FindGameObjectsWithTag("MainMenu"))
        {
            Destroy(m);
        }*/
        Instantiate(menu);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
