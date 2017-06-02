using UnityEngine;
using System.Collections;


public class WaveInfo : MonoBehaviour
{

    public enum GameState { cooldown, play, win, lose}
    GameState state;
    float timer=0;
    int currentwave=0;
    int killedenemies=0;  //total number of enemies killed for the round
    int totalenemies=0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public int getCurrentWave()
    {
        return currentwave;
    }
    
    public void rpcsetGameState(string _s)
    {
        networkView.RPC("setGameState", RPCMode.AllBuffered, _s);
    }
    public void rpcsetWave(int _w)
    {
        networkView.RPC("setWave", RPCMode.AllBuffered, _w);
    }

    public void rpcsetEnemiesKilled(int _e)
    {
        networkView.RPC("setEnemiesKilled", RPCMode.AllBuffered, _e);
    }

    public void rpcsetTotalEnemies(int _e)
    {
        networkView.RPC("setTotalEnemies", RPCMode.AllBuffered, _e);
    }
    public void rpcsetTimer(float _t)
    {
        networkView.RPC("setTimer", RPCMode.AllBuffered, _t);
    }


    [RPC]
    void setGameState(string _s)
    {
        if (_s == "c")
        {
            state = GameState.cooldown;
        }
        if (_s == "p")
        {
            state = GameState.play;
        }
        if (_s == "w")
        {
            state = GameState.win;
        }
        if (_s == "l")
        {
            state = GameState.lose;
        }
        updateInfo();
    }
    [RPC]
    void setWave(int _w)
    {
        currentwave = _w;
        updateInfo();
    }
    [RPC]
    void setEnemiesKilled(int _e)
    {
        killedenemies = _e;
        updateInfo();
    }
    [RPC]
    void setTotalEnemies(int _e)
    {
        Debug.Log("settotalenemies:" + _e);
        totalenemies = _e;
        updateInfo();
    }
    [RPC]
    void setTimer(float _t)
    {
        timer = _t;
        updateInfo();
    }
    void updateInfo()
    {
        if (state == GameState.cooldown)
        {
            guiText.text = string.Format("Next wave in\n{0}", Mathf.Round(timer));
        }
        if (state == GameState.play)
        {
            guiText.text = string.Format("Wave: {0}\n{1}/{2}", currentwave, killedenemies, totalenemies);
        }
        if (state == GameState.win)
        {
            guiText.text = string.Format("You have successfully defended the objective");
        }
        if (state == GameState.lose)
        {
            guiText.text = string.Format("You failed the defend the objective");
        }
    }
}
