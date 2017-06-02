using UnityEngine;
using System.Collections;

public class JetScript : MonoBehaviour {

    public GameObject aphud;
    public GameObject sphud;
    public GameObject crosshair;
    public GameObject spawnhud;

    HealthCounter hc;
    ShieldCounter sc;
    GameObject ch;
    public float chx;
    public float chy;

    public int setap, setsp;
    int ap, sp;

    public GameObject explosion;

    public float mapregen = 0.1f;
    float apregen = 0;
    public float mspregen = 0.01f;
    float spregen = 0;

	// Use this for initialization
	void Start () {
        if (networkView.isMine)
        {
            ap = setap;
            sp = setsp;
            Instantiate(aphud);
            Instantiate(sphud);
            ch = Instantiate(crosshair) as GameObject;
            ch.transform.position = new Vector3(chx, chy, 0);

            hc = GameObject.Find("HUDHealth(Clone)").GetComponent("HealthCounter") as HealthCounter;
            sc = GameObject.Find("HUDShield(Clone)").GetComponent("ShieldCounter") as ShieldCounter;

            hc.set(ap);
            sc.set(sp);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (networkView.isMine)
        {
            //Debug.Log("hp " + ap + " sp " + sp); 
            if (ap <= 0)
            {
                Die();
            }

            if (spregen <= 0)
            {
                if (sp < setsp)
                {
                    spregen = mspregen;
                    sp += 1;
                    sc.set(sp);
                }
            }
            else
            {
                if (sp != setsp)
                {
                    spregen -= Time.deltaTime;
                }
            }

            if (apregen <= 0)
            {
                if (ap < setap)
                {
                    apregen = mapregen;
                    ap += 1;
                    hc.set(ap);
                }
            }
            else
            {
                if (ap != setap)
                {
                    apregen -= Time.deltaTime;
                }
            }
        }
	}

    public void rpcGetHit(int ad, int sd, NetworkPlayer np)
    {
        if (Network.isServer)
        {
            if (networkView.isMine)
            {
                GetHit(ad, sd);
            }
            else
            {
                networkView.RPC("GetHit", np, ad, sd);
            }
        }
    }
    [RPC]
    void GetHit(int ad, int sd)
    {
        if (networkView.isMine)
        {
            //#step 1: subtract shield from sd

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

            if (sp < 0)
            {
                sp = 0;
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
            Debug.Log(ap + " " + sp);
            hc.set(ap);
            sc.set(sp);
        }
    }
    public void Die()
    {
        if (networkView.isMine)
        {
            hc.delete();
            sc.delete();
            Destroy(ch);
            Network.Instantiate(explosion, transform.position, Quaternion.identity, 0);
            Instantiate(spawnhud);
            Network.RemoveRPCs(networkView.viewID);
            Network.Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision hit)
    {

        if (networkView.isMine)
        {
            if (hit.gameObject.name == "Enemy(Clone)")
            {
                EnemyScript script = hit.gameObject.GetComponent("EnemyScript") as EnemyScript;
                script.GetHit(ap, sp);
            }

            else if (hit.gameObject.name == "Tank(Clone)")
            {
                EnemyTankScript script = hit.gameObject.GetComponent("EnemyTankScript") as EnemyTankScript;
                script.GetHit(20000, 20000);
            }

            else if (hit.gameObject.tag == "Bullet" || hit.gameObject.tag == "EnemyBullet")
            {

            }
            else
            {
                GetHit(5000, 1000);
            }
        }
    }

    void OnCollisionStay(Collision hit)
    {
        if (networkView.isMine)
        {
            if (hit.gameObject.name == "Enemy(Clone)")
            {
                EnemyScript script = hit.gameObject.GetComponent("EnemyScript") as EnemyScript;
                script.GetHit(ap, sp);
            }
            
            else if (hit.gameObject.tag == "Bullet" || hit.gameObject.tag == "EnemyBullet")
            {

            }
            else
            {
                GetHit(1000, 1000);
            }
        }
    }
}
