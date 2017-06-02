using UnityEngine;
using System.Collections;

public class EnemyBulletScript : MonoBehaviour {
    public GameObject Sparks;
	public float life = 4;
    public int sd;
    public int ad;
   
	// Use this for initialization
	void Start () {
        renderer.enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (life > 0){
			life -= Time.deltaTime;

            //check if bullet will hit anything in front of it because bullet moves too fast
            RaycastHit hit;
            if (Physics.Raycast(transform.position,transform.forward,out hit,2.4f))
            {
                if (hit.transform.name == "Player(Clone)")
                {
                    if(Network.isServer){
                        PlayerScript script = hit.transform.gameObject.GetComponent("PlayerScript") as PlayerScript;
                        script.rpcGetHit(ad, sd, hit.transform.networkView.owner);
                    }
                    destroy();
                }
                if (hit.transform.name == "Tank(Clone)")
                {
                    if (Network.isServer)
                    {
                        TankScript script = hit.transform.gameObject.GetComponent("TankScript") as TankScript;
                        script.rpcGetHit(ad, sd, hit.transform.networkView.owner);
                    }
                    destroy();
                }
                if (hit.transform.name == "Jet(Clone)")
                {
                    if (Network.isServer)
                    {
                        JetScript script = hit.transform.gameObject.GetComponent("JetScript") as JetScript;
                        script.rpcGetHit(ad, sd, hit.transform.networkView.owner);
                    }
                    destroy();
                }
                if (hit.transform.gameObject.tag == "Objective")
                {
                    if (Network.isServer)
                    {
                        ObjectiveScript script = hit.transform.parent.gameObject.GetComponent("ObjectiveScript") as ObjectiveScript;
                        script.objectiveHit(ad, sd);
                    }
                    destroy();
                }
            }
		}

		else{
            destroy();
		}
	}

	//bullet hits something
	void OnCollisionEnter(Collision hit)
    {
        if (Network.isServer)
        {
            if (hit.transform.gameObject.tag == "Objective")
            {
                ObjectiveScript script = hit.transform.parent.gameObject.GetComponent("ObjectiveScript") as ObjectiveScript;
                script.objectiveHit(ad, sd);
            }
            if (hit.transform.name == "Player(Clone)")
            {
                if (Network.isServer)
                {
                    PlayerScript script = hit.transform.gameObject.GetComponent("PlayerScript") as PlayerScript;
                    script.rpcGetHit(ad, sd, hit.transform.networkView.owner);
                }
            }
            if (hit.transform.name == "Tank(Clone)")
            {
                if (Network.isServer)
                {
                    TankScript script = hit.transform.gameObject.GetComponent("TankScript") as TankScript;
                    script.rpcGetHit(ad, sd, hit.transform.networkView.owner);
                }
            }
            if (hit.transform.name == "Jet(Clone)")
            {
                if (Network.isServer)
                {
                    JetScript script = hit.transform.gameObject.GetComponent("JetScript") as JetScript;
                    script.rpcGetHit(ad, sd, hit.transform.networkView.owner);
                }
            }
        }
        destroy();
	}

    void destroy()
    {
        Network.RemoveRPCs(networkView.viewID);
        Destroy(this.gameObject);
        Instantiate(Sparks, transform.position, Quaternion.identity);
    }
}
