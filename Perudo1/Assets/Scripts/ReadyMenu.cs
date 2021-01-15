using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Client;
using UnityEngine.UI;
using System.Text;
using System;
using UnityEngine.Events;


public class ReadyMenu : MonoBehaviour
{
    ClientHolderObject clientScript;
    GameObject clientGameObject;
    
    public GameObject backgroundPanel;
    public Sprite tick;
    public Sprite cross;

    Image imgCheck;
    bool ready = false;

    bool readyCheck=false;
    void Start()
    {


        //Find GameObject holding script, then retrieve the script component to retrieve variables
        clientGameObject = GameObject.FindGameObjectWithTag("Client");
        clientScript = clientGameObject.GetComponent<ClientHolderObject>();
    

        //Assign function to event delegate method      
        clientScript.client.MessageRecevied += Client_MessagedReceived;
       
        //Connect to server
        clientScript.client.Connect();

        //Send players name to server
        clientScript.client.SendMessage(PlayerPrefs.GetString("name")+" "+ PlayerPrefs.GetInt("cup")+" "+ PlayerPrefs.GetInt("dice"));


    }

    private void Client_MessagedReceived(String e)
    {
        //Check e type
        if (e.StartsWith("READY"))
        {
                    
            //Remove e type indicator
            e = e.Substring(5);

            //Get int value of an ASCII chracter by minusing 0
            int pId = e[0] - '0';
            int readyValue = e[1] - '0';
            
            //Check if ready value == 1 (true)
            if (readyValue == 1)
            {

                //Set the players ready state who ID matches the vlaue sent
                clientScript.players[pId].ReadyState = true;
            }
            else
            {
                clientScript.players[pId].ReadyState = false;
            }
            readyCheck = true;

            return;
        }

        if (e.StartsWith("ID"))
        {

            e = e.Substring(2);
            clientScript.id = int.Parse(e);
            
        }

        if (e.StartsWith("PLAYERS"))
        {
            
            Debug.Log(e);
            e = e.Substring(7);
            
            //Seperate names that are broken up by a space
            char[] sep = { ' ' };
            string[] names = e.Split(sep,StringSplitOptions.RemoveEmptyEntries);

            sep[0] =  '-' ;

            string[] isReady = names[names.Length-1].Split(sep,StringSplitOptions.RemoveEmptyEntries);

            //clientScript.players = new PlayerObject[names.Length-1];

            //clientScript.totalPlayers += names.Length-1;
            //Loop through the names
            for (int i = 0; i < names.Length-1; i++) 
            {
                string values = isReady[i];
                //Make a new playerObject if entry in array is null

                clientScript.players.Add(new PlayerObject());
                clientScript.players[clientScript.totalPlayers].Id = clientScript.totalPlayers;


                //Set the name of the player to the corresponding one in the array
                clientScript.players[clientScript.totalPlayers].Name = names[i];
                clientScript.players[clientScript.totalPlayers].CupColour = values[1]-'0';
                clientScript.players[clientScript.totalPlayers].DiceColour = values[2]-'0';
                

                if ((values[0]-'0') == 1) 
                {
                    clientScript.players[clientScript.totalPlayers].ReadyState = true;
                }
                clientScript.totalPlayers++;

            }
            clientScript.players[clientScript.id].Id = clientScript.id;
            readyCheck = true;
            return;
        }

        if (e.StartsWith("STARTED")) 
        {
            clientScript.started = true;
        }

    }

    //Send whether the player is ready or not to the server
    public void OnReadyClick() 
    {
        if (!ready)
        {
            clientScript.client.SendMessage("READY");
            ready = true;
            
        }
        else 
        {
            clientScript.client.SendMessage("UNREADY");
            ready = false;

        }
        readyCheck = true;

    }

    void UpdateReadyCheck() 
    {

        if (clientScript.totalPlayers > 0)
        {
            //Go through list of playerObjects
            for (int i = 0; i < clientScript.totalPlayers; i++)
            {
                //If there is a player there
                
                //Find the playerPanel corresponding to their ID and activate it
                GameObject panel = backgroundPanel.transform.Find("PlayerPanel" + (i + 1)).gameObject;
                panel.SetActive(true);

                //Find text object on the panel and change it to the name corresponding to the playerObjects name value
                GameObject.Find("PlayerPanel" + (i + 1) + "/Name").GetComponent<Text>().text = clientScript.players[i].Name;
                

                //Find image object of the panel and set it based on the ready value
                imgCheck = GameObject.Find("PlayerPanel" + (i + 1) + "/ReadyImg").GetComponent<Image>();
                if (clientScript.players[i].ReadyState) 
                {
                    imgCheck.sprite = tick;
                }
                else
                {
                    imgCheck.sprite = cross;
                }
            }
            readyCheck = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateReadyCheck();

        if (clientScript.started)
        {
            clientScript.client.MessageRecevied -= Client_MessagedReceived;
            SceneManager.LoadScene("GameScene");
        }

    }

}
