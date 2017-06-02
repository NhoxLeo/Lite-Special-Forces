using UnityEngine;
using System.Collections;

public class PlayerMissileScript : MonoBehaviour {

    public GameObject Sparks;
    public float life = 4;
    public int ad;
    public int sd;

    GameObject[] enemies;

	// Use this for initialization
	void Start () {
        renderer.enabled = false;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
	}
	
	// Update is called once per frame
    void Update()
    {
        if (networkView.isMine)
        {

            RaycastHit closestHit;
            float closestDistance = 999999;
            bool hitsomething = false;

            foreach (GameObject e in enemies)
            {

                if (e.transform.Find("EnemyBomber"))
                {
                    Transform bomber = e.transform.Find("EnemyBomber");
                    RaycastHit hit;
                    Vector3 dir = bomber.position - transform.position;
                    Debug.DrawRay(transform.position, dir);
                    if (Physics.Raycast(transform.position, dir + new Vector3(0, 1, 0), out hit, 200f))
                    {
                        hitsomething = true;
                        //check if it's closest plane
                        if (hit.distance < closestDistance)
                        {
                            //mark player as closest
                            closestHit = hit;
                        }
                    }
                }
            }

            if (hitsomething)
            {
                Vector3 target = closestHit.transform.position + new Vector3(0, 1, 0);
                Vector3 dir = target - transform.position;

                if (Vector3.Angle(transform.forward, dir) <= 40)
                {
                    rotate(target); //turn to plane
                }
            }
            rigidbody.velocity = transform.forward * 150;
        }
    }

        void rotate(Vector3 target)
    {
        //turn
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Quaternion q = Quaternion.RotateTowards(transform.rotation, targetRotation, 10.0f);
        transform.rotation = q;
    }

        void OnCollisionEnter(Collision hit)
        {

            if (networkView.isMine)
            {
                if (hit.transform.gameObject.name == "Enemy(Clone)")
                {
                    EnemyScript script = hit.gameObject.GetComponent("EnemyScript") as EnemyScript;
                    script.rpcGetHit(ad, sd);
                }
                if (hit.transform.gameObject.name == "EnemyTank(Clone)")
                {
                    EnemyTankScript script = hit.gameObject.GetComponent("EnemyTankScript") as EnemyTankScript;
                    script.rpcGetHit(ad, sd);
                }
                if (hit.transform.gameObject.name == "EnemyBomber")
                {
                    EnemyBomberScript script = hit.gameObject.GetComponent("EnemyBomberScript") as EnemyBomberScript;
                    script.rpcGetHit(ad, sd);
                }
                destroy();
            }
        }

        void destroy()
        {
            Network.Destroy(this.gameObject);
            Network.Instantiate(Sparks, transform.position, Quaternion.identity, 0);
            Network.RemoveRPCs(networkView.viewID);
        }
}
