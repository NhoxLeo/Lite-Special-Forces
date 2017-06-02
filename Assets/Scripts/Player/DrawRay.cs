using UnityEngine;
using System.Collections;

public class DrawRay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.DrawRay(transform.position, transform.forward * 1000
        Debug.DrawRay(transform.position + transform.forward * 10.0f + transform.up * 1.5f, transform.forward * 1000);
	}
}
