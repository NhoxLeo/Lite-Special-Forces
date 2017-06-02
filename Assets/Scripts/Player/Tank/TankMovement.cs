using UnityEngine;
using System.Collections;

public class TankMovement : MonoBehaviour {

    public float speed = 10.0F;             //DRIVING SPEED
    public float rotationSpeed = 100.0F;    //TURNING SPEED
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (networkView.isMine)
        {
            //FORWARD BACKWARD
            float translation = Input.GetAxis("Vertical") * speed;  
            translation *= Time.deltaTime;
            transform.Translate(0, 0, translation);


            //TURNING
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed;   
            rotation *= Time.deltaTime;
            transform.Rotate(0, rotation, 0);

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
