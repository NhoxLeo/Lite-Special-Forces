using UnityEngine;
using System.Collections;

public class SpawnWaveController : MonoBehaviour {
    public Transform wavecontroller;

	// Use this for initialization
	void Start () {
        if (Network.isServer)
        {
            Network.Instantiate(wavecontroller, Vector3.zero, Quaternion.identity, 0);
        }
        Destroy(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
