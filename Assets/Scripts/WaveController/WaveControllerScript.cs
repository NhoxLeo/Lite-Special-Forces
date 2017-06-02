using UnityEngine;
using System.Collections;

public class WaveControllerScript : MonoBehaviour {

    public float COOLDOWNTIMER = 10;    //timer for cooldown between waves
    public int TOTALWAVES = 5;    //total number of waves
    public int MAXENEMIES = 20;   //max amount of enemies allowed at the same time

    public Transform infantry, tank, bomber;
    Transform spawn;
    Transform[] spawns;

    Transform jetspawn;
    Transform[] jetspawns;

    public enum GameState { cooldown, play, win, lose }
    float timer;    //cooldown timer

    int currentwave;    
    int spawnedenemies; //total number of enemies spawned
    int killedenemies;  //total number of enemies killed for the round
    int currentenemies; //current amount of enemies
    int totalenemies;   //total number of enemies for the round

    GameState state;

    public GameObject HUDWave;
    WaveInfo wi;

    int tankwave;
    int bomberwave;

	// Use this for initialization
	void Start () {
        if (Network.isServer)
        {
            spawn = GameObject.Find("EnemySpawn").transform;
            spawns = spawn.gameObject.GetComponentsInChildren<Transform>(false);  //get the children enemyspawns from the enemyspawn transform
            jetspawn = GameObject.Find("EnemyJetSpawn").transform;
            jetspawns = jetspawn.gameObject.GetComponentsInChildren<Transform>(false);  //get the children enemyspawns from the enemyspawn transform
            state = GameState.cooldown;
            state = GameState.cooldown;
            
            timer = COOLDOWNTIMER;
            currentwave = 0;

            Network.Instantiate(HUDWave, new Vector3(1.0f, 1.0f, 0.0f), Quaternion.identity, 0);
            wi = GameObject.Find("HUDWave(Clone)").GetComponent("WaveInfo") as WaveInfo;
            setGameState("c");
        }
        tankwave = (TOTALWAVES / 3)+1;
        bomberwave = (TOTALWAVES * 2 / 3)+1;
	}
	
	// Update is called once per frame
	void Update () {
        if (Network.isServer)
        {
            //Debug.Log(Network.connections.Length);
            if (state == GameState.cooldown)
            {
                //Debug.Log(timer);
                setTimer(timer);//set everyone's timer
                timer -= Time.deltaTime;   //start countdown

                if (timer <= 0)
                {
                    
                    
                    state = GameState.play;     //set to play state
                    setGameState("p");  //set everyone's play state
                    currentwave++;  //add wave
                    setWave(currentwave);   //add wave for everyone
                    //reset
                    spawnedenemies = 0;
                    killedenemies = 0;
                    currentenemies = 0;

                    //totalenemies = 5 * currentwave + (Network.connections.Length) * 5 * currentwave; //determine how many enemies based on wave and number of players
                    totalenemies = 4 * currentwave + (Network.connections.Length) * 5 * currentwave; //determine how many enemies based on wave and number of players
                    /*
                    Debug.Log("KILLEDENEMIES:"+killedenemies);
                    Debug.Log("TOTALENEMIES:"+totalenemies);*/
                    //set killed and total enemies counters for everyone
                    setEnemiesKilled(killedenemies);
                    setTotalEnemies(totalenemies);
                    //spawn enemies
                    for (int c = 0; c < MAXENEMIES && c < totalenemies; c++)
                    {
                        SpawnEnemy();
                    }

                    //tell objective a new wave is starting
                    ObjectiveScript script = GameObject.Find("Objective(Clone)").GetComponent("ObjectiveScript") as ObjectiveScript;
                    script.newWave(currentwave);
                }
            }

            else if (state == GameState.play)
            {
                //if all killed
                if (killedenemies == totalenemies)
                {
                    //if last wave end
                    if (currentwave == TOTALWAVES)
                    {
                        state = GameState.win;
                        setGameState("w");
                    }
                        //go to next wave
                    else
                    {
                        state = GameState.cooldown;
                        timer = COOLDOWNTIMER;
                        setGameState("c");
                        setTimer(timer);
                    }
                }
                else
                {
                    //if not all killed
                    if (spawnedenemies < totalenemies)
                    {
                        //if more enemies can be spawned
                        if (currentenemies < MAXENEMIES)
                        {
                            SpawnEnemy();                                                        
                        }
                    }
                }
            }
            else if (state == GameState.win)
            {
            }
            else if (state == GameState.lose)
            {
            }
        }
	}

