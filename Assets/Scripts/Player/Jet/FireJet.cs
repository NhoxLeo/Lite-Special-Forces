using UnityEngine;
using System.Collections;

public class FireJet : MonoBehaviour {

    public Transform bullet;
    public Transform bullet2;
    public Transform bullet3;
    public float force = 20000;
    public float force2 = 6000;
    public int ad = 1000;
    public int sd = 1500;
    public int ad2 = 8000;
    public int sd2 = 20000;
    public int ad3 = 3000;
    public int sd3 = 5000;
    public float FIRETIME = 0.1f;
    public float FIRETIME2 = 0.5f;
    public float FIRETIME3 = 3f;
    float lastfire = 0;
    float lastfire2 = 0;
    float lastfire3 = 0;
    public float checkahead = 3.5f;
    public Transform spark;

    int weapon = 1;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (networkView.isMine)
        {
            if (Input.GetKeyDown("1"))
            {
                weapon = 1;
            }
            if (Input.GetKeyDown("2"))
            {
                weapon = 2;
            }
            if (Input.GetKeyDown("3"))
            {
                weapon = 3;
            }
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            {
                if (weapon == 1 && lastfire <= 0)
                {
                    Transform b = Network.Instantiate(bullet, transform.position + transform.forward * 10.0f + transform.up * 1.5f, transform.rotation, 0) as Transform;
                    PlayerBulletScript bs = b.GetComponent("PlayerBulletScript") as PlayerBulletScript;
                    bs.ad = ad;
                    bs.sd = sd;
                    bs.checkahead = checkahead;
                    b.rigidbody.AddForce(transform.forward * force);
                    lastfire = FIRETIME;
                }
                if (weapon == 2 && lastfire2 <= 0)
                {
                    Transform b = Network.Instantiate(bullet2, transform.position + transform.forward * 15.0f + transform.up * 1.5f, transform.rotation, 0) as Transform;
                    PlayerBulletScript bs = b.GetComponent("PlayerBulletScript") as PlayerBulletScript;
                    bs.ad = ad2;
                    bs.sd = sd2;
                    bs.checkahead = checkahead;
                    b.rigidbody.AddForce(transform.forward * force);
                    lastfire2 = FIRETIME2;
                }
                if (weapon == 3 && lastfire3 <= 0)
                {
                    Transform b = Network.Instantiate(bullet3, transform.position + transform.forward * 15.0f + transform.up * 1.5f, transform.rotation, 0) as Transform;
                    PlayerMissileScript bs = b.GetComponent("PlayerMissileScript") as PlayerMissileScript;
                    bs.ad = ad3;
                    bs.sd = sd3;
                    lastfire3 = FIRETIME3;
                }
            }
            if (lastfire > 0)
            {
                lastfire -= Time.deltaTime;
            }
            if (lastfire2 > 0)
            {
                lastfire2 -= Time.deltaTime;
            }
            if (lastfire3 > 0)
            {
                lastfire3 -= Time.deltaTime;
            }
        }
	}

}
