using UnityEngine;
using System.Collections;

public class EnemyTankScript : MonoBehaviour {

    public Transform bullet;
    public float force = 10000;
    public float FIRETIME = 3f;
    float lastfire = 0;
    public int ap = 50000;
    public int sp = 30000;
    public int ad = 15000;
    public int sd = 10000;

    GameObject[] players;

    bool alive = true;

    Transform turret;
    Transform gun;

	// Use this for initialization
	void Start () {
        if (Network.isServer)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            turret = transform.Find("TURRET");
            gun = turret.Find("GUN");
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
            if ((objective - transform.position).sqrMagnitude <= 40 * 40)
            {
                rigidbody.velocity = Vector3.zero;
                rotateTurret(objective); 
                rotate(objective);
                fire(objective + new Vector3(0, 4, 0));
            }
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
                    Vector3 dir = p.transform.position - (transform.position+transform.up * 4.9f);
                    //Debug.DrawRay(transform.position + transform.up * 4.9f, dir);
                    if (Physics.Raycast(transform.position+transform.up*4.9f, dir, out hit, 200f))
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
                if (hitsomething)
                {
                    Vector3 target = closestHit.transform.position;
                    Vector3 dir = target - transform.position;

                    if (Vector3.Angle(transform.Find("TURRET").forward, dir) <= 90)
                    {

                        rotateTurret(target); //turn turret to player
                        fire(target);
                    }
                    else
                    {
                        rotateTurret(objective); 
                    }
                }
                else
                {
                    rotateTurret(objective);
                }
                rotate(objective);
                //move toward objective
                //Vector3 direction = (objective - transform.position).normalized;
                Vector3 v = rigidbody.velocity;
                Vector3 t = transform.forward*4; //5
                v.x = t.x;
                v.z = t.z;
                rigidbody.velocity = v ;
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
        Network.Destroy(this.gameObject);
        Network.RemoveRPCs(networkView.viewID);
        WaveControllerScript wcs = GameObject.Find("WaveController(Clone)").GetComponent("WaveControllerScript") as WaveControllerScript;
        wcs.EnemyKilled();
    }

    void rotate(Vector3 target)
    {
        
        //turn
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Quaternion q = Quaternion.RotateTowards(transform.rotation,targetRotation,1.0f);
        //q.x = 0;
        //q.z = 0;
        transform.rotation = q;
        
    }

    void rotateTurret(Vector3 target)
    {
        //turn
        /*
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRotation, 1.1f);  //you have to do this
        Quaternion q = turret.localRotation;
        q.x = 0;
        q.z = 0;
        turret.localRotation = q;
         * */
        Quaternion targetRotation = Quaternion.LookRotation(target - turret.position);
        targetRotation.x = 0;
        targetRotation.z = 0;
        turret.localRotation = Quaternion.RotateTowards(turret.localRotation, targetRotation, 1.1f);
        Debug.Log(turret.localEulerAngles);
        /*
        Vector3 fixAngles = turret.localEulerAngles;
        fixAngles.y += 90;
        turret.localEulerAngles = fixAngles;
         */
        /*
        Quaternion targetRotation2 = Quaternion.LookRotation(target - (gun.position));
        Debug.DrawLine(gun.position, target);
        gun.rotation = Quaternion.RotateTowards(gun.rotation, targetRotation2, 1.1f);   //you have to do this
        
         */
        Quaternion targetRotation2 = Quaternion.LookRotation(target -gun.position,Vector3.up);
        targetRotation2.y = 0;
        targetRotation2.z = 0;
        gun.localRotation = Quaternion.RotateTowards(gun.localRotation, targetRotation2, 1.1f);

        Quaternion q2 = gun.localRotation;
        q2.y = 0;
        q2.z = 0;

        if (q2.x < -0.2f)
        {
            q2.x = -0.2f;
        }
        else if (q2.x > 0.1f)
        {
            q2.x = 0.1f;
        }
        gun.localRotation = q2;
    }

    void fire(Vector3 target)
    {
        
        if (lastfire <= 0)
        {
            // they have to aim at you to shoot but this code lets them shoot up and down at you too
            Vector3 dir = target - (gun.position + gun.forward * 3f);
            if (Vector3.Angle(gun.forward, dir) <= 10)
            {
                Transform b = Network.Instantiate(bullet, gun.position + transform.forward * 3f, transform.rotation, 0) as Transform;
                EnemyBulletScript enemybullet = b.GetComponent("EnemyBulletScript") as EnemyBulletScript;
                enemybullet.ad = ad;
                enemybullet.sd = sd;
                dir.Normalize();
                b.rigidbody.AddForce(gun.forward * force);
                lastfire = FIRETIME;
            }
        }
    }

    void OnCollisionEnter(Collision hit)
    {
        if (Network.isServer)
        {
            if (hit.transform.name == "Player(Clone)")
            {
                PlayerScript script = hit.transform.gameObject.GetComponent("PlayerScript") as PlayerScript;
                script.rpcGetHit(3000, 3000, hit.transform.networkView.owner);
            }
            if (hit.transform.name == "EnemyTank(Clone)")
            {
                Vector3 dir = hit.transform.position - transform.position;
                dir.Normalize();
                transform.position -= dir;
                hit.transform.position += dir;
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
