using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour {

    bool zoomed = false;
    public int zoom;

	// Use this for initialization
	void Start () {
        Debug.Log(Camera.main.fieldOfView);
        
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(transform.position, transform.forward * 1000f, Color.white,0,true);
        if (Input.GetMouseButtonDown(1))
        {
            if (zoomed)
            {
                zoomed = false;
                Camera.main.fieldOfView = 60; //default zoom 60
            }
            else
            {
                zoomed = true;
                Camera.main.fieldOfView = zoom;
            }
        }
	}
}