    void SpawnEnemy()
    {
        if (Network.isServer)
        {
            if (true)
            {
                int num = Random.Range(1, 101);// Get a random number 1-100
                //Infantry  1-60
                //Tank      61-85
                //Jet       86-100
                if (num > 85)
                {
                    SpawnBomber();
                }
                else if (num > 60)
                {
                    SpawnTank();
                }
                else if (num <= 60)
                {
                    SpawnInfantry();
                }
                
                
            }
            else if (currentwave >= tankwave){
                int num = Random.Range(1, 101);// Get a random number 1-100
                //Infantry  1-70
                //Tank      71-100
                if (num > 80)
                {
                    SpawnTank();
                }
                else if (num <= 80)
                {
                    SpawnInfantry();
                }
                
            }
            else
            {
                SpawnInfantry();
            }

            spawnedenemies++;
            currentenemies++;
        }
    }

    void SpawnInfantry()
    {
        Transform spawnarea;
        int spawnpoint = (int)Mathf.Round(Random.Range(1, spawns.Length));
        spawnarea = spawns[spawnpoint];
        GameObject go = GameObject.FindGameObjectWithTag("Objective");
        spawnarea.LookAt(new Vector3(go.transform.position.x, spawnarea.position.y, go.transform.position.z));
        Network.Instantiate(infantry, spawnarea.position + new Vector3(Mathf.Round(Random.Range(-10, 10)), 0, Mathf.Round(Random.Range(-10, 10))), spawnarea.rotation, 0);
            
    }
    void SpawnTank()
    {
        Transform spawnarea;
        int spawnpoint = (int)Mathf.Round(Random.Range(1, spawns.Length));
        spawnarea = spawns[spawnpoint];
        GameObject go = GameObject.FindGameObjectWithTag("Objective");
        spawnarea.LookAt(new Vector3(go.transform.position.x, spawnarea.position.y, go.transform.position.z));
        Network.Instantiate(tank, spawnarea.position + new Vector3(Mathf.Round(Random.Range(-10, 10)), 0, Mathf.Round(Random.Range(-10, 10))), spawnarea.rotation, 0);
          
    }

    void SpawnBomber()
    {
        Transform spawnarea;
        int spawnpoint = (int)Mathf.Round(Random.Range(1, jetspawns.Length));
        spawnarea = jetspawns[spawnpoint];
        GameObject go = GameObject.FindGameObjectWithTag("Objective");
        spawnarea.LookAt(new Vector3(go.transform.position.x, spawnarea.position.y, go.transform.position.z));
        GameObject b = Network.Instantiate(bomber, spawnarea.position + new Vector3(Mathf.Round(Random.Range(-50, 50)), Mathf.Round(Random.Range(0, 20)), Mathf.Round(Random.Range(-50, 50))), spawnarea.rotation, 0) as GameObject;
    
    }
    void setGameState(string s)
    {
        wi.rpcsetGameState(s);
    }
    void setWave(int i)
    {
        wi.rpcsetWave(i);
    }
    void setEnemiesKilled(int i)
    {
        wi.rpcsetEnemiesKilled(i);
    }
    void setTotalEnemies(int i)
    {
        wi.rpcsetTotalEnemies(i);
    }
    void setTimer(float f)
    {
        wi.rpcsetTimer(f);
    }
    
    public void EnemyKilled()
    {
        networkView.RPC("networkenemykilled", RPCMode.All);
    }
    [RPC]
    void networkenemykilled()
    {
        killedenemies++;
        currentenemies--;
        setEnemiesKilled(killedenemies);
    }
    //when objective dies, it calls this
    public void ObjectiveDied()
    {
        state = GameState.lose;
        setGameState("l");
    }
    
}
