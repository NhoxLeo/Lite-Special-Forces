using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	
	public Transform bullet;
    public Transform bullet2;
    public Transform bullet3;
    public int ad1 = 300;
    public int sd1 = 350;
    public int ad2 = 18000;
    public int sd2 = 20000;
    public int ad3 = 2000;
    public int sd3 = 3000;
	public float force = 10000;
    public float force2 = 5000;
    public float FIRETIME1 = 0.1f;
    public float FIRETIME2 = 5f;
    public float FIRETIME3 = 6f;
	float lastfire1 = 0;
    float lastfire2 = 0;
    float lastfire3 = 0;
    public float checkahead = 2.5f;
    public Transform spark;

    int weapon = 1;

	// Use this for initialization
	void Start () {
        networkView.RPC("HideWeapon", RPCMode.AllBuffered, "rl");
	}
	
	// Update is called once per frame
	void Update () {
        if (networkView.isMine)
        {
            if (Input.GetKeyDown("1"))
            {
                weapon = 1;
                //networkView.RPC("HideWeapon", RPCMode.AllBuffered, ad, sd);
                networkView.RPC("ShowWeapon", RPCMode.AllBuffered, "xm8");
                networkView.RPC("HideWeapon", RPCMode.AllBuffered, "rl");
            }
            if (Input.GetKeyDown("2"))
            {
                weapon = 2;
                networkView.RPC("HideWeapon", RPCMode.AllBuffered, "xm8");
                networkView.RPC("ShowWeapon", RPCMode.AllBuffered, "rl");
            }
            if (Input.GetKeyDown("3"))
            {
                weapon = 3;
                networkView.RPC("HideWeapon", RPCMode.AllBuffered, "xm8");
                networkView.RPC("ShowWeapon", RPCMode.AllBuffered, "rl");
            }
            if (Input.GetMouseButton(0))
            {
                if (weapon == 1 && lastfire1 <= 0)
                {
                    Transform b = Network.Instantiate(bullet, transform.position+transform.forward*0.9f, transform.rotation, 0) as Transform;
                    PlayerBulletScript bs = b.GetComponent("PlayerBulletScript") as PlayerBulletScript;
                    bs.ad = ad1;
                    bs.sd = sd1;
                    bs.checkahead = checkahead;
                    b.rigidbody.AddForce(transform.forward * force);
                    lastfire1 = FIRETIME1;
                    Transform s = Network.Instantiate(spark, transform.position+transform.forward*1.2f+transform.right*0.1f-transform.up*0.1f, Quaternion.identity, 0) as Transform;
                }
                if (weapon == 2 && lastfire2 <= 0)
                {
                    Transform b = Network.Instantiate(bullet2, transform.position + transform.forward * 0.9f, transform.rotation, 0) as Transform;
                    PlayerBulletScript bs = b.GetComponent("PlayerBulletScript") as PlayerBulletScript;
                    bs.ad = ad2;
                    bs.sd = sd2;
                    bs.checkahead = checkahead;
                    b.rigidbody.AddForce(transform.forward * force2);
                    lastfire2 = FIRETIME2;
                    Transform s = Network.Instantiate(spark, transform.position + transform.forward * 1.2f + transform.right * 0.1f - transform.up * 0.1f, Quaternion.identity, 0) as Transform;
                }
                if (weapon == 3 && lastfire3 <= 0)
                {
                    Transform b = Network.Instantiate(bullet3, transform.position + transform.forward * 0.9f, transform.rotation, 0) as Transform;
                    PlayerMissileScript bs = b.GetComponent("PlayerMissileScript") as PlayerMissileScript;
                    bs.ad = ad3;
                    bs.sd = sd3;
                    lastfire3 = FIRETIME3;
                    Transform s = Network.Instantiate(spark, transform.position + transform.forward * 1.2f + transform.right * 0.1f - transform.up * 0.1f, Quaternion.identity, 0) as Transform;
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
            if (lastfire3 > 0)
            {
                lastfire3 -= Time.deltaTime;
            }
        }
        
	}

    [RPC]
    void HideWeapon(string s)
    {
        Renderer[] rs = transform.Find(s).GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
            r.enabled = false;
    }
    [RPC]
    void ShowWeapon(string s)
    {
        Renderer[] rs = transform.Find(s).GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
            r.enabled = enabled;
    }
}
