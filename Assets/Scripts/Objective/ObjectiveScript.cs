using UnityEngine;
using System.Collections;

public class ObjectiveScript : MonoBehaviour {

    public int ap = 50000;
    public int sp = 50000;
    public Transform explosion;

    bool alive = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (ap < 0 && alive)
        {
            die();
        }
	}

    public void objectiveHit(int ad, int sd)
    {
        if (Network.isServer)
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
        }
        Debug.Log(ap + " " + sp);
    }

    void die()
    {
        alive = false;

        //tell wave controller the objective died
        WaveControllerScript wcs = GameObject.Find("WaveController(Clone)").GetComponent("WaveControllerScript") as WaveControllerScript;
        wcs.ObjectiveDied();


        Network.Instantiate(explosion, transform.position, Quaternion.identity, 0);
        Network.Destroy(this.gameObject);
        Network.RemoveRPCs(networkView.viewID); 
    }

    //increase objective health when new wave starts
    public void newWave(int wave)
    {
        ap += 75000 * (wave - 1);
        sp += 75000 * (wave - 1);

        Debug.Log("Objective: " + ap + " " + sp);
    }
}
