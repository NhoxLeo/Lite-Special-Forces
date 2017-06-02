using UnityEngine;
using System.Collections;

public class PlayerBulletScript : MonoBehaviour {
    public GameObject Sparks;
	public float life = 4;
    public int ad;
    public int sd;
    public float checkahead = 2.4f;
   
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
            if (Physics.Raycast(transform.position,transform.forward,out hit,checkahead))
            {
                if (hit.transform.gameObject.tag == "Objective")
                {
                    Debug.Log("objective hit");
                }
                if (hit.transform.gameObject.name == "Enemy(Clone)")
                {
                    EnemyScript script = hit.transform.gameObject.GetComponent("EnemyScript") as EnemyScript;
                    script.rpcGetHit(ad, sd);
                }
                if (hit.transform.gameObject.name == "EnemyTank(Clone)")
                {
                    EnemyTankScript script = hit.transform.gameObject.GetComponent("EnemyTankScript") as EnemyTankScript;
                    script.rpcGetHit(ad, sd);
                }
                if (hit.transform.gameObject.name == "EnemyBomber")
                {
                    EnemyBomberScript script = hit.transform.gameObject.GetComponent("EnemyBomberScript") as EnemyBomberScript;
                    script.rpcGetHit(ad, sd);
                }
                destroy();
            }
		}

		else{
            destroy();
		}

	}
    //bullet hits something
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
        Network.Instantiate(Sparks, transform.position, Quaternion.identity,0);
        Network.RemoveRPCs(networkView.viewID);
    }
}
