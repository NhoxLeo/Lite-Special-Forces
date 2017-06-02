using UnityEngine;
using System.Collections;

public class ObjectiveSpawnScript : MonoBehaviour {

    public Transform objective;

	// Use this for initialization
	void Start () {
        if (Network.isServer)
        {
            Network.Instantiate(objective, transform.position, transform.rotation, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
