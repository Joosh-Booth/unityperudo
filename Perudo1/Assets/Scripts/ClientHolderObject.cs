using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;



public class ClientHolderObject : MonoBehaviour
{

    public bool started = false;
    public ClientObject client;
    public List<PlayerObject> players;
    public int totalPlayers=0;
    public bool shaken = false;
    public int id;
    public int[] guess = { -1, -1 };
    public int[] previousGuess = {-1,-1};
    public int wrongId;

    // Start is called before the first frame update
    void Start()
    {
        client = new ClientObject();
        players = new List<PlayerObject>();
        //players = new PlayerObject[6];
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
