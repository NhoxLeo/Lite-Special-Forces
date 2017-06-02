using UnityEngine;
using System.Collections;

public class EnemyBomberScript : MonoBehaviour {

    public Transform bullet;
    public float force = 10000;
    public float FIRETIME = 0.1f;
    float lastfire = 0;
    public int ap = 3000;
    public int sp = 0;
    public int ad = 100;
    public int sd = 100;
    public Transform explosion;

    bool alive = true;
    int firedistance = 100;

	// Use this for initialization
	void Start () {
        if (!networkView.isMine)
        {
            this.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            lastfire = FIRETIME;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Network.isServer)
        {
            if (lastfire > 0)
            {
                lastfire -= Time.deltaTime;
            }
            Vector3 objective = GameObject.FindGameObjectWithTag("Objective").transform.position;
            //Debug.Log((objective-transform.position).magnitude);
            if ((objective - transform.position).sqrMagnitude <= firedistance * firedistance)
            {
                fire(objective);
            }
        }
	}

    //enemy gets hit

    public void rpcGetHit(int ad, int sd)
    {
        networkView.RPC("GetHit", RPCMode.AllBuffered, ad, sd);
    }
    [RPC]
    public void GetHit(int ad, int sd)
    {
        if (alive)
        {
            if (sp > 0 && sd > 0)
            {
                int tsp = sp; //temp sp
                sp -= sd;    //shield take hit from sd
                sd -= tsp;   //subtract the sd
            }

            //#step 2:  subtract shield from ad

            if (sp > 0 && ad > 0)
            {
                int tsp = sp;   //temp sp
                ad /= 10;    //shield takes 10% from ad
                sp -= ad;     //shield take hit from ad
                ad -= tsp;   //subtract the ad
                ad *= 10;    //turn ad back to normal
            }

            //#step 3: subtract armor from sd

            if (ap > 0 && sd > 0)
            {
                sd /= 20;    //armor takes 5% from sd
                ap -= sd;     //armor takes hit from sd
            }

            //#step 4: subtract armor from ad
            if (ap > 0 && ad > 0)
            {
                ap -= ad;
            }
            Debug.Log("enemy hp = " + ap + " " + sp);
            if (ap <= 0)
            {

                die();
            }
        }
    }
    //enemy dies and reports back
    void die()
    {
        alive = false;
        WaveControllerScript wcs = GameObject.Find("WaveController(Clone)").GetComponent("WaveControllerScript") as WaveControllerScript;
        wcs.EnemyKilled();
        Network.Instantiate(explosion, transform.position, Quaternion.identity, 0);
        Network.RemoveRPCs(networkView.viewID);
        Network.RemoveRPCs(transform.parent.transform.networkView.viewID);
        Network.Destroy(transform.parent.gameObject);
        
    }

    void fire(Vector3 target)
    {
        
        if (lastfire <= 0)
        {
            Vector3 dir = target - (transform.position);
            Transform b = Network.Instantiate(bullet, transform.position, transform.rotation, 0) as Transform;
            EnemyBulletScript enemybullet = b.GetComponent("EnemyBulletScript") as EnemyBulletScript;
            enemybullet.ad = ad;
            enemybullet.sd = sd;
            dir.Normalize();
            b.rigidbody.AddForce(dir*force);
            lastfire = FIRETIME;
        }
    }

    void OnCollisionEnter(Collision hit)
    {

        if (networkView.isMine)
        {
            if (hit.gameObject.tag == "Objective")
            {
                //Do damage to objective
                die();
            }
            else if (hit.gameObject.tag == "Bullet" || hit.gameObject.tag == "EnemyBullet")
            {

            }
            else
            {
                GetHit(500, 100);
            }
        }
    }

    void OnCollisionStay(Collision hit)
    {
        if (networkView.isMine)
        {
            if (hit.gameObject.tag == "Objective")
            {
                //Do damage to objective
                die();
            }
            if (hit.gameObject.tag == "Bullet" || hit.gameObject.tag == "EnemyBullet")
            {

            }
            else
            {
                GetHit(100, 100);
            }
        }
    }
}
