using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using System;
using cakeslice;
using UnityEngine.UI;

public class GameSceneScript : MonoBehaviour
{
    public  Material[] mats;
    public GameObject guessPanel;
    Sprite[] crossSheet;
    Sprite[] diceSheet;
    Sprite[] shakeSheet;

    ClientHolderObject client;

    public GameObject shakeIndicator;
    public GameObject[] cups;
    float[,] dicePositions = new float[,]{
        {0,.1f,0,180,0,0},//1
        {0,.05f,-.05f,90,0,0},//2
        {.05f,.05f,0,0,0,90},//3
        {-.05f,.05f,0,0,0,-90},//4
        {0,.05f,.05f,-90,0,0},//5
        {0,0,0,0,0,0 }//6
        };


    public bool newRound=false;
    int turn;

    bool showDice = false;
    bool setDice = false;
    bool newGuess;
    bool newTurn;

    float shakeTimer;
    float interval = 0.5f;
    int shakeImg=0;

    // Start is called before the first frame update
    void Start()
    {
        crossSheet = Resources.LoadAll<Sprite>("crossDice");
        diceSheet = Resources.LoadAll<Sprite>("DiceSpriteTable");
        shakeSheet = Resources.LoadAll<Sprite>("Shake");
        float shakeTimer = Time.time;

        

        client = GameObject.FindGameObjectWithTag("Client").GetComponent<ClientHolderObject>();
        client.client.MessageRecevied += Client_MessagedReceived;
        client.client.SendMessage("STARTED");
        int count = 0;

        
        for (int i = 0; i < client.totalPlayers; i++)
        {
            if (client.players[i] != null)
            {
                Debug.Log("Playeer not null id: " + client.players[i].Id);
                if (i!=client.id)
                {

                    client.players[i].Cup = "cup" + (count+1);
                   
                    cups[count+1].SetActive(true);
                    GameObject.Find("cup"+(count+1)+"/Cup").GetComponent<Renderer>().material = mats[client.players[i].CupColour];
                    for (int j = 1; j <= 5; j++) 
                    {
                        GameObject.Find("cup" + (count + 1) + "/dice/Dice"+j).GetComponent<Renderer>().material = mats[client.players[i].DiceColour];
                    }
                    

                    count++;
                    Debug.Log("Count is: " + count);
                }
                else
                {
                    client.players[i].Cup = "Player";
                    GameObject.Find("Player/Cup/Circle.001").GetComponent<Renderer>().material = mats[client.players[i].CupColour];
                    GameObject.Find("Player/Cup/Circle").GetComponent<Renderer>().material = mats[client.players[i].CupColour];
                    for (int j = 1; j <= 5; j++)
                    {
                        GameObject.Find("Player/dice/Dice" + j).GetComponent<Renderer>().material = mats[client.players[i].DiceColour];
                    }
                }
            }
        }
        turn = 0;
        

    }
    private void Client_MessagedReceived(String e) 
    {
        Debug.Log("in Game Event: " + e);
       
        if (e.StartsWith("DICE"))
        {
            char temp = e[4];
            int id = temp - '0';

            e = e.Substring(5);
            
            char[] sep = { ' ' };
            string[] split = e.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            int[] dice = Array.ConvertAll(split, s => int.Parse(s));

            if (client.players[id].PreviousDice != null) 
            {
                client.players[id].PreviousDice.Clear();
                client.players[id].PreviousDice.AddRange(client.players[id].Dice);
                int count = client.players[id].PreviousDice.Count;
                if (count> client.players[id].DiceLeft) 
                {
                    client.players[id].PreviousDice.RemoveAt(count - 1);
                }
            }

            if (client.players[id].Dice == null) 
            {
                client.players[id].Dice = new List<int>(dice);
                client.players[id].PreviousDice = new List<int>(dice);
            }
            else 
            {
                client.players[id].Dice.Clear();
                client.players[id].Dice.AddRange(dice);
            }
            

            

                client.players[id].NewDice = true;
                setDice = true;
            
        }
        
        else if (e.StartsWith("CURRENTTURN")) 
        {
            turn = int.Parse(e.Substring(11));
            newTurn = true;
        }

        else if (e.StartsWith("GUESS")) 
        {

            e = e.Substring(5);
            char[] sep = { ' ' };
            string[] guess = e.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            client.previousGuess[0] = int.Parse(guess[0]);
            client.previousGuess[1] = int.Parse(guess[1]); 
            newGuess = true;
        }

        else if (e.StartsWith("WRONG")) 
        {
            
            e = e.Substring(5);
            char[] sep = { ' ' };
            string[] idDice = e.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            int id = int.Parse(idDice[0]);

            client.wrongId = id;

            if (client.players[id].DiceLeft < 1 && id!=client.id)
            {
                GameObject cup = GameObject.Find(client.players[id].Cup);
                cup.SetActive(false);
            }
        }
        else if (e.StartsWith("CORRECT"))
        {

            e = e.Substring(7);
            char[] sep = { ' ' };
            string[] idDice = e.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            int id = int.Parse(idDice[0]);
            client.wrongId = id;

            if (client.players[id].DiceLeft < 1 && id != client.id) 
            {
                GameObject cup = GameObject.Find(client.players[id].Cup);
                cup.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        ShowDiceScript diceScript = GameObject.Find("Main Camera").GetComponent<ShowDiceScript>();
        if(!diceScript.showDice)
        {
            if (setDice)
            {
                //SET DICE IN CUP
                SetDice();
                setDice = false;
            }
            if (newGuess)
            {
                setGuess();
                newGuess = false;
            }
            if (newTurn)
            {
                //Turn highlight and name
                SetTurn();
                newTurn = false;
            }
            if (showDice)
            {

                diceScript.HideCups();
                diceScript.ShowDice();
                showDice = false;
            }
            if (!client.shaken)
            {
                shakeIndicator.SetActive(true);

                if (Time.time >= shakeTimer)
                {
                    shakeIndicator.GetComponent<Image>().sprite = shakeSheet[shakeImg];
                    shakeTimer  += interval;
                    shakeImg = (++shakeImg) % 2;
                    Debug.Log("ShakeImg is: " + shakeImg);
                }
            }
            else 
            {
                shakeIndicator.SetActive(false);
            }
        }
    }

    


   

    public void SetTurn() 
    {
        Debug.Log("Turn: " + turn);

        if (turn != client.id)
        {
            guessPanel.SetActive(false);
           
            GameObject cup = GameObject.Find(client.players[turn].Cup + "Cup");
            GameObject arrow = GameObject.Find("arrow");
            arrow.transform.position = new Vector3(cup.transform.position.x, cup.transform.position.y + 0.7f, cup.transform.position.z);
            ArrowMovement arrowScript= arrow.GetComponent<ArrowMovement>();
            arrowScript.start = cup.transform.position.y + 0.7f;
            arrowScript.end = cup.transform.position.y + 1.0f;

            arrowScript._timeStartedLerping = Time.time;
            arrowScript.direction = true;
            arrowScript.moving = true;
            
            Text name = GameObject.Find("Canvas/CurrentTurn/CurrentTurnName").GetComponent<Text>();
            name.text = client.players[turn].Name + "'s Turn";
        }
        else
        {
            Text name = GameObject.Find("Canvas/CurrentTurn/CurrentTurnName").GetComponent<Text>();
            name.text = "Your Turn";
            GameObject cup = GameObject.Find("Player");
            GameObject arrow = GameObject.Find("arrow");
            ArrowMovement arrowScript = arrow.GetComponent<ArrowMovement>();
            arrowScript.moving = false;
            arrow.transform.position = new Vector3(cup.transform.position.x, cup.transform.position.y - 5f, cup.transform.position.z);
            guessPanel.SetActive(true);
            SetPanel();
        }
    }

    void setGuess() 
    {
        Text guess;
        if (client.previousGuess[0] < 0)
        {
            GameObject.Find("Canvas/GuessText/GuessNumber").GetComponent<Text>().enabled = true;

            GameObject.Find("Canvas/GuessText/GuessNumber/GuessDice/Apostrophe").GetComponent<Text>().enabled = true;
            GameObject.Find("Canvas/GuessText/GuessNumber/GuessDice").GetComponent<Image>().enabled = true;


        }
        else 
        {
            Debug.Log("Here");
            GameObject.Find("Canvas/GuessText/GuessNumber").GetComponent<Text>().enabled= true;

            GameObject.Find("Canvas/GuessText/GuessNumber/GuessDice/Apostrophe").GetComponent<Text>().enabled = true;
            GameObject.Find("Canvas/GuessText/GuessNumber/GuessDice").GetComponent<Image>().enabled = true;
        }

        guess = GameObject.Find("Canvas/GuessText/GuessNumber").GetComponent<Text>();

        guess.text = client.previousGuess[0]+",";

        Image dice = GameObject.Find("Canvas/GuessText/GuessNumber/GuessDice").GetComponent<Image>();
        dice.sprite = diceSheet[client.previousGuess[1]-1];

        if (client.previousGuess[0] > 1)
        {
            guess = GameObject.Find("Canvas/GuessText/GuessNumber/GuessDice/Apostrophe").GetComponent<Text>();
            guess.text = "'s";
        }
        else 
        {
            guess = GameObject.Find("Canvas/GuessText/GuessNumber/GuessDice/Apostrophe").GetComponent<Text>();
            guess.text = "";
        }
        
    }

    void SetPanel() 
    {
        if (client.previousGuess[1] > 0)
        {
            Text number = GameObject.Find("GuessPanel/Amount").GetComponent<Text>();
            number.text = client.previousGuess[0].ToString();
            for (int i = 1; i < 7; i++) 
            {
                if (i < client.previousGuess[1])
                {
                    Image dice = GameObject.Find("GuessPanel/GuessDice/" + i).GetComponent<Image>();
                    dice.sprite = crossSheet[i - 1];
                }
                else 
                {
                    Image dice = GameObject.Find("GuessPanel/GuessDice/" + i).GetComponent<Image>();
                    dice.sprite = diceSheet[i - 1];
                }
            }
        }
        else 
        {
            Text number = GameObject.Find("GuessPanel/Amount").GetComponent<Text>();
            number.text = "1";
        }
    }

    void SetDice() 
    {
        GameObject gameDice;
        Debug.Log("Playerslength: "+client.players.Count);
        foreach (PlayerObject player in client.players) {
            int i = 1;
            if (player.NewDice)
            {
                if (player.DiceLeft < 5) 
                {
                    gameDice = GameObject.Find(player.Cup + "/dice/Dice" + (player.DiceLeft + 1));
                    gameDice.transform.position = new Vector3(gameDice.transform.position.x, gameDice.transform.position.y - 0.11f, gameDice.transform.position.z);
                
                }
                if (newRound)
                {
                    foreach (int dice in player.PreviousDice)
                    {
                        gameDice = GameObject.Find(player.Cup + "/dice/Dice" + i);

                      
                        gameDice.transform.position = new Vector3(gameDice.transform.position.x - dicePositions[dice - 1, 0],
                                                                    gameDice.transform.position.y - dicePositions[dice - 1, 1],
                                                                    gameDice.transform.position.z - dicePositions[dice - 1, 2]);

                        gameDice.transform.eulerAngles = new Vector3(0 - dicePositions[dice - 1, 3],
                                                                     0 - dicePositions[dice - 1, 4],
                                                                     0 - dicePositions[dice - 1, 5]);
                        i++;
                    }
                }
                i = 1;
                {

                    foreach (int dice in player.Dice)
                    {

                        gameDice = GameObject.Find(player.Cup + "/dice/Dice" + i);

                        gameDice.transform.position = new Vector3(gameDice.transform.position.x + dicePositions[dice - 1, 0],
                                                                    gameDice.transform.position.y + dicePositions[dice - 1, 1],
                                                                    gameDice.transform.position.z + dicePositions[dice - 1, 2]);


                        gameDice.transform.eulerAngles = new Vector3(dicePositions[dice - 1, 3],
                                                                     dicePositions[dice - 1, 4],
                                                                     dicePositions[dice - 1, 5]);
                        i++;
                    }
                }
                if ((player.Id == client.players.Count - 1 && player.Id != client.id) || (player.Id == client.players.Count - 2 && client.players.Count - 1 == client.id))
                {
                    showDice = true;
                }
            }
            player.NewDice = false;
            
        }
        
    }

    public void SendGuess() 
    {
        if (client.guess[0] > client.previousGuess[0] || 
            client.guess[1] > client.previousGuess[1])
        {
            client.client.SendMessage("GUESS" + client.guess[0] + " " + client.guess[1]);
            //clientScript.previousGuess[0] = clientScript.guess[0];
            //clientScript.previousGuess[1] = clientScript.guess[0];
        }
    }

    void Pause(float t) 
    {
    
    }

}
