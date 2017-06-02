using UnityEngine;
using System.Collections;

public class TimedRemoveScript : MonoBehaviour {

    float time = 5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Network.RemoveRPCs(networkView.viewID);
            Destroy(this.gameObject);
        }
	}
}
