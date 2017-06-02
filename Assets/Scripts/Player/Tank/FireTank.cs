using UnityEngine;
using System.Collections;

public class FireTank : MonoBehaviour {

    public Transform bullet1;
    public Transform bullet2;
    public int ad = 10000;
    public int sd = 15000;
    public int ad2 = 150;
    public int sd2 = 200;
    public float force = 15000;
    public float FIRETIME1 = 3.0f;
    public float FIRETIME2 = 0.12f;
    float lastfire1 = 0;
    float lastfire2 = 0;
    public float checkahead = 3.0f;
    public Transform spark1;
    public Transform spark2;

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
            if (Input.GetMouseButton(0))
            {
                if (weapon == 1 && lastfire1 <= 0)
                {

                    Transform b = Network.Instantiate(bullet1, transform.position + transform.forward * 2.3f + transform.up * -0.1f, transform.rotation, 0) as Transform;
                    PlayerBulletScript bs = b.GetComponent("PlayerBulletScript") as PlayerBulletScript;
                    bs.ad = ad;
                    bs.sd = sd;
                    bs.checkahead = checkahead;
                    b.rigidbody.AddForce(transform.forward * force);
                    lastfire1 = FIRETIME1;
                    Transform s = Network.Instantiate(spark1, transform.position + transform.forward * 5.0f + transform.right * 1.0f + transform.up * -0.1f, Quaternion.identity, 0) as Transform;
                }
                if (weapon == 2 && lastfire2 <= 0)
                {

                    Transform b = Network.Instantiate(bullet2, transform.position + transform.forward * 2.3f + transform.up * -0.1f, transform.rotation, 0) as Transform;
                    PlayerBulletScript bs = b.GetComponent("PlayerBulletScript") as PlayerBulletScript;
                    bs.ad = ad2;
                    bs.sd = sd2;
                    bs.checkahead = checkahead;
                    b.rigidbody.AddForce(transform.forward * force);
                    lastfire2 = FIRETIME2;
                    Transform s = Network.Instantiate(spark2, transform.position + transform.forward * 5.0f + transform.right * 1.0f + transform.up * -0.1f, Quaternion.identity, 0) as Transform;
                }
            }
            if (lastfire1 > 0)
            {
                lastfire1 -= Time.deltaTime;
            }
            if (lastfire2 > 0)
            {
                lastfire2 -= Time.deltaTime;
            }
        }
	}
}
