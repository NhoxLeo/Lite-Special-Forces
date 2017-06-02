using UnityEngine;
using System.Collections;

public class EnemyAntiTankFlip : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Network.isServer)
        {
            //ANTI TANK FLIPPING

            if (transform.rotation.x > 0.4f)
            {
                //transform.Rotate(-0.2f, 0, 0);
                Quaternion r = transform.rotation;
                r.x = 0.4f;
                transform.rotation = r;
            }
            else if (transform.rotation.x < -0.4f)
            {
                //transform.Rotate(0.2f, 0, 0);
                Quaternion r = transform.rotation;
                r.x = -0.4f;
                transform.rotation = r;
            }


            if (transform.rotation.z > 0.2f)
            {
                Quaternion r = transform.rotation;
                r.z = 0.2f;
                transform.rotation = r;

            }
            else if (transform.rotation.z < -0.2f)
            {
                Quaternion r = transform.rotation;
                r.z = -0.2f;
                transform.rotation = r;
            }
        }
	}
}
