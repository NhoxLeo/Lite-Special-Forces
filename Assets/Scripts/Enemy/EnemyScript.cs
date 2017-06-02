using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

    public Transform bullet;
    public float force = 10000;
    public float FIRETIME = 0.1f;
    float lastfire = 0;
	public int ap = 3000;
    public int sp = 0;
    public int ad = 100;
    public int sd = 100;
    GameObject[] players;

    bool alive =true;
	
	// Use this for initialization
	void Start () {
        renderer.material.color = Color.red;
        if (Network.isServer)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
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
            //attack objective if less than 30 units
            Vector3 objective = GameObject.Find("Objective(Clone)").transform.position;
            if ((objective - transform.position).sqrMagnitude <= 40*40)
            {
                rigidbody.velocity = Vector3.zero;
                rotate(objective);
                fire(objective+new Vector3(0,4,0));
            }
                //search for targets
            else
            {
                RaycastHit closestHit;
                float closestDistance = 999999;
                bool hitsomething = false;
                players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject p in players)
                {
                    RaycastHit hit;
                    //shoot ray at player
                    Vector3 dir = p.transform.position - transform.position;
                    if (Physics.Raycast(transform.position, dir+new Vector3(0,0.5f,0), out hit, 90f))
                    {
                        if (hit.transform.gameObject.tag == "Player")
                        {
                            hitsomething = true;
                            //check if it's closest player
                            if (hit.distance < closestDistance)
                            {   
                                //mark player as closest
                                closestHit = hit;
                            }
                        }
                    }
                }
                
                //set target
                if (hitsomething){ 
                    Vector3 target = closestHit.transform.position;
                    Vector3 dir = target - transform.position;
                    
                    if (Vector3.Angle(transform.forward, dir) <= 75)
                    {
                        rotate(target); //turn to player
                        fire(target);
                    }
                    else
                    {
                        rotate(objective); //turn to objective
                    }
                }
                else
                {
                    rotate(objective); //turn to objective
                }
                //move toward objective
                Vector3 direction = (objective - transform.position).normalized;
                Vector3 v = rigidbody.velocity;
                v.x = direction.x * 3f;
                v.z = direction.z * 3f;
                rigidbody.velocity = v;
            }
        }
	}
    //enemy gets hit

    public void rpcGetHit(int ad, int sd)
    {
        networkView.RPC("GetHit", RPCMode.AllBuffered, ad, sd);
    }
    [RPC]
	public void GetHit(int ad,int sd){
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
        Network.Destroy(this.gameObject);
        Network.RemoveRPCs(networkView.viewID);
        WaveControllerScript wcs = GameObject.Find("WaveController(Clone)").GetComponent("WaveControllerScript") as WaveControllerScript;
        wcs.EnemyKilled();
    }

    void rotate(Vector3 target)
    {
        //turn
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Quaternion q = Quaternion.RotateTowards(transform.rotation, targetRotation, 2.0f);
        q.x = 0;
        q.z = 0;
        transform.rotation = q;
    }

    void fire(Vector3 target)
    {
        
        if (lastfire <= 0)
        {
            // they have to aim at you to shoot but this code lets them shoot up and down at you too
            Vector3 dir = target - (transform.position + transform.forward * 1.5f);
            if (Vector3.Angle(transform.forward, dir) <= 15)
            {
                Transform b = Network.Instantiate(bullet, transform.position + transform.forward*1.5f+transform.up*0.2f+transform.right*0.2f, transform.rotation, 0) as Transform;
                EnemyBulletScript enemybullet = b.GetComponent("EnemyBulletScript") as EnemyBulletScript;
                enemybullet.ad = ad;
                enemybullet.sd = sd;
                dir.Normalize();
                dir.x = 0;
                dir.z = 0;
                b.rigidbody.AddForce((transform.forward * force) + (dir*force));
                lastfire = FIRETIME;
            }
        }
    }
    [RPC]
    void updatePlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    void OnPlayerConnected()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void OnPlayerDisconnected()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
}
