using UnityEngine;
using System.Collections;

public class ShieldCounter : MonoBehaviour {

    int shield = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
   
	}

    public void set(int sd)
    {
        shield = sd;
        if (shield < 0)
        {
            shield = 0;
        }
        UpdateHUD();
    }

    void UpdateHUD()
    {
        guiText.text = string.Format("S:{0}", shield);

    }

    public void delete()
    {
        Destroy(this.gameObject);
    }
}
