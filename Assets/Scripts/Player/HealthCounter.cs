using UnityEngine;
using System.Collections;

public class HealthCounter : MonoBehaviour {

    int health = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


	}

    public void set(int ad)
    {
        health = ad;
        if (health < 0)
        {
            health = 0;
        }
        UpdateHUD();
    }

    void UpdateHUD()
    {

        guiText.text = string.Format("A:{0}", health);

    }
    public void delete()
    {
        Destroy(this.gameObject);
    }
}
