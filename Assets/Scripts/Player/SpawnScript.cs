using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {
    public GameObject infantry;
    public GameObject tank;
    public GameObject jet;

    Transform spawn;
    Transform[] spawns;

    Transform jetspawn;
    Transform[] jetspawns;

    const float infantrytimermax = 6;//5 6 7 8 9 10
    const float tanktimermax = 9;//8 10 12 14 16 18
    const float jettimermax = 13;//12 15 18 21 24 27

    float infantrytimer;
    float tanktimer;
    float jettimer;

    WaveInfo wi;

	// Use this for initialization
	void Start () {
        //set the spawn menu at centre of the screen
        transform.position = (new Vector3(0.5f,0.7f,0));

        spawn = GameObject.Find("PlayerSpawn").transform;
        spawns = spawn.gameObject.GetComponentsInChildren<Transform>(false);

        jetspawn = GameObject.Find("PlayerJetSpawn").transform;
        jetspawns = jetspawn.gameObject.GetComponentsInChildren<Transform>(false);

        wi = GameObject.Find("HUDWave(Clone)").GetComponent("WaveInfo") as WaveInfo;        
        infantrytimer = infantrytimermax + (wi.getCurrentWave()-1)*1;   //4     5,6,7,8,9
        tanktimer = tanktimermax + (wi.getCurrentWave() - 1) * 2;       //18    20,22,24,26,28
        jettimer = jettimermax + (wi.getCurrentWave() - 1) * 3;         //27    30,33,36,39,42
	}
	
	// Update is called once per frame
	void Update () {
        if (infantrytimer > 0)
        {
            infantrytimer -= Time.deltaTime;
            if (infantrytimer < 0)
            {
                infantrytimer = 0;
            }
        }
        if (tanktimer > 0)
        {
            tanktimer -= Time.deltaTime;
            if (tanktimer < 0)
            {
                tanktimer = 0;
            }
        }
        if (jettimer > 0)
        {
            jettimer -= Time.deltaTime;
            if (jettimer < 0)
            {
                jettimer = 0;
            }
        }
        guiText.text = string.Format("Select your unit\n\n1: Infantry({0}s)\n2: Tank({1}s)\n3: Jet({2}s)", Mathf.Round(infantrytimer), Mathf.Round(tanktimer), Mathf.Round(jettimer));

        if (infantrytimer <= 0 && Input.GetKeyDown("1"))
        {
            SpawnInfantry();
        }
        if (tanktimer <= 0 && Input.GetKeyDown("2"))
        {
            SpawnTank();
        }
        if (jettimer <= 0 && Input.GetKeyDown("3"))
        {
            SpawnJet();
        }
    
    
    }

    void SpawnInfantry()
    {
        Transform spawnarea;
        int spawnpoint = (int)Mathf.Round(Random.Range(1, spawns.Length));
        spawnarea = spawns[spawnpoint];
        Network.Instantiate(infantry, spawnarea.position, spawnarea.rotation, 0);
        Destroy(this.gameObject);
    }

    void SpawnTank()
    {
        
        Transform spawnarea;
        int spawnpoint = (int)Mathf.Round(Random.Range(1, spawns.Length));
        spawnarea = spawns[spawnpoint];
        Network.Instantiate(tank, spawnarea.position, spawnarea.rotation, 0);
        Destroy(this.gameObject);
         
    }

    void SpawnJet()
    {
        Transform spawnarea;
        int spawnpoint = (int)Mathf.Round(Random.Range(1, spawns.Length));
        spawnarea = jetspawns[spawnpoint];
        Network.Instantiate(jet, spawnarea.position, spawnarea.rotation, 0);
        Destroy(this.gameObject);
    }
}
